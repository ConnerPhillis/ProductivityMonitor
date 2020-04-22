namespace ProductivityMonitor.Service.Data.Models
{
	public class MouseInputRecord : AbstractInputRecord
	{
		public int XPosition { get; set; }
		
		public int YPosition { get; set; }

		public bool IsClick { get; set; } = false;

        public ApplicationRecord ActiveApplication { get; set; }

        public int ActiveApplicationId { get; set; }
	}
}