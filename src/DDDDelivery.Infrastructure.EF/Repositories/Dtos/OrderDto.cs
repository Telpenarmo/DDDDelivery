using DDDDelivery.Domain;

namespace DDDDelivery.Infrastructure.EF.Repositories.Dtos;

public class OrderDto
{
    public long Id { get; set; }
    public int Status { get; set; }
    public long CustomerId { get; set; }
    public IEnumerable<OrderLineDto> OrderLines { get; set; } = Enumerable.Empty<OrderLineDto>();
    public DateTime OrderedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string? CancellationReason { get; set; }
    public string? ShipmentId { get; set; }

    public static OrderDto From(Order order)
    {
        string? cancellationReason = order.Status switch
        {
            OrderStatus.T.CancelledByCustomer status => status.Item.Reason,
            OrderStatus.T.CancelledByStore status => status.Item.Reason,
            _ => null
        };
        return new OrderDto
        {
            Id = order.Id.Item,
            Status = order.Status.Tag,
            CustomerId = order.Customer.Item,
            OrderLines = order.OrderLines.Select(ol => new OrderLineDto
            {
                OrderId = order.Id.Item,
                ProductId = ol.Product.Item,
                Amount = ol.Amount,
                Worth = ol.Worth,
                Discount = ol.Discount
            }),
            OrderedAt = order.OrderedAt,
            ModifiedAt = order.ModifiedAt,
            CancellationReason = cancellationReason,
            ShipmentId = (order.Status as OrderStatus.T.Shipped)?.Item.Id ?? null
        };
    }

    public static Order To(OrderDto dto)
    {
        var status = dto.Status switch
        {
            OrderStatus.T.Tags.Pending => OrderStatus.T.Pending,
            OrderStatus.T.Tags.InPreparation => OrderStatus.T.InPreparation,
            OrderStatus.T.Tags.AwaitingShipment => OrderStatus.T.AwaitingShipment,
            OrderStatus.T.Tags.Shipped => OrderStatus.T.NewShipped(new ShipmentId(dto.ShipmentId!)),
            OrderStatus.T.Tags.Delivered => OrderStatus.T.Delivered,
            OrderStatus.T.Tags.CancelledByCustomer => OrderStatus.T.NewCancelledByCustomer(new CancellationReason(dto.CancellationReason!)),
            OrderStatus.T.Tags.CancelledByStore => OrderStatus.T.NewCancelledByStore(new CancellationReason(dto.CancellationReason!)),
            _ => throw new ArgumentOutOfRangeException($"Unknown order status: {dto.Status}")
        };
        return new Order(
            OrderId.NewOrderId(dto.Id),
            status,
            Domain.CustomerId.NewCustomerId(dto.CustomerId),
            dto.OrderLines.Select(ol => new OrderLine(
                ProductId.NewProductId(ol.ProductId),
                ol.Amount,
                ol.Worth,
                ol.Discount
            )),
            dto.OrderedAt,
            dto.ModifiedAt
        );
    }
}

public class OrderLineDto
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public long ProductId { get; set; }
    public ulong Amount { get; set; }
    public double Worth { get; set; }
    public double Discount { get; set; }
}