using AuraKingdomHzUnfucker.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AuraKingdomHzUnfucker.Polling
{
    [Obsolete]
    internal class FastPollingStrategy : PollingStrategy
    {
        public FastPollingStrategy(bool keepOpen) : base(keepOpen)
        {
        }

        public override async Task StartPolling()
        {
            DEVMODE mode = new();
            mode.dmSize = (ushort)Marshal.SizeOf(mode);
            uint targetHz = GetTargetFrequency(ref mode);

            while (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref mode))
            {
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
