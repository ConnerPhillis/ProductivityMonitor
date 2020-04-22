using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ProductivityMonitor.Service.Services.Collection.ApplicationService
{
	public class WindowsApplicationService : IApplicationService
	{

		private const string UserDll = "user32.dll";

		public Process GetFocusedProcess()
		{
			var hWnd = GetForegroundWindow();

			GetWindowThreadProcessId(hWnd, out var processId);

			return Process.GetProcessById(Convert.ToInt32(processId));
		}


		[DllImport(UserDll)]
		static extern IntPtr GetForegroundWindow();

		[DllImport(UserDll)]
		private static extern IntPtr GetWindowThreadProcessId(
			IntPtr hWnd,
			out uint lpdwProcessId);
	}
}
