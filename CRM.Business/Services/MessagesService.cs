using AutoMapper;
using CRM.Business.Interfaces;
using CRM.Business.Services.Constants.Logs;
using MassTransit;
using Serilog;

namespace CRM.Business.Services;

public class MessagesService(IPublishEndpoint publishEndpoint, IMapper mapper) : IMessagesService
{
    private readonly ILogger _logger = Log.ForContext<MessagesService>();
    
    public async Task PublishAsync<TMessage, TDto>(TDto dto)
        where TMessage : class
        where TDto : class
    {
        var message = mapper.Map<TDto, TMessage>(dto);
        _logger.Debug(LeadsServiceLogs.SendInfoToRabbitMq);
        await publishEndpoint.Publish(message);
    }
    
    public async Task PublishAsync<TMessage>(TMessage message)
        where TMessage : class
    {
        _logger.Debug(LeadsServiceLogs.SendInfoToRabbitMq);
        await publishEndpoint.Publish(message);
    }
}