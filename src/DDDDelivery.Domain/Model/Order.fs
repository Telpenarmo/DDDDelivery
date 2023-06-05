namespace DDDDelivery.Domain

open System

module Order =

    type OrderId = OrderId of int64

    type OrderLine =
        { product: Product.ProductId
          amount: uint64
          worth: float
          discount: float }

    type CancellationReason = { reason: string }
    type ShipmentId = { id: string }

    type OrderStatus =
        | Pending
        | InPreparation
        | AwaitingShipment
        | Shipped of ShipmentId
        | Delivered
        | CancelledByCustomer of CancellationReason
        | CancelledByStore of CancellationReason

    [<CustomEquality; NoComparison>]
    type Order =
        { Id: OrderId
          Status: OrderStatus
          Customer: Customer.CustomerId
          // TODO: shipment:
          OrderLines: OrderLine seq
          OrderedAt: DateTime
          ExpectedShipmentTime: DateTime
          ModifiedAt: DateTime }

        override this.Equals(other) =
            match other with
            | :? Order as o -> this.Id = o.Id
            | _ -> false

        override this.GetHashCode() = this.Id.GetHashCode()

    let (|Active|Inactive|) =
        function
        | Pending
        | InPreparation
        | AwaitingShipment
        | Shipped _ -> Active
        | Delivered
        | CancelledByCustomer _
        | CancelledByStore _ -> Inactive

    let create id customerId orderLines expectedDeliveryDays =
        { Id = id
          Status = Pending
          Customer = customerId
          OrderLines = orderLines
          OrderedAt = DateTime.Now
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
                | Active -> Some newStatus
                | Inactive -> None

            changeStatus tryCancel

        let customerCancelled reason =
            CancelledByCustomer { reason = reason } |> cancel

        let storeCancelled reason =
            CancelledByStore { reason = reason } |> cancel

        let accepted: Command = changeStatus (Pending |> advanceTo InPreparation)

        let prepared: Command = changeStatus (InPreparation |> advanceTo AwaitingShipment)

        let shipped shipmentId : Command =
            changeStatus (AwaitingShipment |> advanceTo (Shipped shipmentId))

        let delivered: Command =
            let tryDeliver =
                function
                | Shipped _ -> Some Delivered
                | _ -> None

            changeStatus tryDeliver
