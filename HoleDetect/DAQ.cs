using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FactoryDict = System.Collections.Generic.Dictionary<string, TYG.HoleDetect.Factory>;

namespace TYG.HoleDetect
{
    /// <summary>Data Acquistion Integrated Class</summary>
    public class DAQ : IDisposable
    {

        #region Private Variables

        private bool isStarted = false;
        private FactoryDict factories = new FactoryDict();
        private object lockPlatformHoleChanged = new object();
        private object lockMHoleChanged = new object();
        private object lockMachineDataChanged = new object();
        private object lockMachineMaterialCheck = new object();
        private object lockAreaConnectChanged = new object();
        private object lockPlatformConnectChanged = new object();
        private object lockMHoleConnectChanged = new object();
        private object lockAlarmHappened = new object();

        #endregion

        #region Events

        /// <summary>
        /// Platform Hole Changed Event
        /// </summary>
        public event HoleChangedEventHandler Platform_HoleChanged;

        /// <summary>
        /// MHole Hole Changed Event
        /// </summary>
        public event HoleChangedEventHandler MHole_HoleChanged;

        /// <summary>
        /// Machine Data Changed Event
        /// </summary>
        public event DataChangedEventHandler Machine_DataChanged;

        /// <summary>
        /// Machine Material Check Event
        /// </summary>
        public event MaterialCheckEventHandler Machine_MaterialCheck;

        /// <summary>
        /// Area (Master PLC) Connected Status Changed Event
        /// </summary>
        public event ModbusConnectChangedEventHandler Area_ConnectChanged;

        /// <summary>
        /// Platform (Remote i/O) Connected Status Changed Event
        /// </summary>
        public event ModbusConnectChangedEventHandler Platform_ConnectChanged;

        /// <summary>
        /// MHole (Remote i/O) Connected Status Changed Event
        /// </summary>
        public event ModbusConnectChangedEventHandler MHole_ConnectChanged;

        /// <summary>
        /// Alarm Happened Event
        /// </summary>
        public event ModbusAlarmHappenedEventHandler AlarmHappened;

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Destructor
        /// </summary>
        ~DAQ()
        {
            this.Dispose();
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Get Start Status
        /// </summary>
        public bool Started
        {
            get { return this.isStarted; }
        }

        /// <summary>
        /// Factory Key Collection
        /// </summary>
        public FactoryDict.KeyCollection FactoryKeys
        {
            get { return this.factories.Keys; }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Add Factory
        /// </summary>
        /// <param name="paramID">Factory ID</param>
        /// <param name="paramName">Factory Name</param>
        public void AddFactory(string paramID, string paramName)
        {
            Factory factory = new Factory(this, paramID, paramName);
            factories[paramID] = factory;
        }

        /// <summary>
        /// Get Factory Object
        /// </summary>
        /// <param name="paramFactoryID">Factory ID</param>
        /// <returns>Factory Object</returns>
        public Factory GetFactory(string paramFactoryID)
        {
            Factory factory = null;
            if (factories.ContainsKey(paramFactoryID))
                factory = factories[paramFactoryID];
            return factory;
        }

        /// <summary>
        /// Start Data Collection
        /// </summary>
        public void Start()
        {
            if (!this.isStarted)
            {
                this.AddEventHandler();
                foreach (Factory factory in factories.Values)
                    factory.Start();
                this.isStarted = true;
            }
        }

        /// <summary>
        /// Stop Data Collection
        /// </summary>
        public void Stop()
        {
            if (this.isStarted)
            {
                this.isStarted = false;
                foreach (Factory factory in factories.Values)
                    factory.Stop();
                Thread.Sleep(200);
                this.RemoveEventHandler();
            }
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Add Event Handler
        /// </summary>
        private void AddEventHandler()
        {
            foreach(Factory factory in this.factories.Values)
            {
                foreach (Area area in factory.Areas.Values)
                {
                    area.ConnectChanged += Area_ConnectChanged_EventHandler;
                    area.AlarmHappened += AlarmHappened_EventHandler;
                    foreach (Platform platform in area.PlatForms.Values)
                    {
                        platform.AlarmHappened += AlarmHappened_EventHandler;
                        platform.ConnectChanged += Platform_ConnectChanged_EventHandler;
                        platform.HoleChanged += Platform_HoleChanged_EventHandler;
                        foreach(MHole mhole in platform.MHoles.Values)
                        {
                            mhole.AlarmHappened += AlarmHappened_EventHandler;
                            mhole.ConnectChanged += MHole_ConnectChanged_EventHandler;
                            mhole.HoleChanged += MHole_HoleChanged_EventHandler;
                        }
                    }
                    foreach (Machine machine in area.Machines.Values)
                    {
                        machine.DataChanged += Machine_DataChanged_EventHandler;
                        machine.MaterialCheck += Machine_MaterialCheck_EventHandler;
                    }
                }
            }
        }

        /// <summary>
        /// Remove Event Handler
        /// </summary>
        private void RemoveEventHandler()
        {
            foreach (Factory factory in this.factories.Values)
            {
                foreach (Area area in factory.Areas.Values)
                {
                    foreach (Machine machine in area.Machines.Values)
                    {
                        machine.DataChanged -= Machine_DataChanged_EventHandler;
                        machine.MaterialCheck -= Machine_MaterialCheck_EventHandler;
                    }
                    foreach (Platform platform in area.PlatForms.Values)
                    {
                        platform.AlarmHappened -= AlarmHappened_EventHandler;
                        platform.ConnectChanged -= Platform_ConnectChanged_EventHandler;
                        platform.HoleChanged -= Platform_HoleChanged_EventHandler;
                        foreach (MHole mhole in platform.MHoles.Values)
                        {
                            mhole.AlarmHappened -= AlarmHappened_EventHandler;
                            mhole.ConnectChanged -= MHole_ConnectChanged_EventHandler;
                            mhole.HoleChanged -= MHole_HoleChanged_EventHandler;
                        }
                    }
                    area.ConnectChanged -= Area_ConnectChanged_EventHandler;
                    area.AlarmHappened -= AlarmHappened_EventHandler;
                }
            }
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// Machine Data Changed Event Handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void Machine_DataChanged_EventHandler(object Sender, DataChangedArgument e)
        {
            lock(this.lockMachineDataChanged)
            {
                try
                {
                    if (this.Machine_DataChanged != null)
                        this.Machine_DataChanged(Sender, e);
                }
                catch { }
            }
        }

        /// <summary>
        /// Machine Material Check Event Handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void Machine_MaterialCheck_EventHandler(object Sender, MaterialCheckArgument e)
        {
            lock (this.lockMachineMaterialCheck)
            {
                try
                {
                    if (this.Machine_MaterialCheck != null)
                        this.Machine_MaterialCheck(Sender, e);
                }
                catch { }
            }
        }

        /// <summary>
        /// Alarm Happened Event Handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void AlarmHappened_EventHandler(object Sender, ModbusAlarmHappenedArgument e)
        {
            lock (this.lockAlarmHappened)
            {
                if (this.AlarmHappened != null)
                    this.AlarmHappened(Sender, e);
            }
        }

        /// <summary>
        /// Platform Hole Changed Event Handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void Platform_HoleChanged_EventHandler(object Sender, HoleChangedArgument e)
        {
            lock (this.lockPlatformHoleChanged)
            {
                try
                {
                    if (this.Platform_HoleChanged != null)
                        this.Platform_HoleChanged(Sender, e);
                }
                catch { }
            }
        }

        /// <summary>
        /// MHole Changed Event Handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void MHole_HoleChanged_EventHandler(object Sender, HoleChangedArgument e)
        {
            lock (this.lockMHoleChanged)
            {
                try
                {
                    if (this.MHole_HoleChanged != null)
                        this.MHole_HoleChanged(Sender, e);
                }
                catch { }
            }
        }

        /// <summary>
        /// Area Connect Changed Event Handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void Area_ConnectChanged_EventHandler(object Sender, ModbusConnectChangedArgument e)
        {
            lock (this.lockAreaConnectChanged)
            {
                try
                {
                    if (this.Area_ConnectChanged != null)
                        this.Area_ConnectChanged(Sender, e);
                }
                catch { }
            }
        }

        /// <summary>
        /// Platform Connect Changed Event Handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void Platform_ConnectChanged_EventHandler(object Sender, ModbusConnectChangedArgument e)
        {
            lock (this.lockPlatformConnectChanged)
            {
                try
                {
                    if (this.Platform_ConnectChanged != null)
                        this.Platform_ConnectChanged(Sender, e);
                }
                catch { }
            }
        }

        /// <summary>
        /// MHole Connect Changed Event Handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void MHole_ConnectChanged_EventHandler(object Sender, ModbusConnectChangedArgument e)
        {
            lock (this.lockMHoleConnectChanged)
            {
                try
                {
                    if (this.MHole_ConnectChanged != null)
                        this.MHole_ConnectChanged(Sender, e);
                }
                catch { }
            }
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
                    foreach (Factory factory in factories.Values)
                        factory.Dispose();
                    factories.Clear();
                    factories = null;
                    this.lockPlatformHoleChanged = null;
                    this.lockMachineDataChanged = null;
                    this.lockMachineMaterialCheck = null;
                    this.lockAreaConnectChanged = null;
                    this.lockPlatformConnectChanged = null;
                    this.lockAlarmHappened = null;
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
