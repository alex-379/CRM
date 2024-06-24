using AutoMapper;
using CRM.Business.Interfaces;
using CRM.Core.Dtos;
using MassTransit;
using Messaging.Shared;
using Serilog;

namespace CRM.Business.Services;

public class MessagesService(IPublishEndpoint publishEndpoint, IMapper mapper) : IMessagesService
{
    private readonly ILogger _logger = Log.ForContext<MessagesService>();

    public async Task PublishLeadAsync(LeadDto leadDto)
    {
        _logger.Debug("Sending lead info to RabbitMQ.");
        await publishEndpoint.Publish<LeadCreated>(new
        {
            leadDto.Id,
            leadDto.Name,
            leadDto.Mail,
            leadDto.Phone,
            leadDto.Address,
            leadDto.BirthDate,
            leadDto.Status,
            leadDto.IsDeleted
        });
    }

    public async Task PublishAccountAsync(AccountDto accountDto)
    {
        _logger.Debug("Sending account info to RabbitMQ.");
        await publishEndpoint.Publish<AccountCreated>(new
        {
            accountDto.Id,
            accountDto.Currency,
            accountDto.Status,
            accountDto.Lead,
        });
    }
    
    public async Task PublishAsync<TMessage, TDto>(TDto dto)
        where TMessage : class
        where TDto : class
    {
        // var message = Activator.CreateInstance<TMessage>();
        // foreach (var property in typeof(TMessage).GetProperties())
        // {
        //     var correspondingProperty = typeof(TDto).GetProperty(property.Name);
        //     if (correspondingProperty != null)
        //     {
        //         property.SetValue(message, correspondingProperty.GetValue(dto));
        //     }
        // }
        var message = mapper.Map<TDto, TMessage>(dto);
        _logger.Debug("Sending info to RabbitMQ.");
        await publishEndpoint.Publish(message);
    }

}