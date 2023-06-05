namespace DDDDelivery.Infrastructure.Repositories.InMemory

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories

type ProductsRepository =

    val mutable products: Map<ProductId, Product>

    new() = { products = Map.empty }

    interface IProductsRepository with
        member this.Insert(product: Product) : Task<bool> =
            this.products <- this.products.Add(product.Id, product)
            Task.FromResult(true)

        member this.Delete(id: ProductId) : Task<bool> =
            this.products <- this.products.Remove(id)
            Task.FromResult(true)

        member this.FindById(id: ProductId) : Task<Product option> =
            this.products |> Map.tryFind id |> Task.FromResult

        member this.FindSpecified(spec: Specification.Specification<Product>) : Task<seq<Product>> =
            SpecificationEvaluator.evaluate spec this.products.Values

        member this.Update(product: Product) : Task<bool> =
            this.products <- this.products.Add(product.Id, product)
            Task.FromResult(true)

        member this.UpdateMany(items: seq<Product>) : Task<bool> =
            this.products <-
                items
                |> Seq.fold (fun acc product -> acc.Add(product.Id, product)) this.products

            Task.FromResult(true)
