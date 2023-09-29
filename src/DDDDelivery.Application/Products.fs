namespace DDDDelivery.Application

open System.Threading.Tasks

open DDDDelivery.Domain
open DDDDelivery.Domain.Repositories

module Products =

    type ProductCommand<'Input, 'Output> = IUnitOfWork -> 'Input -> Task<'Output>
    type ProductQuery<'Input> = IUnitOfWork -> 'Input -> Task<Product seq>

    let create (uow: IUnitOfWork) (product: Product) =
        task {
            let! _ = uow.Products.Insert product
            do! uow.SaveChanges()
            return product
        }

    let update (uow: IUnitOfWork) (product: Product) =
        task {
            let! _ = uow.Products.Update product
            do! uow.SaveChanges()
            return product
        }

    let delete (uow: IUnitOfWork) productId =
        task {
            let! _ = uow.Products.Delete productId
            do! uow.SaveChanges()
            return ()
        }

    let getById (uow: IUnitOfWork) productId =
        task {
            match! uow.Products.FindById productId with
            | Some product -> return Some product
            | None -> return None
        }

    let getAll (uow: IUnitOfWork) = uow.Products.FindAll
