using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ProductivityMonitor.Service.Data;
using ProductivityMonitor.Service.Data.Models;
using ProductivityMonitor.Service.Services.Collection.InputService;
using ProductivityMonitor.Service.Services.Collection.MousePositionService;
using ProductivityMonitor.Service.Utilities;

using Timer = System.Timers.Timer;

namespace ProductivityMonitor.Service.HostedServices
{
    public class InputMonitorWorkerService : IHostedService, IDisposable
    {
        private readonly ILogger<InputMonitorWorkerService> _logger;

        private readonly IServiceScope _scope;
        private readonly IInputMonitorService _inputMonitorService;
        private readonly IMousePositionService _mousePositionService;
        private readonly ProductivityMonitorDbContext _dbContext;


        private readonly List<(DateTime eventTime, int keyCode)> _keyPressBuffer;
        private readonly List<(DateTime eventTime, Point mousePosition)> _locationCache;


        private readonly Timer _inputPollTimer;
        private readonly Timer _bufferFlushTimer;

        public InputMonitorWorkerService(
            ILogger<InputMonitorWorkerService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;

            _scope = serviceScopeFactory.CreateScope();

            _inputMonitorService = _scope.ServiceProvider
               .GetRequiredService<IInputMonitorService>();
            _mousePositionService =
                _scope.ServiceProvider.GetRequiredService<IMousePositionService>();
            _dbContext = _scope.ServiceProvider.GetRequiredService<ProductivityMonitorDbContext>();

            _keyPressBuffer = new List<(DateTime eventTime, int keyCode)>();
            _locationCache = new List<(DateTime eventTime, Point mousePosition)>();

            _inputPollTimer = new Timer();
            _bufferFlushTimer = new Timer();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            InitializeTimers();

            _logger.LogInformation($"{nameof(InputMonitorWorkerService)} has started up");

            return Task.CompletedTask;
        }

        private void InitializeTimers()
        {
            var lockObject = new object();

            _inputPollTimer.Interval = 10;
            _inputPollTimer.Elapsed += (sender, args) =>
            {
                if (ApplicationMonitorService.ActiveApplicationId == 0)
                    return;

                // ReSharper disable once InconsistentlySynchronizedField
                var receivedInput = _inputMonitorService.CheckInputReceived()
                   .ToList();

                var mouseClicks = receivedInput
                   .Where(keyCode => _inputMonitorService.IsClick(keyCode))
                   .ToList();

                var keyPresses = receivedInput.Except(mouseClicks);

                lock (lockObject)
                {
                    foreach (var _ in mouseClicks)
                        _locationCache.Add(
                            (DateTime.Now, _mousePositionService.GetMousePosition()));
                    foreach (var keyCode in keyPresses)
                        _keyPressBuffer.Add((DateTime.Now, keyCode));
                }
            };
            _inputPollTimer.Start();

            _bufferFlushTimer.Interval = 1000;
            _bufferFlushTimer.Elapsed += (sender, args) =>
            {
                lock (lockObject)
                {
                    var keyPressCache =
                        new (DateTime eventTime, int keyCodes)[_keyPressBuffer.Count];
                    _keyPressBuffer.CopyTo(keyPressCache);
                    _keyPressBuffer.Clear();

                    var mouseClickCache =
                        new (DateTime eventTime, Point mousePosition)[_locationCache.Count];
                    _locationCache.CopyTo(mouseClickCache);
                    _locationCache.Clear();

                    foreach (var (eventTime, keyCode) in keyPressCache)
                    {
                        var keyPressRecord = new KeyboardInputRecord
                        {
                            ActiveApplicationId = ApplicationMonitorService.ActiveApplicationId,
                            RecordDate = eventTime,
                            KeyPressed = _inputMonitorService.GetKeyName(keyCode)
                        };
                        _dbContext.Add(keyPressRecord);
                    }

                    foreach (var (eventTime, mousePosition) in mouseClickCache)
                    {
                        var clickRecord = new MouseInputRecord
                        {
                            ActiveApplicationId = ApplicationMonitorService.ActiveApplicationId,
                            RecordDate = eventTime,
                            IsClick = true,
                            XPosition = mousePosition.X,
                            YPosition = mousePosition.Y
                        };
                        _dbContext.Add(clickRecord);
                    }

                    _dbContext.SaveChanges();
                }
            };
            _bufferFlushTimer.Start();
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _inputPollTimer.Stop();
            _bufferFlushTimer.Stop();

            return Task.CompletedTask;
        }

        ~InputMonitorWorkerService()
        {
            Dispose();
        }

        public void Dispose()
        {
            _inputPollTimer?.Dispose();
            _bufferFlushTimer?.Dispose();
            _scope?.Dispose();
        }
    }
}