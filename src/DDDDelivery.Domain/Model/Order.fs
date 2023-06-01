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
        { Id: OrderId
          Status: OrderStatus
          Customer: Customer.CustomerId
          // TODO: shipment:
          OrderLines: OrderLine seq
          OrderTime: DateTime
          ExpectedShipmentTime: DateTime
          ModifiedAt: DateTime }

        override this.Equals(other) =
            match other with
            | :? Order as o -> this.Id = o.Id
            | _ -> false

        override this.GetHashCode() = this.Id.GetHashCode()

    let (|Cancellable|NotCancellable|) =
        function
        | Delivered
        | CancelledByBuyer _
        | CancelledBySeller _ -> Cancellable
        | _ -> NotCancellable

    let internal create id customerId orderLines expectedDeliveryDays =
        { Id = id
          Status = Pending
          Customer = customerId
          OrderLines = orderLines
          OrderTime = DateTime.Now
          ExpectedShipmentTime =
            let expectedDeliveryTime = expectedDeliveryDays |> TimeSpan.FromDays
            DateTime.Now + expectedDeliveryTime
          ModifiedAt = DateTime.Now }

    module Commands =
        type Command = Order -> Order option

        let private changeStatus f order =
            f order.Status
            |> Option.map (fun status ->
                { order with
                    ModifiedAt = DateTime.Now
                    Status = status })

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
