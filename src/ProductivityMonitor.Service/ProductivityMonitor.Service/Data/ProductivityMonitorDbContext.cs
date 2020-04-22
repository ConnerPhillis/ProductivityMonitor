using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductivityMonitor.Service.Data.Models;

namespace ProductivityMonitor.Service.Data
{
	public class ProductivityMonitorDbContext : DbContext
	{
		public DbSet<ApplicationRecord> ApplicationRecords { get; set; }

		public DbSet<KeyboardInputRecord> KeyboardInputRecords { get; set; }

		public DbSet<MouseInputRecord> MouseInputRecords { get; set; }

		public ProductivityMonitorDbContext(
			DbContextOptions options)
			: base(options)
		{
			
		}
	}
}
