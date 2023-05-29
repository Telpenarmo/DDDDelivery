namespace DDDDelivery.Domain

module Product =

    type ProductId = ProductId of int64

    type Availiability =
        | Unavailable
        | Infinite
        | Available of uint64

    [<CustomEquality; NoComparison>]
    type Product =
        { id: ProductId
          name: string
          description: string
          price: float
          availableUnits: Availiability }
        
        override this.Equals(other) =
            match other with
            | :? Product as p -> this.id = p.id
            | _ -> false
        
        override this.GetHashCode() = this.id.GetHashCode()
