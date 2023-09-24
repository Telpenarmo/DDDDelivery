namespace DDDDelivery.Application

open System
open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories

module Orders =

    type OrderCommand<'Input, 'Output> = IUnitOfWork -> 'Input -> Task<'Output>
    type OrderQuery<'Input> = IUnitOfWork -> 'Input -> Task<Order seq>

    [<AutoOpen>]
    module Common =

        let updateOrder (uow: IUnitOfWork) orderId onNotFound tryUpdate =
            task {
                match! uow.Orders.FindById orderId with
                | Some order ->
                    match tryUpdate order with
                    | Ok order ->
                        let! _ = uow.Orders.Update order
                        do! uow.SaveChanges()
                        return Ok order
                    | Error e -> return Error e
                | None -> return Error(onNotFound orderId)
            }

        let getProductsInOrder (uow: IUnitOfWork) (order: Order) =
            order.OrderLines
            |> Seq.map (fun ol -> ol.Product)
            |> uow.Products.FindMany

    module OrderCreation =

        type CreationError =
            | CustomerNotFound of CustomerId
            | NotEnoughProductsAvailable of ProductId seq

        let create (uow: IUnitOfWork) data =
            task {
                let! order = uow.Orders.Insert data

                let tryReserveProduct (product: Product) =
                    let orderLine =
                        order.OrderLines
                        |> Seq.find (fun ol -> ol.Product = product.Id)

                    match product.AvailableUnits - orderLine.Amount with
                    | Some x ->
                        product.AvailableUnits <- x
                        None
                    | None -> Some product.Id

                let! products = getProductsInOrder uow order
                let unavailableProducts = products |> Seq.choose tryReserveProduct

                if unavailableProducts |> Seq.isEmpty then
                    let! _ = uow.Products.UpdateMany products
                    do! uow.SaveChanges()
                    return Ok order
                else
                    return Error(NotEnoughProductsAvailable unavailableProducts)
            }

    module OrderCancellation =

        type CancellationError =
            | OrderNotFound
            | OrderNotCancellable

        type CancellationInput = (OrderId * string * DateTime)

        let returnProducts (uow: IUnitOfWork) (order: Order) =
            let returnProduct (product: Product) =
                let orderLine =
                    order.OrderLines
                    |> Seq.find (fun ol -> ol.Product = product.Id)

                product.AvailableUnits <- product.AvailableUnits + orderLine.Amount

            task {
                let! products = getProductsInOrder uow order
                products |> Seq.iter returnProduct
                let! _ = uow.Products.UpdateMany products
                return ()
            }

        let cancel (uow: IUnitOfWork) cancelCommand ((OrderId id) as orderId) timestamp =
            let doCancel order =
                task {
                    uow.Orders.Update order |> ignore
                    do! returnProducts uow order
                    do! uow.SaveChanges()
                }

            task {
                match! uow.Orders.FindById orderId with
                | Some order ->
                    match cancelCommand (order, timestamp) with
                    | Some order ->
                        do! doCancel order
                        return Ok order
                    | None -> return (Error OrderNotCancellable)
                | None -> return (Error OrderNotFound)
            }

        let cancelByCustomer uow (id, reason, timestamp) =
            cancel uow (Order.Commands.customerCancelled reason) id timestamp

        let cancelBySeller uow (id, reason, timestamp) =
            cancel uow (Order.Commands.storeCancelled reason) id timestamp

    module OrderAcceptance =

        type AcceptanceError =
            | OrderNotFound
            | OrderNotAcceptable

        let accept (uow: IUnitOfWork) ((OrderId id) as orderId, timestamp) =
            let doAccept order =
                task {
                    let! _ = uow.Orders.Update order
                    do! uow.SaveChanges()
                    printfn "Order %d accepted" id
                }

            task {
                match! uow.Orders.FindById orderId with
                | Some order ->
                    match Order.Commands.accepted (order, timestamp) with
                    | Some order ->
                        do! doAccept order
                        return Ok order
                    | None -> return Error OrderNotAcceptable
                | None -> return Error OrderNotFound
            }

    module OrderPreparation =
        type PreparationError =
            | OrderNotFound
            | OrderNotPreparable

        let prepare (uow: IUnitOfWork) ((OrderId id) as orderId, timestamp) =
            let doPrepare order =
                task {
                    let! _ = uow.Orders.Update order
                    do! uow.SaveChanges()
                    printfn "Order %d prepared" id
                }

            task {
                match! uow.Orders.FindById orderId with
                | Some order ->
                    match Order.Commands.prepared (order, timestamp) with
                    | Some order ->
                        do! doPrepare order
                        return Ok order
                    | None -> return Error OrderNotPreparable
                | None -> return Error OrderNotFound
            }

    module OrderShipment =

        type ShipmentError =
            | OrderNotFound
            | OrderNotShippable

        type ShipmentInput = (ShipmentId * OrderId * DateTime)

        let ship (uow: IUnitOfWork) (shipmentId, ((OrderId id) as orderId), timestamp) =
            let doShip order =
                task {
                    let! _ = uow.Orders.Update order
                    do! uow.SaveChanges()
                    printfn "Order %d shipped" id
                }

            task {
                match! uow.Orders.FindById orderId with
                | Some order ->
                    match Order.Commands.shipped shipmentId (order, timestamp) with
                    | Some order ->
                        do! doShip order
                        return Ok order
                    | None -> return Error OrderNotShippable
                | None -> return Error OrderNotFound
            }

    module OrderDelivery =

        type DeliveryError =
            | OrderNotFound
            | OrderNotDeliverable

        let deliver (uow: IUnitOfWork) ((OrderId id) as orderId, timestamp) =
            let doDeliver order =
                match Order.Commands.delivered (order, timestamp) with
                | Some order -> Ok order
                | None -> Error OrderNotDeliverable

            updateOrder uow orderId (fun _ -> OrderNotFound) doDeliver

    module OrdersFetching =

        let All (uow: IUnitOfWork) = uow.Orders.FindAll

        let WithProduct (uow: IUnitOfWork) = uow.Orders.FindOrdersWithProduct

        let WithCustomer (uow: IUnitOfWork) = uow.Orders.FindOrdersFromCustomer

        let PendingForThreeDays (uow: IUnitOfWork) () =
            uow.Orders.FindStaleOrders OrderStatus.Pending 3

        let ProcessingForFiveDays (uow: IUnitOfWork) () =
            uow.Orders.FindStaleOrders OrderStatus.InPreparation 5

        let AwaitingShipmentForTwoDays (uow: IUnitOfWork) () =
            uow.Orders.FindStaleOrders OrderStatus.AwaitingShipment 2
