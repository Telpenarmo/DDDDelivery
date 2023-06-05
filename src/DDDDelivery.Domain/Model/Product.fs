namespace DDDDelivery.Domain


type ProductId = ProductId of int64

type Availability =
    | Available of uint64
    | Infinite
    | Unavailable

    static member (-)(availability, amount) =
        match availability with
        | Available a -> Available(a - amount) |> Some
        | Infinite -> Some Infinite
        | Unavailable -> None

    static member (+)(availability, amount) =
        match availability with
        | Available a -> Available(a + amount)
        | Infinite -> Infinite
        | Unavailable -> Available amount

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
