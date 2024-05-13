using Quartz;

namespace Dummy.Infrastructure.Services.JobService
{
    public interface IJobService
    {
        public Task ExecuteNow<T>(string referenceId, IDictionary<string, object> data = default, CancellationToken cancellationToken = default) where T : IJob;
        public Task ExecuteAt<T>(DateTimeOffset startAt, string referenceId, IDictionary<string, object> data = default, CancellationToken cancellationToken = default) where T : IJob;
        public Task ExecuteWithCronSchedule<T>(string cronExpression, string referenceId, IDictionary<string, object> data = default, CancellationToken cancellationToken = default) where T : IJob;
    }
}
