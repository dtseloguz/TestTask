namespace TestProject.Core.Entities
{
    public class AppLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Level { get; set; }
        public string? Message { get; set; }
        public string? Exception { get; set; }
        public string? Logger { get; set; }
        public string? Service { get; set; }
    }
}
