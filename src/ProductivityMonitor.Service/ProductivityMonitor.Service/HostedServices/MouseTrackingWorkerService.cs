using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProductivityMonitor.Service.Data;
using ProductivityMonitor.Service.Data.Models;
using ProductivityMonitor.Service.Services.Collection.MousePositionService;
using ProductivityMonitor.Service.Utilities;

using Timer = System.Timers.Timer;

namespace ProductivityMonitor.Service.HostedServices
{
    public class MouseTrackingWorkerService : IHostedService, IDisposable
    {
        private readonly ILogger<MouseTrackingWorkerService> _logger;

        private readonly IServiceScope _scope;
        private readonly IMousePositionService _mousePositionService;
        private readonly ProductivityMonitorDbContext _dbContext;

        private readonly List<(DateTime eventTime, Point mousePosition)> _locationCache;

        private readonly Timer _locationPollingTimer;
        private readonly Timer _cacheFlushTimer;

        private Point _lastPosition;

        public MouseTrackingWorkerService(
            ILogger<MouseTrackingWorkerService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;

            _scope = serviceScopeFactory.CreateScope();

            _mousePositionService =
                _scope.ServiceProvider.GetRequiredService<IMousePositionService>();
            _dbContext = _scope.ServiceProvider.GetRequiredService<ProductivityMonitorDbContext>();

            _locationCache = new List<(DateTime eventTime, Point mousePosition)>();

            _locationPollingTimer = new Timer();
            _cacheFlushTimer = new Timer();
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            InitializeTimers();

            _logger.LogInformation($"{nameof(MouseTrackingWorkerService)} has started up");

            return Task.CompletedTask;
        }

        private void InitializeTimers()
        {
            var lockObject = new object();

            _locationPollingTimer.Interval = 100;
            _locationPollingTimer.Elapsed += (sender, args) =>
            {

                if (ApplicationMonitorService.ActiveApplicationId == 0)
                    return;

                var location = _mousePositionService.GetMousePosition();
                var currentTime = DateTime.Now;

                lock (lockObject)
                    _locationCache.Add((currentTime, location));
            };
            _locationPollingTimer.Start();

            _cacheFlushTimer.Interval += 10;
            _cacheFlushTimer.Elapsed += (sender, args) =>
            {
                lock (lockObject)
                {
                    var locations =
                        new (DateTime eventTime, Point mousePosition)[_locationCache.Count];
                    _locationCache.CopyTo(locations, 0);
                    _locationCache.Clear();

                    foreach (var (eventTime, mousePosition) in locations)
                    {
                        var moved = mousePosition.X != _lastPosition.X
                            || mousePosition.Y != _lastPosition.Y;

                        if (!moved)
                            continue;

                        _lastPosition = mousePosition;

                        var locationRecord = new MouseInputRecord()
                        {
                            ActiveApplicationId = ApplicationMonitorService.ActiveApplicationId,
                            RecordDate = eventTime,
                            IsClick = false,
                            XPosition = mousePosition.X,
                            YPosition = mousePosition.Y
                        };

                        _dbContext.Add(locationRecord);
                    }

                    _dbContext.SaveChanges();
                }
            };
            _cacheFlushTimer.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _locationPollingTimer.Stop();
            _cacheFlushTimer.Stop();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _locationPollingTimer?.Dispose();
            _cacheFlushTimer?.Dispose();
            _scope?.Dispose();
        }
    }
}