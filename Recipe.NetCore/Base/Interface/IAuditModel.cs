using System;

namespace Recipe.NetCore.Base.Interface
{
    public interface IAuditModel<TKey> : IAuditModel, IBase<TKey>
    {
    }

    public interface IAuditModel : IBase
    {
        long CreatedBy { get; set; }

        DateTime CreatedOn { get; set; }

        long? LastModifiedBy { get; set; }

        DateTime? LastModifiedOn { get; set; }

        bool IsDeleted { get; set; }
    }
}
