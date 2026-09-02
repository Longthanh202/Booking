using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.RabbitMQ
{
    public interface IRabbitMQPublisher
    {
        Task PublishAsync<T>(string queueName, T message);
    }
}
