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

            HandleFlags(args, out bool keepOpen, out bool minimize, out bool hide, out double pollingDelay);

            PollingStrategy pollingStrategy = pollingDelay <= 0 ? new FastPollingStrategy(keepOpen) : new SlowPollingStrategy(keepOpen, pollingDelay);

            if (hide)
            {
                ShowWindow(GetConsoleWindow(), 0);
            }
            else if (minimize)
            {
                ShowWindow(GetConsoleWindow(), 6);
            }

            await pollingStrategy.StartPolling();
        }

        private static void HandleFlags(string[] args, out bool keepOpen, out bool minimize, out bool hide, out double pollingDelay)
        {
            keepOpen = false;
            minimize = false;
            hide = false;
            pollingDelay = -1L;

            foreach (var arg in args)
            {
                if (arg == "-keep-open" || arg == "-ko" || arg == "-k")
                {
                    keepOpen = true;
                }
                else if (!hide && (arg == "-minimize" || arg == "-m"))
                {
                    minimize = true;
                }
                else if (!minimize && (arg == "-hide" || arg == "-h"))
                {
                    hide = true;
                }
                else if (pollingDelay <= 0 && (arg.StartsWith("-delay:") || arg.StartsWith("-d:")))
                {
                    string[] delaySplit = arg.Split(':');
                    if (!double.TryParse(delaySplit[1].Replace('.', ','), out double result))
                    {
                        throw new FormatException("Could not parse " + delaySplit[1] + " to a number!");
                    }
                    pollingDelay = result;
                }
            }
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