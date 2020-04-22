using System.Diagnostics;

namespace ProductivityMonitor.Service.Services.Collection.ApplicationService
{
	public interface IApplicationService
	{

		Process GetFocusedProcess();

	}
}
