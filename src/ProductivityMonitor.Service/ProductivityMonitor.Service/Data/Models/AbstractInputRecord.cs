using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductivityMonitor.Service.Data.Models
{
	public class AbstractInputRecord
	{
		[Key]
		public int Id { get; set; }

		public DateTime RecordDate { get; set; }
	}
}
