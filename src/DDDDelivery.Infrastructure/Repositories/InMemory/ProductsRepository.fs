namespace DDDDelivery.Infrastructure.Repositories.InMemory

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories
open DDDDelivery.Infrastructure.Repositories.InMemory

type ProductsRepository() =

    inherit RepositoryBase<ProductId, Product>()

    interface IProductsRepository with

        member this.Insert product =
            let id = ProductId(Map.count this.items |> int64)

            ``base``.Insert(id, product) |> ignore

            Task.FromResult(true)

        member _.Delete(id: ProductId) : Task<bool> = base.Delete id
        member _.FindById(id: ProductId) : Task<Product option> = base.FindById id
        member _.FindSpecified(spec: Specification<Product>) : Task<seq<Product>> = base.FindSpecified spec
        member _.Update(product: Product) : Task<bool> = ``base``.Update(product.Id, product)

        member this.UpdateMany(products: seq<Product>) : Task<bool> =
            products
            |> Seq.map (fun p -> this.Update(p.Id, p))
            |> ignore

            Task.FromResult(true)
