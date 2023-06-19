namespace DDDDelivery.Infrastructure.Repositories.InMemory

open System
open System.Linq.Expressions
open System.Threading.Tasks

open DDDDelivery.Domain.Repositories

module SpecificationEvaluator =
    let evaluate<'Entity> (spec: Specification<'Entity>) seq =
        let compile (f: Expression<Func<'Entity, 'To>>) = f.Compile() |> FuncConvert.FromFunc

        let where = compile spec.Where

        let notOrdered =
            seq
            |> Seq.filter where

        let notPaginated =
            spec.OrderBy
            |> Seq.map compile
            |> Seq.fold (fun seq projection -> seq |> Seq.sortBy projection) notOrdered
            |> Seq.skip (int spec.Skip)

        let res =
            match spec.Take with
            | None -> notPaginated
            | Some take -> notPaginated |> Seq.truncate (int take)

        Task.FromResult res
