namespace DDDDelivery.Domain.Repositories

open DDDDelivery.Domain

open System.Threading.Tasks

open Specification

type ICustomersRepository =
    abstract member Insert: Customer.Customer -> Task<bool>
    abstract member FindById: Customer.CustomerId -> Task<Customer.Customer option>
    abstract member FindAll: Specification<Customer.Customer> -> Task<seq<Customer.Customer>>
    abstract member Update: Customer.Customer -> Task<bool>
    abstract member Delete: Customer.CustomerId -> Task<bool>
