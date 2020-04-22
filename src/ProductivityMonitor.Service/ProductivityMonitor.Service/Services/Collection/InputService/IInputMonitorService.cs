using System.Collections.Generic;

namespace ProductivityMonitor.Service.Services.Collection.InputService
{
	public interface IInputMonitorService
	{
		IEnumerable<int> CheckInputReceived();

		bool IsClick(int keyCode);

		string GetKeyName(int keyCode);
	}
}
