using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Recipe.NetCore.Base.Interface
{
    public interface IUnitOfWork
    {
        DbContext DBContext { get; }

        Task<int> SaveAsync();

        int Save();
    }
}
