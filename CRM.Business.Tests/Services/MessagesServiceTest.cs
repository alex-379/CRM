using CRM.Business.Services;

namespace CRM.Business.Tests.Services;

public class MessagesServiceTest() : MessagesService(null, null)
{
    public override Task PublishAsync<TMessage, TDto>(TDto dto) => Task.CompletedTask;
    public override Task PublishAsync<TMessage>(TMessage message) => Task.CompletedTask;
}