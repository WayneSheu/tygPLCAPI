using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace Dands.Modbus
{
    internal class PInvoke
    {
        [DllImport("WININET", CharSet = CharSet.Auto)]
        internal static extern bool InternetGetConnectedState(ref InternetConnectionState lpdwFlags, int dwReserved);
        internal enum InternetConnectionState : int
        {
            INTERNET_CONNECTION_MODEM = 0x1,
            INTERNET_CONNECTION_LAN = 0x2,
            INTERNET_CONNECTION_PROXY = 0x4,
            INTERNET_RAS_INSTALLED = 0x10,
            INTERNET_CONNECTION_OFFLINE = 0x20,
            INTERNET_CONNECTION_CONFIGURED = 0x40
        }

    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    unsafe struct TcpKeepAlive
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 12)]
        public fixed byte Bytes[12];
        [System.Runtime.InteropServices.FieldOffset(0)]
        public uint Enable;
        [System.Runtime.InteropServices.FieldOffset(4)]
        public uint KeepAliveIdleTime;
        [System.Runtime.InteropServices.FieldOffset(8)]
        public uint KeepAliveInterval;
    }

}
