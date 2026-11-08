using Core.Entities;

namespace Core.interfaces;

public interface IUnitOfWork:IDisposable
{
    
    IGenericRepo<TEntity> Repo<TEntity>() where TEntity : BaseEntity;

    Task<bool> Complete();
}