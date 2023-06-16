namespace DDDDelivery.Domain.Repositories

open DDDDelivery.Domain

open System.Threading.Tasks

open Specification

type CreationData =
    { Id: OrderId option
      CustomerId: CustomerId
      OrderLines: OrderLine seq }

type IOrdersRepository =
    abstract member Insert: CreationData -> Task<Order>
    abstract member FindById: OrderId -> Task<Order option>
    abstract member FindSpecified: Specification<Order> -> Task<seq<Order>>
    abstract member Update: Order -> Task<bool>
    abstract member Delete: OrderId -> Task<bool>
