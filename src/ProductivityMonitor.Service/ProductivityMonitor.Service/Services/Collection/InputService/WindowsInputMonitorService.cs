using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using ProductivityMonitor.Service.Utilities;

using static System.Enum;

namespace ProductivityMonitor.Service.Services.Collection.InputService
{
    public class WindowsInputMonitorService : IInputMonitorService
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const int KeyDownCode = -32767;

        public IEnumerable<int> CheckInputReceived()
            => GetValues(typeof(Keys))
               .Cast<int>()
               .Where(keyCode => GetAsyncKeyState(keyCode) == KeyDownCode)
               .Where(NotCombinationPress)
               .ToList();

        private bool IsClickInternal(Keys keyCode)
            => keyCode switch
            {
                Keys.LButton  => true,
                Keys.RButton  => true,
                Keys.MButton  => true,
                Keys.XButton1 => true,
                Keys.XButton2 => true,
                _             => false
            };

        public bool IsClick(int keyCode)
            => IsClickInternal((Keys) keyCode);

        private string GetKeyNameInternal(Keys keyCode)
            => keyCode switch

            {
                Keys.D0      => "0",
                Keys.NumPad0 => "0",
                Keys.D1      => "1",
                Keys.NumPad1 => "1",
                Keys.D2      => "2",
                Keys.NumPad2 => "2",
                Keys.D3      => "3",
                Keys.NumPad3 => "3",
                Keys.D4      => "4",
                Keys.NumPad4 => "4",
                Keys.D5      => "5",
                Keys.NumPad5 => "5",
                Keys.D6      => "6",
                Keys.NumPad6 => "6",
                Keys.D7      => "7",
                Keys.NumPad7 => "7",
                Keys.D8      => "8",
                Keys.NumPad8 => "8",
                Keys.D9      => "9",
                Keys.NumPad9 => "9",
                _            => GetName(typeof(Keys), keyCode)
            };

        public string GetKeyName(int keyCode)
            => GetKeyNameInternal((Keys) keyCode);

        private bool NotCombinationPress(int value)
            => (Keys) value switch
            {
                Keys.ShiftKey   => false,
                Keys.ControlKey => false,
                _               => true
            };
    }
}