using Dands.Modbus;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Dands.Modbus
{

    public class Master : IDisposable
    {
        #region Private Variabes

        private readonly object masterLock = new object();

        private string strIPAddress = null;
        private int intPort = 502;
        private string strMHoleIPAddress = null;
        private int intMHolePort = 502;
        private string strError = "";
        private byte byteSlaveID = 1;
        private int intTimeout = 300;
        private uint reconnectInterval = 5000;
        private uint checkInterval = 5000;
        private bool boolConnected = false;
        private bool boolStarted = false;
        private ConnectType connectType = ConnectType.TCP;

        private TcpClient tcpClient = null;
        private UdpClient udpClient = null;
        private SerialPort serialPort = null;
        private ModbusMaster master = null;
        private DateTime? connectedTime = null;
        private System.Threading.Timer connectTimer = null;
        private System.Threading.Timer checkTimer = null;

        #endregion

        #region Destructor

        /// <summary>
        /// Destructor
        /// </summary>
        ~Master()
        {
            Dispose();
        }

        #endregion

        #region Events

        /// <summary>
        /// Connected Status Changed Event
        /// </summary>
        public event ConnectChangedEventHandler ConnectChanged;
        /// <summary>
        /// Alarm Happened Event
        /// </summary>
        public event AlarmHappenedEventHandler AlarmHappened;

        #endregion

        #region Public Properties

        /// <summary>
        /// Master Connect Type, Default = TCP
        /// </summary>
        public ConnectType ConnectType
        {
            get { return this.connectType; }
            set
            {
                if (!this.boolStarted)
                    this.connectType = value;
            }
        }

        /// <summary>
        /// Slave ID, Default = 1
        /// </summary>
        public byte SlaveID
        {
            get { return this.byteSlaveID; }
            set
            {
                if (!this.boolStarted)
                    this.byteSlaveID = value;
            }
        }

        /// <summary>
        /// Slave IP Address
        /// </summary>
        public string IPAddress
        {
            get { return this.strIPAddress; }
            set
            {
                if (!this.boolStarted)
                    this.strIPAddress = value;
            }
        }
        
        /// <summary>
        /// Slave Port No. , Default = 502
        /// </summary>
        public int Port
        {
            get { return this.intPort; }
            set
            {
                if (!this.boolStarted)
                    this.intPort = value;
            }
        }

        /// <summary>
        /// Slave IP Address
        /// </summary>
        public string MHoleIPAddress
        {
            get { return this.strMHoleIPAddress; }
            set
            {
                if (!this.boolStarted)
                    this.strMHoleIPAddress = value;
            }
        }

        /// <summary>
        /// Slave Port No. , Default = 502
        /// </summary>
        public int MHolePort
        {
            get { return this.intMHolePort; }
            set
            {
                if (!this.boolStarted)
                    this.intMHolePort = value;
            }
        }

        /// <summary>
        /// SerialPort for RTU & ASCII Type
        /// </summary>
        public SerialPort SerialPort
        {
            get { return this.serialPort; }
            set
            {
                if (!this.boolStarted)
                    this.serialPort = value;
            }
        }

        /// <summary>
        /// Time out (ms), Default = 300
        /// </summary>
        public int Timeout
        {
            get { return this.intTimeout; }
            set
            {
                if (!this.boolStarted)
                    this.intTimeout = value;
            }
        }

        /// <summary>
        /// Auto Reconnect Interval for TCP/UDP, Default is 5000 (ms), if set zero do not auto reconnect.
        /// </summary>
        public uint ConnectInterval
        {
            get { return this.reconnectInterval; }
            set
            {
                if (!this.boolStarted)
                    this.reconnectInterval = value;
            }
        }

        /// <summary>
        /// Auto Check Connect Status Interval for TCP, Default is 5000 (ms), if set zero do not auto check.
        /// </summary>
        public uint CheckInterval
        {
            get { return this.checkInterval; }
            set
            {
                if (!this.boolStarted)
                    this.checkInterval = value;
            }
        }


        /// <summary>
        /// Connect status.
        /// </summary>
        public bool Connected
        {
            get { return this.boolConnected; }
        }

        /// <summary>
        /// Start status
        /// </summary>
        public bool Started
        {
            get { return this.boolStarted; }
        }

        /// <summary>
        /// Error Message
        /// </summary>
        public string ErrorMessage
        {
            get { return this.strError; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start command
        /// </summary>
        public void Start()
        {
            if (!this.boolStarted)
            {
                this.boolStarted = true;
                this.Connect();
            }
        }

        /// <summary>
        /// Stop command
        /// </summary>
        public void Stop()
        {
            if (this.boolStarted)
            {
                this.boolStarted = false;
                this.DisConnect();
            }
        }

        #endregion

        #region Public Modbus Method

        /// <summary>
        /// Read Multiple Input Coils status
        /// </summary>
        /// <param name="paramSlaveID">Slave ID</param>
        /// <param name="paramStartIndex">Start Index</param>
        /// <param name="paramNumofPoints">Number of Points</param>
        /// <returns>Status Array</returns>
        public bool[] ReadMultiInputCoils(byte paramSlaveID, ushort paramStartIndex, ushort paramNumofPoints)
        {
            bool[] Coils = null;
            try
            {
                if (this.Connected)
                {
                    lock (this.masterLock)
                    {
                        if (this.master != null)
                        {
                            Coils = this.master.ReadInputs(paramSlaveID, paramStartIndex, paramNumofPoints);
                            this.strError = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
            }
            finally
            {
                this.CheckConnectStatus();
            }
            return Coils;
        }

        /// <summary>
        /// Read Multiple Input Coils status
        /// </summary>
        /// <param name="paramStartIndex">Start Index</param>
        /// <param name="paramNumofPoints">Number of Points</param>
        /// <returns>Status Array</returns>
        public bool[] ReadMultiInputCoils(ushort paramStartIndex, ushort paramNumofPoints)
        {
            return ReadMultiInputCoils(this.byteSlaveID, paramStartIndex, paramNumofPoints);
        }

        /// <summary>
        /// Read Multiple Coils status
        /// </summary>
        /// <param name="paramSlaveID">Slave ID</param>
        /// <param name="paramStartIndex">Start Index</param>
        /// <param name="paramNumofPoints">Number of Points</param>
        /// <returns>Status Array</returns>
        public bool[] ReadMultiCoils(byte paramSlaveID, ushort paramStartIndex, ushort paramNumofPoints)
        {
            bool[] Coils = null;
            try
            {
                if (this.Connected)
                {
                    lock (this.masterLock)
                    {
                        if (this.master != null)
                        {
                            Coils = this.master.ReadCoils(paramSlaveID, paramStartIndex, paramNumofPoints);
                            this.strError = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
            }
            finally
            {
                this.CheckConnectStatus();
            }
            return Coils;
        }

        /// <summary>
        /// Read Multiple Coils status
        /// </summary>
        /// <param name="paramStartIndex">Start Index</param>
        /// <param name="paramNumofPoints">Number of Points</param>
        /// <returns>Status Array</returns>
        public bool[] ReadMultiCoils(ushort paramStartIndex, ushort paramNumofPoints)
        {
            return ReadMultiCoils(this.byteSlaveID, paramStartIndex, paramNumofPoints);
        }

        /// <summary>
        /// Read Multiple Input Register values
        /// </summary>
        /// <param name="paramSlaveID">Slave ID</param>
        /// <param name="paramStartIndex">Start Index</param>
        /// <param name="paramNumofPoints">Number of Points</param>
        /// <returns>Values Array</returns>
        public ushort[] ReadMultiInputRegisters(byte paramSlaveID, ushort paramStartIndex, ushort paramNumofPoints)
        {
            ushort[] Registers = null;
            try
            {
                if (this.Connected)
                {
                    lock (this.masterLock)
                    {
                        if (this.master != null)
                        {
                            Registers = this.master.ReadInputRegisters(paramSlaveID, paramStartIndex, paramNumofPoints);
                            this.strError = "";
                        }
                    }
               }
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
            }
            finally
            {
                this.CheckConnectStatus();
            }

            return Registers;
        }

        /// <summary>
        /// Read Multiple Input Register values
        /// </summary>
        /// <param name="paramStartIndex">Start Index</param>
        /// <param name="paramNumofPoints">Number of Points</param>
        /// <returns>Values Array</returns>
        public ushort[] ReadMultiInputRegisters(ushort paramStartIndex, ushort paramNumofPoints)
        {
            return ReadMultiInputRegisters(this.byteSlaveID, paramStartIndex, paramNumofPoints);
        }

        /// <summary>
        /// Read Multiple Hold Register values
        /// </summary>
        /// <param name="paramSlaveID">Slave ID</param>
        /// <param name="paramStartIndex">Start Index</param>
        /// <param name="paramNumofPoints">Number of Points</param>
        /// <returns>Values Array</returns>
        public ushort[] ReadMultiHoldRegisters(byte paramSlaveID, ushort paramStartIndex, ushort paramNumofPoints)
        {
            ushort[] Registers = null;
            try
            {
                if (this.Connected)
                {
                    lock (this.masterLock)
                    {
                        if (this.master != null)
                        {
                            Registers = this.master.ReadHoldingRegisters(paramSlaveID, paramStartIndex, paramNumofPoints);
                            this.strError = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
            }
            finally
            {
                this.CheckConnectStatus();
            }

            return Registers;
        }

        /// <summary>
        /// Read Multiple Hold Register values
        /// </summary>
        /// <param name="paramStartIndex">Start Index</param>
        /// <param name="paramNumofPoints">Number of Points</param>
        /// <returns>Values Array</returns>
        public ushort[] ReadMultiHoldRegisters(ushort paramStartIndex, ushort paramNumofPoints)
        {
            return ReadMultiHoldRegisters(this.byteSlaveID, paramStartIndex, paramNumofPoints);
        }

        /// <summary>
        /// Write Single Coil status
        /// </summary>
        /// <param name="paramSlaveID">Slave ID</param>
        /// <param name="paramIndex">Index</param>
        /// <param name="paramData">Status</param>
        /// <returns>True/False</returns>
        public bool WriteSingleCoil(byte paramSlaveID, ushort paramIndex, bool paramData)
        {
            bool isSuccess = false;
            try
            {
                if (this.Connected)
                {
                    lock (this.masterLock)
                    {
                        if (this.master != null)
                        {
                            this.master.WriteSingleCoil(paramSlaveID, paramIndex, paramData);
                            this.strError = "";
                            isSuccess = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
            }
            finally
            {
                this.CheckConnectStatus();
            }

            return isSuccess;
        }

        /// <summary>
        /// Write Single Coil status
        /// </summary>
        /// <param name="paramIndex">Index</param>
        /// <param name="paramData">Status</param>
        /// <returns>True/False</returns>
        public bool WriteSingleCoil(ushort paramIndex, bool paramData)
        {
            return WriteSingleCoil(this.byteSlaveID, paramIndex, paramData);
        }

        /// <summary>
        /// Write Multiple Coils Status
        /// </summary>
        /// <param name="paramSlaveID">Slave ID</param>
        /// <param name="paramStartIndex">Start Index</param>
        /// <param name="paramData">Status Array</param>
        /// <returns>True/False</returns>
        public bool WriteMultipleCoils(byte paramSlaveID, ushort paramStartIndex, bool[] paramData)
        {
            bool isSuccess = false;
            try
            {
                if (this.Connected)
                {
                    lock (this.masterLock)
                    {
                        if (this.master != null)
                        {
                            this.master.WriteMultipleCoils(paramSlaveID, paramStartIndex, paramData);
                            this.strError = "";
                            isSuccess = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
            }
            finally
            {
                this.CheckConnectStatus();
            }

            return isSuccess;
        }

        /// <summary>
        /// Write Multiple Coils status
        /// </summary>
        /// <param name="paramStartIndex">Start Index</param>
        /// <param name="paramData">Status Array</param>
        /// <returns>True/False</returns>
        public bool WriteMultipleCoils(ushort paramStartIndex, bool[] paramData)
        {
            return WriteMultipleCoils(this.byteSlaveID, paramStartIndex, paramData);
        }

        /// <summary>
        /// Write Single Register value
        /// </summary>
        /// <param name="paramSlaveID">Slave ID</param>
        /// <param name="paramIndex">index</param>
        /// <param name="paramData">Value</param>
        /// <returns>True/False</returns>
        public bool WriteSingleRegister(byte paramSlaveID, ushort paramIndex, ushort paramData)
        {
            bool isSuccess = false;
            try
            {
                if (this.Connected)
                {
                    lock (this.masterLock)
                    {
                        if (this.master != null)
                        {
                            this.master.WriteSingleRegister(paramSlaveID, paramIndex, paramData);
                            this.strError = "";
                            isSuccess = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
            }
            finally
            {
                this.CheckConnectStatus();
            }

            return isSuccess;
        }

        /// <summary>
        /// Write Single Register value
        /// </summary>
        /// <param name="paramIndex">index</param>
        /// <param name="paramData">Value</param>
        /// <returns>True/False</returns>
        public bool WriteSingleRegister(ushort paramIndex, ushort paramData)
        {
            return WriteSingleRegister(this.byteSlaveID, paramIndex, paramData);
        }

        /// <summary>
        /// Write multiple Register values
        /// </summary>
        /// <param name="paramSlaveID">Slave ID</param>
        /// <param name="paramStartIndex">Start Index</param>
        /// <param name="paramData">Value Array</param>
        /// <returns>True/False</returns>
        public bool WriteMultipleRegisters(byte paramSlaveID, ushort paramStartIndex, ushort[] paramData)
        {
            bool isSuccess = false;
            try
            {
                if (this.Connected)
                {
                    lock (this.masterLock)
                    {
                        if (this.master != null)
                        {
                            this.master.WriteMultipleRegisters(paramSlaveID, paramStartIndex, paramData);
                            this.strError = "";
                            isSuccess = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
            }
            finally
            {
                this.CheckConnectStatus();
            }

            return isSuccess;
        }

        /// <summary>
        /// Write multiple Register values
        /// </summary>
        /// <param name="paramStartIndex">Start Index</param>
        /// <param name="paramData">Value Array</param>
        /// <returns>True/False</returns>
        public bool WriteMultipleRegisters(ushort paramStartIndex, ushort[] paramData)
        {
            return WriteMultipleRegisters(this.byteSlaveID, paramStartIndex, paramData);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Modbus connect status changed event
        /// </summary>
        /// <param name="e">Connect changed event argument</param>
        protected void OnConnectChanged(ConnectChangedArgument e)
        {
            if (this.ConnectChanged != null)
                ConnectChanged(this, e);
        }

        /// <summary>
        /// Modbus alarm happened event
        /// </summary>
        /// <param name="e">Alarm happened event argument</param>
        protected void OnAlarmHappened(AlarmHappenedArgument e)
        {
            if (this.AlarmHappened != null)
                AlarmHappened(this, e);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Connect to device.
        /// </summary>
        /// <returns>if false, Check ErrorMessage property</returns>
        private bool Connect()
        {
            if (!this.Connected)
            {
                lock (this.masterLock)
                {
                    ConnectDevice();
                }
            }
            return this.Connected;
        }

        /// <summary>
        /// Disconnect to device.
        /// </summary>
        private void DisConnect()
        {
            try
            {
                lock (this.masterLock)
                {
                    if (this.Connected)
                        this.strError = "Disconnect by Stop().";
                    if (this.connectTimer != null)
                    {
                        this.connectTimer.Dispose();
                        this.connectTimer = null;
                    }
                    if (this.master != null)
                    {
                        this.master.Dispose();
                        this.master = null;
                    }
                    if (this.tcpClient != null)
                    {
                        this.tcpClient.Close();
                        this.tcpClient = null;
                    }
                    if (this.udpClient != null)
                    {
                        this.udpClient.Close();
                        this.udpClient = null;
                    }
                    if (this.serialPort != null)
                    {
                        if (this.serialPort.IsOpen)
                            this.serialPort.Close();
                    }
                    this.SetCheckTimer(false);
                    this.SetConnectTimer(false);
                    this.ConnectStatus(false);
                }
            }
            catch (Exception) { }
        }

        /// <summary> 
        /// Check protocol and connect to device
        /// </summary>
        private void ConnectDevice()
        {
            bool isOk = false;

            switch (this.connectType)
            {
                case ConnectType.TCP:
                    try
                    {
                        if (this.strIPAddress == null)
                            this.strError = "IP Address not Assigned !!";
                        else if (this.strIPAddress.Trim().Length == 0)
                            this.strError = "IP Address not Assigned !!";
                        else if (!CheckNetwork())
                            this.strError = "Network Lan failure !!";
                        else
                        {
                            lock (this.masterLock)
                            {
                                this.tcpClient = new TcpClient();
                                this.SetScoketParameter(this.tcpClient.Client);
                                IAsyncResult asyncResult = this.tcpClient.BeginConnect(this.strIPAddress, this.intPort, null, null);
                                asyncResult.AsyncWaitHandle.WaitOne(this.intTimeout * 10, true);
                                if ((!asyncResult.IsCompleted) || (!this.tcpClient.Connected))
                                    this.strError = "TcpClient connect failure !!";
                                else
                                {
                                    this.master = ModbusIpMaster.CreateIp(this.tcpClient);
                                    this.master.Transport.Retries = 0;   //don't have to do retries
                                    this.master.Transport.ReadTimeout = this.intTimeout;
                                    this.connectedTime = DateTime.Now;
                                    this.strError = "";
                                    isOk = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Source.Equals("nModbusPC"))
                            this.strError = "Modbus create failure !!";
                        else
                            this.strError = "TcpClient connect failure !!";
                    }
                    finally
                    {
                        if (!isOk)
                        {
                            lock (this.masterLock)
                            {
                                if (this.master != null)
                                {
                                    this.master.Dispose();
                                    this.master = null;
                                }
                                if (this.tcpClient != null)
                                {
                                    this.tcpClient.Close();
                                    this.tcpClient = null;
                                }
                            }
                        }
                        this.SetCheckTimer(isOk);
                        this.SetConnectTimer(!isOk);
                        this.ConnectStatus(isOk);
                    }
                    break;
                case ConnectType.UDP:
                    try
                    {
                        if (this.strIPAddress == null)
                            this.strError = "IP Address not Assigned !!";
                        else if (this.strIPAddress.Trim().Length == 0)
                            this.strError = "IP Address not Assigned !!";
                        else if (!CheckNetwork())
                            this.strError = "Network Lan failure !!";
                        else
                        {
                            lock (this.masterLock)
                            {
                                this.udpClient = new UdpClient();
                                this.udpClient.Connect(this.strIPAddress, this.intPort);
                                this.master = ModbusIpMaster.CreateIp(this.udpClient);
                                this.master.Transport.Retries = 0;   //don't have to do retries
                                this.master.Transport.ReadTimeout = this.intTimeout;
                                this.connectedTime = DateTime.Now;
                                this.strError = "";
                                isOk = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Source.Equals("nModbusPC"))
                            this.strError = "Modbus create failure !!";
                        else
                            this.strError = "UdpClient connect failure !!";
                    }
                    finally
                    {
                        if (!isOk)
                        {
                            lock (this.masterLock)
                            {
                                if (this.master != null)
                                {
                                    this.master.Dispose();
                                    this.master = null;
                                }
                                if (this.udpClient != null)
                                {
                                    this.udpClient.Close();
                                    this.udpClient = null;
                                }
                            }
                        }
                        this.SetConnectTimer(!isOk);
                        this.ConnectStatus(isOk);
                    }
                    break;
                case ConnectType.RTU:
                    try
                    {
                        if (this.serialPort == null)
                            this.strError = "Serial Port not Assigned !!";
                        else
                        {
                            lock (this.masterLock)
                            {
                                this.serialPort.Open();
                                this.master = ModbusSerialMaster.CreateRtu(this.serialPort);
                                this.master.Transport.Retries = 0;   //don't have to do retries
                                this.master.Transport.ReadTimeout = this.intTimeout;
                                this.connectedTime = DateTime.Now;
                                this.strError = "";
                                isOk = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Source.Equals("nModbusPC"))
                            this.strError = "Modbus create failure !!";
                        else
                            this.strError = "Serial Port open failure !!";
                    }
                    finally
                    {
                        lock (this.masterLock)
                        {
                            if ((this.master != null) && (!isOk))
                            {
                                this.master.Dispose();
                                this.master = null;
                            }
                        }
                        this.ConnectStatus(isOk);
                    }
                    break;
                case ConnectType.ASCII:
                    try
                    {
                        if (this.serialPort == null)
                            this.strError = "Serial Port not Assigned !!";
                        {
                            lock (this.masterLock)
                            {
                                this.serialPort.Open();
                                this.master = ModbusSerialMaster.CreateAscii(this.serialPort);
                                this.master.Transport.Retries = 0;   //don't have to do retries
                                this.master.Transport.ReadTimeout = this.intTimeout;
                                this.connectedTime = DateTime.Now;
                                this.strError = "";
                                isOk = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Source.Equals("nModbusPC"))
                            this.strError = "Modbus create failure !!";
                        else
                            this.strError = "Serial Port open failure !!";
                    }
                    finally
                    {
                        lock (this.masterLock)
                        {
                            if ((this.master != null) && (!isOk))
                            {
                                this.master.Dispose();
                                this.master = null;
                            }
                        }
                        this.ConnectStatus(isOk);
                    }
                    break;
            }
        }

        /// <summary>
        /// Modbus reconnect method for timer
        /// </summary>
        /// <param name="state">parameter of callback function</param>
        private void Reconnect(object state)
        {
            try
            {
                if (this.CheckNetwork())
                    this.Connect();
            }
            catch { }
        }

        /// <summary>
        /// Modbus Connected Check for timer
        /// </summary>
        /// <param name="state">parameter of callback function</param>
        private void CheckConnected(object state)
        {
            string strMsg = "";
            try
            {
                if (!this.CheckNetwork())
                    strMsg = "Network Lan failure !!";
                else if (! this.Connected)
                    strMsg = "Abnormal Disconnect !!";
                if (strMsg.Length > 0)
                {
                    this.strError = strMsg;
                    this.ConnectStatus(false);
                }
            }
            catch {}
        }

        /// <summary>
        /// Check Connect Status
        /// </summary>
        private void CheckConnectStatus()
        {
            if (this.ConnectType == ConnectType.TCP)
            {
                if (!this.Connected)
                {
                    this.strError = "Abnormal Disconnect !!";
                    this.ConnectStatus(false);
                }
            }
        }

        /// <summary>
        /// Set error/alarm message on exception
        /// </summary>
        /// <param name="paramException">exception</param>
        private void SetErrorMessage(Exception paramException)
        {
            //Connection exception
            //No response from server.
            //The server maybe close the com port, or socket disconnect, or response timeout.
            if (paramException.Source.Equals("System"))
            {
                if (this.connectType == ConnectType.TCP)
                {
                    this.strError = "TCP Client Disconnected !!";
                    this.ConnectStatus(false);
                }
                else
                {
                    this.strError = "Response Timeout !!";
                    this.SetAlarm(this.strError);
                }
            }

            //The server return error code.
            //You can get the function code and exception code.
            if (paramException.Source.Equals("nModbusPC"))
            {
                string str = paramException.Message;
                int FunctionCode;
                string ExceptionCode;

                str = str.Remove(0, str.IndexOf("\r\n") + 17);
                FunctionCode = Convert.ToInt16(str.Remove(str.IndexOf("\r\n")));

                str = str.Remove(0, str.IndexOf("\r\n") + 17);
                ExceptionCode = str.Remove(str.IndexOf("-"));
                switch (ExceptionCode.Trim())
                {
                    case "1":
                        this.strError = "Exception Code: " + ExceptionCode.Trim() + "----> Illegal function !!";
                        break;
                    case "2":
                        this.strError = "Exception Code: " + ExceptionCode.Trim() + "----> Illegal data address !!";
                        break;
                    case "3":
                        this.strError = "Exception Code: " + ExceptionCode.Trim() + "----> Illegal data value !!";
                        break;
                    case "4":
                        this.strError = "Exception Code: " + ExceptionCode.Trim() + "----> Slave device failure !!";
                        break;
                    case "5":
                        this.strError = "Exception Code: " + ExceptionCode.Trim() + "----> Slave time out !!";
                        break;
                    case "6":
                        this.strError = "Exception Code: " + ExceptionCode.Trim() + "----> Slave device busy !!";
                        break;
                    case "8":
                        this.strError = "Exception Code: " + ExceptionCode.Trim() + "----> Slave memory parity error !!";
                        break;
                    case "A":
                        this.strError = "Exception Code: " + ExceptionCode.Trim() + "----> Gateway path unavilable !!";
                        break;
                    case "B":
                        this.strError = "Exception Code: " + ExceptionCode.Trim() + "----> Gateway target device falied !!";
                        break;
                }
                SetAlarm(this.strError);
            }
            /*
                //Modbus exception codes definition                            
                * Code   * Name                                      * Meaning
                    01       ILLEGAL FUNCTION                            The function code received in the query is not an allowable action for the server.

                    02       ILLEGAL DATA ADDRESS                        The data addrdss received in the query is not an allowable address for the server.

                    03       ILLEGAL DATA VALUE                          A value contained in the query data field is not an allowable value for the server.

                    04       SLAVE DEVICE FAILURE                        An unrecoverable error occurred while the server attempting to perform the requested action.

                    05       ACKNOWLEDGE                                 This response is returned to prevent a timeout error from occurring in the client (or master)
                                                                        when the server (or slave) needs a long duration of time to process accepted request.

                    06       SLAVE DEVICE BUSY                           The server (or slave) is engaged in processing a long–duration program command , and the
                                                                        client (or master) should retransmit the message later when the server (or slave) is free.

                    08       MEMORY PARITY ERROR                         The server (or slave) attempted to read record file, but detected a parity error in the memory.

                    0A       GATEWAY PATH UNAVAILABLE                    The gateway is misconfigured or overloaded.

                    0B       GATEWAY TARGET DEVICE FAILED TO RESPOND     No response was obtained from the target device. Usually means that the device is not present on the network.
                */
        }

        /// <summary>
        /// Check Local network
        /// </summary>
        /// <returns>Network Status</returns>
        private bool CheckNetwork()
        {
            PInvoke.InternetConnectionState flag = PInvoke.InternetConnectionState.INTERNET_CONNECTION_LAN;
            return PInvoke.InternetGetConnectedState(ref flag, 0);
        }

        /// <summary>
        /// Set Connect Status
        /// </summary>
        /// <param name="paramStatus">Connect Status</param>
        private void ConnectStatus(bool paramStatus)
        {
            bool isChanged = false;

            lock (this.masterLock)
            {
                if (paramStatus != this.boolConnected)
                {
                    this.boolConnected = paramStatus;
                    isChanged = true;
                }
            }

            if (!isChanged)
                return;

            ConnectChangedArgument e = new ConnectChangedArgument();
            e.ConnectType = this.connectType;
            if ((this.connectType == ConnectType.TCP) || (this.connectType == ConnectType.UDP))
                e.RemoteIPEndPoint = new IPEndPoint(System.Net.IPAddress.Parse(this.strIPAddress), this.Port);
            else
                e.SerialPort = this.serialPort;

            e.IsConnected = this.Connected;
            if (!this.Connected)
            {
                e.ConnectedTime = this.connectedTime;
                e.DisconnectedTime = DateTime.Now;
                e.DisconnectReason = this.strError;
                this.connectedTime = null;
            }
            else
            {
                e.ConnectedTime = this.connectedTime;
                e.DisconnectedTime = null;
            }

            if (this.connectType == ConnectType.TCP)
            {
                this.SetConnectTimer(!this.Connected);
                this.SetCheckTimer(this.Connected);
            }
            this.OnConnectChanged(e);
        }

        /// <summary>
        /// Set alarm message for alarm happened event
        /// </summary>
        /// <param name="paramAlarm">Alarm Message</param>
        private void SetAlarm(string paramAlarm)
        {
            AlarmHappenedArgument e = new AlarmHappenedArgument();
            e.ConnectType = this.connectType;
            if ((this.connectType == ConnectType.TCP) || (this.connectType == ConnectType.UDP))
                e.RemoteIPEndPoint = new IPEndPoint(System.Net.IPAddress.Parse(this.strIPAddress), this.Port);
            else
                e.SerialPort = this.serialPort;
            e.AlarmMessage = paramAlarm;
            this.OnAlarmHappened(e);
        }

        /// <summary>
        /// Enable/Disable Connect Timer
        /// </summary>
        /// <param name="paramEnable"></param>
        private void SetConnectTimer(bool paramEnable)
        {
            if (this.reconnectInterval <= 0)
                return;

            if ((paramEnable) && (this.boolStarted))
            {
                if (this.connectTimer == null)
                    this.connectTimer = new System.Threading.Timer(this.Reconnect, null, this.reconnectInterval, this.reconnectInterval);
            }
            else
            {
                if (this.connectTimer != null)
                {
                    this.connectTimer.Dispose();
                    this.connectTimer = null;
                }
            }
        }

        /// <summary>
        /// Enable/Disable Check Timer
        /// </summary>
        /// <param name="paramEnable"></param>
        private void SetCheckTimer(bool paramEnable)
        {
            if (this.checkInterval <= 0)
                return;

            if ((paramEnable) && (this.boolStarted))
            {
                if (this.checkTimer == null)
                    this.checkTimer = new System.Threading.Timer(this.CheckConnected, null, this.checkInterval, this.checkInterval);
            }
            else
            {
                if (this.checkTimer != null)
                {
                    this.checkTimer.Dispose();
                    this.checkTimer = null;
                }
            }
        }

        /// <summary>
        /// Set Socket Parameter
        /// </summary>
        /// <param name="Socket">Socket Object</param>
        private void SetScoketParameter(System.Net.Sockets.Socket Socket)
        {
            Socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.NoDelay, 1);
            Socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.KeepAlive, 1);
            Socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReuseAddress, 1);
            Socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.DontLinger, 0);
            Socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReceiveTimeout, 10000);
            this.SetKeepAliveValues(Socket, true, 5, 2);
        }

        /// <summary>
        /// Setting Keep Alive Parameter
        /// </summary>
        /// <param name="Socket">Socket Object</param>
        /// <param name="Enable">Enable Keep Alive Function or not</param>
        /// <param name="KeepaLiveIdleTime">Keep Alive Idle Time</param>
        /// <param name="KeepaLiveInterval">Keep Alive Interval Time</param>
        /// <returns></returns>
        private int SetKeepAliveValues(System.Net.Sockets.Socket Socket, bool Enable, uint KeepaLiveIdleTime, uint KeepaLiveInterval)
        {
            int Result = -1;
            unsafe
            {
                TcpKeepAlive KeepAliveValues = new TcpKeepAlive();

                KeepAliveValues.Enable = Convert.ToUInt32(Enable);
                KeepAliveValues.KeepAliveIdleTime = KeepaLiveIdleTime;
                KeepAliveValues.KeepAliveInterval = KeepaLiveInterval;

                byte[] InValue = new byte[12];

                for (int I = 0; I < 12; I++)
                    InValue[I] = KeepAliveValues.Bytes[I];

                Result = Socket.IOControl(IOControlCode.KeepAliveValues, InValue, null);
            }
            return Result;
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
                    // TODO: Release Managed Objects
                }

                this.Stop();
                this.serialPort = null;
                this.strIPAddress = null;
                this.strError = null;

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
