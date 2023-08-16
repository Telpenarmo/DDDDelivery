namespace DDDDelivery.Domain.Repositories

open DDDDelivery.Domain

open System.Threading.Tasks

type ICustomersRepository =
    abstract member Insert: Customer -> Task<bool>
    abstract member FindById: CustomerId -> Task<Customer option>
    abstract member FindAll: unit -> Task<Customer seq>
    abstract member Update: Customer -> Task<bool>
    abstract member Delete: CustomerId -> Task<bool>
