using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dands.Modbus
{
    /// <summary>
    /// Modbus Connect status changed argument
    /// </summary>
    public class ConnectChangedArgument : System.EventArgs
    {
        private ConnectType connectType = ConnectType.TCP;
        private IPEndPoint remoteIPEndPoint = null;
        private SerialPort serialPort = null;
        private DateTime? connectedTime = null;
        private DateTime? disconnectedTime = null;
        private string disconnectReason = "";
        private bool isConnected = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~ConnectChangedArgument()
        {
            this.RemoteIPEndPoint = null;
            this.SerialPort = null;
        }

        /// <summary> Modbus Connect Type</summary>
        public ConnectType ConnectType
        {
            get { return this.connectType; }
            set { this.connectType = value; }
        }

        /// <summary> Modbus Remote IP EndPoint (for TCP/UDP)</summary>
        public IPEndPoint RemoteIPEndPoint
        {
            get { return this.remoteIPEndPoint; }
            set { this.remoteIPEndPoint = value; }
        }

        /// <summary> Modbus Serial Port (for RTU/ASCII)</summary>
        public SerialPort SerialPort
        {
            get { return this.serialPort; }
            set { this.serialPort = value; }
        }

        /// <summary>Connected Time</summary>
        public DateTime? ConnectedTime
        {
            get { return this.connectedTime; }
            set { this.connectedTime = value; }
        }

        /// <summary>Disonnected Time</summary>
        public DateTime? DisconnectedTime
        {
            get { return this.disconnectedTime; }
            set { this.disconnectedTime = value; }
        }

        /// <summary>Disconnect Reason</summary>
        public string DisconnectReason
        {
            get { return this.disconnectReason; }
            set { this.disconnectReason = value; }
        }

        /// <summary>Connect Status</summary>
        public bool IsConnected
        {
            get { return this.isConnected; }
            set { this.isConnected = value; }
        }
    }

    /// <summary>
    /// Modbus Alarm Happened argument
    /// </summary>
    public class AlarmHappenedArgument : System.EventArgs
    {
        /// <summary>
        /// Destructor
        /// </summary>
        ~AlarmHappenedArgument()
        {
            this.RemoteIPEndPoint = null;
            this.SerialPort = null;
            this.AlarmMessage = null;
        }
        /// <summary> Modbus Connect Type</summary>
        public ConnectType ConnectType = ConnectType.TCP;
        /// <summary> Modbus Remote IP EndPoint (for TCP/UDP)</summary>
        public IPEndPoint RemoteIPEndPoint = null;
        /// <summary> Modbus Serial Port (for RTU/ASCII)</summary>
        public SerialPort SerialPort = null;
        /// <summary>Alarm Message</summary>
        public string AlarmMessage = "";
    }

}
