using AuraKingdomHzUnfucker.Data;
using AuraKingdomHzUnfucker.Enums;
using AuraKingdomHzUnfucker.Polling;
using AuraKingdomHzUnfucker.Structs;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AuraKingdomHzUnfucker
{

    public class Program
    {
        [DllImport("Kernel32.dll")]
        internal static extern IntPtr GetConsoleWindow();
        [DllImport("User32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        public static async Task Main(string[] args)
        {
            Console.Title = "AuraKingdomHzUnfucker by Xepos v" + Assembly.GetEntryAssembly()?.GetName().Version ?? "null";
            if (AlreadyRunning())
            {
                Exit();
            }

            ApplicationFlags applicationFlags = HandleFlags(args);
            PollingStrategy pollingStrategy = new SlowPollingStrategy(applicationFlags);

            if (applicationFlags.WindowState == WindowState.Hide)
            {
                ShowWindow(GetConsoleWindow(), 0);
            }
            else if (applicationFlags.WindowState == WindowState.Minimize)
            {
                ShowWindow(GetConsoleWindow(), 6);
            }
            await pollingStrategy.StartPolling();
        }

        private static ApplicationFlags HandleFlags(string[] args)
        {
            bool keepOpen = false;
            WindowState windowState = WindowState.Normal;
            Monitors monitors = Monitors.Current;
            double pollingDelay = -1L;

            foreach (var arg in args)
            {
                if (arg == "--keep-open" || arg == "--ko" || arg == "-k")
                {
                    keepOpen = true;
                }
                else if (windowState != WindowState.Hide && (arg == "--minimize" || arg == "-m"))
                {
                    windowState = WindowState.Minimize;
                }
                else if (windowState != WindowState.Minimize && (arg == "--hide" || arg == "-h"))
                {
                    windowState = WindowState.Hide;
                }
                else if (pollingDelay <= 0 && (arg.StartsWith("--delay:") || arg.StartsWith("-d:")))
                {
                    string[] delaySplit = arg.Split(':');
                    if (!double.TryParse(delaySplit[1].Replace('.', ','), out double result))
                    {
                        throw new FormatException("Could not parse " + delaySplit[1] + " to a number!");
                    }
                    pollingDelay = result;
                }
                else if (arg == "-a" || arg == "--all")
                {
                    monitors = Monitors.All;
                }
            }

            return new ApplicationFlags(keepOpen, windowState, monitors, pollingDelay <= 0.1 ? 0.1 : pollingDelay);
        }

        private static bool AlreadyRunning()
        {
            string name = Process.GetCurrentProcess().ProcessName;
            return Process.GetProcessesByName(name).Length > 1;
        }

        internal static void Exit()
        {
            Environment.Exit(0);
        }

    }

}