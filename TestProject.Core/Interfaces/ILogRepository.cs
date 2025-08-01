namespace TestProject.Core.Interfaces
{
    public interface ILogRepository
    {
        Task LogInformationAsync(string message, string service);
        Task LogErrorAsync(string message, Exception ex, string service);
    }
}
