using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Dands.Modbus;

namespace TYG.HoleDetect
{

    /// <summary>
    /// Platform Hole Detection Changed Event Argument
    /// </summary>
    public class HoleChangedArgument : System.EventArgs
    {
        private string strFactoryID, strAreaID, strPlatFormID, strMHoleID, strMachineID;
        private string[] holeDetection = null;
        private bool[] holeStatus = null;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paramFactoryID">Factory ID</param>
        /// <param name="paramAreaID">Area ID</param>
        /// <param name="paramPlatFormID">PlatForm ID</param>
        internal HoleChangedArgument(string paramFactoryID, string paramAreaID, string paramPlatFormID, string paramMHoleID = null,string paramMachineID = null)
        {
            this.strFactoryID = paramFactoryID;
            this.strAreaID = paramAreaID;
            this.strPlatFormID = paramPlatFormID;
            this.strMHoleID = paramMHoleID;
            this.strMachineID = paramMachineID;
        }

        /// <summary>Factory ID</summary>
        public string FactoryID { get { return this.strFactoryID; } }

        /// <summary>Area ID</summary>
        public string AreaID { get { return this.strAreaID; } }

        /// <summary>PlatForm ID</summary>
        public string PlatFormID { get { return this.strPlatFormID; } }

        /// <summary>PlatForm ID</summary>
        public string MHoleID { get { return this.strMHoleID; } }

        /// <summary>PlatForm ID</summary>
        public string MachineID { get { return this.strMachineID; } }

        /// <summary>
        /// Hole Detection Array
        /// </summary>
        public string[] HoleDetection
        {
            get { return this.holeDetection; }
            internal set { this.holeDetection = value; }
        }

        /// <summary>
        /// Hole Status Array
        /// </summary>
        public bool[] HoleStatus
        {
            get { return this.holeStatus; }
            internal set { this.holeStatus = value; }
        }
    }

    /// <summary>
    /// Machine DAQ Data Changed Event Argument
    /// </summary>
    public class DataChangedArgument : System.EventArgs
    {
        private string strFactoryID, strAreaID, strPlatFormID, strMachineID;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paramFactoryID">Factory ID</param>
        /// <param name="paramAreaID">Area ID</param>
        /// <param name="paramPlatFormID">PlatForm ID</param>
        /// <param name="paramMachineID">Machine ID</param>
        internal DataChangedArgument(string paramFactoryID, string paramAreaID, string paramPlatFormID, string paramMachineID)
        {
            this.strFactoryID = paramFactoryID;
            this.strAreaID = paramAreaID;
            this.strPlatFormID = paramPlatFormID;
            this.strMachineID = paramMachineID;
        }

        /// <summary>Factory ID</summary>
        public string FactoryID { get { return this.strFactoryID; } }

        /// <summary>Area ID</summary>
        public string AreaID { get { return this.strAreaID; } }

        /// <summary>PlatForm ID</summary>
        public string PlatFormID { get { return this.strPlatFormID; } }

        /// <summary>Machine ID</summary>
        public string MachineID { get { return this.strMachineID; } }

    }

    /// <summary>
    /// Machine Material Check Event Argument
    /// </summary>
    public class MaterialCheckArgument : DataChangedArgument
    {
        private string strWorkOrderNo = "";
        private string strHoleID = "";
        private bool forbidMotor = false;
        private int lampSignal = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paramFactoryID">Factory ID</param>
        /// <param name="paramAreaID">Area ID</param>
        /// <param name="paramPlatFormID">PlatForm IDparam>
        /// <param name="paramMachineID">Machine ID</param>
        internal MaterialCheckArgument(string paramFactoryID, string paramAreaID, string paramPlatFormID, string paramMachineID) :
                                                              base(paramFactoryID,  paramAreaID,  paramPlatFormID, paramMachineID)
        { }


        /// <summary>Work Order No.</summary>
        public string WorkOrderNo { get { return this.strWorkOrderNo; } }

        /// <summary>Hole ID</summary>
        public string HoleID { get { return this.strHoleID; } }

        /// <summary>Forbid Motor Run</summary>
        /// <remark>Feedback after Material Check</remark>
        public bool ForbidMotor
        {
            get { return this.forbidMotor; }
            set { this.forbidMotor = value; }
        }

        /// <summary>Set Lamp Signal</summary>
        /// <remark>Feedback after Material Check</remark>
        public int LampSignal
        {
            get { return this.lampSignal; }
            set { this.lampSignal = value; }
        }
    }


    /// <summary>
    /// Modbus Connect status changed argument
    /// </summary>
    public class ModbusConnectChangedArgument : ConnectChangedArgument
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="e"></param>
        internal ModbusConnectChangedArgument(DeviceType paramType, ConnectChangedArgument e) : base()
        {
            this.RemoteIPEndPoint = e.RemoteIPEndPoint;
            this.ConnectedTime = e.ConnectedTime;
            this.DisconnectedTime = e.DisconnectedTime;
            this.DisconnectReason = e.DisconnectReason;
            this.IsConnected = e.IsConnected;
            this.ModbusType = paramType;
        }

        /// <summary>Modbus Device Type</summary>
        public DeviceType ModbusType;
    }

    /// <summary>
    /// Modbus Alarm Happened argument
    /// </summary>
    public class ModbusAlarmHappenedArgument : AlarmHappenedArgument
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paramType">ModbusType</param>
        /// <param name="senderID">Sender ID</param>
        /// <param name="message">Message Information</param>
        /// <param name="errorLevel">Error Level</param>
        public ModbusAlarmHappenedArgument(DeviceType paramType, string senderID, string message, ErrorLevel errorLevel)
        {
            this.SenderID = senderID;
            this.AlarmMessage = message;
            this.ModbusType = paramType;
            this.ErrorLevel = errorLevel;
        }

        /// <summary>Modbus Device Type</summary>
        public DeviceType ModbusType;
        /// <summary>Modbus Device Type</summary>
        public string SenderID;
        /// <summary>Error Level</summary>
        public ErrorLevel ErrorLevel;
    }
}
