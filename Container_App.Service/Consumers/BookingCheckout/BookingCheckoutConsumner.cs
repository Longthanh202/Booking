using Container_App.Common.Shared;
using Container_App.Data.Repository.Emails;
using Container_App.Service.Dtos.DatPhongs;
using Container_App.Service.Services.HoaHongs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Container_App.Service.Consumers.BookingCheckout
{
    public class BookingCheckoutConsumner: BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BookingCheckoutConsumner> _logger;

        private IConnection? _connection;
        private IChannel? _channel;

        public BookingCheckoutConsumner(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<BookingCheckoutConsumner> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("BookingCheckoutConsumer STARTED");
            Console.WriteLine("====================================");
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQ:Host"],
                    UserName = _configuration["RabbitMQ:Username"],
                    Password = _configuration["RabbitMQ:Password"]
                };

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        Console.WriteLine("Đang kết nối RabbitMQ...");
                        _connection = await factory.CreateConnectionAsync(stoppingToken);
                        Console.WriteLine(
                            "RabbitMQ CONNECTED");
                        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
                        Console.WriteLine(
                            "RabbitMQ CHANNEL CREATED");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"RabbitMQ ERROR: {ex.Message}");
                        _logger.LogWarning(ex, "RabbitMQ chưa sẵn sàng, thử lại sau 5 giây...");
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    }
                }

                await _channel.QueueDeclareAsync(
                    queue: "booking_checked_out",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();

                        var hoaHongService =
                            scope.ServiceProvider.GetRequiredService<IHoaHongService>();

                        var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                        var message =
                            JsonSerializer.Deserialize<BookingCheckedOutEvent>(json);

                        if (message != null)
                        {
                            await hoaHongService.TinhHoaHong(message.DatPhongId);
                        }

                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi xử lý message");

                        await _channel.BasicNackAsync(
                            ea.DeliveryTag,
                            multiple: false,
                            requeue: true);
                    }
                };

                await _channel.BasicConsumeAsync(
                    queue: "booking_checked_out",
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xử lý message");
            }
        }
    }
}
