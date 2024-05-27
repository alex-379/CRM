using AutoMapper;
using CRM.Business.Configuration;
using CRM.Business.Interfaces;
using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;
using CRM.Business.Models.Tokens.Responses;
using CRM.Core.Constants.Exceptions.Business;
using CRM.Core.Constants.Logs.Business;
using CRM.Core.Dtos;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.DataLayer.Interfaces;
using Serilog;
using System.Security.Claims;

namespace CRM.Business.Services;

public class LeadsService(ILeadsRepository leadsRepository, IAccountsRepository accountsRepository, ITransactionsRepository transactionsRepository,
    IPasswordsService passwordsService, ITokensService tokensService, IMapper mapper, JwtToken jwt) : ILeadsService
{
    private readonly ILeadsRepository _leadsRepository = leadsRepository;
    private readonly IAccountsRepository _accountsRepository = accountsRepository;
    private readonly ITransactionsRepository _transactionsRepository = transactionsRepository;
    private readonly IPasswordsService _passwordsService = passwordsService;
    private readonly ITokensService _tokensService = tokensService;
    private readonly IMapper _mapper = mapper;
    private readonly JwtToken _jwt = jwt;
    private readonly ILogger _logger = Log.ForContext<LeadsService>();

    public Guid AddLead(RegisterLeadRequest request)
    {
        var lead = _mapper.Map<LeadDto>(request);
        if (_leadsRepository.GetLeadByMail(lead.Mail.ToLower()) is not null)
        {
            throw new ConflictException(LeadsServiceExceptions.ConflictException);
        }
        lead.Mail = lead.Mail.ToLower();
        _logger.Information(LeadsServiceLogs.SetLowerRegister);
        var (hash, salt) = _passwordsService.HashPasword(lead.Password);
        lead.Password = hash;
        lead.Salt = salt;
        AccountDto account = new()
        {
            Currency = Currency.Rub,
            Lead = lead
        };

        using var transaction = _transactionsRepository.BeginTransaction();
        try
        {
            _logger.Information(LeadsServiceLogs.AddLead);
            lead.Id = _leadsRepository.AddLead(lead);
            _logger.Information(LeadsServiceLogs.CompleteLead, lead.Id);

            _logger.Information(AccountsServiceLogs.AddDefaultAccount);
            _accountsRepository.AddAccount(account);
            _logger.Information(AccountsServiceLogs.CompleteAccount, account.Id);
            _transactionsRepository.CommitTransaction(transaction);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Log.Error(ex.Message);
        }

        return lead.Id;
    }

    public AuthenticatedResponse LoginLead(LoginLeadRequest request)
    {
        LeadDto lead = _mapper.Map<LeadDto>(request);
        _logger.Information(LeadsServiceLogs.CheckLeadByMail, lead.Mail);
        var leadDb = _leadsRepository.GetLeadByMail(lead.Mail.ToLower())
            ?? throw new UnauthenticatedException();
        _logger.Information(LeadsServiceLogs.CheckLeadPassword);
        var confirmPassword = _passwordsService.VerifyPassword(lead.Password, leadDb.Password, leadDb.Salt);
        if (!confirmPassword)
        {
            throw new UnauthenticatedException();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, leadDb.Id.ToString()),
            new(ClaimTypes.Email, leadDb.Mail),
            new(ClaimTypes.Role, leadDb.Status.ToString()),
        };

        var accessToken = _tokensService.GenerateAccessToken(claims);
        var refreshToken = _tokensService.GenerateRefreshToken();
        leadDb.RefreshToken = refreshToken;
        leadDb.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwt.LifeTimeRefreshToken);
        _leadsRepository.UpdateLead(leadDb);

        return new AuthenticatedResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public List<LeadResponse> GetLeads()
    {
        _logger.Information(LeadsServiceLogs.GetLeads);
        var leads = _mapper.Map<List<LeadResponse>>(_leadsRepository.GetLeads());

        return leads;
    }

    public LeadFullResponse GetLeadById(Guid id)
    {
        _logger.Information(LeadsServiceLogs.GetLeadById, id);
        var lead = _leadsRepository.GetLeadById(id)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, id));
        var leadResponse = _mapper.Map<LeadFullResponse>(lead);

        return leadResponse;
    }

    public void UpdateLead(Guid leadId, UpdateLeadDataRequest request)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, leadId);
        var lead = _leadsRepository.GetLeadById(leadId)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        _logger.Information(LeadsServiceLogs.UpdateLeadData, leadId);
        lead.Name = request.Name;
        lead.Phone = request.Phone;
        lead.Address = request.Address;
        _logger.Information(LeadsServiceLogs.UpdateLeadById, leadId);
        _leadsRepository.UpdateLead(lead);
    }

    public void UpdateLeadPassword(Guid leadId, UpdateLeadPasswordRequest request)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, leadId);
        var lead = _leadsRepository.GetLeadById(leadId)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        _logger.Information(LeadsServiceLogs.UpdateLeadPassword, leadId);
        lead.Password = request.Password;
        var (hash, salt) = _passwordsService.HashPasword(lead.Password);
        lead.Password = hash;
        lead.Salt = salt;
        _logger.Information(LeadsServiceLogs.UpdateLeadById, leadId);
        _leadsRepository.UpdateLead(lead);
    }

    public void UpdateLeadMail(Guid leadId, UpdateLeadMailRequest request)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, leadId);
        var lead = _leadsRepository.GetLeadById(leadId)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        if (_leadsRepository.GetLeadByMail(request.Mail.ToLower()) is not null)
        {
            throw new ConflictException(LeadsServiceExceptions.ConflictException);
        }
        _logger.Information(LeadsServiceLogs.UpdateLeadMail, leadId);
        lead.Mail = request.Mail.ToLower();
        _logger.Information(LeadsServiceLogs.UpdateLeadById, leadId);
        _leadsRepository.UpdateLead(lead);
    }

    public void UpdateLeadStatus(Guid id, UpdateLeadStatusRequest request)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, id);
        var lead = _leadsRepository.GetLeadById(id)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _logger.Information(LeadsServiceLogs.UpdateLeadStatus, request.Status, id);
        lead.Status = request.Status;
        _logger.Information(LeadsServiceLogs.UpdateLeadById, id);
        _leadsRepository.UpdateLead(lead);
    }

    public void UpdateLeadBirthDate(Guid leadId, UpdateLeadBirthDateRequest request)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, leadId);
        var lead = _leadsRepository.GetLeadById(leadId)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        _logger.Information(LeadsServiceLogs.UpdateLeadBirthDate, leadId);
        lead.BirthDate = request.BirthDate;
        _logger.Information(LeadsServiceLogs.UpdateLeadById, leadId);
        _leadsRepository.UpdateLead(lead);
    }

    public void DeleteLeadById(Guid id)
    {
        _logger.Information(LeadsServiceLogs.CheckLeadById, id);
        var lead = _leadsRepository.GetLeadById(id)
            ?? throw new NotFoundException(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _logger.Information(LeadsServiceLogs.SetIsDeletedLeadById, id);
        lead.IsDeleted = true;
        using var transaction = _transactionsRepository.BeginTransaction();
        try
        {
            _logger.Information(LeadsServiceLogs.UpdateLeadById, id);
            _leadsRepository.UpdateLead(lead);

            foreach (var account in lead.Accounts)
            {
                _logger.Information(AccountsServiceLogs.BlockAccount, account.Id);
                account.Status = AccountStatus.Block;
                _logger.Information(AccountsServiceLogs.UpdateAccountById, account.Id);
                _accountsRepository.UpdateAccount(account);
            }
            _transactionsRepository.CommitTransaction(transaction);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Log.Error(ex.Message);
        }
    }
}
