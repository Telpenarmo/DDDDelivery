using DDDDelivery.Domain;
using DDDDelivery.Domain.Repositories;
using DDDDelivery.Infrastructure.EF.Repositories.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;

namespace DDDDelivery.Infrastructure.EF.Repositories;

public class CustomersRepository : ICustomersRepository
{
    private readonly DDDDeliveryContext _context;

    public CustomersRepository(DDDDeliveryContext context)
    {
        _context = context;
    }

    public async Task<bool> Delete(CustomerId value)
    {
        var customer = await _context.Customers.FindAsync(value);
        if (customer == null)
            return false;
        _context.Customers.Remove(customer);
        return true;
    }

    public async Task<FSharpOption<Customer>> FindById(CustomerId id)
    {
        CustomerDto? customer = await _context.Customers.FindAsync(id.Item);
        return customer == null
            ? FSharpOption<Customer>.None
            : FSharpOption<Customer>.Some(customer.ToEntity());
    }

    public async Task<IEnumerable<Customer>> FindSpecified(Specification<Customer> specification)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Insert(Customer customer)
    {
        await _context.Customers.AddAsync(CustomerDto.From(customer));
        return true;
    }

    public async Task<bool> Update(Customer customer)
    {
        var tracked = _context.Customers.Local.(customer.Id);
        if (tracked == null)
            return false;
        _context.Entry(tracked).CurrentValues.SetValues(CustomerDto.From(customer));
        return true;
    }
}

