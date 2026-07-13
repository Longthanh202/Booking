using Container_App.Core.Interface.RabbitMQ;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Container_App.Service.Services.RabbitMQ
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly IConfiguration _configuration;

        public RabbitMQPublisher(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task PublishAsync<T>(string queueName,T message)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQ:Host"],
                    Port = int.Parse(
                    _configuration["RabbitMQ:Port"]),

                    UserName =
                    _configuration["RabbitMQ:Username"],

                    Password =
                    _configuration["RabbitMQ:Password"]
                };

                using var connection =
                    await factory.CreateConnectionAsync();

                using var channel =
                    await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                var body = Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(message));

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    body: body);
            }
            catch (Exception ex)
            {
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
