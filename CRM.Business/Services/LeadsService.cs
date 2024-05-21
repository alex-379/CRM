using AutoMapper;
using CRM.Business.Configuration;
using CRM.Business.Interfaces;
using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Tokens.Responses;
using CRM.Core.Constants.Exceptions.Business;
using CRM.Core.Constants.Logs.Business;
using CRM.Core.Dtos;
using CRM.Core.Enums;
using CRM.Core.Exсeptions;
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

    public Guid AddLead(RegistrationLeadRequest request)
    {
        var lead = _mapper.Map<LeadDto>(request);
        if (_leadsRepository.GetLeadByMail(lead.Mail.ToLower()) is not null)
        {
            throw new ConflictException(LeadsServiceExceptions.ConflictException);
        }
        _logger.Information(LeadsServiceLogs.SetLowerRegister);
        lead.Mail = lead.Mail.ToLower();
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

            _logger.Information(LeadsServiceLogs.AddLead);
            _accountsRepository.AddAccount(account);
            _logger.Information(LeadsServiceLogs.AddLead);
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

        _logger.Information(LeadsServiceLogs.CheckLeadByMail);
        var leadDb = _leadsRepository.GetLeadByMail(lead.Mail.ToLower())
            ?? throw new UnauthenticatedException();

        _logger.Information(LeadsServiceLogs.CheckUserPassword);
        var confirmPassword = _passwordsService.VerifyPassword(lead.Password, leadDb.Password, leadDb.Salt);
        if (!confirmPassword)
        {
            throw new UnauthenticatedException();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, leadDb.Id.ToString()),
            new(ClaimTypes.Email, leadDb.Mail),
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
}
