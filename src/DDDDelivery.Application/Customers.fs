namespace DDDDelivery.Application

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories

module Customers =

    type CustomerCommand<'Input, 'Output> = IUnitOfWork -> 'Input -> Task<'Output>
    type CustomerQuery<'Input> = IUnitOfWork -> 'Input -> Task<Customer seq>

    let create (uow: IUnitOfWork) (customer: Customer) =
        task {
            let! _added = uow.Customers.Insert customer
            do! uow.SaveChanges()
            return customer
        }

    let update (uow: IUnitOfWork) (customer: Customer) =
        task {
            let! _updated = uow.Customers.Update customer
            do! uow.SaveChanges()
            return customer
        }

    let delete (uow: IUnitOfWork) customerId =
        task {
            let! _deleted = uow.Customers.Delete customerId
            do! uow.SaveChanges()
            return ()
        }

    let getById (uow: IUnitOfWork) (customerId: CustomerId) =
        task {
            match! uow.Customers.FindById customerId with
            | Some customer -> return Some customer
            | None -> return None
        }

    let getAll (uow: IUnitOfWork) = uow.Customers.FindAll
