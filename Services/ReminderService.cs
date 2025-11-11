using LibraryApi.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Services;

public class ReminderService : IHostedService, IDisposable
{
    private readonly ILogger<ReminderService> _logger;
    private readonly IServiceProvider _services;
    private Timer? _timer = null;

    public ReminderService(ILogger<ReminderService> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reminder Service running.");

        // Timer will start immediately and then repeat every 24 hours
        _timer = new Timer(DoWork, null, TimeSpan.Zero, 
            TimeSpan.FromHours(24));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        _logger.LogInformation("Reminder Service is working. Checking for loans due tomorrow...");

        using (var scope = _services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

            // Find loans due tomorrow
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);
            var loansDueTomorrow = dbContext.Loans
                .Include(l => l.User) // User details for the email
                .Include(l => l.Book) // Book details for the title
                .Where(l => l.ReturnDate == null && l.DueDate.Date == tomorrow)
                .ToList();

            if (loansDueTomorrow.Any())
            {
                _logger.LogInformation($"Found {loansDueTomorrow.Count} loans due tomorrow. Sending reminders...");
                foreach (var loan in loansDueTomorrow)
                {
                    // "Fake" sending an email
                    _logger.LogInformation($"REMINDER > To: {loan.User.Email} | Subject: Book '{loan.Book.Title}' is due tomorrow!");
                }
            }
            else
            {
                _logger.LogInformation("No loans due tomorrow.");
            }
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reminder Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
