namespace DDDDelivery.Domain

module Customer =
    type CustomerId = CustomerId of int64

    type Address =
        { street: string
          city: string
          postalCode: string }

    type Phone = Phone of string

    type ContactInfo =
        | Email of Email
        | Phone of Phone

    [<NoEquality; NoComparison>]
    type Customer =
        val Id: CustomerId
        val mutable Name: string
        val mutable PrimaryContactInfo: ContactInfo
        val mutable SecondaryContactInfo: ContactInfo option

        new(id: CustomerId, name: string, primaryContactInfo: ContactInfo, secondaryContactInfo: ContactInfo option) =
            { Id = id
              Name = name
              PrimaryContactInfo = primaryContactInfo
              SecondaryContactInfo = secondaryContactInfo }

        new(id: CustomerId, name: string, contactInfo: ContactInfo) =
            { Id = id
              Name = name
              PrimaryContactInfo = contactInfo
              SecondaryContactInfo = None }
