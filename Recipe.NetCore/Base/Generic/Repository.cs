using Recipe.NetCore.Base.Interface;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Recipe.NetCore.Base.Generic
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IBase<TKey>
        where TKey : IEquatable<TKey>
    {
        protected IRequestInfo RequestInfo { get; private set; }
        
        protected DbContext DBContext
        {
            get { return this.RequestInfo.Context; }
        }

        protected DbSet<TEntity> DbSet
        {
            get
            {
                return this.DBContext.Set<TEntity>();
            }
        }

        protected virtual IQueryable<TEntity> DefaultListQuery
        {
            get
            {
                return this.DBContext.Set<TEntity>().AsQueryable().OrderBy(x => x.Id);
            }
        }

        protected virtual IQueryable<TEntity> DefaultSingleQuery
        {
            get
            {
                return this.DBContext.Set<TEntity>().AsQueryable();
            }
        }

        public Repository(IRequestInfo requestInfo)
        {
            this.RequestInfo = requestInfo;
        }

        public virtual async Task<TEntity> GetAsync(TKey id)
        {
            return await this.DefaultSingleQuery.SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(IList<TKey> ids)
        {
            return await this.DefaultSingleQuery.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public virtual async Task<int> GetCount()
        {
            return await this.DefaultListQuery.CountAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            return await this.DefaultListQuery.ToListAsync();
        }

        public virtual TEntity Create(TEntity entity)
        {
            this.DBContext.Entry(entity).State = EntityState.Added;
            return entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            var localEntity = this.GetExisting(entity);
            if (localEntity != null)
            {
                if (!this.RemoveExistingEntity(localEntity))
                {
                    throw new ApplicationException("Unexpected Error Occured");
                }
            }
            this.DBContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            var entity = await this.GetAsync(id);
            this.DBContext.Entry(entity).State = EntityState.Deleted;
        }

        protected void DeleteRange<TEntityList>(TEntityList entityList) where TEntityList : IQueryable
        {
            foreach (var each in entityList)
            {
                this.DBContext.Entry(each).State = EntityState.Deleted;
            }
        }

        public virtual async Task<TEntity> GetEntityOnly(TKey id)
        {
            return await this.DBContext.Set<TEntity>().AsQueryable().SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        private TEntity GetExisting(TEntity entity)
        {
            return this.DBContext.Set<TEntity>().Local.FirstOrDefault(x => x.Id.Equals(entity.Id));
        }

        private bool RemoveExistingEntity(TEntity entity)
        {
            return this.DBContext.Set<TEntity>().Local.Remove(entity);
        }

        /// <summary>
        /// GetPagedResultAsync
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual async Task<Tuple<int, IEnumerable<TEntity>>> GetPagedResultAsync(Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           int? page = null,
           int? pageSize = null)
        {
            IQueryable<TEntity> query = this.DbSet;
            int totalCount = 0;

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                query = query.OrderBy(x => x.Id);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }
            totalCount = await query.CountAsync();

            if (page != null)
            {
                query = query.Skip(((int)page - 1) * (int)pageSize);
            }

            if (pageSize != null)
            {
                query = query.Take((int)pageSize);
            }

            var data = await query.ToListAsync();
            return new Tuple<int, IEnumerable<TEntity>>(totalCount, data);
        }
    }
}
