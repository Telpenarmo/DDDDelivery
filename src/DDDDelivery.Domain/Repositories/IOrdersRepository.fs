namespace DDDDelivery.Domain.Repositories

open DDDDelivery.Domain

open System.Threading.Tasks

type CreationData =
    { Id: OrderId option
      CustomerId: CustomerId
      OrderLines: OrderLine seq }

type IOrdersRepository =
    abstract member Insert: CreationData -> Task<Order>
    abstract member FindById: OrderId -> Task<Order option>
    abstract member FindAll: unit -> Task<Order seq>
    abstract member Update: Order -> Task<bool>
    abstract member Delete: OrderId -> Task<bool>
