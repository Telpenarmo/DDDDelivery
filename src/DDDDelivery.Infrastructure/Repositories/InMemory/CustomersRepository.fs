namespace DDDDelivery.Infrastructure.Repositories.InMemory

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories
open DDDDelivery.Infrastructure.Repositories.InMemory

type CustomersRepository =
    val mutable customers: Map<Customer.CustomerId, Customer.Customer>

    new() = { customers = Map.empty }


    interface ICustomersRepository with
        member this.Insert(customer: Customer.Customer) : Task<bool> =
            this.customers <- this.customers.Add(customer.Id, customer)
            Task.FromResult(true)

        member this.Delete(id: Customer.CustomerId) : Task<bool> =
            this.customers <- this.customers.Remove(id)
            Task.FromResult(true)

        member this.FindById(id: Customer.CustomerId) : Task<Customer.Customer option> =
            this.customers
            |> Map.tryFind id
            |> Task.FromResult

        member this.FindSpecified(spec: Specification.Specification<Customer.Customer>) : Task<seq<Customer.Customer>> =
            SpecificationEvaluator.evaluate spec this.customers.Values

        member this.Update(customer: Customer.Customer) : Task<bool> =
            this.customers <- this.customers.Add(customer.Id, customer)
            Task.FromResult(true)
