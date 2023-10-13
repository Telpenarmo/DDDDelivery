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
            let product = Product(id, product.Name, product.Description, product.Price, product.AvailableUnits)

            ``base``.Insert(id, product) |> ignore

            Task.FromResult(true)

        member _.Delete(id: ProductId) : Task<bool> = base.Delete id

        member _.FindById(id: ProductId) : Task<Product option> = base.FindById id

        member _.FindAll() : Task<seq<Product>> = base.FindAll()

        member this.FindMany(ids: seq<ProductId>) : Task<seq<Product>> =
            this.items.Values
            |> Seq.filter (fun p -> ids |> Seq.exists (fun arg -> arg = p.Id))
            |> Task.FromResult

        member _.Update(product: Product) : Task<bool> = ``base``.Update(product.Id, product)

        member this.UpdateMany(products: seq<Product>) : Task<bool> =
            products
            |> Seq.map (fun p -> this.Update(p.Id, p))
            |> ignore

            Task.FromResult(true)
