using System.Runtime.InteropServices;
using ProductivityMonitor.Service.Utilities;

namespace ProductivityMonitor.Service.Services.Collection.MousePositionService
{
	public class WindowsMousePositionService : IMousePositionService
	{

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetCursorPos(out Point point);

		public Point GetMousePosition()
		{
			GetCursorPos(out var point);
			return point;
		}
	}
}
