using TestProject.Core.Entities;
using TestProject.Core.Interfaces;

namespace TestProject.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly AppDbContext _dbContext;

        public LogRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task LogInformationAsync(string message, string service)
        {
            var log = new AppLog
            {
                Level = "Information",
                Message = message,
                Service = service,
            };

            await _dbContext.Logs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }

        public async Task LogErrorAsync(string message, Exception ex, string service)
        {
            var log = new AppLog
            {
                Level = "Error",
                Message = $"{message}: {ex.Message}",
                Exception = ex?.ToString(),
                Service = service,
            };

            await _dbContext.Logs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}
