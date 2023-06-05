namespace DDDDelivery.Domain.Repositories

open DDDDelivery.Domain

open System.Threading.Tasks

open Specification

type IProductsRepository =
    abstract member Insert: Product.Product -> Task<bool>
    abstract member FindById: Product.ProductId -> Task<Product.Product option>
    abstract member FindSpecified: Specification<Product.Product> -> Task<seq<Product.Product>>
    abstract member Update: Product.Product -> Task<bool>
    abstract member Delete: Product.ProductId -> Task<bool>
    abstract member UpdateMany: seq<Product.Product> -> Task<bool>
