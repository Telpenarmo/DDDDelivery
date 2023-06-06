namespace DDDDelivery.Domain.Repositories

module SpecificationsFsharp =

    open System
    open System.Linq.Expressions
    open FSharp.Quotations

    open Microsoft.FSharp.Linq.RuntimeHelpers
    open Specification

    let private funcExprToDelegateExpr<'Src, 'Target> (expr: Expr<'Src -> 'Target>) =
        let (v, body) =
            match expr with
            | ExprShape.ShapeLambda (v, body) -> (v, body)
            | _ -> failwith "Expected lambda expression"

        Expr.NewDelegate(
            Expression.GetFuncType [| typeof<'Src>
                                      typeof<'Target> |],
            [ v ],
            body
        )

    /// Project F# function expressions to Linq LambdaExpression nodes
    let private funcExprToLinqFunc<'Src, 'Target> (expr: Expr<'Src -> 'Target>) =
        funcExprToDelegateExpr<'Src, 'Target>(expr)
        |> LeafExpressionConverter.QuotationToExpression
        :?> Expression<Func<'Src, 'Target>>

    let Filtered<'T> = funcExprToLinqFunc<'T, bool> >> Filtered

    let Filter (where: Expr<'T -> bool>) spec =
        let where = funcExprToLinqFunc where
        Filter where spec

    let Include (includeExpr: Expr<'T -> obj>) spec =
        let includeExpr = funcExprToLinqFunc includeExpr
        Include includeExpr spec

    let OrderBy (orderByExpr: Expr<'T -> IComparable>) spec =
        let orderByExpr = funcExprToLinqFunc orderByExpr
        OrderBy orderByExpr spec
