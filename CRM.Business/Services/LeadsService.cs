using AutoMapper;
using CRM.Business.Configuration;
using CRM.Business.Interfaces;
using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;
using CRM.Business.Models.Tokens.Responses;
using CRM.Business.Services.Constants.Exceptions;
using CRM.Business.Services.Constants.Logs;
using CRM.Core.Dtos;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.DataLayer.Interfaces;
using Messaging.Shared;
using Serilog;

namespace CRM.Business.Services;

public class LeadsService(ILeadsRepository leadsRepository, IAccountsRepository accountsRepository, ITransactionsManager transactionsManager, 
    ITokensService tokensService, IMapper mapper, SecretSettings secret, JwtToken jwt, IMessagesService messagesService) : ILeadsService
{
    private readonly ILogger _logger = Log.ForContext<LeadsService>();

    public async Task<Guid> AddLeadAsync(RegisterLeadRequest request)
    {
        var lead = mapper.Map<LeadDto>(request);
        var account = SetupDefaultRubAccount(await SetupLeadAsync(lead));
        await using var transaction = await transactionsManager.BeginTransactionAsync();
        try
        {
            await AddToDatabaseLeadAsync(lead);
            await AddToDatabaseAccountAsync(account);
            await transactionsManager.CommitTransactionAsync(transaction);
        }
        catch (Exception ex)
        {
            await transactionsManager.RollbackTransactionAsync(transaction, ex);
        }
        
        await messagesService.PublishAsync<LeadCreated, LeadDto>(lead);
        await messagesService.PublishAsync<AccountCreated, AccountDto>(lead.Accounts.FirstOrDefault());

        return lead.Id;
    }

    private async Task<LeadDto> SetupLeadAsync(LeadDto lead)
    {
        var mailLower = lead.Mail.ToLower();
        if (await leadsRepository.GetLeadByMailAsync(mailLower) is not null)
        {
            throw new ConflictException(LeadsServiceExceptions.ConflictException);
        }
        lead.Mail = mailLower;
        _logger.Information(LeadsServiceLogs.SetLowerRegister);
        var (hash, salt) = PasswordsService.HashPassword(lead.Password, secret.SecretPassword);
        lead.Password = hash;
        lead.Salt = salt;

        return lead;
    }
    
    private static AccountDto SetupDefaultRubAccount(LeadDto lead)
    {
        AccountDto account = new()
        {
            Currency = Currency.Rub,
            Lead = lead
        };

        return account;
    }
    
    private async Task AddToDatabaseLeadAsync(LeadDto lead)
    {
        _logger.Information(LeadsServiceLogs.AddLead);
        lead.Id = await leadsRepository.AddLeadAsync(lead);
        _logger.Information(LeadsServiceLogs.CompleteLead, lead.Id);
    }
    
    private async Task AddToDatabaseAccountAsync(AccountDto account)
    {
        _logger.Information(AccountsServiceLogs.AddDefaultAccount);
        await accountsRepository.AddAccountAsync(account);
        _logger.Information(AccountsServiceLogs.CompleteAccount, account.Id);
    }

    public async Task<AuthenticatedResponse> LoginLeadAsync(LoginLeadRequest request)
    {
        var lead = mapper.Map<LeadDto>(request);
        _logger.Information(LeadsServiceLogs.CheckLeadByMail, lead.Mail);
        var leadDb = await leadsRepository.GetLeadByMailAsync(lead.Mail.ToLower())
            ?? throw new UnauthenticatedException();
        ConfirmPassword(lead,leadDb);
        var (accessToken, refreshToken) = SetTokens(leadDb);
        await leadsRepository.UpdateLeadAsync(leadDb);

        return new AuthenticatedResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
    
    private void ConfirmPassword(LeadDto lead, LeadDto leadDb)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadPassword);
        var confirmPassword = PasswordsService.VerifyPassword(lead.Password, secret.SecretPassword, leadDb.Password, leadDb.Salt);
        if (!confirmPassword)
        {
            throw new UnauthenticatedException();
        }
    }
    
    private (string accessToken, string refreshToken) SetTokens(LeadDto leadDb)
    {
        var (accessToken, refreshToken) = tokensService.GenerateTokens(leadDb);
        leadDb.RefreshToken = refreshToken;
        leadDb.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwt.LifeTimeRefreshToken);

        return (accessToken, refreshToken);
    }

    public async Task<List<LeadResponse>> GetLeadsAsync()
    {
        _logger.Information(LeadsServiceLogs.GetLeads);
        var leads = mapper.Map<List<LeadResponse>>(await leadsRepository.GetLeadsAsync());

        return leads;
    }

    public async Task<LeadFullResponse> GetLeadByIdAsync(Guid id)
    {
        _logger.Information(LeadsServiceLogs.GetLeadById, id);
        var lead = await leadsRepository.GetLeadByIdAsync(id)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, id));
        var leadResponse = mapper.Map<LeadFullResponse>(lead);

        return leadResponse;
    }

    public async Task UpdateLeadAsync(Guid leadId, UpdateLeadDataRequest request)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, leadId);
        var lead = await leadsRepository.GetLeadByIdAsync(leadId)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        SetLeadData(lead,request);
        _logger.Information(LeadsServiceLogs.UpdateLeadById, leadId);
        await leadsRepository.UpdateLeadAsync(lead);
        await messagesService.PublishAsync<LeadUpdated, LeadDto>(lead);
    }
    
    private void SetLeadData(LeadDto lead, UpdateLeadDataRequest request)
    {
        _logger.Information(LeadsServiceLogs.UpdateLeadData, lead.Id);
        lead.Name = request.Name;
        lead.Phone = request.Phone;
        lead.Address = request.Address;
    }

    public async Task UpdateLeadPasswordAsync(Guid leadId, UpdateLeadPasswordRequest request)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, leadId);
        var lead = await leadsRepository.GetLeadByIdAsync(leadId)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        SetLeadPassword(lead,request);
        _logger.Information(LeadsServiceLogs.UpdateLeadById, leadId);
        await leadsRepository.UpdateLeadAsync(lead);
        await messagesService.PublishAsync<LeadPasswordUpdated, LeadDto>(lead);
    }
    
    private void SetLeadPassword(LeadDto lead, UpdateLeadPasswordRequest request)
    {
        _logger.Information(LeadsServiceLogs.UpdateLeadPassword, lead.Id);
        lead.Password = request.Password;
        var (hash, salt) = PasswordsService.HashPassword(lead.Password, secret.SecretPassword);
        lead.Password = hash;
        lead.Salt = salt;
    }

    public async Task UpdateLeadStatusAsync(Guid id, UpdateLeadStatusRequest request)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, id);
        var lead = await leadsRepository.GetLeadByIdAsync(id)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _logger.Information(LeadsServiceLogs.UpdateLeadStatus, request.Status, id);
        lead.Status = request.Status;
        _logger.Information(LeadsServiceLogs.UpdateLeadById, id);
        await leadsRepository.UpdateLeadAsync(lead);
        await messagesService.PublishAsync<LeadStatusUpdated, LeadDto>(lead);
    }

    public async Task UpdateLeadBirthDateAsync(Guid leadId, UpdateLeadBirthDateRequest request)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, leadId);
        var lead = await leadsRepository.GetLeadByIdAsync(leadId)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        _logger.Information(LeadsServiceLogs.UpdateLeadBirthDate, leadId);
        lead.BirthDate = request.BirthDate;
        _logger.Information(LeadsServiceLogs.UpdateLeadById, leadId);
        await leadsRepository.UpdateLeadAsync(lead);
        await messagesService.PublishAsync<LeadBirthDateUpdated, LeadDto>(lead);
    }

    public async Task DeleteLeadByIdAsync(Guid id)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, id);
        var lead = await leadsRepository.GetLeadByIdAsync(id)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _logger.Information(LeadsServiceLogs.SetIsDeletedLeadById, id);
        lead.IsDeleted = true;
        await using var transaction = await transactionsManager.BeginTransactionAsync();
        try
        {
            await BlockLeadAsync(lead);
            _logger.Information(AccountsServiceLogs.BlockAccount);
            await accountsRepository.SetBlockedStatusForAccountsAsync(lead.Accounts);
            await transactionsManager.CommitTransactionAsync(transaction);
        }
        catch (Exception ex)
        {
            await transactionsManager.RollbackTransactionAsync(transaction, ex);
        }
    }
    
    private async Task BlockLeadAsync(LeadDto lead)
    {
        _logger.Information(LeadsServiceLogs.Revoke);
        await tokensService.RevokeAsync(lead.Id);
        _logger.Information(LeadsServiceLogs.UpdateLeadById, lead.Id);
        await leadsRepository.UpdateLeadAsync(lead);
    }
}
