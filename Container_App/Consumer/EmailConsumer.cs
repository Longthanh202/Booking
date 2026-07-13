using Container_App.Core.Interface.Emails;
using Container_App.Core.Model.Email;
using Container_App.Model.Emails;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Container_App.Consumer
{
    public class EmailConsumer
    : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;

        public EmailConsumer(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName =
                    _configuration["RabbitMQ:Host"],

                    UserName =
                    _configuration["RabbitMQ:Username"],

                    Password =
                    _configuration["RabbitMQ:Password"]
                };

                var connection =
                    await factory.CreateConnectionAsync();

                var channel =
                    await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(
                    "email_queue",
                    true,
                    false,
                    false);

                var consumer =
                    new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (
                    sender,
                    ea) =>
                {
                    var body = ea.Body.ToArray();

                    var json =
                        Encoding.UTF8.GetString(body);

                    var email =
                        JsonSerializer.Deserialize
                        <SendEmailEvent>(json);

                    using var scope =
                        _scopeFactory.CreateScope();

                    var emailService =
                        scope.ServiceProvider
                        .GetRequiredService<IEmailService>();

                    await emailService.SendEmailAsync(
                        new MailRequest
                        {
                            ToEmail =
                                email.ToEmail,

                            Subject =
                                email.Subject,

                            Body =
                                email.Body
                        });

                    await channel.BasicAckAsync(
                        ea.DeliveryTag,
                        false);
                };

                await channel.BasicConsumeAsync(
                    "email_queue",
                    false,
                    consumer);

                await Task.Delay(
                    Timeout.Infinite,
                    stoppingToken);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
