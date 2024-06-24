namespace CRM.Business.Interfaces;

public interface IMessagesService
{
    Task PublishAsync<TMessage, TDto>(TDto dto)
        where TMessage : class
        where TDto : class;
}