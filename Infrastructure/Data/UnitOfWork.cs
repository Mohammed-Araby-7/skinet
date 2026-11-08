using System.Collections.Concurrent;
using Core.Entities;
using Core.interfaces;

namespace Infrastructure.Data;

public class UnitOfWork(StoreContext context) : IUnitOfWork
{
    private readonly ConcurrentDictionary<string,object> _repos = new();
    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        context.Dispose();
    }

    public IGenericRepo<TEntity> Repo<TEntity>() where TEntity : BaseEntity
    {
        var type = typeof(TEntity).Name;

        return (IGenericRepo<TEntity>) _repos.GetOrAdd(type , t =>
        {
            var repoType = typeof(GenericRepo<>).MakeGenericType(typeof(TEntity));
            return Activator.CreateInstance(repoType,context)
            ?? throw new InvalidOperationException(
                $"Could not create repositary instance for {t}"
            );
        });
    }
}