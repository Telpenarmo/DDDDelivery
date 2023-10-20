namespace DDDDelivery.API

open Giraffe

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
        | Create ->
            POST
            >=> route "/create"
            >=> notImplemented ()

        | Cancel ->
            PUT
            >=> routeStartsWith "/cancel"
            >=> notImplemented ()
        | Accept ->
            PUT
            >=> routeStartsWith "/accept"
            >=> notImplemented ()
        | Prepare ->
            PUT
            >=> routeStartsWith "/prepare"
            >=> notImplemented ()
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
