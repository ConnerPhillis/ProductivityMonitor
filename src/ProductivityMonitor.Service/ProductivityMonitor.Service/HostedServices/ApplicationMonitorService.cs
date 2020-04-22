using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProductivityMonitor.Service.Data;
using ProductivityMonitor.Service.Data.Models;
using ProductivityMonitor.Service.Services.Collection.ApplicationService;

using Timer = System.Timers.Timer;

namespace ProductivityMonitor.Service.HostedServices
{
    public class ApplicationMonitorService : IHostedService, IDisposable
    {
        private const int PollingTimeMilliseconds = 1 * 1000;

        private readonly ILogger<ApplicationMonitorService> _logger;

        private readonly IServiceScope _scope;
        private readonly IApplicationService _applicationService;
        private readonly ProductivityMonitorDbContext _dbContext;

        private readonly Timer _applicationPollTimer;

        public static int ActiveApplicationId { get; set; }


        public ApplicationMonitorService(
            ILogger<ApplicationMonitorService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;

            _applicationPollTimer = new Timer();

            _scope = serviceScopeFactory.CreateScope();

            _applicationService = _scope.ServiceProvider.GetRequiredService<IApplicationService>();

            _dbContext = _scope.ServiceProvider.GetRequiredService<ProductivityMonitorDbContext>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(ApplicationMonitorService)} has started up");

            InitializeTimers();

            return Task.CompletedTask;
        }

        private void InitializeTimers()
        {
            var lockObject = new object();

            _applicationPollTimer.Interval = PollingTimeMilliseconds;
            _applicationPollTimer.Elapsed += (sender, args) =>
            {
                var (pid, application, title) = GetFocusedApplication();

                var applicationRecord = new ApplicationRecord
                {
                    RecordDate = DateTime.Now,
                    Pid = pid,
                    ApplicationName = application,
                    Title = title
                };

                lock (lockObject)
                {
                    _dbContext.ApplicationRecords.Add(applicationRecord);
                    _dbContext.SaveChanges();
                    ActiveApplicationId = applicationRecord.Id;
                }
            };
            _applicationPollTimer.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _applicationPollTimer.Stop();
            return Task.CompletedTask;
        }

        private (int pid, string application, string mainWindowTitle) GetFocusedApplication()
        {
            using var focusedProcess = _applicationService.GetFocusedProcess();
            var pid = focusedProcess.Id;
            var processName = focusedProcess.ProcessName;
            var mainWindowName = focusedProcess.MainWindowTitle;

            if (Path.IsPathFullyQualified(processName))
                processName = Path.GetFileName(processName);

            if (Path.IsPathFullyQualified(mainWindowName))
                mainWindowName = Path.GetFileName(mainWindowName);

            return (pid, processName, mainWindowName);
        }

        ~ApplicationMonitorService()
        {
            Dispose();
        }

        public void Dispose()
        {
            _applicationPollTimer?.Dispose();
            _scope?.Dispose();
        }
    }
}