namespace DDDDelivery.API

module Task =
    open System.Threading.Tasks

    let map (f: 'a -> 'b) (this: Task<'a>) : Task<'b> =
        task {
            let! result = this
            return f result
        }

[<AutoOpen>]
module Common =
    open Microsoft.AspNetCore.Http
    open Giraffe
    open DDDDelivery.Domain.Repositories

    let withUow (f: IUnitOfWork -> 'a -> 'b) =
        fun arg (next: HttpFunc) (ctx: HttpContext) ->
            let uow = ctx.GetService<IUnitOfWork>()

            task {
                let! continuation = f uow arg
                return! continuation next ctx
            }

    let notImplemented () =
        ServerErrors.notImplemented (text "Not implemented")

    let serialize: 'a -> HttpHandler = json

    let inline createEndpointsHandler<
        'TEndpoint
            when 'TEndpoint: (member Handle: unit -> HttpHandler)
            and 'TEndpoint: (static member Route: string)> () =

        let handleCase (case: Reflection.UnionCaseInfo) =
            (Reflection.FSharpValue.MakeUnion(case, [||]) :?> 'TEndpoint)
                .Handle()

        Reflection.FSharpType.GetUnionCases(typeof<'TEndpoint>)
        |> Array.where (fun case -> case.GetFields().Length = 0)
        |> Array.map handleCase
        |> Array.toList
        |> choose
        |> subRoute ("/" + 'TEndpoint.Route)
