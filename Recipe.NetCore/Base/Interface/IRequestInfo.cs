using Microsoft.EntityFrameworkCore;

namespace Recipe.NetCore.Base.Interface
{
    public interface IRequestInfo
    {
        long UserId { get; }

        string UserName { get; }

        string Role { get; }

        DbContext Context { get; }
    }
}
