using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraKingdomHzUnfucker.Data
{
    public class MonitorInfo
    {
        internal byte[] Id { get; private init; }
        internal string Name { get; private init; }
        public uint TargetHz { get; internal set; }
        public MonitorInfo(byte[] id, string name, uint targetHz)
        {
            Id = id;
            Name = name;
            TargetHz = targetHz;
        }

        public MonitorInfo(byte[] id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
