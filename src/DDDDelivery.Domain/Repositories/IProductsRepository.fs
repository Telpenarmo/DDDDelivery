namespace DDDDelivery.Domain.Repositories

open DDDDelivery.Domain

open System.Threading.Tasks

type IProductsRepository =
    abstract member Insert: Product -> Task<bool>
    abstract member FindById: ProductId -> Task<Product option>
    abstract member FindAll: unit -> Task<Product seq>
    abstract member Update: Product -> Task<bool>
    abstract member Delete: ProductId -> Task<bool>
    abstract member UpdateMany: seq<Product> -> Task<bool>
