using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYG.HoleDetect
{
    /// <summary>Device Type</summary>
    public enum DeviceType : byte
    {
        Area = 0x1,
        Platform = 0x2,
        Machine = 0x4,
        MHole = 0x8
    }

    /// <summary>Error Level</summary>
    public enum ErrorLevel : byte
    {
        Alarm = 0x1,
        Error = 0x2
    }
}
