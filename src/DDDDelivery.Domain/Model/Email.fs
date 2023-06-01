[<AutoOpen>]
module Email

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
