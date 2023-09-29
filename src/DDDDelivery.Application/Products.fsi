namespace DDDDelivery.Application

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories

module Products =
    
    type ProductCommand<'Input, 'Output> = IUnitOfWork -> 'Input -> Task<'Output>
    type ProductQuery<'Input> = IUnitOfWork -> 'Input -> Task<Product seq>

    val create: ProductCommand<Product, Product>
    val update: ProductCommand<Product, Product>
    val delete: ProductCommand<ProductId, unit>
    val getById: ProductCommand<ProductId, Product option>
    val getAll: ProductQuery<unit>
