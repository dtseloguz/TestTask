using MeteoriteSync.Core.Results;

namespace MeteoriteSync.Application.Services.Results
{
    public class SyncResult : Result
    {
        public int ProcessedCount { get; }

        public SyncResult(bool isSuccess, int count, Error error)
            : base(isSuccess, error)
        {
            ProcessedCount = count;
        }

        public static SyncResult Success(int count) =>
            new(true, count, Error.None);

        public static new SyncResult Failure(Error error, int count = 0) =>
            new(false, count, error);
    }
}