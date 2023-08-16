namespace DDDDelivery.Domain.Repositories

open DDDDelivery.Domain

open System.Threading.Tasks

type CreationData =
    { Id: OrderId option
      CustomerId: CustomerId
      OrderLines: OrderLine seq }

type DaysSinceOrdering = int

type IOrdersRepository =
    abstract member Insert: CreationData -> Task<Order>
    abstract member FindById: OrderId -> Task<Order option>
    abstract member FindAll: unit -> Task<Order seq>
    abstract member FindOrdersWithProduct: ProductId -> Task<Order seq>
    abstract member FindOrdersFromCustomer: CustomerId -> Task<Order seq>
    abstract member FindStaleOrders: OrderStatus.T -> DaysSinceOrdering -> Task<Order seq>

    abstract member Update: Order -> Task<bool>
    abstract member Delete: OrderId -> Task<bool>
