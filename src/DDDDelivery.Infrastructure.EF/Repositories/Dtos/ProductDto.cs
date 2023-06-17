using DDDDelivery.Domain;

namespace DDDDelivery.Infrastructure.EF.Repositories.Dtos;

public class ProductDto
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public double Price { get; set; }
    public ulong? AvailableAmount { get; set; }
    public int Availability { get; set; }

    public static ProductDto From(Product product)
    {
        return new ProductDto
        {
            Id = product.Id.Item,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            AvailableAmount = ((Availability.Available)product.AvailableUnits)?.Item,
            Availability = product.AvailableUnits!.Tag
        };
    }

    public static Product To(ProductDto dto)
    {
        return new Product(
            ProductId.NewProductId(dto.Id),
            dto.Name,
            dto.Description,
            dto.Price,
            dto.Availability switch
            {
                Domain.Availability.Tags.Available => Domain.Availability.NewAvailable(dto.AvailableAmount ?? 0),
                Domain.Availability.Tags.Infinite => Domain.Availability.Infinite,
                Domain.Availability.Tags.Unavailable => Domain.Availability.Unavailable,
                _ => throw new ArgumentOutOfRangeException($"Unknown availability: {dto.Availability}")
            }
        );
    }
}