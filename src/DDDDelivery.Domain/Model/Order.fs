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
        | CancelledByBuyer of CancellationReason
        | CancelledBySeller of CancellationReason

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
        | CancelledByBuyer _
        | CancelledBySeller _ -> false
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

        let private cancel newStatus : Command =
            let tryCancel =
                fun status ->
                    if cancellable status then
                        Some(newStatus)
                    else
                        None

            Lens.changeStatus tryCancel

        let buyerCancelled reason =
            CancelledByBuyer { reason = reason } |> cancel

        let sellerCancelled reason =
            CancelledBySeller { reason = reason } |> cancel

        let accepted: Command = Lens.changeStatus (Pending |> advanceTo Processed)

        let prepared: Command = Lens.changeStatus (Processed |> advanceTo Awaiting)

        let shipped: Command = Lens.changeStatus (Awaiting |> advanceTo Shipped)

        let delivered: Command = Lens.changeStatus (Shipped |> advanceTo Delivered)
