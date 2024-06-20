using CRM.Core.Dtos;
using CRM.DataLayer.Interfaces;
using CRM.DataLayer.Repositories.Constants.Logs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CRM.DataLayer.Repositories;

public class LeadsRepository(CrmContext context) : BaseRepository(context), ILeadsRepository
{
    private readonly ILogger _logger = Log.ForContext<LeadsRepository>();

    public async Task<Guid> AddLeadAsync(LeadDto lead)
    {
        await _ctx.Leads.AddAsync(lead);
        await _ctx.SaveChangesAsync();
        _logger.Information(LeadsRepositoryLogs.AddLead, lead.Id);

        return lead.Id;
    }

    public async Task<List<LeadDto>> GetLeadsAsync()
    {
        _logger.Information(LeadsRepositoryLogs.GetLeads);
        var leads = await _ctx.Leads
            .Where(d => !d.IsDeleted)
            .AsNoTracking()
            .ToListAsync();

        return leads;
    }

    public async Task<LeadDto> GetLeadByIdAsync(Guid id)
    {
        _logger.Information(LeadsRepositoryLogs.GetLeadById, id);
        var lead = await _ctx.Leads
            .Include(d => d.Accounts)
            .FirstOrDefaultAsync(d => d.Id == id
                                 && !d.IsDeleted);

        return lead;
    }

    public async Task<LeadDto> GetLeadByMailAsync(string mail)
    {
        _logger.Information(LeadsRepositoryLogs.GetLeadByMail, mail);
        var lead = await _ctx.Leads
            .FirstOrDefaultAsync(d => d.Mail == mail
                                 && !d.IsDeleted);

        return lead;
    }

    public async Task UpdateLeadAsync(LeadDto lead)
    {
        _ctx.Leads.Update(lead);
        await _ctx.SaveChangesAsync();
        _logger.Information(LeadsRepositoryLogs.UpdateLead, lead.Id);
    }
}
