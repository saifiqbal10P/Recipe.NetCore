﻿using Recipe.NetCore.Attribute;
using Recipe.NetCore.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Recipe.NetCore.Base.Interface
{
    public interface IService
    {
        IUnitOfWork UnitOfWork { get; }
    }

    public interface IService<TDTO, TKey> : IService
    {
        [AuditOperation(OperationType.Read)]
        Task<TDTO> GetAsync(TKey id);

        [AuditOperation(OperationType.Read)]
        Task<int> GetCount();

        [AuditOperation(OperationType.Read)]
        Task<IList<TDTO>> GetAllAsync();

        [AuditOperation(OperationType.Read)]
        //Task<IList<TDTO>> GetAllAsync(JsonApiRequest request);

        [AuditOperation(OperationType.Create)]
        Task<TDTO> CreateAsync(TDTO dtoObject);

        [AuditOperation(OperationType.Update)]
        Task<TDTO> UpdateAsync(TDTO dtoObject);

        [AuditOperation(OperationType.Delete)]
        Task DeleteAsync(TKey id);

        [AuditOperation(OperationType.Create)]
        Task<IList<TDTO>> CreateAsync(IList<TDTO> dtoObjects);

        [AuditOperation(OperationType.Delete)]
        Task DeleteAsync(IList<TKey> ids);

        [AuditOperation(OperationType.Update)]
        Task<IList<TDTO>> UpdateAsync(IList<TDTO> dtoObjects);
    }

    public interface IService<TRepository, TEntity, TDTO, TKey> : IService<TDTO, TKey>
    {
        TRepository Repository { get; }
    }
}

