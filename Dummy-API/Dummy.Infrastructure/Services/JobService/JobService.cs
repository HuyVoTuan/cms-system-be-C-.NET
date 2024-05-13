using Dummy.Infrastructure.Helpers;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Dummy.Infrastructure.Services.JobService
{
    public class JobService : IJobService
    {
        private readonly ILogger<JobService> _logger;
        private readonly ISchedulerFactory _schedulerFactory;

        public JobService(ISchedulerFactory schedulerFactory, ILogger<JobService> logger)
        {
            _logger = logger;
            _schedulerFactory = schedulerFactory;
        }

        public async Task ExecuteAt<T>(DateTimeOffset startAt, string referenceId, IDictionary<string, object> data = null, CancellationToken cancellationToken = default) where T : IJob
        {
            try
            {
                _logger.LogInformation($"Start to execute job {referenceId}-{typeof(T).Name} at {startAt.ToString("U")}");
                IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
                IJobDetail jobDetail;

                if (data is null)
                {
                    jobDetail = JobBuilder.Create<T>()
                                          .WithIdentity(StringHelper.GenerateJobDetailId(referenceId) + typeof(T).Name, typeof(T).Name)
                                          .Build();
                }
                else
                {
                    jobDetail = JobBuilder.Create<T>()
                                          .WithIdentity(StringHelper.GenerateJobDetailId(referenceId) + typeof(T).Name, typeof(T).Name)
                                          .UsingJobData(new JobDataMap(data))
                                          .Build();
                }

                ITrigger trigger = TriggerBuilder.Create()
                                                 .WithIdentity(StringHelper.GenerateJobTriggerId(referenceId) + typeof(T).Name, typeof(T).Name)
                                                 .StartAt(startAt)
                                                 .Build();

                await scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
                await scheduler.Start(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task ExecuteNow<T>(string referenceId, IDictionary<string, object> data = null, CancellationToken cancellationToken = default) where T : IJob
        {
            try
            {
                _logger.LogInformation($"Start to execute job {referenceId}-{typeof(T).Name} now");
                IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
                IJobDetail jobDetail;

                if (data is null)
                {
                    jobDetail = JobBuilder.Create<T>()
                                          .WithIdentity(StringHelper.GenerateJobDetailId(referenceId) + typeof(T).Name, typeof(T).Name)
                                          .Build();
                }
                else
                {
                    jobDetail = JobBuilder.Create<T>()
                                          .WithIdentity(StringHelper.GenerateJobDetailId(referenceId) + typeof(T).Name, typeof(T).Name)
                                          .UsingJobData(new JobDataMap(data))
                                          .Build();
                }

                ITrigger trigger = TriggerBuilder.Create()
                                                 .WithIdentity(StringHelper.GenerateJobTriggerId(referenceId) + typeof(T).Name, typeof(T).Name)
                                                 .StartNow()
                                                 .Build();

                await scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
                await scheduler.Start(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task ExecuteWithCronSchedule<T>(string cronExpression, string referenceId, IDictionary<string, object> data = null, CancellationToken cancellationToken = default) where T : IJob
        {
            try
            {
                _logger.LogInformation($"Start to execute job {referenceId}-{typeof(T).Name} with cron expression {cronExpression}");
                IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
                IJobDetail jobDetail;

                if (data is null)
                {
                    jobDetail = JobBuilder.Create<T>()
                                          .WithIdentity(StringHelper.GenerateJobDetailId(referenceId) + typeof(T).Name, typeof(T).Name)
                                          .Build();
                }
                else
                {
                    jobDetail = JobBuilder.Create<T>()
                                          .WithIdentity(StringHelper.GenerateJobDetailId(referenceId) + typeof(T).Name, typeof(T).Name)
                                          .UsingJobData(new JobDataMap(data))
                                          .Build();
                }

                // Create a cron job base on the current time zone with the given cron expression
                ITrigger trigger = TriggerBuilder.Create()
                                                 .WithIdentity(StringHelper.GenerateJobTriggerId(referenceId) + typeof(T).Name, typeof(T).Name)
                                                 .WithCronSchedule(cronExpression, x => x.InTimeZone(TimeZoneInfo.Utc))
                                                 .Build();

                await scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
                await scheduler.Start(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
