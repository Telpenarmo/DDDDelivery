namespace DDDDelivery.Domain

[<AutoOpen>]
module Email =

    type Email = private Email of string

    let (|Email|) (Email email) = email

    let createEmail email =
        let valid email =
            System.Text.RegularExpressions.Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")

        if valid email then
            Some <| Email email
        else
            None

[<AutoOpen>]
module ContactInfo =
    type Address =
        { Street: string
          City: string
          PostalCode: string }

    type PhoneNumber = PhoneNumber of string

    type ContactInfo =
        | Email of Email
        | Phone of PhoneNumber
