namespace DDDDelivery.Domain

open System

module Order =

    type OrderId = OrderId of int64

    type OrderLine =
        { product: Product.ProductId
          amount: int
          worth: float
          discount: float }

    type OrderStatus =
        | Pending
        | Processed
        | Awaiting
        | Shipped
        | Delivered
        | Cancelled

    [<NoEquality; NoComparison>]
    type Order =
        { id: OrderId
          status: OrderStatus
          customer: Customer.CustomerId
          // TODO: shipment:
          orders: OrderLine seq
          orderTime: DateTime
          expectedShipmentTime: DateTime }
