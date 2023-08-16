namespace DDDDelivery.Infrastructure.Repositories.InMemory

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories
open DDDDelivery.Infrastructure.Repositories.InMemory

type CustomersRepository() =
    inherit RepositoryBase<CustomerId, Customer>()

    interface ICustomersRepository with
        member this.Insert customer =
            let id = CustomerId(Map.count this.items |> int64)

            ``base``.Insert(id, customer) |> ignore

            Task.FromResult(true)

        member _.Delete(id: CustomerId) : Task<bool> = base.Delete id
        member _.FindById(id: CustomerId) : Task<Customer option> = base.FindById id
        member _.FindAll() : Task<seq<Customer>> = base.FindAll ()
        member _.Update(customer: Customer) : Task<bool> = ``base``.Update(customer.Id, customer)
