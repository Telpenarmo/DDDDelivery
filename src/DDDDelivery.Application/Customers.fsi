namespace DDDDelivery.Application

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories

module Customers =
    
    type CustomerCommand<'Input, 'Output> = IUnitOfWork -> 'Input -> Task<'Output>
    type CustomerQuery<'Input> = IUnitOfWork -> 'Input -> Task<Customer seq>

    val create: CustomerCommand<Customer, Customer>
    val update: CustomerCommand<Customer, Customer>
    val delete: CustomerCommand<CustomerId, unit>
    val getById: CustomerCommand<CustomerId, Customer option>
    val getAll: CustomerQuery<unit>
