using IdentiyMail.Web.Context;
using Microsoft.EntityFrameworkCore;

namespace IdentiyMail.Web.Services
{
    public class TrashCleanupBackgroundService(IServiceScopeFactory _scopeFactory, ILogger<TrashCleanupBackgroundService> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupTrashAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex, "Çöp kutusu otomatik temizleme sırasında hata oluştu.");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        #region 30 günü geçen çöp kutusu mesajlarını temizleme
        private async Task CleanupTrashAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var deleteBefore = DateTime.Now.AddDays(-30);

            var messages = await context.UserMessages
                .Where(x =>
                    (
                        x.IsDeletedBySender &&
                        !x.IsPermanentlyDeletedBySender &&
                        x.DeletedBySenderAt.HasValue &&
                        x.DeletedBySenderAt.Value <= deleteBefore
                    )
                    ||
                    (
                        x.IsDeletedByReceiver &&
                        !x.IsPermanentlyDeletedByReceiver &&
                        x.DeletedByReceiverAt.HasValue &&
                        x.DeletedByReceiverAt.Value <= deleteBefore
                    ))
                .ToListAsync(cancellationToken);

            foreach (var message in messages)
            {
                if (message.IsDeletedBySender && !message.IsPermanentlyDeletedBySender && message.DeletedBySenderAt.HasValue && message.DeletedBySenderAt.Value <= deleteBefore)
                {
                    message.IsPermanentlyDeletedBySender = true;
                }

                if (message.IsDeletedByReceiver && !message.IsPermanentlyDeletedByReceiver && message.DeletedByReceiverAt.HasValue && message.DeletedByReceiverAt.Value <= deleteBefore)
                {
                    message.IsPermanentlyDeletedByReceiver = true;
                }
            }
            await context.SaveChangesAsync(cancellationToken);
        }
        #endregion
    }
}
