using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Dands.Modbus;
using MachineDict = System.Collections.Generic.Dictionary<int, TYG.HoleDetect.Machine>;
using MPileDict = System.Collections.Generic.Dictionary<int, TYG.HoleDetect.MPile>;
using MHoleDict = System.Collections.Generic.Dictionary<string, TYG.HoleDetect.MHole>;
using System.Threading;

namespace TYG.HoleDetect
{
    /// <summary>Data Acquistion for Platform</summary>
    public class Platform : IDisposable
    {

        #region Private Variabes

        private Area parent = null;
        private string strID = "";
        private string strName = "";
        private string strIPAddress = "";
        private int port = 0;
        private bool isStarted = false;
        private string[] channelDetection = null;
        private RemoteIO remoteIO = null;
        private MachineDict machines = new MachineDict();
        private MPileDict mpiles = new MPileDict();
        private MHoleDict mholes = new MHoleDict();
        private object lockHole = new object();
        private DateTime? connectedTime;
        private DateTime? disConnectedTime;
        private DateTime refreshTime = DateTime.Now;

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
        /// <param name="parent">Parent Object (Factory)</param>
        /// <param name="paramID">Platform ID</param>
        /// <param name="paramName">Platform Name</param>
        /// <param name="paramIPAddress">Remote I/O IP Address</param>
        /// <param name="paramPort">Remote I/O TCP Port No.</param>
        internal Platform(Area parent, string paramID, string paramName, string paramIPAddress, int paramPort)
        {
            this.parent = parent;
            this.strID = paramID;
            this.strName = paramName;
            this.strIPAddress = paramIPAddress;
            this.port = paramPort;
            this.channelDetection = new string[] { "  ", "  ", "  ", "  ", "  ", "  ", "  ", "  " };
            this.remoteIO = new RemoteIO(this);
            this.remoteIO.ConnectChanged += Platform_ConnectChanged;
            this.remoteIO.AlarmHappened += Platform_AlarmHappened;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Platform()
        {
            this.Dispose();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Parent Object (Area)
        /// </summary>
        public Area Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// Platform ID
        /// </summary>
        public string ID
        {
            get { return this.strID; }
        }

        /// <summary>
        /// Platform Name
        /// </summary>
        public string Name
        {
            get { return this.strName; }
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
        /// Machine Key Collection
        /// </summary>
        public MachineDict.KeyCollection MachineKeys
        {
            get { return this.machines.Keys; }
        }

        /// <summary>
        /// MPile Key Collection
        /// </summary>
        public MPileDict.KeyCollection MPileDictKeys
        {
            get { return this.mpiles.Keys; }
        }

        /// <summary>
        /// MHole Key Collection
        /// </summary>
        public MHoleDict.KeyCollection MHoleDictKeys
        {
            get { return this.mholes.Keys; }
        }


        /// <summary>
        /// Hole Detection Array
        /// </summary>
        public string[] ChannelDetection
        {
            get { return this.GetHoleDetection(); }
        }

        /// <summary>
        /// Data Refresh Time
        /// </summary>
        public DateTime RefreshTime
        {
            get { return this.refreshTime; }
            internal set
            {
                this.refreshTime = value;
            }
        }

        #endregion

        #region Internal Property

        /// <summary>
        /// Machine collection
        /// </summary>
        internal MachineDict Machines
        {
            get { return this.machines; }
        }

        /// <summary>
        /// MPile collection
        /// </summary>
        internal MPileDict MPiles
        {
            get { return this.mpiles; }
        }

        /// <summary>
        /// MPile collection
        /// </summary>
        internal MHoleDict MHoles
        {
            get { return this.mholes; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Machine Object
        /// </summary>
        /// <param name="paramChannelNo">Remote I/O Channel No.</param>
        /// <returns>Machine Object (Not found return null)</returns>
        public Machine GetMachine(int paramChannelNo)
        {
            Machine machine = null;
            if (this.machines.ContainsKey(paramChannelNo))
                machine = this.machines[paramChannelNo];
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
        /// Add MPile
        /// </summary>
        /// <param name="paramSortNo">MPile Sort No</param>
        public void AddMPile(int paramSortNo)
        {
            if (!this.isStarted)
            {
                MPile mpile = new MPile(this, paramSortNo);
                this.mpiles[paramSortNo] = mpile;
            }
        }

        /// <summary>
        /// Get MPile Object
        /// </summary>
        /// <param name="paramSortNo">MPile Sort No.</param>
        /// <returns>MPile Object (Not found return null)</returns>
        public MPile GetMPile(int paramSortNo)
        {
            MPile mpile = null;
            if (this.mpiles.ContainsKey(paramSortNo))
                mpile = this.mpiles[paramSortNo];
            return mpile;
        }

        /// <summary>
        /// Set Material List of MPile
        /// </summary>
        /// <param name="paramSortNo">MPile Sort No.</param>
        /// <param name="paramMaterial1">Material one</paramMaterial1>
        /// <param name="paramMaterial2">Material two</paramMaterial2>
        /// <param name="paramMaterial3">Material three</paramMaterial3>
        /// <param name="paramMRatio">Material three</paramMRatio>
        public bool SetMPile(int paramSortNo, string paramMaterial1, string paramMaterial2, string paramMaterial3, string paramMRatio)
        {
            bool result = false;
            if (this.mpiles.ContainsKey(paramSortNo))
            {
                this.mpiles[paramSortNo].SetMaterials(paramMaterial1, paramMaterial2, paramMaterial3, paramMRatio);
                foreach (Machine machine in this.machines.Values)
                {
                    if (machine.HoleID.Length > 0)
                    {
                        if (machine.HoleID.Substring(0, 1).Equals(this.mpiles[paramSortNo].ID))
                        {
                            machine.SetMaterialCheck();
                        }
                    }
                }
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Check Material List of MPile
        /// </summary>
        /// <param name="paramSortNo">MPile Sort No.</param>
        /// <param name="paramMaterial1">Material one</param>
        /// <param name="paramMaterial2">Material two</param>
        /// <param name="paramMaterial3">Material three</param>
        /// <param name="paramMRatio">Material Ratio</param>
        public bool CheckMPile(int paramSortNo, string paramMaterial1, string paramMaterial2, string paramMaterial3, string paramMRatio)
        {
            bool result = false;
            if (this.mpiles.ContainsKey(paramSortNo))
                result = this.mpiles[paramSortNo].CheckMaterials(paramMaterial1, paramMaterial2, paramMaterial3, paramMRatio);
            return result;
        }


        /// <summary>
        /// Add MPile
        /// </summary>
        /// <param name="paramHoleId">MHole Id</param>
        public void AddMHole(string paramHoleId, string paramIP, int paramPort, string paramMachineID)
        {
            if (!this.isStarted)
            {
                MHole mhole = new MHole(this, paramHoleId, paramIP, paramPort, paramMachineID);
                this.mholes[paramHoleId] = mhole;
            }
        }

        /// <summary>
        /// Get MHole Object
        /// </summary>
        /// <param name="paramHoleId">MHole ID.</param>
        /// <returns>MHole Object (Not found return null)</returns>
        public MHole GetMHole(string paramHoleId)
        {
            MHole mhole = null;
            if (this.mholes.ContainsKey(paramHoleId))
                mhole = this.mholes[paramHoleId];
            return mhole;
        }

        /// <summary>
        /// Set Material List of MHole
        /// </summary>
        /// <param name="paramHoleId">MHole ID.</param>
        /// <param name="paramMaterial1">Material one</paramMaterial1>
        /// <param name="paramMaterial2">Material two</paramMaterial2>
        /// <param name="paramMaterial3">Material three</paramMaterial3>
        /// <param name="paramMRatio">Material three</paramMRatio>
        public bool SetHole(string paramHoleId, string paramMaterial1, string paramMaterial2, string paramMaterial3, string paramMRatio)
        {
            bool result = false;
            if (this.mholes.ContainsKey(paramHoleId))
            {
                this.mholes[paramHoleId].SetMaterials(paramMaterial1, paramMaterial2, paramMaterial3, paramMRatio);
                foreach (Machine machine in this.machines.Values)
                {
                    if (machine.HoleID.Length > 0)
                    {
                        if (machine.HoleID.Substring(0, 1).Equals(this.mholes[paramHoleId].ID))
                        {
                            machine.SetMaterialCheck();
                        }
                    }
                }
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Set Material List of MHole
        /// </summary>
        /// <param name="paramHoleId">MHole Id.</param>
        /// <param name="paramMaterial1">Material one</paramMaterial1>
        /// <param name="paramMaterial2">Material two</paramMaterial2>
        /// <param name="paramMaterial3">Material three</paramMaterial3>
        /// <param name="paramMRatio">Material three</paramMRatio>
        public bool SetMHole(string paramHoleId, string paramMaterial1, string paramMaterial2, string paramMaterial3, string paramMRatio)
        {
            bool result = false;
            if (this.mholes.ContainsKey(paramHoleId))
            {
                this.mholes[paramHoleId].SetMaterials(paramMaterial1, paramMaterial2, paramMaterial3, paramMRatio);
                foreach (Machine machine in this.machines.Values)
                {
                    if (machine.HoleID.Length > 0)
                    {
                        if (machine.HoleID.Substring(0, 1).Equals(this.mholes[paramHoleId].ID))
                        {
                            machine.SetMaterialCheck();
                        }
                    }
                }
                result = true;
            }
            return result;
        }


        /// <summary>
        /// Check Material List of MHole
        /// </summary>
        /// <param name="paramHoleId">MHole Id.</param>
        /// <param name="paramMaterial1">Material one</param>
        /// <param name="paramMaterial2">Material two</param>
        /// <param name="paramMaterial3">Material three</param>
        /// <param name="paramMRatio">Material Ratio</param>
        public bool CheckMHole(string paramHoleId, string paramMaterial1, string paramMaterial2, string paramMaterial3, string paramMRatio)
        {
            bool result = false;
            if (this.mholes.ContainsKey(paramHoleId))
                result = this.mholes[paramHoleId].CheckMaterials(paramMaterial1, paramMaterial2, paramMaterial3, paramMRatio);
            return result;
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
            string old1 = "", old2 = "", old3 = "";
            string new1 = "", new2 = "", new3 = "";

            if (paramOldMaterials.Length > 0)
                old1 = paramOldMaterials[0];
            if (paramOldMaterials.Length > 1)
                old2 = paramOldMaterials[1];
            if (paramOldMaterials.Length > 2)
                old3 = paramOldMaterials[2];

            if (paramNewMaterials.Length > 0)
                new1 = paramNewMaterials[0];
            if (paramNewMaterials.Length > 1)
                new2 = paramNewMaterials[1];
            if (paramNewMaterials.Length > 2)
                new3 = paramNewMaterials[2];

            foreach(int sortNo in this.MPiles.Keys)
            {
                if (this.CheckMPile(sortNo, old1, old2, old3, paramOldMRatio))
                    this.SetMPile(sortNo, new1, new2, new3, paramNewMRatio);
            }

            foreach (string holeId in this.MHoles.Keys)
            {
                if (this.CheckMHole(holeId, old1, old2, old3, paramOldMRatio))
                    this.SetMHole(holeId, new1, new2, new3, paramNewMRatio);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Start RemoteIO Check
        /// </summary>
        /// <returns></returns>
        internal bool Start()
        {
            string strError = "";
            if (!this.Started)
            {
                foreach (MHole mhole in this.mholes.Values)
                    mhole.Start();

                //if ((this.IPAddress.Length == 0) || (this.Port == 0))
                //    strError = "Remote I/O IP Address or TCP port not setting !!";
                //else if (this.machines.Count == 0)
                //    strError = "Remote I/O channel not setting !!";
                //else
                //{
                //    this.remoteIO.Start();
                //    this.isStarted = true;
                //}

                //if (strError.Length > 0)
                //{
                //    if (this.AlarmHappened != null)
                //        AlarmHappened(this, new ModbusAlarmHappenedArgument(DeviceType.Platform, this.ID, strError, ErrorLevel.Error));
                //}
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
        /// Set Hole Detection Array
        /// </summary>
        /// <param name="paramHoleDetection">Hole Detection Array</param>
        internal void SetHoleDetection(ref string[] paramHoleDetection)
        {
            bool isChanged = false;
            lock (this.lockHole)
            {
                if (this.channelDetection.Length != paramHoleDetection.Length)
                    isChanged = true;
                else
                {
                    for (int index = 0; index < this.channelDetection.Length; index++)
                    {
                        if (!this.channelDetection[index].Equals(paramHoleDetection[index]))
                        {
                            isChanged = true;
                            break;
                        }
                    }
                }
                if (isChanged)
                {
                    this.channelDetection = new string[paramHoleDetection.Length];
                    paramHoleDetection.CopyTo(this.channelDetection, 0);
                }
            }
            if (isChanged)
            {
                HoleChangedArgument e = new HoleChangedArgument(this.Parent.Parent.ID, this.Parent.ID, this.ID);
                e.HoleDetection = this.GetHoleDetection();
                this.refreshTime = DateTime.Now;
                this.OnHoleChanged(e);
            }
        }

        #endregion

        #region Protected Methods

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
        /// Get Hole Detection Array
        /// </summary>
        /// <returns>Hole Detection Array</returns>
        private string[] GetHoleDetection()
        {
            string[] holeIDs = null;
            lock (this.lockHole)
            {
                holeIDs = new string[this.channelDetection.Length];
                this.channelDetection.CopyTo(holeIDs, 0);
            }
            return holeIDs;
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// Platform alarm happened event handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void Platform_AlarmHappened(object Sender, ModbusAlarmHappenedArgument e)
        {
            if (this.AlarmHappened != null)
                AlarmHappened(this, e);
        }
        
        /// <summary>
        /// Platform connect changed event handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void Platform_ConnectChanged(object Sender, ModbusConnectChangedArgument e)
        {
            this.refreshTime = DateTime.Now;
            this.connectedTime = e.ConnectedTime;
            this.disConnectedTime = e.DisconnectedTime;
            if (this.ConnectChanged != null)
                ConnectChanged(this, e);
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

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
                    this.remoteIO.AlarmHappened -= this.Platform_AlarmHappened;
                    this.remoteIO.ConnectChanged -= this.Platform_ConnectChanged;
                    this.remoteIO.Dispose();
                    this.remoteIO = null;
                    if (this.machines != null)
                    {
                        this.machines.Clear();
                        this.machines = null;
                    }
                    if (this.mpiles != null)
                    {
                        this.mpiles.Clear();
                        this.mpiles = null;
                    }
                    this.parent = null;
                    this.channelDetection = null;
                    this.lockHole = null;
                    this.strID = null;
                    this.strIPAddress = null;
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
