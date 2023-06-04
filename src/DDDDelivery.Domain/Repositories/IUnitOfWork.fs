namespace DDDDelivery.Domain.Repositories
open System.Threading.Tasks

type IUnitOfWork =
    abstract member SaveChanges: unit -> Task
    abstract member Orders: IOrdersRepository
    abstract member Customers: ICustomersRepository
    abstract member Products: IProductsRepository
