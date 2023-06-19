namespace DDDDelivery.Infrastructure.EF.Repositories.Dtos
{
    public interface IDto<TEntity>
    {
        long Id { get; set; }
        TEntity ToEntity();
        void CopyFrom(TEntity entity);
    }
}