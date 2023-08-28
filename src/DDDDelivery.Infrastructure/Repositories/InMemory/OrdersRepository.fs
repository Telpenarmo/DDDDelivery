namespace DDDDelivery.Infrastructure.Repositories.InMemory

open System
open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories

type OrdersRepository() =
    inherit RepositoryBase<OrderId, Order>()

    interface IOrdersRepository with
        member this.Insert form =
            let id = OrderId(Map.count this.items |> int64)

            let order = Order.create form (fun _ -> id)

            ``base``.Insert(id, order)

        member _.Delete(id: OrderId) : Task<bool> = base.Delete id

        member _.FindById(id: OrderId) : Task<Order option> = base.FindById id

        member this.FindOrdersFromCustomer(customerId: CustomerId) : Task<seq<Order>> =
            this.items.Values
            |> Seq.filter (fun o -> o.Customer = customerId)
            |> Task.FromResult

        member this.FindOrdersWithProduct(productId: ProductId) : Task<seq<Order>> =
            this.items.Values
            |> Seq.filter (fun o ->
                o.OrderLines
                |> Seq.exists (fun ol -> ol.Product = productId))
            |> Task.FromResult

        member this.FindStaleOrders (status: OrderStatus.T) (days: DaysSinceOrdering) : Task<seq<Order>> =
            this.items.Values
            |> Seq.filter (fun o ->
                o.Status = status
                && (DateTime.Now - o.OrderedAt).TotalDays > days)
            |> Task.FromResult

        member _.FindAll() : Task<seq<Order>> = base.FindAll()

        member _.Update(order: Order) : Task<bool> = ``base``.Update(order.Id, order)
