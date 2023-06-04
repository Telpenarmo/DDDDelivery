namespace DDDDelivery.Domain.Repositories

module SpecificationsFsharp =

    open System
    open System.Linq.Expressions
    open FSharp.Quotations

    open Microsoft.FSharp.Linq.RuntimeHelpers
    open Specification

    let private FuncExprToDelegateExpr<'src, 'target> (expr: Expr<'src -> 'target>) =
        let (v, body) =
            match expr with
            | ExprShape.ShapeLambda (v, body) -> (v, body)
            | _ -> failwith "Expected lambda expression"

        Expr.NewDelegate(
            Expression.GetFuncType [| typeof<'src>
                                      typeof<'target> |],
            [ v ],
            body
        )

    /// Project F# function expressions to Linq LambdaExpression nodes
    let private FuncExprToLinqFunc<'src, 'target> (expr: Expr<'src -> 'target>) =
        FuncExprToDelegateExpr<'src, 'target>(expr)
        |> LeafExpressionConverter.QuotationToExpression
        :?> Expression<Func<'src, 'target>>

    let Filtered<'a> = FuncExprToLinqFunc<'a, bool> >> Filtered

    let Filter (where: Expr<'a -> bool>) spec =
        let where = FuncExprToLinqFunc where
        Filter where spec

    let Include (includeExpr: Expr<'a -> obj>) spec =
        let includeExpr = FuncExprToLinqFunc includeExpr
        Include includeExpr spec

    let OrderBy (orderByExpr: Expr<'a -> IComparable>) spec =
        let orderByExpr = FuncExprToLinqFunc orderByExpr
        OrderBy orderByExpr spec
