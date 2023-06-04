namespace DDDDelivery.Domain.Repositories

type IUnitOfWork =
    abstract member SaveChanges: unit -> unit
    abstract member Orders: IOrdersRepository
    abstract member Customers: ICustomersRepository
    abstract member Products: IProductsRepository
