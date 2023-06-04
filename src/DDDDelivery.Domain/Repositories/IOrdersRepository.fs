namespace DDDDelivery.Domain.Repositories

open DDDDelivery.Domain

open Specification

type CreationData =
    { Id: Order.OrderId option
      CustomerId: Customer.CustomerId
      OrderLines: Order.OrderLine seq
      ExpectedDeliveryDays: int }

type IOrdersRepository =
    abstract member Insert: CreationData -> Order.Order
    abstract member FindById: Order.OrderId -> Order.Order option
    abstract member FindAll: Specification<Order.Order> -> seq<Order.Order>
    abstract member Update: Order.Order -> bool
    abstract member Delete: Order.OrderId -> bool
