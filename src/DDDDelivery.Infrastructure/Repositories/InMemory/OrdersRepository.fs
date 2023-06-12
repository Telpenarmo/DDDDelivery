namespace DDDDelivery.Infrastructure.Repositories.InMemory

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories
open DDDDelivery.Infrastructure.Repositories.InMemory

type OrdersRepository() =
    inherit RepositoryBase<OrderId, Order>()

    interface IOrdersRepository with
        member this.Insert creationData =
            let id = OrderId(Map.count this.items |> int64)

            let order =
                Order.create id creationData.CustomerId creationData.OrderLines creationData.ExpectedDeliveryDays

            ``base``.Insert(id, order)

        member _.Delete(id: OrderId) : Task<bool> = base.Delete id

        member _.FindById(id: OrderId) : Task<Order option> = base.FindById id

        member _.FindSpecified(spec: Specification<Order>) : Task<seq<Order>> = base.FindSpecified spec

        member _.Update(order: Order) : Task<bool> = ``base``.Update(order.Id, order)
