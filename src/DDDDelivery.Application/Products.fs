namespace DDDDelivery.Application

open System

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories

module ProductsSpecifications =
    let baseSpec = Specification.Specification<Product.Product>.Zero


module Products =

    let createProduct (uow: IUnitOfWork) product =
        task {
            let! _ = uow.Products.Insert product
            do! uow.SaveChanges()
            return Ok product
        }

    let updateProduct (uow: IUnitOfWork) product =
        task {
            let! _ = uow.Products.Update product
            do! uow.SaveChanges()
            return Ok product
        }

    let deleteProduct (uow: IUnitOfWork) productId =
        task {
            let! _ = uow.Products.Delete productId
            do! uow.SaveChanges()
            return Ok()
        }

    let getProduct (uow: IUnitOfWork) productId =
        task {
            match! uow.Products.FindById productId with
            | Some product -> return Ok product
            | None -> return Error(sprintf "Product with id %A not found" productId)
        }

    let getProducts (uow: IUnitOfWork) =
        uow.Products.FindSpecified(ProductsSpecifications.baseSpec ())
