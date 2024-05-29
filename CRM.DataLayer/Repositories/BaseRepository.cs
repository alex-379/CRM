namespace CRM.DataLayer.Repositories;

public class BaseRepository(CrmContext context)
{
    protected readonly CrmContext _ctx = context;
}
