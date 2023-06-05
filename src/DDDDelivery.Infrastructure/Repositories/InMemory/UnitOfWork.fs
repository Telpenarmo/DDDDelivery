namespace DDDDelivery.Infrastructure.Repositories.InMemory

open DDDDelivery.Domain.Repositories

type UnitOfWork() =

    let ordersRepository = OrdersRepository()
    let productsRepository = ProductsRepository()
    let customersRepository = CustomersRepository()

    interface IUnitOfWork with
        member _.Customers: ICustomersRepository = customersRepository

        member _.Orders: IOrdersRepository = ordersRepository
        member _.Products: IProductsRepository = productsRepository

        member _.SaveChanges() : System.Threading.Tasks.Task =
            System.Threading.Tasks.Task.FromResult()
