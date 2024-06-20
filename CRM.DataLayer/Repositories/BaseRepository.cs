using CRM.Core.Exceptions;

namespace CRM.DataLayer.Repositories;

public class BaseRepository
{
    protected readonly CrmContext _ctx;

    protected BaseRepository(CrmContext context)
    {
        _ctx = context;
        if (!_ctx.Database.CanConnect())
        {
            throw new ServiceUnavailableException();
        }
    }
}
