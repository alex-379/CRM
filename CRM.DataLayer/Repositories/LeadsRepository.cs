using CRM.Core.Constants.Logs.DataLayer;
using CRM.Core.Dtos;
using CRM.DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CRM.DataLayer.Repositories;

public class LeadsRepository(CrmContext context) : BaseRepository(context), ILeadsRepository
{
    private readonly ILogger _logger = Log.ForContext<LeadsRepository>();

    public Guid AddLead(LeadDto lead)
    {
        _ctx.Leads.Add(lead);
        _ctx.SaveChanges();
        _logger.Information(LeadsRepositoryLogs.AddLead, lead.Id);

        return lead.Id;
    }

    public LeadDto GetLeadById(Guid id)
    {
        _logger.Information(LeadsRepositoryLogs.GetLeadById, id);

        return _ctx.Leads
            .Include(d => d.Accounts)
            .FirstOrDefault(d => d.Id == id
                && !d.IsDeleted);
    }

    public LeadDto GetLeadByMail(string mail)
    {
        _logger.Information(LeadsRepositoryLogs.GetLeadByMail, mail);

        return _ctx.Leads
            .FirstOrDefault(d => d.Mail == mail
                && !d.IsDeleted);
    }

    public void UpdateLead(LeadDto lead)
    {
        _logger.Information(LeadsRepositoryLogs.UpdateLead, lead.Id);
        _ctx.Leads.Update(lead);
        _ctx.SaveChanges();
    }
}
