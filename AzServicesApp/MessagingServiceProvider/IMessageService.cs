using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingServiceProvider;

public interface IMessageService
{
    Task PublishMessage<T>(string topic, T message, MessageType messageType);
    //void Subscribe(string topicOrQueue, Func<Message, Task> messageHandler);
    Task SubscribeAsync<T>(string subscriptionName, Func<T, ProcessMessageEventArgs, Task> handler, string filterExpression = null, CancellationToken cancellationToken = default);
}
