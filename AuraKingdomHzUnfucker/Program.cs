using AuraKingdomHzUnfucker.Structs;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AuraKingdomHzUnfucker
{

    public class Program
    {
        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumDisplaySettings([param: MarshalAs(UnmanagedType.LPTStr)] string lpszDeviceName, [param: MarshalAs(UnmanagedType.U4)] int iModeNum, [In, Out] ref DEVMODE lpDevMode);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern int ChangeDisplaySettings([In, Out] ref DEVMODE lpDevMode, [param: MarshalAs(UnmanagedType.U4)] uint dwflags);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        const int ENUM_CURRENT_SETTINGS = -1;

        const int ENUM_REGISTRY_SETTINGS = -2;



        public static void Main(string[] args)
        {
            Console.Title = "AuraKingdomHzUnfucker by Xepos";
            if (AlreadyRunning())
            {
                Exit();
            }

            HandleFlags(args, out bool keepOpen, out bool minimize, out bool hide);

            if (hide)
            {
                ShowWindow(GetConsoleWindow(), 0);
            }
            else if (minimize)
            {
                ShowWindow(GetConsoleWindow(), 6);
            }

            DEVMODE mode = new();
            mode.dmSize = (ushort)Marshal.SizeOf(mode);
            uint targetHz = 0;

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

                targetHz = mode.dmDisplayFrequency;
            }

            while (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref mode))
            {
                if (mode.dmDisplayFrequency != targetHz)
                {
                    mode.dmDisplayFrequency = targetHz;
                    ChangeDisplaySettings(ref mode, 0);
                    if (keepOpen) continue;

                    Exit();
                    break;
                }
            }
        }

        private static void HandleFlags(string[] args, out bool keepOpen, out bool minimize, out bool hide)
        {
            keepOpen = false;
            minimize = false;
            hide = false;

            foreach (var arg in args)
            {
                switch (arg.ToLower())
                {
                    case "-keep-open":
                    case "-ko":
                    case "-k":
                        keepOpen = true;
                        break;
                    case "-minimize":
                    case "-m":
                        minimize = true;
                        break;
                    case "-hide":
                    case "-h":
                        hide = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private static bool AlreadyRunning()
        {
            string name = Process.GetCurrentProcess().ProcessName;
            return Process.GetProcessesByName(name).Length > 1;
        }

        private static void Exit()
        {
            Environment.Exit(0);
        }

    }

}