using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dands.Modbus
{
    /// <summary>
    /// Modbus Connect Type
    /// </summary>
    public enum ConnectType : byte
    {
        TCP = 0x1,
        UDP = 0x2,
        RTU = 0x10,
        ASCII = 0x20
    }

}
