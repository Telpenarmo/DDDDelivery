namespace DDDDelivery.Infrastructure.Repositories.InMemory

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories

type ProductsRepository =

    val mutable products: Map<Product.ProductId, Product.Product>

    new() = { products = Map.empty }

    interface IProductsRepository with
        member this.Insert(product: Product.Product) : Task<bool> =
            this.products <- this.products.Add(product.Id, product)
            Task.FromResult(true)

        member this.Delete(id: Product.ProductId) : Task<bool> =
            this.products <- this.products.Remove(id)
            Task.FromResult(true)

        member this.FindById(id: Product.ProductId) : Task<Product.Product option> =
            this.products |> Map.tryFind id |> Task.FromResult

        member this.FindSpecified(spec: Specification.Specification<Product.Product>) : Task<seq<Product.Product>> =
            SpecificationEvaluator.evaluate spec this.products.Values

        member this.Update(product: Product.Product) : Task<bool> =
            this.products <- this.products.Add(product.Id, product)
            Task.FromResult(true)

        member this.UpdateMany(items: seq<Product.Product>) : Task<bool> =
            this.products <-
                items
                |> Seq.fold (fun acc product -> acc.Add(product.Id, product)) this.products

            Task.FromResult(true)
