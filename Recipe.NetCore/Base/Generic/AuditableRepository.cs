using Recipe.NetCore.Base.Interface;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.NetCore.Base.Generic
{
    public class AuditableRepository<TEntity, TKey> : Repository<TEntity, TKey>
            where TEntity : class, IAuditModel<TKey>
            where TKey : IEquatable<TKey>
    {
        public AuditableRepository(IRequestInfo requestInfo)
            : base(requestInfo)
        {
        }

        protected override IQueryable<TEntity> DefaultListQuery
        {
            get
            {
                return base.DefaultListQuery.Where(x => !x.IsDeleted);
            }
        }

        protected override IQueryable<TEntity> DefaultSingleQuery
        {
            get
            {
                return base.DefaultSingleQuery.Where(x => !x.IsDeleted);
            }
        }

        public override TEntity Create(TEntity entity)
        {
            entity.CreatedBy = this.RequestInfo.UserId;
            entity.CreatedOn = DateTime.UtcNow;
            entity.LastModifiedBy = this.RequestInfo.UserId;
            entity.LastModifiedOn = DateTime.UtcNow;
            entity.IsDeleted = false;
            return base.Create(entity);
        }

        public override TEntity Update(TEntity entity)
        {
            entity.LastModifiedOn = DateTime.UtcNow;
            entity.LastModifiedBy = this.RequestInfo.UserId;
            return base.Update(entity);
        }

        public override async Task DeleteAsync(TKey id)
        {
            var entity = await this.GetAsync(id);
            if (entity != null)
            {
                entity.LastModifiedOn = DateTime.UtcNow;
                entity.LastModifiedBy = this.RequestInfo.UserId;
                entity.IsDeleted = true;
                base.Update(entity);
            }
        }

        protected void UpdateChildrenWithoutLog<TChildEntity>(ICollection<TChildEntity> childEntities) where TChildEntity : class, IBase<int>
        {
            foreach (var entity in childEntities)
            {
                this.UpdateChildrenWithOutLog(entity);
            }
        }

        public virtual void UpdateChildrenWithOutLog<TChildEntity>(TChildEntity childEntity) where TChildEntity : class, IBase<int>
        {
            if (childEntity.Id > 0)
            {
                this.DBContext.Entry(childEntity).State = EntityState.Modified;
            }
            else
            {
                this.DBContext.Entry(childEntity).State = EntityState.Added;
            }
        }
    }
}
