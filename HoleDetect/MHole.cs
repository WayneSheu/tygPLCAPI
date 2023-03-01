using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TYG.HoleDetect
{
    public class MHole : IDisposable
    {
        #region Private Variables

        private Platform parent = null;
        private string holeId;
        private string strID = "";
        private int materialNo = 0;
        private string machineID = "";
        private bool isStarted = false;
        private List<string> materials = new List<string>();
        private string ratio = "";
        private DateTime refreshTime = DateTime.Now;
        private bool[] channelStatus = null;
        private RemoteIO remoteIO = null;
        private string strIPAddress = "";
        private int port = 0;
        private object lockHole = new object();
        private DateTime? connectedTime;
        private DateTime? disConnectedTime;

        #endregion

        #region Events

        /// <summary>
        /// Hole Changed Event
        /// </summary>
        internal event HoleChangedEventHandler HoleChanged;

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
        /// <param name="parent">Parent Object (Platform)</param>
        /// <param name="paramHoleId">MHole ID</paramSortNo>
        internal MHole(Platform parent, string paramHoleID, string paramIPAddress, int paramPort, string paramMachineID)
        {
            this.parent = parent;
            this.holeId = paramHoleID;
            this.strIPAddress = paramIPAddress;
            this.port = paramPort;
            this.machineID = paramMachineID;    
            this.channelStatus = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
            this.remoteIO = new RemoteIO(this);
            //this.strID += Convert.ToChar(paramHoleID + 64);
            this.remoteIO.ConnectChanged += MHole_ConnectChanged;
            this.remoteIO.AlarmHappened += MHole_AlarmHappened;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MHole()
        {
            this.Dispose();
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Parent Object (Platform)
        /// </summary>
        public Platform Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// Hole Id        /// </summary>
        public string HoleId
        {
            get { return this.holeId; }
        }

        public string MachineID
        {
            get { return this.machineID; }
        }

        /// <summary>
        /// MHole ID
        /// </summary>
        public string ID
        {
            get { return this.holeId; }
        }

        /// <summary>
        /// Materia lList
        /// </summary>
        public List<string> MaterialList
        {
            get { return this.materials; }
        }

        /// <summary>
        /// Material Ratio
        /// </summary>
        public string MaterialRatio
        {
            get { return this.ratio; }
        }
        /// <summary>
        /// Remote I/O IP Address
        /// </summary>
        public string IPAddress
        {
            get { return this.strIPAddress; }
        }

        /// <summary>
        /// Remote I/O TCP Port No.
        /// </summary>
        public int Port
        {
            get { return this.port; }
        }

        /// <summary>
        /// RemoteIO Check Started or not
        /// </summary>
        public bool Started
        {
            get { return this.isStarted; }
        }

        /// <summary>
        /// Remote I/O Connect Status
        /// </summary>
        public bool Connected
        {
            get { return this.remoteIO.Connected; }
        }

        /// <summary>
        /// Remote I/O Connected Time
        /// </summary>
        public DateTime? ConnectedTime
        {
            get { return this.connectedTime; }
        }

        /// <summary>
        /// Remote I/O Disconnected Time
        /// </summary>
        public DateTime? DisconnectedTime
        {
            get { return this.disConnectedTime; }
        }

        /// <summary>
        /// Data Refresh Time
        /// </summary>
        public DateTime RefreshTime
        {
            get { return this.refreshTime; }
            private set
            {
                if (this.refreshTime != value)
                {
                    this.refreshTime = value;
                    this.Parent.RefreshTime = this.refreshTime;
                }
            }
        }

        /// <summary>
        /// Hole Detection Array
        /// </summary>
        public bool[] ChannelStatus
        {
            get { return this.GetHoleStatus(); }
        }
        #endregion

        #region Internal Method

        /// <summary>
        /// Start RemoteIO Check
        /// </summary>
        /// <returns></returns>
        internal bool Start()
        {
            string strError = "";
            if (!this.Started)
            {

                if ((this.IPAddress.Length == 0) || (this.Port == 0))
                    strError = "Remote I/O IP Address or TCP port not setting !!";
                //else if (this..Count == 0)
                //    strError = "Remote I/O channel not setting !!";
                else
                {
                    this.remoteIO.Start2(this.HoleId);
                    this.isStarted = true;
                }

                if (strError.Length > 0)
                {
                    if (this.AlarmHappened != null)
                        AlarmHappened(this, new ModbusAlarmHappenedArgument(DeviceType.MHole, this.ID, strError, ErrorLevel.Error));
                }
            }
            return this.isStarted;
        }

        /// <summary>
        /// Stop RemoteIO Check
        /// </summary>
        internal void Stop()
        {
            if (this.isStarted)
            {
                this.isStarted = false;
                this.remoteIO.Stop();
            }
        }

        /// <summary>
        /// Set Hole Status Array
        /// </summary>
        /// <param name="paramHoleStatus">Hole Status Array</param>
        internal void SetHoleStatus(ref bool[] paramHoleStatus)
        {
            bool isChanged = false;
            lock (this.lockHole)
            {
                if (this.channelStatus.Length != paramHoleStatus.Length)
                    isChanged = true;
                else
                {
                    for (int index = 0; index < this.channelStatus.Length; index++)
                    {
                        if (!this.channelStatus[index].Equals(paramHoleStatus[index]))
                        {
                            isChanged = true;
                            break;
                        }
                    }
                }
                if (isChanged)
                {
                    this.channelStatus = new bool[paramHoleStatus.Length];
                    paramHoleStatus.CopyTo(this.channelStatus, 0);
                }
            }
            if (isChanged)
            {
                HoleChangedArgument e = new HoleChangedArgument(this.Parent.Parent.ID, this.Parent.ID, this.ID, this.MachineID);
                e.HoleStatus = this.GetHoleStatus();
                this.refreshTime = DateTime.Now;
                this.OnHoleChanged(e);
            }

        }

        /// <summary>
        /// Set Material List
        /// </summary>
        /// <param name="paramMaterial1">Material one</paramMaterial1>
        /// <param name="paramMaterial2">Material two</paramMaterial2>
        /// <param name="paramMaterial3">Material three</paramMaterial3>
        /// <param name="paramMRatio">Material Ratio</paramMRatio>
        internal void SetMaterials(string paramMaterial1, string paramMaterial2, string paramMaterial3, string paramMRatio)
        {
            string material;
            this.materials.Clear();
            material = paramMaterial1;
            if(! string.IsNullOrEmpty(material))
            {
                if (material.Length > 0)
                {
                    if (!this.materials.Contains(material))
                        this.materials.Add(material);
                }
            }
            
            material = paramMaterial2;
            if (!string.IsNullOrEmpty(material))
            {
                if (material.Length > 0)
                {
                    if (!this.materials.Contains(material))
                        this.materials.Add(material);
                }
            }
            material = paramMaterial3;
            if (!string.IsNullOrEmpty(material))
            {
                if (material.Length > 0)
                {
                    if (!this.materials.Contains(material))
                        this.materials.Add(material);
                }
            }
            this.ratio = paramMRatio;
            this.RefreshTime = DateTime.Now;
        }

        /// <summary>
        /// Check Material List
        /// </summary>
        /// <param name="paramMaterial1">Material one</param>
        /// <param name="paramMaterial2">Material two</param>
        /// <param name="paramMaterial3">Material three</param>
        /// <param name="paramMRatio">Material ratio</param>
        /// <returns>Compared Result(True/False)</returns>
        internal bool CheckMaterials(string paramMaterial1, string paramMaterial2, string paramMaterial3, string paramMRatio)
        {
            int count = 0;
            string material;
            material = paramMaterial1.Trim();
            if (this.materials.Contains(material))
                count += 1;
            material = paramMaterial2.Trim();
            if (this.materials.Contains(material))
                count += 1;
            material = paramMaterial3.Trim();
            if (this.materials.Contains(material))
                count += 1;

            return ((count > 0) && (count == this.materials.Count) && (paramMRatio == this.MaterialRatio));
        }

        /// <summary>
        /// On Hole Changed
        /// </summary>
        /// <param name="e">Event Argument</param>
        protected void OnHoleChanged(HoleChangedArgument e)
        {
            if (this.HoleChanged != null)
                this.HoleChanged(this, e);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get Hole Status Array
        /// </summary>
        /// <returns>Hole Status Array</returns>
        private bool[] GetHoleStatus()
        {
            bool[] holeStatus = null;
            lock (this.lockHole)
            {
                holeStatus = new bool[this.channelStatus.Length];
                this.channelStatus.CopyTo(holeStatus, 0);
            }
            return holeStatus;

        }

        #endregion

        #region event handler

        /// <summary>
        /// MHole alarm happened event handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void MHole_AlarmHappened(object Sender, ModbusAlarmHappenedArgument e)
        {
            if (this.AlarmHappened != null)
                AlarmHappened(this, e);
        }

        /// <summary>
        /// MHole connect changed event handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void MHole_ConnectChanged(object Sender, ModbusConnectChangedArgument e)
        {
            this.refreshTime = DateTime.Now;
            this.connectedTime = e.ConnectedTime;
            this.disConnectedTime = e.DisconnectedTime;
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
                    this.remoteIO.AlarmHappened -= this.MHole_AlarmHappened;
                    this.remoteIO.ConnectChanged -= this.MHole_ConnectChanged;
                    this.remoteIO.Dispose();
                    this.remoteIO = null;

                    if (this.materials != null)
                    {
                        this.materials.Clear();
                        this.materials = null;
                    }
                    this.parent = null;
                    this.strID = null;
                    this.strIPAddress = null;
                    this.lockHole = null;
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
