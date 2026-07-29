using Container_App.Core.Model.Email;
using Container_App.Data.Repository.Emails;
using Container_App.Service.Dtos.DatPhongs;
using MailKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class SendEmailBookingConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendEmailBookingConsumer> _logger;

    private IConnection? _connection;
    private IChannel? _channel;

    public SendEmailBookingConsumer(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<SendEmailBookingConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"],
            UserName = _configuration["RabbitMQ:Username"],
            Password = _configuration["RabbitMQ:Password"]
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: "booking_email_queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                var message = JsonSerializer.Deserialize<DatPhongEvent>(json);

                if (message == null)
                {
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    return;
                }

                await SendEmail(message);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);

                _logger.LogInformation(
                    "Đã gửi email booking {BookingId}",
                    message.BookingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gửi email booking.");

                if (_channel != null)
                {
                    await _channel.BasicNackAsync(
                        ea.DeliveryTag,
                        multiple: false,
                        requeue: true);
                }
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "booking_email_queue",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        // Giữ BackgroundService chạy
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task SendEmail(DatPhongEvent message)
    {
        var body = $@"
        <h2>Đặt phòng thành công</h2>

        <p>Xin chào <b>{message.HoTen}</b>,</p>

        <p>Cảm ơn bạn đã đặt phòng tại <b>{message.TenKhachSan}</b>.</p>

        <table border='1' cellpadding='6' cellspacing='0'>
            <tr>
                <td>Mã đặt phòng</td>
                <td>{message.BookingId}</td>
            </tr>
            <tr>
                <td>Ngày nhận phòng</td>
                <td>{message.NgayNhanPhong:dd/MM/yyyy}</td>
            </tr>
            <tr>
                <td>Ngày trả phòng</td>
                <td>{message.NgayTraPhong:dd/MM/yyyy}</td>
            </tr>
            <tr>
                <td>Tổng tiền</td>
                <td>{message.TongTien:N0} VNĐ</td>
            </tr>
        </table>

        <p>Chúc bạn có một kỳ nghỉ vui vẻ!</p>";

        //await emailService.SendEmailAsync(new MailRequest
        //{
        //    ToEmail = message.Email,
        //    Subject = "Đặt phòng thành công",
        //    Body = body,
        //    IsHtml = true
        //});
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();

        base.Dispose();
    }
}