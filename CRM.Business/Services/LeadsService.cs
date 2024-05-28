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
using Serilog;

namespace CRM.Business.Services;

public class LeadsService(ILeadsRepository leadsRepository, IAccountsRepository accountsRepository, ITransactionsManager transactionsManager,
    IPasswordsService passwordsService, ITokensService tokensService, IMapper mapper, JwtToken jwt) : ILeadsService
{
    private readonly ILogger _logger = Log.ForContext<LeadsService>();

    public Guid AddLead(RegisterLeadRequest request)
    {
        var lead = mapper.Map<LeadDto>(request);
        var account = SetupDefaultRubAccount(SetupLead(lead));
        using var transaction = transactionsManager.BeginTransaction();
        try
        {
            AddLead(lead);
            AddAccount(account);
            transactionsManager.CommitTransaction(transaction);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Log.Error(ex.Message);
        }

        return lead.Id;
    }

    private LeadDto SetupLead(LeadDto lead)
    {
        var mailLower = lead.Mail.ToLower();
        if (leadsRepository.GetLeadByMail(mailLower) is not null)
        {
            throw new ConflictException(LeadsServiceExceptions.ConflictException);
        }
        lead.Mail = mailLower;
        _logger.Information(LeadsServiceLogs.SetLowerRegister);
        var (hash, salt) = passwordsService.HashPassword(lead.Password);
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
    
    private void AddLead(LeadDto lead)
    {
        _logger.Information(LeadsServiceLogs.AddLead);
        lead.Id = leadsRepository.AddLead(lead);
        _logger.Information(LeadsServiceLogs.CompleteLead, lead.Id);
    }
    
    private void AddAccount(AccountDto account)
    {
        _logger.Information(AccountsServiceLogs.AddDefaultAccount);
        accountsRepository.AddAccount(account);
        _logger.Information(AccountsServiceLogs.CompleteAccount, account.Id);
    }

    public AuthenticatedResponse LoginLead(LoginLeadRequest request)
    {
        var lead = mapper.Map<LeadDto>(request);
        _logger.Information(LeadsServiceLogs.CheckLeadByMail, lead.Mail);
        var leadDb = leadsRepository.GetLeadByMail(lead.Mail.ToLower())
            ?? throw new UnauthenticatedException();
        ConfirmPassword(lead,leadDb);
        var (accessToken, refreshToken) = SetTokens(leadDb);
        leadsRepository.UpdateLead(leadDb);

        return new AuthenticatedResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
    
    private void ConfirmPassword(LeadDto lead, LeadDto leadDb)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadPassword);
        var confirmPassword = passwordsService.VerifyPassword(lead.Password, leadDb.Password, leadDb.Salt);
        if (!confirmPassword)
        {
            throw new UnauthenticatedException();
        }
    }
    
    private (string accessToken, string refreshToken) SetTokens(LeadDto leadDb)
    {
        var accessToken = tokensService.GenerateAccessToken(leadDb);
        var refreshToken = tokensService.GenerateRefreshToken();
        leadDb.RefreshToken = refreshToken;
        leadDb.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwt.LifeTimeRefreshToken);

        return (accessToken, refreshToken);
    }

    public List<LeadResponse> GetLeads()
    {
        _logger.Information(LeadsServiceLogs.GetLeads);
        var leads = mapper.Map<List<LeadResponse>>(leadsRepository.GetLeads());

        return leads;
    }

    public LeadFullResponse GetLeadById(Guid id)
    {
        _logger.Information(LeadsServiceLogs.GetLeadById, id);
        var lead = leadsRepository.GetLeadById(id)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, id));
        var leadResponse = mapper.Map<LeadFullResponse>(lead);

        return leadResponse;
    }

    public void UpdateLead(Guid leadId, UpdateLeadDataRequest request)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, leadId);
        var lead = leadsRepository.GetLeadById(leadId)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        SetLeadData(lead,request);
        _logger.Information(LeadsServiceLogs.UpdateLeadById, leadId);
        leadsRepository.UpdateLead(lead);
    }
    
    private void SetLeadData(LeadDto lead, UpdateLeadDataRequest request)
    {
        _logger.Information(LeadsServiceLogs.UpdateLeadData, lead.Id);
        lead.Name = request.Name;
        lead.Phone = request.Phone;
        lead.Address = request.Address;
    }

    public void UpdateLeadPassword(Guid leadId, UpdateLeadPasswordRequest request)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, leadId);
        var lead = leadsRepository.GetLeadById(leadId)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        SetLeadPassword(lead,request);
        _logger.Information(LeadsServiceLogs.UpdateLeadById, leadId);
        leadsRepository.UpdateLead(lead);
    }
    
    private void SetLeadPassword(LeadDto lead, UpdateLeadPasswordRequest request)
    {
        _logger.Information(LeadsServiceLogs.UpdateLeadPassword, lead.Id);
        lead.Password = request.Password;
        var (hash, salt) = passwordsService.HashPassword(lead.Password);
        lead.Password = hash;
        lead.Salt = salt;
    }

    public void UpdateLeadStatus(Guid id, UpdateLeadStatusRequest request)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, id);
        var lead = leadsRepository.GetLeadById(id)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _logger.Information(LeadsServiceLogs.UpdateLeadStatus, request.Status, id);
        lead.Status = request.Status;
        _logger.Information(LeadsServiceLogs.UpdateLeadById, id);
        leadsRepository.UpdateLead(lead);
    }

    public void UpdateLeadBirthDate(Guid leadId, UpdateLeadBirthDateRequest request)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, leadId);
        var lead = leadsRepository.GetLeadById(leadId)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        _logger.Information(LeadsServiceLogs.UpdateLeadBirthDate, leadId);
        lead.BirthDate = request.BirthDate;
        _logger.Information(LeadsServiceLogs.UpdateLeadById, leadId);
        leadsRepository.UpdateLead(lead);
    }

    public void DeleteLeadById(Guid id)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, id);
        var lead = leadsRepository.GetLeadById(id)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _logger.Information(LeadsServiceLogs.SetIsDeletedLeadById, id);
        lead.IsDeleted = true;
        using var transaction = transactionsManager.BeginTransaction();
        try
        {
            BlockLead(lead);
            foreach (var account in lead.Accounts)
            {
                BlockAccount(account);
            }
            transactionsManager.CommitTransaction(transaction);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Log.Error(ex.Message);
        }
    }
    
    private void BlockLead(LeadDto lead)
    {
        _logger.Information(LeadsServiceLogs.Revoke, lead.Id);
        tokensService.Revoke(lead.Id);
        _logger.Information(LeadsServiceLogs.UpdateLeadById, lead.Id);
        leadsRepository.UpdateLead(lead);
    }
    
    private void BlockAccount(AccountDto account)
    {
        _logger.Information(AccountsServiceLogs.BlockAccount, account.Id);
        account.Status = AccountStatus.Blocked;
        _logger.Information(AccountsServiceLogs.UpdateAccountById, account.Id);
        accountsRepository.UpdateAccount(account);
    }
}
