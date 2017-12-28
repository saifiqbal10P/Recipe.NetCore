using Recipe.NetCore.Base.Abstract;
using Recipe.NetCore.Base.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Recipe.NetCore.Base.Generic
{
    public class Service : IService
    {
        public IUnitOfWork UnitOfWork { get; private set; }

        public Service(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }
    }

    public class Service<TRepository, TEntity, TDTO, TKey> : Service, IService<TRepository, TEntity, TDTO, TKey>
     where TEntity : IAuditModel<TKey>, new()
     where TDTO : DTO<TEntity, TKey>, new()
     where TRepository : IRepository<TEntity, TKey>
     where TKey : IEquatable<TKey>
    {
        private TRepository _repository;

        public TRepository Repository
        {
            get
            {
                return this._repository;
            }
        }

        public Service(IUnitOfWork unitOfWork, TRepository repository)
            : base(unitOfWork)
        {
            this._repository = repository;
        }

        protected TEntity Create(TDTO dtoObject)
        {
            TEntity entity = dtoObject.ConvertToEntity();
            entity.CreatedBy = dtoObject.CreatedBy;
            entity.CreatedOn = DateTime.UtcNow;
            return this._repository.Create(entity);
        }

        public virtual async Task<TDTO> CreateAsync(TDTO dtoObject)
        {
            var result = this.Create(dtoObject);
            await this.UnitOfWork.SaveAsync();

            dtoObject.ConvertFromEntity(result);
            return dtoObject;
        }

        public virtual async Task<IList<TDTO>> CreateAsync(IList<TDTO> dtoObjects)
        {
            List<TEntity> results = new List<TEntity>();
            foreach (TDTO dtoObject in dtoObjects)
            {
                results.Add(this.Create(dtoObject));
            }
            this.UnitOfWork.DBContext.ChangeTracker.AutoDetectChangesEnabled = false;
            await this.UnitOfWork.SaveAsync();
            this.UnitOfWork.DBContext.ChangeTracker.AutoDetectChangesEnabled = true;

            return DTO<TEntity, TKey>.ConvertEntityListToDTOList<TDTO>(results);
        }

        protected async Task Delete(TKey id)
        {
            await this._repository.DeleteAsync(id);
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            await this.Delete(id);
            await this.UnitOfWork.SaveAsync();
        }

        public virtual async Task DeleteAsync(IList<TKey> ids)
        {
            foreach (TKey id in ids)
            {
                await this.Delete(id);
            }

            this.UnitOfWork.DBContext.ChangeTracker.AutoDetectChangesEnabled = false;
            await this.UnitOfWork.SaveAsync();
            this.UnitOfWork.DBContext.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        public virtual async Task<int> GetCount()
        {
            return await this._repository.GetCount();
        }

        public virtual async Task<IList<TDTO>> GetAllAsync()
        {
            IEnumerable<TEntity> entity = await this._repository.GetAll();
            return DTO<TEntity, TKey>.ConvertEntityListToDTOList<TDTO>(entity);
        }

        public virtual async Task<TDTO> GetAsync(TKey id)
        {
            TEntity entity = await this._repository.GetAsync(id);
            if (entity == null)
                return null;
            TDTO dto = new TDTO();
            dto.ConvertFromEntity(entity);
            return dto;
        }

        protected async Task<TEntity> Update(TDTO dtoObject)
        {
            var dbEntity = await this._repository.GetAsync(dtoObject.Id);
            var entity = dtoObject.ConvertToEntity(dbEntity);
            entity.LastModifiedBy = dtoObject.ModifiedBy;
            entity.LastModifiedOn = DateTime.UtcNow;
            return this._repository.Update(entity);
        }

        public virtual async Task<TDTO> UpdateAsync(TDTO dtoObject)
        {
            var result = await this.Update(dtoObject);
            await this.UnitOfWork.SaveAsync();
            dtoObject.ConvertFromEntity(result);
            return dtoObject;
        }

        public virtual async Task<IList<TDTO>> UpdateAsync(IList<TDTO> dtoObjects)
        {
            List<TEntity> results = new List<TEntity>();
            foreach (TDTO dtoObject in dtoObjects)
            {
                results.Add(await this.Update(dtoObject));
            }

            this.UnitOfWork.DBContext.ChangeTracker.AutoDetectChangesEnabled = false;
            await this.UnitOfWork.SaveAsync();
            this.UnitOfWork.DBContext.ChangeTracker.AutoDetectChangesEnabled = true;

            return DTO<TEntity, TKey>.ConvertEntityListToDTOList<TDTO>(results);
        }

        public virtual async Task<IList<TEntity>> UpdateAsync(IList<TEntity> entityObjects)
        {
            foreach (var entityObject in entityObjects)
            {
                this._repository.Update(entityObject);
            }

            this.UnitOfWork.DBContext.ChangeTracker.AutoDetectChangesEnabled = false;
            await this.UnitOfWork.SaveAsync();
            this.UnitOfWork.DBContext.ChangeTracker.AutoDetectChangesEnabled = true;

            return entityObjects;
        }

        protected async Task<TEntity> SoftDelete(TDTO dtoObject)
        {
            var dbEntity = await this._repository.GetAsync(dtoObject.Id);
            var entity = dtoObject.ConvertToEntity(dbEntity);
            entity.LastModifiedBy = dtoObject.ModifiedBy;
            entity.LastModifiedOn = DateTime.UtcNow;
            entity.IsDeleted = true;
            return this._repository.Update(entity);           
        }      
    }
}
