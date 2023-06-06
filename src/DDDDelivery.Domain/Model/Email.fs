namespace DDDDelivery.Domain.HelperTypes

[<AutoOpen>]
module Email =

    type Email = private Email of string

    let (|Email|) =
        function
        | Email email -> email

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

    type Phone = Phone of string

    type ContactInfo =
        | Email of Email
        | Phone of Phone
