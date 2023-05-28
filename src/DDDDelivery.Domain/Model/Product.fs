namespace DDDDelivery.Domain

module Product =

    type ProductId = ProductId of int64

    type Product = {
        id: ProductId
        name: string
        decsription: string
        price: float
        availableUnits: float
    }