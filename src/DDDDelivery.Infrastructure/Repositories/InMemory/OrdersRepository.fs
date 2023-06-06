namespace DDDDelivery.Infrastructure.Repositories.InMemory

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories
open DDDDelivery.Infrastructure.Repositories.InMemory

type OrdersRepository =
    val mutable orders: Map<OrderId, Order>

    new() = { orders = Map.empty }

    interface IOrdersRepository with
        member this.Insert creationData =
            let id = OrderId(Map.count this.orders |> int64)

            let order =
                Order.create id creationData.CustomerId creationData.OrderLines creationData.ExpectedDeliveryDays

            this.orders <- this.orders.Add(order.Id, order)
            order |> Task.FromResult

        member this.Delete(id: OrderId) : Task<bool> =
            this.orders <- this.orders.Remove(id)
            Task.FromResult(true)

        member this.FindById(id: OrderId) : Task<Order option> =
            this.orders |> Map.tryFind id |> Task.FromResult

        member this.FindSpecified(spec: Specification<Order>) : Task<seq<Order>> =
            SpecificationEvaluator.evaluate spec this.orders.Values

        member this.Update(order: Order) : Task<bool> =
            this.orders <- this.orders.Add(order.Id, order)
            Task.FromResult(true)
