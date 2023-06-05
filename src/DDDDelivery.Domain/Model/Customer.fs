namespace DDDDelivery.Domain

open DDDDelivery.Domain.HelperTypes

type CustomerId = CustomerId of int64

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
