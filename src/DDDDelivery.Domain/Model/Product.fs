namespace DDDDelivery.Domain

module Product =

    type ProductId = ProductId of int64

    type Availability =
        | Available of uint64
        | Infinite
        | Unavailable

    [<NoEquality; NoComparison>]
    type Product =
        val Id: ProductId
        val mutable Name: string
        val mutable Description: string
        val mutable Price: float
        val mutable AvailableUnits: Availability

        new(id: ProductId, name: string, description: string, price: float, availableUnits: Availability) =
            { Id = id
              Name = name
              Description = description
              Price = price
              AvailableUnits = availableUnits }
