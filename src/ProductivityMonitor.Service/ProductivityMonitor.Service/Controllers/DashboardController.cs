using System;

using Microsoft.AspNetCore.Mvc;

namespace ProductivityMonitor.Service.Controllers
{
	public class DashboardController : Controller
	{

		[HttpGet]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None,
			NoStore = true)]
		public IActionResult Index(DateTime? date) => View(date);

	}
}
