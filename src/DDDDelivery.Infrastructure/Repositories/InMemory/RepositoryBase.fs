namespace DDDDelivery.Infrastructure.Repositories.InMemory

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories

[<AbstractClass>]
type RepositoryBase<'Id, 'Entity> when 'Id: comparison =
    val mutable items: Map<'Id, 'Entity>

    new() = { items = Map.empty }

    member this.Insert(id: 'Id, item: 'Entity) : Task<'Entity> =
        this.items <- this.items.Add(id, item)
        item |> Task.FromResult
    
    member this.Delete(id: 'Id) : Task<bool> =
        this.items <- this.items.Remove(id)
        Task.FromResult(true)

    member this.FindById(id: 'Id) : Task<'Entity option> =
        this.items |> Map.tryFind id |> Task.FromResult

    member this.FindAll() : Task<seq<'Entity>> = Task.FromResult <| upcast this.items.Values
    
    member this.Update(id: 'Id, item: 'Entity) : Task<bool> =
        this.items <- this.items.Add(id, item)
        Task.FromResult(true)