namespace DDDDelivery.Infrastructure.Repositories.InMemory

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories

type OrdersRepository() =
    inherit RepositoryBase<OrderId, Order>()

    interface IOrdersRepository with
        member this.Insert creationData =
            let id = OrderId(Map.count this.items |> int64)

            let order = Order.create id creationData.CustomerId creationData.OrderLines

            ``base``.Insert(id, order)

        member _.Delete(id: OrderId) : Task<bool> = base.Delete id

        member _.FindById(id: OrderId) : Task<Order option> = base.FindById id

        member _.FindAll() : Task<seq<Order>> = base.FindAll()

        member _.Update(order: Order) : Task<bool> = ``base``.Update(order.Id, order)
