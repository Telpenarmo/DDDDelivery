namespace DDDDelivery.Domain

open System

module Order =

    type OrderId = OrderId of int64

    type OrderLine =
        { product: Product.ProductId
          amount: int
          worth: float
          discount: float }

    type CancellationReason = { reason: string }

    type OrderStatus =
        | Pending
        | Processed
        | Awaiting
        | Shipped
        | Delivered
        | Cancelled of CancellationReason

    [<NoEquality; NoComparison>]
    type Order =
        { id: OrderId
          status: OrderStatus
          customer: Customer.CustomerId
          // TODO: shipment:
          orderLines: OrderLine seq
          orderTime: DateTime
          expectedShipmentTime: DateTime
          modifiedAt: DateTime }

    let cancellable =
        function
        | Delivered
        | Cancelled _ -> false
        | _ -> true

    let internal create id customerId orderLines expectedDeliveryDays =
        { id = id
          status = Pending
          customer = customerId
          orderLines = orderLines
          orderTime = DateTime.Now
          expectedShipmentTime =
            DateTime.Now
            + TimeSpan.FromDays expectedDeliveryDays
          modifiedAt = DateTime.Now }

    module Lens =
        let changeStatus f order =
            f order.status
            |> Option.map (fun status ->
                { order with
                    modifiedAt = DateTime.Now
                    status = status })

    module Commands =
        type Command = Order -> Order option

        let private advanceTo next expected actual =
            if actual = expected then
                Some next
            else
                None

        let cancel reason : Command =
            let tryCancel =
                fun status ->
                    if cancellable status then
                        Some(Cancelled { reason = reason })
                    else
                        None

            Lens.changeStatus tryCancel

        let accept: Command = Lens.changeStatus (Pending |> advanceTo Processed)

        let prepare: Command = Lens.changeStatus (Processed |> advanceTo Awaiting)

        let ship: Command = Lens.changeStatus (Awaiting |> advanceTo Shipped)

        let deliver: Command = Lens.changeStatus (Shipped |> advanceTo Delivered)
