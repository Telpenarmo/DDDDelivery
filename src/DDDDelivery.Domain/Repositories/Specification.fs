namespace DDDDelivery.Domain.Repositories

open System
open System.Linq.Expressions

module Specification =

    type FuncExpression<'From, 'To> = Expression<Func<'From, 'To>>

    type Specification<'T> =
        { Where: FuncExpression<'T, bool>
          Includes: FuncExpression<'T, obj> seq
          IncludeStrings: string seq
          OrderBy: FuncExpression<'T, IComparable> seq
          Skip: uint64
          Take: uint64 option }
