namespace DDDDelivery.Domain.Repositories

open DDDDelivery.Domain

open Specification

type ICustomersRepository =
    abstract member Insert: Customer.Customer -> bool
    abstract member FindById: Customer.CustomerId -> Customer.Customer option
    abstract member FindAll: Specification<Customer.Customer> -> seq<Customer.Customer>
    abstract member Update: Customer.Customer -> bool
    abstract member Delete: Customer.CustomerId -> bool
