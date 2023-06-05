namespace DDDDelivery.Domain.Repositories

open DDDDelivery.Domain

open System.Threading.Tasks

open Specification

type CreationData =
    { Id: Order.OrderId option
      CustomerId: Customer.CustomerId
      OrderLines: Order.OrderLine seq
      ExpectedDeliveryDays: int }

type IOrdersRepository =
    abstract member Insert: CreationData -> Task<Order.Order>
    abstract member FindById: Order.OrderId -> Task<Order.Order option>
    abstract member FindSpecified: Specification<Order.Order> -> Task<seq<Order.Order>>
    abstract member Update: Order.Order -> Task<bool>
    abstract member Delete: Order.OrderId -> Task<bool>
