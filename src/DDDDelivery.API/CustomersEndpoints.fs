namespace DDDDelivery.API

open Giraffe
open DDDDelivery.Domain
open DDDDelivery.Application

type private CustomerDto = { Id: int64; Name: string }

type CustomersEndpoints =
    | GetById
    | GetAll
    | Create
    | Update
    | Delete

    member this.Handle() : HttpHandler =

        let fromDto (dto: CustomerDto) =
            new Customer(CustomerId dto.Id, dto.Name, Phone(PhoneNumber "123"))

        match this with
        | GetAll ->
            GET
            >=> route "/"
            >=> withUow (fun uow -> Customers.getAll uow >> Task.map serialize) ()
        | GetById ->
            let getById uow id =
                task {
                    match! Customers.getById uow (CustomerId id) with
                    | Some customer -> return serialize customer |> Successful.ok
                    | None -> return RequestErrors.notFound (text "Customer not found")
                }

            GET >=> routef "/%d" (withUow getById)
        | Create ->
            let create uow =
                fromDto
                >> Customers.create uow
                >> Task.map serialize

            POST >=> bindJson<CustomerDto> (withUow create)
            |> Successful.created
        | Update ->
            let update uow =
                fromDto
                >> Customers.update uow
                >> Task.map serialize

            PUT >=> bindJson<CustomerDto> (withUow update)
        | Delete ->
            let delete uow =
                CustomerId
                >> Customers.delete uow
                >> Task.map serialize

            DELETE >=> routef "/%d" (withUow delete)

    static member Route = "customers"
