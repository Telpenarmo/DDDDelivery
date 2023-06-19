using DDDDelivery.Domain;
using DDDDelivery.Domain.HelperTypes;

namespace DDDDelivery.Infrastructure.EF.Repositories.Dtos;

public class CustomerDto : IDto<Customer>
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public ContactInfoDto PrimaryContactInfo { get; set; } = null!;
    public ContactInfoDto? SecondaryContactInfo { get; set; }

    public void CopyFrom(Customer customer)
    {
        Id = customer.Id.Item;
        Name = customer.Name;
        PrimaryContactInfo = ContactInfoDto.From(customer.PrimaryContactInfo);
        SecondaryContactInfo = customer.SecondaryContactInfo is null ? ContactInfoDto.From(customer.SecondaryContactInfo!.Value) : null;
    }

    public Customer ToEntity()
    {
        return new Customer(
            CustomerId.NewCustomerId(Id),
            Name,
            ContactInfoDto.To(PrimaryContactInfo),
            SecondaryContactInfo is null ? null : ContactInfoDto.To(this.SecondaryContactInfo!)
        );
    }
}

public class ContactInfoDto
{
    public string Value { get; set; } = "";
    public int Type { get; set; }

    public static ContactInfo To(ContactInfoDto dto)
    {
        return dto.Type switch
        {
            ContactInfo.Tags.Email => ContactInfo.NewEmail(Email.createEmail(dto.Value).Value),
            ContactInfo.Tags.Phone => ContactInfo.NewPhone(Phone.NewPhone(dto.Value)),
            _ => throw new ArgumentOutOfRangeException($"Unknown contact info type: {dto.Type}")
        };
    }

    public static ContactInfoDto From(ContactInfo contactInfo)
    {
        return new ContactInfoDto
        {
            Type = contactInfo.Tag,
            Value = contactInfo switch
            {
                _ when contactInfo.IsEmail => ((ContactInfo.Email)contactInfo).Item.ToString(),
                _ when contactInfo.IsPhone => ((ContactInfo.Phone)contactInfo).Item.Item,
                _ => throw new ArgumentOutOfRangeException($"Unknown contact info type: {contactInfo.GetType()}")
            }
        };
    }
}