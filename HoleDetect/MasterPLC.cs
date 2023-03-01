using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using Dands.Modbus;

namespace TYG.HoleDetect
{
    /// <summary>Data Acquistion for Master PLC</summary>
    public class MasterPLC : IDisposable
    {

        #region Static Constant Variables

        private static readonly uint ReadInterval = 1000;

        private static readonly ushort DRegisterBase = 6000;
        private static readonly ushort ReadOffset = 100;
        private static readonly ushort SlaveDataSize = 50;
        private static readonly ushort ReadMaxSize = 100;
        private static readonly ushort MotorOffset = 21;
        private static readonly ushort LampOffset = 71;

        private static readonly ushort RUNSTATUS = 0;
        //private static readonly ushort YEAR = 1;
        //private static readonly ushort MONTH = 2;
        //private static readonly ushort DAY = 3;
        //private static readonly ushort HOUR = 4;
        //private static readonly ushort MINUTE = 5;
        //private static readonly ushort SECOND = 6;
        private static readonly ushort SECOND = 1;
        private static readonly ushort MINUTE = 2;
        private static readonly ushort HOUR = 3;
        private static readonly ushort DAY = 4; 
        private static readonly ushort MONTH = 5;
        private static readonly ushort YEAR = 6;
        private static readonly ushort QUANTITY = 7;
        private static readonly ushort FORBIDMOTOR = 8;
        private static readonly ushort LAMPSTATUS = 9;
        private static readonly ushort PQUANTITY = 10;
        private static readonly ushort WEIGHT = 11;
        private static readonly ushort WEIGHTLEN = 7;
        private static readonly ushort WEIGHTUNIT = 18;
        private static readonly ushort TEMPERATURENUMBER = 21;//20220602新增溫度模組數量
        private static readonly ushort POWERFLAG = 22;//20210913新增PLC過電註記，開電會持續送1
        private static readonly ushort instantVoltage = 23;//20210701新增即時電壓數據
        private static readonly ushort instantCurrent = 24;//20210701新增即時電流數據
        private static readonly ushort MODBUSWEIGHT = 25;//20210315新增Modbus秤重方式的位置
        private static readonly ushort ElectricPowerIni = 27;//20210505新增用量消耗紀錄，從27~29共三個暫存器
        private static readonly ushort SCANLEN = 30;
        private static readonly ushort PSCANLEN = 40;

        #endregion

        #region Private Variables

        private Area parent = null;
        private Master master = new Master();
        private System.Threading.Timer readTimer = null;
        private object timerLock = new object();
        private System.Threading.Thread readThread = null;
        private object readLock = new object();
        private AutoResetEvent readWaitEvent = new AutoResetEvent(false);
        private ushort[] dRegisterData, handleData;
        private bool isStarted = false;
        private bool isConnected = false;
        private ushort startReadAddress = 0;
        private ushort startForbidAddress = 0;
        private ushort startLampAddress = 0;
        private ushort readSize = 0;
        private int slaveMax = 0;

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
        internal MasterPLC(Area parent)
        {
            this.parent = parent;
            master.ConnectType = ConnectType.TCP;
            this.startReadAddress = (ushort)(DRegisterBase + ReadOffset);
            this.startForbidAddress = (ushort)(DRegisterBase + MotorOffset - 1);
            this.startLampAddress = (ushort)(DRegisterBase + LampOffset - 1);
            this.master.ConnectChanged += MasterPLC_ConnectChanged;
            this.master.AlarmHappened += MasterPLC_AlarmHappened;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MasterPLC()
        {
            this.Dispose();
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Parent Object (Area)
        /// </summary>
        public Area Parent
        {
            get { return this.parent; }
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
                    this.slaveMax = this.Parent.Machines.Keys.Max();
                    this.readSize = (ushort)(this.slaveMax * SlaveDataSize);
                    lock (this.readLock)
                    {
                        dRegisterData = new ushort[(int)(this.readSize)];
                        handleData = new ushort[(int)(this.readSize)];
                    }

                    this.master.IPAddress = this.Parent.IPAddress;
                    this.master.Port = this.Parent.Port;
                    this.master.Start();
                    this.isStarted = true;
                }
                catch (Exception ex)
                {
                    if (this.AlarmHappened != null)
                        AlarmHappened(this, new ModbusAlarmHappenedArgument(DeviceType.Area, this.Parent.ID, ex.Message, ErrorLevel.Error));
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
                this.master.Stop();
                this.isStarted = false;
            }
        }

        /// <summary>
        /// Set Motor forbid or not
        /// </summary>
        /// <param name="paramSlaveNo">Machine slave no.</param>
        /// <param name="paramForbid">True/False</param>
        /// <returns>Is Sucess</returns>
        internal bool SetMotorForbid(int paramSlaveNo, bool paramForbid)
        {
            bool isSucess = false;
            ushort forbid = 0;
            if (paramForbid)
                forbid = 1;
            if (this.Connected)
            {
                isSucess = this.master.WriteSingleRegister((ushort)(this.startForbidAddress + paramSlaveNo), forbid);
            }
            return isSucess;
        }

        /// <summary>
        /// Set PLC Lamp Condition
        /// </summary>
        /// <param name="paramSlaveNo">Machine slave no.</param>
        /// <param name="paramLampValue">PLC Lamp condition setting</param>
        /// <returns>Is Sucess</returns>
        internal bool SetSlavePlcLamp(int paramSlaveNo, int paramLampValue)
        {
            bool isSucess = false;
            if (this.Connected)
            {
                isSucess = this.master.WriteSingleRegister((ushort)(this.startLampAddress + paramSlaveNo), (ushort)paramLampValue);
            }
            return isSucess;
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
                    this.readTimer = new System.Threading.Timer(this.ReadPlcData, null, 0, Timeout.Infinite);
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
        /// Read Master PLC Data
        /// </summary>
        /// <param name="state">parameter of callback function</param>
        private void ReadPlcData(object state)
        {
            try
            {
                int count = (int)Math.Ceiling((double)this.readSize / ReadMaxSize);
                ushort registerLen, startAddress;
                ushort sizes = this.readSize;
                ushort[] data;
                int index = 0;
                DateTime dtStart = DateTime.Now;
                TimeSpan tsSpent;
                lock (this.readLock)
                {
                    try
                    {
                        for (index = 0; index < count; index++)
                        {
                            // Caculate number of points
                            if (sizes > ReadMaxSize)
                            {
                                registerLen = ReadMaxSize;
                                sizes -= ReadMaxSize;
                            }
                            else
                                registerLen = sizes;
                            // Caculate Start Address
                            startAddress = (ushort)(this.startReadAddress + index * ReadMaxSize);
                            // Read Master PLC Data
                            data = this.master.ReadMultiHoldRegisters(startAddress, registerLen);
                            if (data == null)
                                break;
                            // Write to dRegisterData Array
                            Array.Copy(data, 0, this.dRegisterData, index * ReadMaxSize, registerLen);
                        }
                        tsSpent = DateTime.Now.Subtract(dtStart);
                        // Check Sucess or not
                        if (index == count)
                        {
                            // Let Read Thread Going
                            this.readWaitEvent.Set();
                        }
                    }
                    catch (Exception ex)
                    {
                        string stre = ex.Message;
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

                    this.dRegisterData.CopyTo(this.handleData, 0);
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
            int runStatus, qty, forbidMotor, lampStatus, pqty;
            float weight = 0.0f;
            DateTime slaveTime = DateTime.Now;
            string scanCode = "";
            string pscanCode = "";
            string barCode = "";
            string weightUnit = "";
            ushort tmp;
            int startIndex, scanLen, pscanLen;
            int powerflag;
            int instantvoltage;
            int instantcurrent;
            int temperaturenumber;
            int modbusweight;
            int electricpower;
            string[] temperaturevalue = new string[8] { "", "", "", "", "", "", "", "" };
            for (int idx = 1; idx <= this.slaveMax; idx++)
            {
                if (!this.Parent.Machines.ContainsKey(idx))
                    continue;

                temperaturevalue = new string[8] { "", "", "", "", "", "", "", "" };
                startIndex = (idx - 1) * SlaveDataSize;
                runStatus = this.handleData[startIndex + RUNSTATUS];
                qty = this.handleData[startIndex + QUANTITY];
                forbidMotor = this.handleData[startIndex + FORBIDMOTOR];
                lampStatus = this.handleData[startIndex + LAMPSTATUS];
                pqty = this.handleData[startIndex + PQUANTITY];
                weight = this.GetWeight(startIndex + WEIGHT);
                temperaturenumber = this.handleData[startIndex + TEMPERATURENUMBER];//20220602新增溫度模組數量
                powerflag = this.handleData[startIndex + POWERFLAG];//20210701新增即時電壓數據
                instantvoltage = this.handleData[startIndex + instantVoltage];//20210701新增即時電壓數據
                instantcurrent = this.handleData[startIndex + instantCurrent];//20210701新增即時電流數據
                modbusweight = this.handleData[startIndex + MODBUSWEIGHT];//20210315新增Modbus重量,預設是公克並且沒有單位
                electricpower = this.handleData[startIndex + ElectricPowerIni + 2];//20210505新增電力消耗統計，因還不清楚多少溢位先取最低位元
                scanLen = this.handleData[startIndex + SCANLEN];
                pscanLen = this.handleData[startIndex + PSCANLEN];
                if (scanLen >= PSCANLEN - SCANLEN)
                    scanLen = PSCANLEN - SCANLEN - 1;
                if (pscanLen >= PSCANLEN - SCANLEN)
                    pscanLen = PSCANLEN - SCANLEN - 1;

                try
                {
                    if(temperaturenumber != 0)
                    {
                        //20220602變頻器溫控程式
                        for (int index = 0; index < temperaturevalue.Length; index++)
                        {
                            var temp = this.handleData[startIndex + WEIGHT + index];
                            if (temp != 32768 )
                                temperaturevalue[index] = Convert.ToString(temp);
                            else
                                temperaturevalue[index] = "";
                        }
                    }
                    else
                    {
                        weightUnit = "";
                        for (int index = 0; index < 2; index++)
                        {
                            tmp = this.handleData[startIndex + WEIGHTUNIT + index];
                            if (tmp > 0)
                                weightUnit += Convert.ToChar(tmp);
                        }
                        weightUnit = weightUnit.Trim();
                    }

                    if (modbusweight != 0)
                    {
                        weight = ((float)modbusweight) / 1000;
                        weightUnit = "kg";
                    }

                    barCode = "";
                    for (int index = 1; index <= scanLen; index++)
                    {
                        tmp = this.handleData[startIndex + SCANLEN + index];
                        if (tmp > 0)
                            barCode += Convert.ToChar(tmp);
                    }
                    scanCode = barCode.TrimEnd('\r');
                    barCode = "";
                    for (int index = 1; index <= pscanLen; index++)
                    {
                        tmp = this.handleData[startIndex + PSCANLEN + index];
                        if (tmp > 0)
                            barCode += Convert.ToChar(tmp);
                    }
                    pscanCode = barCode.TrimEnd('\r');

                    //20220901新增年月日時分秒紀錄
                    slaveTime = new DateTime(2017, 8, 15, 0, 0, 0);//將PLC開始啟用的日期當成預設值
                    bool yearR = int.TryParse(this.handleData[startIndex + YEAR].ToString(), out int year);
                    bool monthR = int.TryParse(this.handleData[startIndex + MONTH].ToString(), out int month);
                    bool dayR = int.TryParse(this.handleData[startIndex + DAY].ToString(), out int day);
                    bool hourR = int.TryParse(this.handleData[startIndex + HOUR].ToString(), out int hour);
                    bool minuteR = int.TryParse(this.handleData[startIndex + MINUTE].ToString(), out int minute);
                    bool secondR = int.TryParse(this.handleData[startIndex + SECOND].ToString(), out int second);

                    if(yearR && monthR && dayR && hourR && minuteR && secondR)
                    {
                        if (year == 0 || month == 0 || day == 0)
                        {
                        }
                        else
                        {
                            //排除不合理數值
                            if(year+2000 > 9999 || month > 12 || day > 31 || hour > 23 || minute > 59 || second > 59)
                            {
                            }
                            else
                            {
                                slaveTime = new DateTime(2000 + year, month, day, hour, minute, second);
                            }
                        }
                    }
                }
                catch { }
                finally
                {
                    this.Parent.Machines[idx].PlcData(runStatus, forbidMotor, lampStatus, scanCode, qty, pscanCode, pqty, slaveTime, weight, weightUnit, electricpower, instantvoltage, instantcurrent, powerflag, temperaturenumber, temperaturevalue);
                }
            }
        }

        private float GetWeight(int startPosition)
        {
            float weight = 0.0f;
            string strWeight = "";
            ushort tmp;
            try
            {
                for (int index = 0; index < WEIGHTLEN; index++)
                {
                    tmp = this.handleData[startPosition + index];
                    if (tmp > 0)
                        strWeight += Convert.ToChar(tmp);
                }
                strWeight = strWeight.Trim();
                if (strWeight.Length > 0)
                    float.TryParse(strWeight, out weight);
            }
            catch { }
            return weight;
        }

        #endregion

        #region Modbus Event Handler

        /// <summary>
        /// MasterPLC connect changed event handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void MasterPLC_ConnectChanged(object Sender, ConnectChangedArgument e)
        {
            try
            {
                this.Connected = e.IsConnected;
                if (this.ConnectChanged != null)
                    ConnectChanged(this, new ModbusConnectChangedArgument(DeviceType.Area, e));
            }
            catch { }
        }

        /// <summary>
        /// MasterPLC alarm happened event handler
        /// </summary>
        /// <param name="Sender">Sender</param>
        /// <param name="e">Event Argument</param>
        private void MasterPLC_AlarmHappened(object Sender, AlarmHappenedArgument e)
        {
            if (this.AlarmHappened != null)
                AlarmHappened(this, new ModbusAlarmHappenedArgument(DeviceType.Area, this.Parent.ID, e.AlarmMessage, ErrorLevel.Alarm));
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
                    this.master.ConnectChanged -= MasterPLC_ConnectChanged;
                    this.master.AlarmHappened -= MasterPLC_AlarmHappened;
                    this.master.Dispose();
                    this.master = null;
                    this.parent = null;
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
                    this.dRegisterData = null;
                    this.handleData = null;
                    this.timerLock = null;
                    this.readLock = null;
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
