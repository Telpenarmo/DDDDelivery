namespace DDDDelivery.Domain.Repositories

open DDDDelivery.Domain

open Specification

type IProductsRepository =
    abstract member Insert: Product.Product -> bool
    abstract member FindById: Product.ProductId -> Product.Product option
    abstract member FindAll: Specification<Product.Product> -> seq<Product.Product>
    abstract member Update: Product.Product -> bool
    abstract member Delete: Product.ProductId -> bool
