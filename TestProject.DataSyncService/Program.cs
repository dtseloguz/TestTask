using Quartz;
using TestProject.Application.Job;
using TestProject.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("MeteoriteSyncJob");
    q.AddJob<MeteoriteSyncJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("MeteoriteSyncJobTrigger")
        .WithCronSchedule("0 * * * * ?"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var host = builder.Build();
host.Run();
