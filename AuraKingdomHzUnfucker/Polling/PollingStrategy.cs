using AuraKingdomHzUnfucker.Data;
using AuraKingdomHzUnfucker.Enums;
using AuraKingdomHzUnfucker.Extensions;
using AuraKingdomHzUnfucker.Structs;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AuraKingdomHzUnfucker.Polling
{
    public abstract class PollingStrategy
    {
        protected delegate R FuncRef<R, T>(ref T item);

        protected IList<MonitorInfo> _monitors = new List<MonitorInfo>();
        internal readonly ApplicationFlags _flags;
        internal PollingStrategy(ApplicationFlags flags)
        {
            _flags = flags;
            InitMonitors();
        }
        public abstract Task StartPolling();

        #region User32

        [DllImport("User32.dll")]
        static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip,EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

        delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern int ChangeDisplaySettings([In, Out] ref DEVMODE lpDevMode, [param: MarshalAs(UnmanagedType.U4)] uint dwflags);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumDisplaySettings(byte[] lpszDeviceName, [param: MarshalAs(UnmanagedType.U4)] int iModeNum, [In, Out] ref DEVMODE lpDevMode);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        #endregion User32

        protected const int ENUM_CURRENT_SETTINGS = -1;

        public uint GetTargetFrequency(ref DEVMODE mode, MonitorInfo? monitorInfo, Monitors monitors)
        {
            if (EnumDisplaySettings(monitors == Monitors.Current ? null : monitorInfo.Id, ENUM_CURRENT_SETTINGS, ref mode) == true)
            {
                string monitorName = monitors == Monitors.Current ? string.Empty : monitorInfo.Name + " ";
                Console.WriteLine(
                    $"{monitorName}Current Mode:\n\t" +
                    $"{mode.dmPelsWidth} by {mode.dmPelsHeight}, " +
                    $"{mode.dmBitsPerPel} bit, " +
                    $"{mode.dmDisplayOrientation * 90} degrees, " +
                    $"{mode.dmDisplayFrequency} hertz"
                );

                return mode.dmDisplayFrequency;
            }

            throw new Exception();
        }

        private void InitMonitors()
        {
            bool initCompleted = EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
            {
                MONITORINFOEX mon_info = new MONITORINFOEX();
                mon_info.Size = (uint)Marshal.SizeOf(mon_info);
                bool success = GetMonitorInfo(hMonitor, ref mon_info);

                if (success)
                {
                    string name = mon_info.DeviceName;
                    if (!string.IsNullOrEmpty(name))
                    {
                        DEVMODE mode = new DEVMODE();
                        mode.dmSize = (ushort)Marshal.SizeOf(mode);

                        byte[] nameBytes = ToLPTStr(name);
                        MonitorInfo monitorInfo = new MonitorInfo(nameBytes, name.RemoveSpecialCharacters());

                        uint hz = GetTargetFrequency(ref mode, monitorInfo, _flags.Monitors);
                        monitorInfo.TargetHz = hz;

                        _monitors.Add(monitorInfo);
                        return true;
                    }
                    return false;
                }

                return true;
            }, IntPtr.Zero);

            if (!initCompleted)
            {
                throw new Exception();
            }
        }

        private bool UpdateAllMonitors(ref DEVMODE mode)
        {
            bool anyChanged = false;
            foreach (MonitorInfo monitor in _monitors)
            {
                if (EnumDisplaySettings(monitor.Id, ENUM_CURRENT_SETTINGS, ref mode))
                {
                    if (mode.dmDisplayFrequency != monitor.TargetHz)
                    {
                        mode.dmDisplayFrequency = monitor.TargetHz;
                        ChangeDisplaySettings(ref mode, 0);
                        anyChanged = true;
                    }
                }
            }
            return anyChanged;
        }

        private bool UpdateSingleMonitor(ref DEVMODE mode)
        {
            if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref mode))
            {
                MonitorInfo monitor = _monitors[0];
                if (mode.dmDisplayFrequency != monitor.TargetHz)
                {
                    mode.dmDisplayFrequency = monitor.TargetHz;
                    ChangeDisplaySettings(ref mode, 0);
                    return true;
                }
            }
            return false;
        }

        protected FuncRef<bool, DEVMODE> GetSettingsAction()
        {
            return _flags.Monitors == Enums.Monitors.All ? UpdateAllMonitors : UpdateSingleMonitor;
        }

        public static byte[] ToLPTStr(string str)
        {
            var lptArray = new byte[str.Length + 1];

            var index = 0;
            foreach (char c in str.ToCharArray())
                lptArray[index++] = Convert.ToByte(c);

            lptArray[index] = Convert.ToByte('\0');

            return lptArray;
        }

        protected static DEVMODE CreateDevMode()
        {
            DEVMODE mode = new();
            mode.dmSize = (ushort)Marshal.SizeOf(mode);

            return mode;
        }
    }
}
