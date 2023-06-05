namespace DDDDelivery.Application

open DDDDelivery.Domain

module CustomersSpecifications =
    open DDDDelivery.Domain.Repositories

    let baseSpec =
        Specification.Specification<Customer.Customer>
            .Zero

module Customers =
    open DDDDelivery.Domain.Repositories

    let createCustomer (uow: IUnitOfWork) customer =
        task {
            let! _ = uow.Customers.Insert customer
            do! uow.SaveChanges()
            return Ok customer
        }

    let updateCustomer (uow: IUnitOfWork) customer =
        task {
            let! _ = uow.Customers.Update customer
            do! uow.SaveChanges()
            return Ok customer
        }

    let deleteCustomer (uow: IUnitOfWork) customerId =
        task {
            let! _ = uow.Customers.Delete customerId
            do! uow.SaveChanges()
            return Ok()
        }

    let getCustomer (uow: IUnitOfWork) customerId =
        task {
            match! uow.Customers.FindById customerId with
            | Some customer -> return Ok customer
            | None -> return Error(sprintf "Customer with id %A not found" customerId)
        }

    let getCustomers (uow: IUnitOfWork) =
        uow.Customers.FindAll(CustomersSpecifications.baseSpec ())
