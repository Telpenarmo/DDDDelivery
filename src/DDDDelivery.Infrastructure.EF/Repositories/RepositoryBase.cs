using DDDDelivery.Domain.Repositories;
using DDDDelivery.Infrastructure.EF.Repositories.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DDDDelivery.Infrastructure.EF.Repositories;

public abstract class RepositoryBase<TEntity, TDto> where TDto : class, IDto<TEntity> where TEntity : class
{
    private readonly DDDDeliveryContext _context;
    private readonly Func<TEntity, TDto> _toDto;

    protected RepositoryBase(DDDDeliveryContext context, Func<TEntity, TDto> toDto)
    {
        _context = context;
        _toDto = toDto;
    }

    protected async Task<TEntity?> FindByIdAsync(long id)
    {
        TDto? dto = await _context.Set<TDto>().FindAsync(id);
        return dto?.ToEntity();
    }

    protected async Task<IEnumerable<TEntity>> FindSpecifiedAsync(Specification<TEntity> specification)
    {
        var result = _context.Set<TDto>().Where(specification.Where);
        foreach (var order in specification.OrderBy)
        {
            result = result.OrderBy(order);
        }
        result = result.Skip((int)specification.Skip);
        if (specification.Take != null)
            result = result.Take((int)specification.Take.Value);

        return await result.ToListAsync();
    }

    protected async Task<bool> InsertAsync(TEntity entity)
    {
        _context.Set<TDto>().Add(_toDto(entity));
        await _context.SaveChangesAsync();
        return true;
    }

    protected async Task<bool> UpdateAsync(TDto dto)
    {
        TDto? actual = await _context.Set<TDto>().FindAsync(dto.Id);
        if (actual == null)
            return false;
        _context.Entry(actual).CurrentValues.SetValues(dto);
        await _context.SaveChangesAsync();
        return true;
    }

    protected async Task<bool> DeleteAsync(long id)
    {
        TDto? actual = await _context.Set<TDto>().FindAsync(id);
        if (actual == null)
            return false;
        _context.Set<TDto>().Remove(actual);
        await _context.SaveChangesAsync();
        return true;
    }
}