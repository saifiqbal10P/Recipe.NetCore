using Recipe.NetCore.Attribute;
using Recipe.NetCore.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Recipe.NetCore.Base.Interface
{
    public interface IRepository
    {
    }

    public interface IRepository<TEntity, TKey> : IRepository
    {
        [AuditOperation(OperationType.Read)]
        Task<TEntity> GetAsync(TKey id);

        [AuditOperation(OperationType.Read)]
        Task<IEnumerable<TEntity>> GetAsync(IList<TKey> ids);

        [AuditOperation(OperationType.Read)]
        Task<TEntity> GetEntityOnly(TKey id);

        [AuditOperation(OperationType.Read)]
        Task<int> GetCount();

        [AuditOperation(OperationType.Read)]
        Task<IEnumerable<TEntity>> GetAll();

        [AuditOperation(OperationType.Read)]
        //Task<IEnumerable<TEntity>> GetAll(JsonApiRequest request);

        [AuditOperation(OperationType.Create)]
        TEntity Create(TEntity entity);

        [AuditOperation(OperationType.Update)]
        TEntity Update(TEntity entity);

        [AuditOperation(OperationType.Delete)]
        Task DeleteAsync(TKey id);

        Task<Tuple<int, IEnumerable<TEntity>>> GetPagedResultAsync(Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          int? page = null,
          int? pageSize = null);
    }
}
