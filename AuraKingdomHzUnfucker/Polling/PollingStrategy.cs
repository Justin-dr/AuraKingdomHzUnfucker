using AuraKingdomHzUnfucker.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AuraKingdomHzUnfucker.Polling
{
    public abstract class PollingStrategy
    {
        protected const int ENUM_CURRENT_SETTINGS = -1;

        protected readonly bool _keepOpen;
        public PollingStrategy(bool keepOpen)
        {
            _keepOpen = keepOpen;
        }
        public abstract Task StartPolling();

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumDisplaySettings([param: MarshalAs(UnmanagedType.LPTStr)] string lpszDeviceName, [param: MarshalAs(UnmanagedType.U4)] int iModeNum, [In, Out] ref DEVMODE lpDevMode);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern int ChangeDisplaySettings([In, Out] ref DEVMODE lpDevMode, [param: MarshalAs(UnmanagedType.U4)] uint dwflags);

        public uint GetTargetFrequency(ref DEVMODE mode)
        {
            if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref mode) == true)
            {
                Console.WriteLine("Current Mode:\n\t" +
                    "{0} by {1}, " +
                    "{2} bit, " +
                    "{3} degrees, " +
                    "{4} hertz",
                    mode.dmPelsWidth,
                    mode.dmPelsHeight,
                    mode.dmBitsPerPel,
                    mode.dmDisplayOrientation * 90,
                    mode.dmDisplayFrequency);

                return mode.dmDisplayFrequency;
            }

            throw new Exception();
        }
    }
}
