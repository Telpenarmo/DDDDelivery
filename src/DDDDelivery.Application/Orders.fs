namespace DDDDelivery.Application

open System

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories

[<AutoOpen>]
module private Common =

    let updateOrder (uow: IUnitOfWork) orderId onNotFound f =
        task {
            match! uow.Orders.FindById orderId with
            | Some order ->
                match f order with
                | Ok order ->
                    let! _ = uow.Orders.Update order
                    do! uow.SaveChanges()
                    return Ok order
                | Error e -> return Error e
            | None -> return Error(onNotFound orderId)
        }

module OrderSpecifications =
    let baseSpec =
        Specification.Specification<Order>.Zero ()
        |> SpecificationsFsharp.Include <@ fun (o: Order) -> o.OrderLines @>

    let OrderedProductsSpec (order: Order) =
        Specification.Specification<Product.Product>.Zero ()
        |> SpecificationsFsharp.Filter
            <@ fun (p: Product.Product) ->
                order.OrderLines
                |> Seq.exists (fun ol -> ol.product = p.Id) @>

    let AllOrdersSpec () = baseSpec

    let OrdersWithProductSpec (productId: Product.ProductId) =
        baseSpec
        |> SpecificationsFsharp.Filter
            <@ fun (o: Order) ->
                o.OrderLines
                |> Seq.exists (fun ol -> ol.product = productId) @>

    let OrdersFromCustomerSpec (customerId: Customer.CustomerId) =
        baseSpec
        |> SpecificationsFsharp.Filter <@ fun (o: Order) -> o.Customer = customerId @>

    let StaleOrdersSpec status days =
        baseSpec
        |> SpecificationsFsharp.Filter
            <@ fun (o: Order) ->
                o.Status = status
                && DateTime.Now - o.OrderedAt > TimeSpan.FromDays(float days) @>

open OrderSpecifications

module OrderCreation =

    type CreationError =
        | CustomerNotFound of Customer.CustomerId
        | NotEnoughProductsAvailable of Product.ProductId seq

    let create (uow: IUnitOfWork) data =
        task {
            let! order = uow.Orders.Insert data

            let tryReserveProduct (product: Product.Product) =
                let orderLine =
                    order.OrderLines
                    |> Seq.find (fun ol -> ol.product = product.Id)

                match product.AvailableUnits - orderLine.amount with
                | Some x ->
                    product.AvailableUnits <- x
                    None
                | None -> Some product.Id

            let! products = uow.Products.FindSpecified(OrderedProductsSpec order)
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

    let private returnProducts (uow: IUnitOfWork) (order: Order) =
        let returnProduct (product: Product.Product) =
            let orderLine =
                order.OrderLines
                |> Seq.find (fun ol -> ol.product = product.Id)

            product.AvailableUnits <- product.AvailableUnits + orderLine.amount

        task {
            let! products = uow.Products.FindSpecified(OrderedProductsSpec order)
            products |> Seq.iter returnProduct
            let! _ = uow.Products.UpdateMany products
            return ()
        }

    let private cancel cancelCommand (uow: IUnitOfWork) ((OrderId id) as orderId) =
        let doCancel order =
            task {
                uow.Orders.Update order |> ignore
                do! returnProducts uow order
                do! uow.SaveChanges()
            }

        task {
            match! uow.Orders.FindById orderId with
            | Some order ->
                match cancelCommand order with
                | Some order ->
                    do! doCancel order
                    return Ok order
                | None -> return (Error OrderNotCancellable)
            | None -> return (Error OrderNotFound)
        }

    let cancelByCustomer reason =
        cancel (Order.Commands.customerCancelled reason)

    let cancelBySeller reason =
        cancel (Order.Commands.storeCancelled reason)

module OrderAcceptance =

    type AcceptanceError =
        | OrderNotFound
        | OrderNotAcceptable

    let accept (uow: IUnitOfWork) ((OrderId id) as orderId) =
        let doAccept order =
            task {
                let! _ = uow.Orders.Update order
                do! uow.SaveChanges()
                printfn "Order %d accepted" id
            }

        task {
            match! uow.Orders.FindById orderId with
            | Some order ->
                match Order.Commands.accepted order with
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

    let prepare (uow: IUnitOfWork) ((OrderId id) as orderId) =
        let doPrepare order =
            task {
                let! _ = uow.Orders.Update order
                do! uow.SaveChanges()
                printfn "Order %d prepared" id
            }

        task {
            match! uow.Orders.FindById orderId with
            | Some order ->
                match Order.Commands.prepared order with
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

    let ship shipmentId (uow: IUnitOfWork) ((OrderId id) as orderId) =
        let doShip order =
            task {
                let! _ = uow.Orders.Update order
                do! uow.SaveChanges()
                printfn "Order %d shipped" id
            }

        task {
            match! uow.Orders.FindById orderId with
            | Some order ->
                match Order.Commands.shipped shipmentId order with
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

    let deliver (uow: IUnitOfWork) ((OrderId id) as orderId) =
        let doDeliver order =
            match Order.Commands.delivered order with
            | Some order -> Ok order
            | None -> Error OrderNotDeliverable

        updateOrder uow orderId (fun _ -> OrderNotFound) doDeliver

module OrdersFetching =

    let private fetch (uow: IUnitOfWork) spec =
        task {
            let! orders = uow.Orders.FindSpecified(spec)
            return orders
        }

    let All (uow: IUnitOfWork) = AllOrdersSpec() |> fetch uow

    let WithProduct (uow: IUnitOfWork) productId =
        OrdersWithProductSpec productId |> fetch uow

    let WithCustomer (uow: IUnitOfWork) customerId =
        OrdersFromCustomerSpec customerId |> fetch uow

    let PendingForThreeDays (uow: IUnitOfWork) =
        StaleOrdersSpec OrderStatus.Pending 3
        |> fetch uow

    let ProcessingForFiveDays (uow: IUnitOfWork) =
        StaleOrdersSpec OrderStatus.InPreparation 5
        |> fetch uow

    let AwaitingShipmentForTwoDays (uow: IUnitOfWork) =
        StaleOrdersSpec OrderStatus.AwaitingShipment 2
        |> fetch uow
