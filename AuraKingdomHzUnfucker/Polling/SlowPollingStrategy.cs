using AuraKingdomHzUnfucker.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AuraKingdomHzUnfucker.Polling
{
    internal class SlowPollingStrategy : PollingStrategy
    {
        PeriodicTimer timer;
        public SlowPollingStrategy(bool keepOpen, double seconds) : base(keepOpen)
        {
            timer = new PeriodicTimer(TimeSpan.FromSeconds(seconds));
        }

        public override async Task StartPolling()
        {
            DEVMODE mode = new();
            mode.dmSize = (ushort)Marshal.SizeOf(mode);
            uint targetHz = GetTargetFrequency(ref mode);

            while (await timer.WaitForNextTickAsync() && EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref mode))
            {
                Console.WriteLine("Poll: " + mode.dmDisplayFrequency);
                if (mode.dmDisplayFrequency != targetHz)
                {
                    mode.dmDisplayFrequency = targetHz;
                    ChangeDisplaySettings(ref mode, 0);
                    if (_keepOpen) continue;

                    Program.Exit();
                    break;
                }
            }
        }
    }
}
