namespace DDDDelivery.Domain

open System


type OrderId = OrderId of int64

type OrderLine =
    { Product: ProductId
      Amount: uint64
      Worth: float
      Discount: float }

type CancellationReason = { Reason: string }
type ShipmentId = { Id: string }


module OrderStatus =
    type T =
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
      Status: OrderStatus.T
      Customer: CustomerId
      // TODO: shipment:
      OrderLines: OrderLine seq
      OrderedAt: DateTime
      ModifiedAt: DateTime }

    override this.Equals(other) =
        match other with
        | :? Order as o -> this.Id = o.Id
        | _ -> false

    override this.GetHashCode() = this.Id.GetHashCode()

module Order =
    open OrderStatus

    let (|Active|Inactive|) =
        function
        | Pending
        | InPreparation
        | AwaitingShipment
        | Shipped _ -> Active
        | Delivered
        | CancelledByCustomer _
        | CancelledByStore _ -> Inactive

    let create id customerId orderLines =
        { Id = id
          Status = Pending
          Customer = customerId
          OrderLines = orderLines
          OrderedAt = DateTime.Now
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
            CancelledByCustomer { Reason = reason } |> cancel

        let storeCancelled reason =
            CancelledByStore { Reason = reason } |> cancel

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
