namespace DDDDelivery.Infrastructure.Repositories.InMemory

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories
open DDDDelivery.Infrastructure.Repositories.InMemory

type OrdersRepository =
    val mutable orders: Map<Order.OrderId, Order.Order>

    new() = { orders = Map.empty }

    interface IOrdersRepository with
        member this.Insert creationData =
            let id = Order.OrderId(Map.count this.orders |> int64)

            let order =
                Order.create id creationData.CustomerId creationData.OrderLines creationData.ExpectedDeliveryDays

            this.orders <- this.orders.Add(order.Id, order)
            order |> Task.FromResult

        member this.Delete(id: Order.OrderId) : Task<bool> =
            this.orders <- this.orders.Remove(id)
            Task.FromResult(true)

        member this.FindById(id: Order.OrderId) : Task<Order.Order option> =
            this.orders |> Map.tryFind id |> Task.FromResult

        member this.FindSpecified(spec: Specification.Specification<Order.Order>) : Task<seq<Order.Order>> =
            SpecificationEvaluator.evaluate spec this.orders.Values

        member this.Update(order: Order.Order) : Task<bool> =
            this.orders <- this.orders.Add(order.Id, order)
            Task.FromResult(true)
