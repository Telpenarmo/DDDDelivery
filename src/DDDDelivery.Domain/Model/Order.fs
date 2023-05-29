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
    type ShipmentId = { id: string }

    type OrderStatus =
        | Pending
        | Processed
        | Awaiting
        | Shipped of ShipmentId
        | Delivered
        | CancelledByBuyer of CancellationReason
        | CancelledBySeller of CancellationReason

    [<CustomEquality; NoComparison>]
    type Order =
        { id: OrderId
          status: OrderStatus
          customer: Customer.CustomerId
          // TODO: shipment:
          orderLines: OrderLine seq
          orderTime: DateTime
          expectedShipmentTime: DateTime
          modifiedAt: DateTime }

        override this.Equals(other) =
            match other with
            | :? Order as o -> this.id = o.id
            | _ -> false

        override this.GetHashCode() = this.id.GetHashCode()

    let (|Cancellable|NotCancellable|) =
        function
        | Delivered
        | CancelledByBuyer _
        | CancelledBySeller _ -> Cancellable
        | _ -> NotCancellable

    let internal create id customerId orderLines expectedDeliveryDays =
        { id = id
          status = Pending
          customer = customerId
          orderLines = orderLines
          orderTime = DateTime.Now
          expectedShipmentTime =
            let expectedDeliveryTime = expectedDeliveryDays |> TimeSpan.FromDays
            DateTime.Now + expectedDeliveryTime
          modifiedAt = DateTime.Now }

    module Commands =
        type Command = Order -> Order option

        let private changeStatus f order =
            f order.status
            |> Option.map (fun status ->
                { order with
                    modifiedAt = DateTime.Now
                    status = status })

        let private advanceTo next expected actual =
            if actual = expected then
                Some next
            else
                None

        let private cancel newStatus : Command =
            let tryCancel =
                function
                | Cancellable -> Some newStatus
                | NotCancellable -> None

            changeStatus tryCancel

        let buyerCancelled reason =
            CancelledByBuyer { reason = reason } |> cancel

        let sellerCancelled reason =
            CancelledBySeller { reason = reason } |> cancel

        let accepted: Command = changeStatus (Pending |> advanceTo Processed)

        let prepared: Command = changeStatus (Processed |> advanceTo Awaiting)

        let shipped shipmentId : Command =
            changeStatus (Awaiting |> advanceTo (Shipped shipmentId))

        let delivered: Command =
            let tryDeliver =
                function
                | Shipped _ -> Some Delivered
                | _ -> None

            changeStatus tryDeliver
