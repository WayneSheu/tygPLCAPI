using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using Dands.Modbus;

namespace TYG.HoleDetect
{
    /// <summary>Data Acquistion for Remote I/O</summary>
    public class RemoteIO : IDisposable
    {

        #region Static Constant Variables

        private static readonly uint ReadInterval = 500;
        private static readonly ushort StartAddress = 0;

        #endregion

        #region Private Variables

        private Platform parent = null;
        private MHole mhole = null;
        private Master master = new Master();
        private System.Threading.Timer readTimer = null;
        private readonly object timerLock = new object();
        private System.Threading.Thread readThread = null;
        private readonly object readLock = new object();
        private AutoResetEvent readWaitEvent = new AutoResetEvent(false);

        private ushort[] AIData, handleData;
        private bool[] DIStatus, handleStatus;
        private string[] holeIDs;
        private bool isStarted = false;
        private bool isConnected = false;
        private ushort readSize = 0;
        private int channelMax = 0;

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
        /// <param name="parent">Parent Object (Area)</param>
        internal RemoteIO(Platform parent)
        {
            this.parent = parent;
            this.master.ConnectType = ConnectType.TCP;
            this.master.ConnectChanged += RemoteIO_ConnectChanged;
            this.master.AlarmHappened += RemoteIO_AlarmHappened;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Parent Object (Area)</param>
        internal RemoteIO(MHole mHole)
        {
            this.parent = mHole.Parent;
            this.mhole = mHole;
            this.master.ConnectType = ConnectType.TCP;
            this.master.ConnectChanged += RemoteIO_ConnectChanged;
            this.master.AlarmHappened += RemoteIO_AlarmHappened;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~RemoteIO()
        {
            this.Dispose();
        }

        #endregion

        #region Public Property

        public Platform Parent
        {
            get { return this.parent;}
        }

        public MHole MHole
        {
            get { return this.mhole; }
        }

        /// <summary>
        /// Start status
        /// </summary>
        public bool Started
        {
            get { return this.isStarted; }
        }

        /// <summary>
        /// Connect status.
        /// </summary>
        public bool Connected
        {
            get { return this.isConnected; }
            set
            {
                if (this.isConnected != value)
                {
                    this.isConnected = value;
                    this.ThreadSet(this.isConnected);
                }
            }
        }

        #endregion

        #region Internal Method

        /// <summary>
        /// Start Command
        /// </summary>
        /// <returns>Start status</returns>
        internal bool Start()
        {
            if (!this.isStarted)
            {
                try
                {
                    this.channelMax = this.Parent.Machines.Keys.Max();
                    this.readSize = (ushort)(this.channelMax+1);
                    lock (this.readLock)
                    {
                        this.AIData = new ushort[(int)(this.readSize)];
                        this.handleData = new ushort[(int)(this.readSize)];
                        this.holeIDs = new string[(int)(this.readSize)];
                    }

                    this.master.IPAddress = this.Parent.IPAddress;
                    this.master.Port = this.Parent.Port;
                    this.master.Start();
                    this.isStarted = true;
                }
                catch (Exception ex)
                {
                    if (this.AlarmHappened != null)
                        AlarmHappened(this, new ModbusAlarmHappenedArgument(DeviceType.Platform, this.Parent.ID, ex.Message, ErrorLevel.Error));
                }
            }
            return this.isStarted;
        }


        /// <summary>
        /// Start Command
        /// </summary>
        /// <returns>Start status</returns>
        internal bool Start2(string HoleId)
        {
            if (!this.isStarted)
            {
                try
                {
                    //this.channelMax = this.Parent.MHoles.Keys.Max();
                    this.readSize = 16;//(ushort)(this.channelMax + 1);
                    lock (this.readLock)
                    {
                        //this.AIData = new ushort[(int)(this.readSize)];
                        this.handleStatus = new bool[(int)(this.readSize)];
                        //this.holeIDs = new string[(int)(this.readSize)];
                        this.DIStatus = new bool[16];
                    }

                    this.master.IPAddress = this.Parent.MHoles[HoleId].IPAddress;
                    this.master.Port = this.Parent.MHoles[HoleId].Port;
                    this.master.Start();
                    this.isStarted = true;
                }
                catch (Exception ex)
                {
                    if (this.AlarmHappened != null)
                        AlarmHappened(this, new ModbusAlarmHappenedArgument(DeviceType.Platform, this.Parent.ID, ex.Message, ErrorLevel.Error));
                }
            }
            return this.isStarted;
        }

        /// <summary>
        /// Stop Command
        /// </summary>
        internal void Stop()
        {
            if (this.isStarted)
            {
                this.isStarted = false;
                this.master.Stop();
            }
        }

        #endregion

        #region Thread and Timer Call Back Function

        /// <summary>
        /// Read Timer and Thread setting
        /// </summary>
        private void ThreadSet(bool paramConnected)
        {
            if (paramConnected)
            {
                lock (this.readLock)
                {
                    // Let Read Thread Waiting
                    this.readWaitEvent.Reset();
                    this.readThread = new Thread(this.ReadMethod);
                    this.readThread.IsBackground = true;
                    this.readThread.Start();
                }
                lock (this.timerLock)
                {
                    //this.readTimer = new System.Threading.Timer(this.ReadAIData, null, 0, Timeout.Infinite);
                    this.readTimer = new System.Threading.Timer(this.ReadDIData, null, 0, Timeout.Infinite);
                }
            }
            else
            {
                lock (this.timerLock)
                {
                    try
                    {
                        if (this.readTimer != null)
                            this.readTimer.Dispose();
                    }
                    catch { }
                    finally
                    {
                        this.readTimer = null;
                    }
                }
                lock (this.readLock)
                {
                    try
                    {
                        if (this.readThread != null)
                        {
                            this.readWaitEvent.Set();
                            Thread.Sleep(5);
                            this.readThread.Abort();
                        }
                    }
                    catch { }
                    finally
                    {
                        this.readThread = null;
                    }
                }
            }
        }

        /// <summary>
        /// Read Remote I/O Data
        /// </summary>
        /// <param name="state">parameter of callback function</param>
        private void ReadAIData(object state)
        {
            try
            {
                ushort[] data = null;
                DateTime dtStart = DateTime.Now;
                TimeSpan tsSpent;

                lock (this.readLock)
                {
                    try
                    {
                        data = this.master.ReadMultiInputRegisters(StartAddress, this.readSize);
                        if (data != null)
                        {
                            Array.Copy(data, 0, this.AIData, 0, this.readSize);
                            this.readWaitEvent.Set();
                        }
                        tsSpent = DateTime.Now.Subtract(dtStart);

                    }
                    catch (Exception ex)
                    {
                        string strer = ex.Message;
                    }
                }
            }
            catch (Exception) { }
            finally
            {
                lock (this.timerLock)
                {
                    if (this.readTimer != null)
                    {
                        if (this.master.Connected)
                            this.readTimer.Change(ReadInterval, Timeout.Infinite);
                        else
                            this.readTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    }
                }
            }
        }

        /// <summary>
        /// Read Remote I/O Data
        /// </summary>
        /// <param name="state">parameter of callback function</param>
        private void ReadDIData(object state)
        {
            try
            {
                bool[] data = null;
                DateTime dtStart = DateTime.Now;
                TimeSpan tsSpent;

                lock (this.readLock)
                {
                    try
                    {
                        data = this.master.ReadMultiCoils(StartAddress, this.readSize);
                        if (data != null)
                        {
                            Array.Copy(data, 0, this.DIStatus, 0, this.readSize);
                            this.readWaitEvent.Set();
                        }
                        tsSpent = DateTime.Now.Subtract(dtStart);

                    }
                    catch (Exception ex)
                    {
                        string strer = ex.Message;
                    }
                }
            }
            catch (Exception) { }
            finally
            {
                lock (this.timerLock)
                {
                    if (this.readTimer != null)
                    {
                        if (this.master.Connected)
                            this.readTimer.Change(ReadInterval, Timeout.Infinite);
                        else
                            this.readTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    }
                }
            }
        }

        /// <summary>
        /// Call Back Function of Read Thread
        /// </summary>
        private void ReadMethod()
        {
            do
            {
                this.readWaitEvent.WaitOne();
                lock (this.readLock)
                {
                    if (!this.master.Connected)
                        continue;

                    //this.AIData.CopyTo(this.handleData, 0);
                    this.DIStatus.CopyTo(this.handleStatus, 0);
                    this.OnDataReceived();
                }
            } while (this.master.Connected);
        }

        #endregion

        #region Data Received Handler

        /// <summary>
        /// Data Received Handler
        /// </summary>
        private void OnDataReceived()
        {
            try
            {
                //HoleDetection.HoleConverter(this.handleData, ref this.holeIDs);
                //for (int index = 1; index <= this.channelMax; index++)
                //{
                //    if (this.Parent.Machines.ContainsKey(index))
                //        this.Parent.Machines[index].SetHoleID(this.holeIDs[index]);
                //}
                //this.Parent.SetHoleDetection(ref this.holeIDs);
                this.mhole.SetHoleStatus(ref this.DIStatus);
            }
            catch { }
        }

        #endregion

        #region Modbus Event Handler

        /// <summary>
        /// Remote I/O connect changed event handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void RemoteIO_ConnectChanged(object Sender, ConnectChangedArgument e)
        {
            try
            {
                this.Connected = e.IsConnected;
                if (this.ConnectChanged != null)
                    ConnectChanged(this, new ModbusConnectChangedArgument(DeviceType.MHole, e));
            }
            catch { }
        }

        /// <summary>
        /// Remote I/O alarm happened event handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void RemoteIO_AlarmHappened(object Sender, AlarmHappenedArgument e)
        {
            if (this.AlarmHappened != null)
                AlarmHappened(this, new ModbusAlarmHappenedArgument(DeviceType.Platform, this.Parent.ID, e.AlarmMessage, ErrorLevel.Alarm));
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
                    this.master.ConnectChanged -= RemoteIO_ConnectChanged;
                    this.master.AlarmHappened -= RemoteIO_AlarmHappened;
                    this.master.Dispose();
                    this.master = null;
                    this.parent = null;
                    this.mhole = null;
                    if (this.readTimer != null)
                    {
                        this.readTimer.Dispose();
                        this.readTimer = null;
                    }
                    if (this.readThread != null)
                    {
                        this.readThread.Abort();
                        this.readThread = null;
                    }
                    if (this.readWaitEvent != null)
                    {
                        this.readWaitEvent.Dispose();
                        this.readWaitEvent = null;
                    }
                    this.AIData = null;
                    this.handleData = null;
                    this.DIStatus = null;
                    //this.timerLock = null;
                    //this.readLock = null;
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
