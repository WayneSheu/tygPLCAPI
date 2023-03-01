using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dands.Modbus;
using MachineDict = System.Collections.Generic.Dictionary<int, TYG.HoleDetect.Machine>;
using PlatformDict = System.Collections.Generic.Dictionary<string, TYG.HoleDetect.Platform>;
using MHoleDict = System.Collections.Generic.Dictionary<string, TYG.HoleDetect.MHole>;
using System.Threading;

namespace TYG.HoleDetect
{
    /// <summary>Data Acquistion for Area</summary>
    public class Area : IDisposable
    {

        #region Private Variables

        private string strID = "";
        private string strName = "";
        private string strIPAddress = "";
        private int port = 0;
        private bool isStarted = false;
        private bool isStartFault = false;
        private bool testMode = true;

        private Factory parent = null;
        private MasterPLC masterPLC = null;
        private MachineDict machines = new MachineDict();
        private PlatformDict platForms = new PlatformDict();
        private MHoleDict mholes = new MHoleDict();
        private DateTime? connectedTime;
        private DateTime? disConnectedTime;
        private DateTime refreshTime = DateTime.Now;

        #endregion

        #region Events

        /// <summary>
        /// Connected Status Changed Event
        /// </summary>
        internal event ModbusConnectChangedEventHandler ConnectChanged;

        /// <summary>
        /// Alarm Happened Event
        /// </summary>
        internal event ModbusAlarmHappenedEventHandler AlarmHappened;

        #endregion


        #region Constructor/Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Parent Object (Factory)</param>
        /// <param name="paramID">Area ID</param>
        /// <param name="paramName">Area Name</param>
        /// <param name="paramIPAddress">Master PLC IP Address</param>
        /// <param name="paramPort">Master PLC TCP Port No.</param>
        internal Area(Factory parent, string paramID, string paramName, string paramIPAddress, int paramPort, bool paraTestMode)
        {
            this.parent = parent;
            this.strID = paramID;
            this.strName = paramName;
            this.strIPAddress = paramIPAddress;
            this.port = paramPort;
            this.testMode = paraTestMode;
            this.masterPLC = new MasterPLC(this);
            this.masterPLC.ConnectChanged += Area_ConnectChanged;
            this.masterPLC.AlarmHappened += Area_AlarmHappened;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Area()
        {
            this.Dispose();
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Area ID
        /// </summary>
        public string ID
        {
            get { return this.strID; }
        }

        /// <summary>
        /// Area Name
        /// </summary>
        public string Name
        {
            get { return this.strName; }
        }

        /// <summary>
        /// Parent Object (Factory)
        /// </summary>
        public Factory Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// Master PLC IP Address
        /// </summary>
        public string IPAddress
        {
            get { return this.strIPAddress; }
        }

        /// <summary>
        /// Master PLC TCP Port No.
        /// </summary>
        public int Port
        {
            get { return this.port; }
        }

        /// <summary>
        /// Master PLC Read Started or not
        /// </summary>
        public bool Started
        {
            get { return this.isStarted; }
        }

        /// <summary>
        /// Master PLC Connect Status
        /// </summary>
        public bool Connected
        {
            get { return this.masterPLC.Connected; }
        }

        /// <summary>
        /// Test Mode
        /// </summary>
        public bool TestMode
        {
            get { return this.testMode; }
            set
            {
                if (this.testMode != value)
                {
                    foreach(Machine machine in this.machines.Values)
                    {
                        machine.SetTestMode(this.testMode);
                    }
                }
            }
        }

        /// <summary>
        /// Master PLC Connected Time
        /// </summary>
        public DateTime? ConnectedTime
        {
            get { return this.connectedTime; }
        }

        /// <summary>
        /// Master PLC Disconnected Time
        /// </summary>
        public DateTime? DisconnectedTime
        {
            get { return this.disConnectedTime; }
        }

        /// <summary>
        /// Platform Key Collection
        /// </summary>
        public PlatformDict.KeyCollection PlatformKeys
        {
            get { return this.platForms.Keys; }
        }

        /// <summary>
        /// Machine Key Collection
        /// </summary>
        public MachineDict.KeyCollection MachineKeys
        {
            get { return this.machines.Keys; }
        }

        /// <summary>
        /// Machine Key Collection
        /// </summary>
        public MHoleDict.KeyCollection MHoleKeys
        {
            get { return this.mholes.Keys; }
        }

        /// <summary>
        /// Data Refresh Time
        /// </summary>
        public DateTime RefreshTime
        {
            get { return this.refreshTime; }
        }

        #endregion

        #region Internal Property

        /// <summary>
        /// Get PlatForm Dictionary
        /// </summary>
        internal PlatformDict PlatForms
        {
            get { return this.platForms; }
        }

        /// <summary>
        /// Get Machine Dictionary
        /// </summary>
        internal MachineDict Machines
        {
            get { return this.machines; }
        }

        /// <summary>
        /// Get MHole Dictionary
        /// </summary>
        internal MHoleDict MHoles
        {
            get { return this.mholes; }
        }

        /// <summary>
        /// Master PLC Object
        /// </summary>
        internal MasterPLC MasterPLC
        {
            get { return this.masterPLC; }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Add Platform
        /// </summary>
        /// <param name="paramPlatFormID">PlatForm ID</param>
        /// <param name="paramPlatFormName">PlatForm Name</param>
        /// <param name="paramIPAddress">Remote I/O IP Address</param>
        /// <param name="paramPort">Remote I/O TCP Port No.</param>
        public void AddPlatForm(string paramPlatFormID, string paramPlatFormName, string paramIPAddress, int paramPort)
        {
            if ((!this.isStartFault) && (!this.isStarted))
            {
                Platform platform = new Platform(this, paramPlatFormID, paramPlatFormName, paramIPAddress, paramPort);
                this.platForms[paramPlatFormID] = platform;
            }
        }

        /// <summary>
        /// Get Platform Object
        /// </summary>
        /// <param name="paramPlatFormID">PlatForm ID</param>
        /// <returns>Platform Object (Not found return null)</returns>
        public Platform GetPlatform(string paramPlatFormID)
        {
            Platform platform = null;
            if (this.platForms.ContainsKey(paramPlatFormID))
                platform = this.platForms[paramPlatFormID];
            return platform;
        }

        /// <summary>
        /// Add Area
        /// </summary>
        /// <param name="paramMachineID">Machine ID</param>
        /// <param name="paramSlaveNo">Machine PLC Slave No.</param>
        /// <param name="paramPlatFormID">PlatForm ID</param>
        /// <param name="paramIOChannelNo">Remote I/O Channel No.</param>
        /// <param name="paramTestMode">Test Mode (No forbid motor output)</param>
        public void AddMachine(string paramMachineID, int paramSlaveNo, string paramPlatFormID, int paramIOChannelNo, bool paramTestMode, string paramMHoleID)
        {
            if ((!this.isStartFault) && (!this.isStarted))
            { 
                Machine machine = new Machine(this, paramMachineID, paramSlaveNo, paramPlatFormID, paramIOChannelNo, paramTestMode, paramMHoleID);
                this.machines[paramSlaveNo] = machine;
                if (this.platForms.ContainsKey(paramPlatFormID))
                    this.platForms[paramPlatFormID].Machines[paramIOChannelNo] = machine;
            }
        }

        /// <summary>
        /// Get Machine Object
        /// </summary>
        /// <param name="paramSlaveNo">Machine PLC Slave No.</param>
        /// <returns>Machine Object (Not found return null)</returns>
        public Machine GetMachine(int paramSlaveNo)
        {
            Machine machine = null;
            if (this.machines.ContainsKey(paramSlaveNo))
                machine = this.machines[paramSlaveNo];
            return machine;
        }

        /// <summary>
        /// Get Machine Object
        /// </summary>
        /// <param name="paramMachineID">Machine ID</param>
        /// <returns>Machine Object (Not found return null)</returns>
        public Machine GetMachine(string paramMachineID)
        {
            Machine machine = null;
            foreach (Machine obj in this.machines.Values)
            {
                if (paramMachineID == obj.ID)
                {
                    machine = obj;
                    break;
                }
            }
            return machine;
        }

        /// <summary>
        /// Set PLC Lamp Condition
        /// </summary>
        /// <param name="paramSlaveNo">Machine slave no.</param>
        /// <param name="paramLampValue">PLC Lamp condition setting</param>
        /// <returns>Is Sucess</returns>
        public bool SetSlavePlcLamp(int paramSlaveNo, int paramLampValue)
        {
            return this.masterPLC.SetSlavePlcLamp(paramSlaveNo, paramLampValue);
        }

        /// <summary>
        /// Set PLC Forbid Motor
        /// </summary>
        /// <param name="paramSlaveNo">Machine slave no.</param>
        /// <param name="paramForbidMotor">PLC Forbid Motor setting</param>
        /// <returns>Is Sucess</returns>
        public bool SetMotorForbid(int paramSlaveNo, bool paramForbidMotor)
        {
            return this.masterPLC.SetMotorForbid(paramSlaveNo, paramForbidMotor);
        }

        /// <summary>
        /// Update MPile Materials
        /// </summary>
        /// <param name="paramOldMaterials">Existing Material List</param>
        /// <param name="paramOldMRatio">Existing Material Ratio</param>
        /// <param name="paramNewMaterials">New Material List</param>
        /// <param name="paramNewMRatio">New Material Ratio</param>
        public void UpdateMaterialData(string[] paramOldMaterials, string paramOldMRatio, string[] paramNewMaterials, string paramNewMRatio)
        {
            foreach (Platform platform in this.PlatForms.Values)
            {
                platform.UpdateMaterialData(paramOldMaterials, paramOldMRatio, paramNewMaterials, paramNewMRatio);
            }
        }


        #endregion

        #region Internal Method

        /// <summary>
        /// Start Data Collection
        /// </summary>
        internal void Start()
        {
            string strError = "";
            if (!this.isStarted)
            {
                if ((this.IPAddress.Length == 0) || (this.Port == 0))
                    strError = "Master PLC IP Address or TCP Port not setting !!";
                else if (this.machines.Count == 0)
                    strError = "Machine PLC Slave No. not setting !!";
                else
                {
                    this.masterPLC.Start();
                    this.isStarted = true;
                }

                foreach (Platform platform in this.platForms.Values)
                    platform.Start();
                foreach (Machine machine in this.Machines.Values)
                    machine.Start();
               

                if (strError.Length > 0)
                {
                    this.isStartFault = true;
                    if (this.AlarmHappened != null)
                        AlarmHappened(this, new ModbusAlarmHappenedArgument(DeviceType.Area, this.ID, strError, ErrorLevel.Error));
                }
            }
        }

        /// <summary>
        /// Stop Data Collection
        /// </summary>
        internal void Stop()
        {
            if (this.isStarted)
            {
                foreach (Machine machine in this.Machines.Values)
                    machine.Stop();
                foreach (Platform platform in this.platForms.Values)
                    platform.Stop();
            }

            if (this.isStarted)
            {
                this.masterPLC.Stop();
                this.isStarted = false;
            }
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// Area alarm happened event handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void Area_AlarmHappened(object Sender, ModbusAlarmHappenedArgument e)
        {
            if (this.AlarmHappened != null)
                AlarmHappened(this, e);
        }

        /// <summary>
        /// Area connect changed event handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void Area_ConnectChanged(object Sender, ModbusConnectChangedArgument e)
        {
            this.connectedTime = e.ConnectedTime;
            this.disConnectedTime = e.DisconnectedTime;
            this.refreshTime = DateTime.Now;
            if (this.ConnectChanged != null)
                ConnectChanged(this, e);
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // 偵測多餘的呼叫

        /// <summary>
        /// Dispose (IDisposable)
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Stop();
                    this.masterPLC.Dispose();
                    this.masterPLC = null;
                    foreach (Platform platform in this.platForms.Values)
                        platform.Dispose();
                    this.platForms.Clear();
                    this.platForms = null;
                    foreach (Machine machine in this.machines.Values)
                        machine.Dispose();
                    this.machines.Clear();
                    this.machines = null;
                    this.parent = null;
                    this.strIPAddress = null;
                    this.strName = null;
                    this.strID = null;
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose (IDisposable)
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion


    }
}
