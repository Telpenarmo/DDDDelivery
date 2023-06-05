namespace DDDDelivery.Domain.Repositories

open DDDDelivery.Domain

open System.Threading.Tasks

open Specification

type ICustomersRepository =
    abstract member Insert: Customer -> Task<bool>
    abstract member FindById: CustomerId -> Task<Customer option>
    abstract member FindSpecified: Specification<Customer> -> Task<seq<Customer>>
    abstract member Update: Customer -> Task<bool>
    abstract member Delete: CustomerId -> Task<bool>
