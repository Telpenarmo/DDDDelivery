namespace DDDDelivery.Domain

module Customer =
    type CustomerId = CustomerId of int64

    type Customer = { id: CustomerId }
