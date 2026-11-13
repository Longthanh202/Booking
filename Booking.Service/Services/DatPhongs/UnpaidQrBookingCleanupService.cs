using Booking.Data.Repository.DatPhongs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Booking.Service.Services.DatPhongs
{
    public sealed class UnpaidQrBookingCleanupService : BackgroundService
    {
        private static readonly TimeSpan CleanupInterval = TimeSpan.FromDays(7);
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<UnpaidQrBookingCleanupService> _logger;

        public UnpaidQrBookingCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<UnpaidQrBookingCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var succeeded = false;
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IDatPhongRepository>();
                    var result = await repository.DeleteExpiredQrBookings(
                        DateTime.Now.AddDays(-7), stoppingToken);
                    succeeded = true;

                    if (result.DeletedCount > 0 || result.SkippedReferencedCount > 0)
                    {
                        _logger.LogInformation(
                            "Dọn booking QR hết hạn: đã xóa {DeletedCount}, giữ lại {SkippedCount} booking có tham chiếu tài chính.",
                            result.DeletedCount,
                            result.SkippedReferencedCount);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Không thể dọn booking QR hết hạn.");
                }

                try
                {
                    await Task.Delay(
                        succeeded ? CleanupInterval : TimeSpan.FromHours(1),
                        stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }
}
