using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DollyProtocol
{
    public static class TargetObject
    {
        public const char Dolly = 'D';
        public const char Camera = 'C';
        public const char Motor = 'M';
    }

    public static class Command
    {
        public const char Get = 'G';
        public const char Set = 'S';
        public const char GetResponse = 'H';
        public const char SetResponse = 'T';
    }

    public static class Protocol
    {
        public const char SyncByte = 'X';
        public const char ValueSeparator = ':';
    }
}
