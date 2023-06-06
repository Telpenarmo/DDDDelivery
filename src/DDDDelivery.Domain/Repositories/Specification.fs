namespace DDDDelivery.Domain.Repositories

open System
open System.Linq.Expressions

type private FuncExpression<'From, 'To> = Expression<Func<'From, 'To>>

type Specification<'T> =
    { Where: FuncExpression<'T, bool>
      Includes: FuncExpression<'T, obj> seq
      IncludeStrings: string seq
      OrderBy: FuncExpression<'T, IComparable> seq
      Skip: uint64
      Take: uint64 option }

    static member Zero() =
        { Where =
            let trueExpr = Expression.Constant(true)
            let param = Expression.Parameter(typeof<'T>)
            Expression.Lambda<Func<'T, bool>>(trueExpr, [| param |])
          Includes = Seq.empty
          IncludeStrings = Seq.empty
          OrderBy = Seq.empty
          Skip = 0UL
          Take = None }

module Specification =

    let Filtered where =
        { Where = where
          Includes = Seq.empty
          IncludeStrings = Seq.empty
          OrderBy = Seq.empty
          Skip = 0UL
          Take = None }

    let Filter (where: FuncExpression<'T, bool>) spec =
        { spec with Where = Expression.Lambda<_>(Expression.AndAlso(spec.Where.Body, where.Body), spec.Where.Parameters) }

    let Include includeExpr spec =
        { spec with Includes = spec.Includes |> Seq.append [ includeExpr ] }

    let IncludeString includeString spec =
        { spec with
            IncludeStrings =
                spec.IncludeStrings
                |> Seq.append [ includeString ] }

    let OrderBy orderByExpr spec =
        { spec with OrderBy = spec.OrderBy |> Seq.append [ orderByExpr ] }

    let Skip skip spec = { spec with Skip = skip }

    let Take take spec = { spec with Take = Some take }

    let Page (skip, take) spec =
        { spec with
            Skip = skip
            Take = Some take }
