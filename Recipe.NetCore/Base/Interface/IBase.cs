using System.ComponentModel.DataAnnotations;

namespace Recipe.NetCore.Base.Interface
{
    public interface IBase<TKey> : IBase
    {
        [Key]
        TKey Id { get; set; }
    }

    public interface IBase
    {
    }
}

