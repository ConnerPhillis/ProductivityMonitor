using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using ProductivityMonitor.Service.Data;
using ProductivityMonitor.Service.Data.Models;
using ProductivityMonitor.Service.Utilities;

namespace ProductivityMonitor.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly ProductivityMonitorDbContext _dbContext;

        public DataController(ProductivityMonitorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("applications/")]
        public async Task<IActionResult> Applications(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.MinValue;
            endDate ??= DateTime.MaxValue;

            var applications = GetApplicationRecords(startDate.Value, endDate.Value)
               .AsNoTracking();

            var applicationList = await applications.ToListAsync();

            return Ok(applicationList);
        }

        [HttpGet("applications/productivity")]
        public async Task<IActionResult> ApplicationProductivityClean(
            DateTime? startDate,
            DateTime? endDate)
        {
            startDate ??= DateTime.MinValue;
            endDate ??= DateTime.MaxValue;

            var applicationRecords = await GetApplicationRecords(startDate.Value, endDate.Value)
               .Include(value => value.KeyboardInputs)
               .Include(value => value.MouseInputs)
               .Select(
                    value => new
                    {
                        value.ApplicationName,
                        TimePoint = value.RecordDate,
                        Productive = value.MouseInputs.Any() || value.KeyboardInputs.Any()
                    })
               .AsNoTracking()
               .ToListAsync();

            var cleanRecords = applicationRecords.AsParallel()
               .GroupBy(value => value.ApplicationName)
               .Select(
                    value => new
                    {
                        ApplicationName = value.Key,
                        TotalSeconds = value.Count(),
                        ProductiveSeconds = value.Count(record => record.Productive)
                    });

            return Ok(cleanRecords.ToList());
        }


        private IQueryable<ApplicationRecord> GetApplicationRecords(
            DateTime startDate,
            DateTime endDate)
            => from application in _dbContext.ApplicationRecords
                where application.RecordDate >= startDate && application.RecordDate <= endDate
                select application;

        [HttpGet("KeyPresses/Summary")]
        public async Task<IActionResult> KeyPressSummary(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.MinValue;
            endDate ??= DateTime.MaxValue;

            var results = (from keyPress in GetInputRecords(startDate.Value, endDate.Value)
                    where keyPress.KeyPressed != "None"
                    group keyPress by keyPress.KeyPressed into keyPressGroups
                    select new {keyPressGroups.Key, Count = keyPressGroups.Count()})
               .OrderByDescending(value => value.Count)
               .AsNoTracking();

            var resultList = await results.ToListAsync();

            return Ok(resultList);
        }

        private IQueryable<KeyboardInputRecord> GetInputRecords(
            DateTime startDate,
            DateTime endDate)
            => from keyPress in _dbContext.KeyboardInputRecords
                where keyPress.RecordDate >= startDate && keyPress.RecordDate <= endDate
                select keyPress;


        [HttpGet("mouse/movements/totalDistance")]
        public async Task<IActionResult> TotalMouseMovementDistance(
            DateTime? startDate,
            DateTime? endDate)
        {
            startDate ??= DateTime.MinValue;
            endDate ??= DateTime.MaxValue;

            var mouseMovements = await GetMouseInputRecords(startDate.Value, endDate.Value)
               .AsNoTracking()
               .ToListAsync();

            double totalDistance = 0.0;

            for (int i = 1; i < mouseMovements.Count; i++)
            {
                var sideX = mouseMovements[i - 1]
                       .XPosition
                    - mouseMovements[i]
                       .XPosition;
                var sideY = mouseMovements[i - 1]
                       .YPosition
                    - mouseMovements[i]
                       .YPosition;

                sideX *= sideX;
                sideY *= sideY;

                totalDistance += Math.Sqrt(sideX + sideY);
            }

            return Ok(totalDistance);
        }

        [HttpGet("mouse/clicks/summary")]
        public async Task<IActionResult> MouseClicksSummary(
            DateTime? startDate,
            DateTime? endDate,
            int quadrantSizePx = 100)
        {
            startDate ??= DateTime.MinValue;
            endDate ??= DateTime.MaxValue;

            int quadrantSizeDivider = quadrantSizePx / 2;

            var clicks = (from mouseMovement in GetMouseInputRecords(startDate.Value, endDate.Value)
                where mouseMovement.IsClick
                group mouseMovement by new
                {
                    XPosition =
                        mouseMovement.XPosition / quadrantSizePx * quadrantSizePx
                        + quadrantSizeDivider,
                    YPosition = mouseMovement.YPosition / quadrantSizePx * quadrantSizePx
                        + quadrantSizeDivider
                } into groupedMovements
                select new
                {
                    groupedMovements.Key.XPosition,
                    groupedMovements.Key.YPosition,
                    Count = groupedMovements.Count()
                }).OrderByDescending(value => value.Count);

            var clickList = await clicks.AsNoTracking()
               .ToListAsync();

            return Ok(clickList);
        }

        private IQueryable<MouseInputRecord> GetMouseInputRecords(
            DateTime startDate,
            DateTime endDate)
            => from movement in _dbContext.MouseInputRecords
                where movement.RecordDate >= startDate && movement.RecordDate <= endDate
                select movement;
    }
}