namespace DDDDelivery.Application

open System
open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories

module Orders =

    type OrderCommand<'Input, 'Output> = IUnitOfWork -> 'Input -> Task<'Output>
    type OrderQuery<'Input> = IUnitOfWork -> 'Input -> Task<Order seq>

    module OrderCreation =

        type CreationError =
            | CustomerNotFound of CustomerId
            | NotEnoughProductsAvailable of ProductId seq

        val create: OrderCommand<OrderForm, Result<Order, CreationError>>

    module OrderCancellation =

        type CancellationError =
            | OrderNotFound
            | OrderNotCancellable

        type CancellationInput = (OrderId * string * DateTime)

        val cancelByCustomer: OrderCommand<CancellationInput, Result<Order, CancellationError>>

        val cancelBySeller: OrderCommand<CancellationInput, Result<Order, CancellationError>>

    module OrderAcceptance =

        type AcceptanceError =
            | OrderNotFound
            | OrderNotAcceptable

        val accept: OrderCommand<(OrderId * DateTime), Result<Order, AcceptanceError>>

    module OrderPreparation =

        type PreparationError =
            | OrderNotFound
            | OrderNotPreparable

        val prepare: OrderCommand<(OrderId * DateTime), Result<Order, PreparationError>>

    module OrderShipment =

        type ShipmentError =
            | OrderNotFound
            | OrderNotShippable

        type ShipmentInput = (ShipmentId * OrderId * DateTime)

        val ship: OrderCommand<ShipmentInput, Result<Order, ShipmentError>>

    module OrderDelivery =

        type DeliveryError =
            | OrderNotFound
            | OrderNotDeliverable

        val deliver: OrderCommand<(OrderId * DateTime), Result<Order, DeliveryError>>

    module OrdersFetching =

        val All: OrderQuery<unit>
        val WithProduct: OrderQuery<ProductId>
        val WithCustomer: OrderQuery<CustomerId>
        val PendingForThreeDays: OrderQuery<unit>
        val ProcessingForFiveDays: OrderQuery<unit>
        val AwaitingShipmentForTwoDays: OrderQuery<unit>
