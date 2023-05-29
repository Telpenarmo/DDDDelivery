namespace DDDDelivery.Domain

module Customer =
    type CustomerId = CustomerId of int64

    type Address =
        { street: string
          city: string
          postalCode: string }

    type Email = Email of string
    type Phone = Phone of string

    type ContactInfo =
        | Email of Email
        | Phone of Phone

    [<CustomEquality; NoComparison>]
    type Customer =
        { id: CustomerId
          name: string
          PrimaryContactInfo: ContactInfo
          SecondaryContactInfo: ContactInfo }

        override this.Equals(other) =
            match other with
            | :? Customer as c -> this.id = c.id
            | _ -> false

        override this.GetHashCode() = this.id.GetHashCode()