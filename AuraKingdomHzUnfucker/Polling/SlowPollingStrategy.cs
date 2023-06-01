﻿using AuraKingdomHzUnfucker.Data;
using AuraKingdomHzUnfucker.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AuraKingdomHzUnfucker.Polling
{
    internal class SlowPollingStrategy : PollingStrategy
    {
        private readonly PeriodicTimer timer;
        public SlowPollingStrategy(ApplicationFlags flags) : base(flags)
        {
            timer = new PeriodicTimer(TimeSpan.FromSeconds(_flags.PollingDelay));
        }

        public override async Task StartPolling()
        {
            DEVMODE mode = CreateDevMode();
            FuncRef<bool, DEVMODE> actionRef = GetSettingsAction();

            Console.WriteLine("Running with polling delay: " + _flags.PollingDelay + " second" + (_flags.PollingDelay == 1 ? "." : "s."));

            while (await timer.WaitForNextTickAsync())
            {
                bool anyChange = actionRef.Invoke(ref mode);
                if (!_flags.KeepOpen && anyChange)
                {
                    Program.Exit();
                    break;
                }

                continue;
            }
        }

        
    }
}
