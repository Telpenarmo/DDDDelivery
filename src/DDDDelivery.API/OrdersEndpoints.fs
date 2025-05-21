namespace DDDDelivery.API

open System

open Giraffe

module private Handlers =
    open DDDDelivery.Domain.Repositories
    open DDDDelivery.Application.Orders
    open DDDDelivery.Domain

    let accept (uow: IUnitOfWork) id =
        task {
            match! OrderAcceptance.accept uow (OrderId id, DateTime.Now) with
            | Ok order -> return serialize order
            | Error OrderAcceptance.OrderNotFound -> return RequestErrors.notFound (text "Order not found")
            | Error OrderAcceptance.OrderNotAcceptable ->
                return RequestErrors.badRequest (text "Order is not acceptable")
        }

    let prepare (uow: IUnitOfWork) id =
        task {
            match! OrderPreparation.prepare uow (OrderId id, DateTime.Now) with
            | Ok order -> return serialize order
            | Error OrderPreparation.OrderNotFound -> return RequestErrors.notFound (text "Order not found")
            | Error OrderPreparation.OrderNotPreparable ->
                return RequestErrors.badRequest (text "Order is not preparable")
        }

type OrdersEndpoints =
    | Create
    | Cancel
    | Accept
    | Prepare
    | Ship
    | Deliver
    | GetById
    | GetByProduct
    | GetByCustomer
    | GetAll

    static member Route = "orders"

    member this.Handle() : HttpHandler =
        match this with
        | Create -> POST >=> route "/create" >=> notImplemented ()

        | Cancel ->
            PUT
            >=> routef "/cancel/%d" (withUow Handlers.cancel)
        | Accept ->
            PUT
            >=> routef "/accept/%d" (withUow Handlers.accept)
        | Prepare ->
            PUT
            >=> routef "/prepare/%d" (withUow Handlers.prepare)
        | Ship ->
            POST
            >=> routeStartsWith "/ship"
            >=> notImplemented ()
        | Deliver ->
            POST
            >=> routeStartsWith "/deliver"
            >=> notImplemented ()
        | GetById ->
            GET
            >=> routef "/%d" (fun (_: int64) -> notImplemented ())
        | GetByProduct ->
            GET
            >=> routef "/product/%d" (fun (_: int64) -> notImplemented ())
        | GetByCustomer ->
            GET
            >=> routef "/customer/%d" (fun (_: int64) -> notImplemented ())
        | GetAll -> GET >=> route "/" >=> notImplemented ()
