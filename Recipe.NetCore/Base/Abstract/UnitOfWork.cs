using Recipe.NetCore.Base.Interface;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Recipe.NetCore.Base.Abstract
{
    public class UnitOfWork : IUnitOfWork
    {
        IRequestInfo _requestInfo;

        public UnitOfWork(IRequestInfo requestInfo)
        {
            this._requestInfo = requestInfo;
        }

        public DbContext DBContext
        {
            get
            {
                return this._requestInfo.Context;
            }
        }

        public int Save()
        {
            return this._requestInfo.Context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await this._requestInfo.Context.SaveChangesAsync();
        }
    }

}
