using Recipe.NetCore.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recipe.NetCore.Base.Abstract
{
    public class DTO<TEntity, TKey> : IBase<TKey>
        where TEntity : IAuditModel<TKey>, new()
    {
        #region Private_Attributes
        private DateTime createdon;
        private DateTime? modifiedOn;
        #endregion Private_Attributes

        public TKey Id { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedOn
        {
            get
            {
                return this.createdon;
            }

            set
            {
                if (value == DateTime.MinValue || value.Kind == DateTimeKind.Local)
                    this.createdon = value;
                else
                {
                    this.createdon = TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.Local);
                }

            }
        }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedOn {
            get
            {
                    return this.modifiedOn;
            }

            set
            {
                if (value.HasValue == false)                    
                    this.modifiedOn = value;
                else if (value.Value == DateTime.MinValue || value.Value.Kind == DateTimeKind.Local)
                    this.modifiedOn = value;
                else
                {
                    this.modifiedOn = TimeZoneInfo.ConvertTimeFromUtc(value.Value, TimeZoneInfo.Local);
                }
            }
        }

        public bool isDeleted { get; set; }

        public DTO()
        {

        }

        public DTO(TEntity entity)
        {
            this.ConvertFromEntity(entity);
        }

        public TEntity ConvertToEntity()
        {
            TEntity entity = new TEntity();
            return this.ConvertToEntity(entity);
        }

        public virtual TEntity ConvertToEntity(TEntity entity)
        {
            entity.Id = this.Id == null || this.Id.Equals(default(TKey)) ? entity.Id : this.Id;
            return entity;
        }

        public virtual void ConvertFromEntity(TEntity entity)
        {
            this.Id = entity.Id;
        }

        public static List<TDTO> ConvertEntityListToDTOList<TDTO>(IEnumerable<TEntity> entityList) where TDTO : DTO<TEntity, TKey>, new()
        {
            var result = new List<TDTO>();
            if (entityList != null)
                foreach (var entity in entityList)
                {
                    var dto = new TDTO();
                    dto.ConvertFromEntity(entity);
                    result.Add(dto);
                }
            return result;
        }

        public static IList<TEntity> ConvertDTOListToEntity(IEnumerable<DTO<TEntity, TKey>> dtoList, IEnumerable<TEntity> entityList)
        {
            var result = new List<TEntity>();
            if (dtoList != null)
                foreach (var dto in dtoList)
                {
                    var entityFromDb = entityList.SingleOrDefault(x => x.Id.Equals(dto.Id));
                    if (entityFromDb != null)
                    {
                        result.Add(dto.ConvertToEntity(entityFromDb));
                    }
                    else
                    {
                        result.Add(dto.ConvertToEntity());
                    }
                }
            foreach (var entity in entityList.Where(x => !dtoList.Any(y => y.Id.Equals(x.Id))))
            {
                entity.IsDeleted = true;
                result.Add(entity);
            }
            return result;
        }

        public static IList<TEntity> ConvertDTOListToEntity(IEnumerable<DTO<TEntity, TKey>> dtoList)
        {
            var result = new List<TEntity>();
            if (dtoList != null)
                foreach (var dto in dtoList)
                {
                    result.Add(dto.ConvertToEntity());
                }
            return result;
        }
    }
}
