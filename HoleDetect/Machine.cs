using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TYG.HoleDetect
{
    /// <summary>Data Acquistion for Machine</summary>
    public class Machine : IDisposable
    {

        #region Static Constant Variables

        /// <summary>Hole Detect Steady Counter</summary>
        private static readonly int SteadyHoleCount = 2;
        /// <summary>Scan Detect Steady Counter</summary>
        private static readonly int SteadyScanCount = 2;
        /// <summary>Event Internal (ms)</summary>
        private static readonly int EventInternal = 1000;

        #endregion

        #region Private Variabes

        private Area parent = null;
        private string strID = "";
        private int slaveNo = 0;
        private string strPlatFormID = "";
        private int ioChannelNo = 0;
        private string strHoleID = "00";
        private string strMHoleID = "";
        private string strByPassState = "";
        private string strTempHoleID = "";
        private int countHollID = 0;
        private string strWrkOrderNo = "";
        private string strTempWrkOrderNo = "";
        private int countWrkOrderNo = 0;

        private string strPreWorkOrderNo = "";
        private int runStatus = 0;
        private int prodQty = 0;
        private int preProdQty = 0;
        private DateTime plcTime;
        private bool forbidMotor = false;
        private int lampStatus = 0;
        private bool boolMaterialCheck = true;
        private bool boolDataChanged = true;
        private object lockMaterialCheck = new object();
        private object lockDataChanged = new object();
        private System.Threading.Timer eventTimer = null;
        private object timerLock = new object();
        private bool isStarted = false;
        private DateTime refreshTime = DateTime.Now;
        private bool testMode = false;
        private string productID = "";
        private int orderQty = 0;
        private string orderType = "";
        private string orderStatus = "";
        private bool onDuty = false;
        private List<string> orderMaterialList = null;
        private string orderMRatio = "";
        private string specMaterial = "";
        private float weight = 0.0f;
        private string weightUnit = "";
        private int electricPower = 0;
        private int instantVoltage = 0;
        private int instantCurrent = 0;
        private int powerFlag = 0;
        private int temperatureNumber = 0;
        private string[] temperatureValue = new string[8] { "", "", "", "", "", "", "", "" };

        #endregion

        #region Events

        /// <summary>
        /// Data Changed Event
        /// </summary>
        internal event DataChangedEventHandler DataChanged;

        /// <summary>
        /// Material Check Event
        /// </summary>
        internal event MaterialCheckEventHandler MaterialCheck;

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Parent Object (Area)</param>
        /// <param name="paramID">Machine ID</param>
        /// <param name="paramSlaveNo">Machine PLC Slave No.</param>
        /// <param name="paramPlatFormID">PlatForm ID</param>
        /// <param name="paramIOChannelNo">Remote I/O Channel No.</param>
        /// <param name="paramTestMode">Test Mode (No forbid motor output)</param>
        internal Machine(Area parent, string paramID, int paramSlaveNo, string paramPlatFormID, int paramIOChannelNo, bool paramTestMode, string paramMHoleID)
        {
            this.parent = parent;
            this.strID = paramID;
            this.slaveNo = paramSlaveNo;
            this.strPlatFormID = paramPlatFormID;
            this.ioChannelNo = paramIOChannelNo;
            this.testMode = paramTestMode;
            this.MHoleID = paramMHoleID;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        ~Machine()
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
        /// Machine ID
        /// </summary>
        public string ID
        {
            get { return this.strID; }
        }

        /// <summary>
        /// Machine PLC Slave No.
        /// </summary>
        public int PlcSlaveNo
        {
            get { return this.slaveNo; }
        }

        /// <summary>
        /// PlatForm ID
        /// </summary>
        public string PlatFormID
        {
            get { return this.strPlatFormID; }
        }

        /// <summary>
        /// Area ID
        /// </summary>
        public string AreaID
        {
            get { return this.Parent.ID; }
        }

        /// <summary>
        /// Factory ID
        /// </summary>
        public string FactoryID
        {
            get { return this.Parent.Parent.ID; }
        }

        /// <summary>
        /// PLC Connect Status
        /// </summary>
        public bool PlcConnected
        {
            get { return this.Parent.Connected; }
        }

        /// <summary>
        /// Remote I/O Connect Status
        /// </summary>
        public bool IOConnected
        {
            get { return this.Parent.PlatForms[this.strPlatFormID].Connected; }
        }


        /// <summary>
        /// Remote I/O Channel No.
        /// </summary>
        public int IOChannelNo
        {
            get { return this.ioChannelNo; }
        }

        /// <summary>
        /// Event Check Enable or not
        /// </summary>
        public bool Started
        {
            get { return this.isStarted; }
        }

        /// <summary>
        /// Hole ID
        /// </summary>
        public string HoleID
        {
            get { return this.strHoleID; }
            set
            {
                if (this.strHoleID != value)
                {
                    this.strHoleID = value;
                    lock (this.lockMaterialCheck)
                    {
                        this.boolMaterialCheck = true;
                    }
                }
            }
        }

        /// <summary>
        /// RemoteIO Number
        /// </summary>
        public string MHoleID
        {
            get { return this.strMHoleID; }
            set
            {
                if (this.strMHoleID != value)
                {
                    this.strMHoleID = value;
                    lock (this.lockMaterialCheck)
                    {
                        this.boolMaterialCheck = true;
                    }
                }
            }
        }

        /// <summary>
        /// ByPassState
        /// </summary>
        public string ByPassState
        {
            get { return this.strByPassState; }
            set
            {
                if (this.strByPassState != value)
                {
                    this.strByPassState = value;
                    lock (this.lockMaterialCheck)
                    {
                        this.boolMaterialCheck = true;
                    }
                }
            }
        }

        /// <summary>
        /// Scan Code
        /// </summary>
        public string WorkOrderNo
        {
            get { return this.strWrkOrderNo; }
            private set
            {
                if (this.strWrkOrderNo != value)
                {
                    this.strWrkOrderNo = value;
                    this.orderMaterialList = null;
                    lock (this.lockMaterialCheck)
                    {
                        this.boolMaterialCheck = true;
                    }
                }
            }
        }

        /// <summary>
        /// Forbid Motor
        /// </summary>
        public bool ForbidMotor
        {
            get { return this.forbidMotor; }
            private set
            {
                if (this.forbidMotor != value)
                {
                    this.forbidMotor = value;
                    lock (this.lockDataChanged)
                    {
                        this.boolDataChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Lamp Status
        /// </summary>
        public int LampStatus
        {
            get { return this.lampStatus; }
            private set
            {
                if (this.lampStatus != value)
                {
                    this.lampStatus = value;
                    lock (this.lockDataChanged)
                    {
                        this.boolDataChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Run Status
        /// </summary>
        public int RunStatus
        {
            get { return this.runStatus; }
            private set
            {
                if (this.runStatus != value)
                {
                    this.runStatus = value;
                    lock (this.lockDataChanged)
                    {
                        this.boolDataChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Quantity
        /// </summary>
        public int ProdQty
        {
            get { return this.prodQty; }
            private set
            {
                if (this.prodQty != value)
                {
                    this.prodQty = value;
                    lock (this.lockDataChanged)
                    {
                        this.boolDataChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Previous Quantity
        /// </summary>
        public int PreProdQty
        {
            get { return this.preProdQty; }
            private set
            {
                if (this.preProdQty != value)
                {
                    this.preProdQty = value;
                    lock (this.lockDataChanged)
                    {
                        this.boolDataChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Previous Scan Code
        /// </summary>
        public string PreWorkOrderNo
        {
            get { return this.strPreWorkOrderNo; }
            private set
            {
                if (this.strPreWorkOrderNo != value)
                {
                    this.strPreWorkOrderNo = value;
                    lock (this.lockDataChanged)
                    {
                        this.boolDataChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Piecw Weight
        /// </summary>
        public float Weight
        {
            get { return this.weight; }
            private set
            {
                if (this.weight != value)
                {
                    this.weight = value;
                    lock (this.lockDataChanged)
                    {
                        this.boolDataChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Weight Unit
        /// </summary>
        public string WeightUnit
        {
            get { return this.weightUnit; }
            private set
            {
                if (this.weightUnit != value)
                {
                    this.weightUnit = value;
                    lock (this.lockDataChanged)
                    {
                        this.boolDataChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// ElectricPower 20210505新增電力消耗紀錄
        /// </summary>
        public int ElectricPower
        {
            get { return this.electricPower; }
            private set
            {
                if (this.electricPower != value)
                {
                    this.electricPower = value;
                    lock (this.lockDataChanged)
                    {
                        this.boolDataChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// InstantVoltage 20210913新增POWERFLAG
        /// </summary>
        public int PowerFlag
        {
            get { return this.powerFlag; }
            private set
            {
                if (this.powerFlag != value)
                {
                    this.powerFlag = value;
                    //lock (this.lockDataChanged)
                    //{
                    //    this.boolDataChanged = true;
                    //}
                }
            }
        }

        /// <summary>
        /// TemperatureNumber 20220607新增
        /// </summary>
        public int TemperatureNumber
        {
            get { return this.temperatureNumber; }
            private set
            {
                if (this.temperatureNumber != value)
                {
                    this.temperatureNumber = value;
                    //lock (this.lockDataChanged)
                    //{
                    //    this.boolDataChanged = true;
                    //}
                }
            }
        }

        /// <summary>
        ///  TemperatureValue[] 20220607新增
        /// </summary>
        public string[] TemperatureValue
        {
            get { return this.temperatureValue; }
            private set
            {
                if (!this.temperatureValue.SequenceEqual(value))
                {
                    this.temperatureValue = value;
                    //lock (this.lockDataChanged)
                    //{
                    //    this.boolDataChanged = true;
                    //}
                }
            }
        }

        /// <summary>
        /// InstantVoltage 20210701新增即時電壓數據
        /// </summary>
        public int InstantVoltage
        {
            get { return this.instantVoltage; }
            private set
            {
                if (this.instantVoltage != value)
                {
                    this.instantVoltage = value;
                    //lock (this.lockDataChanged)
                    //{
                    //    this.boolDataChanged = true;
                    //}
                }
            }
        }

        /// <summary>
        /// InstantCurrent 20210701新增即時電流數據
        /// </summary>
        public int InstantCurrent
        {
            get { return this.instantCurrent; }
            private set
            {
                if (this.instantCurrent != value)
                {
                    this.instantCurrent = value;
                    //lock (this.lockDataChanged)
                    //{
                    //    this.boolDataChanged = true;
                    //}
                }
            }
        }


        /// <summary>
        /// PLC Time
        /// </summary>
        public DateTime PlcTime
        {
            get { return this.plcTime; }
            private set
            {
                if (this.plcTime != value)
                {
                    this.plcTime = value;
                    //lock (this.lockDataChanged)
                    //{ this.boolDataChanged = true; }
                }
            }
        }

        /// <summary>
        /// Data Refresh Time
        /// </summary>
        public DateTime RefreshTime
        {
            get { return this.refreshTime; }
        }

        /// <summary>
        /// Test Mode
        /// </summary>
        public bool TestMode
        {
            get { return this.testMode; }
        }

        /// <summary>
        /// On Duty
        /// </summary>
        public bool OnDuty
        {
            get { return this.onDuty; }
            internal set
            {
                if (this.onDuty != value)
                {
                    this.onDuty = value;
                }
            }
        }

        /// <summary>
        /// Order Material List
        /// </summary>
        public List<string> OrderMaterialList
        {
            get { return this.orderMaterialList; }
            internal set
            {
                if (this.orderMaterialList != value)
                {
                    this.orderMaterialList = value;
                }
            }
        }

        /// <summary>
        /// Order Material Ratio
        /// </summary>
        public string OrderMRatio
        {
            get { return this.orderMRatio; }
            internal set
            {
                if (this.orderMRatio != value)
                {
                    this.orderMRatio = value;
                }
            }
        }

        public string SpecMaterial
        {
            get { return this.specMaterial; }
            internal set
            {
                if (this.specMaterial != value)
                {
                    this.specMaterial = value;
                }
            }
        }

        /// <summary>
        /// Product ID
        /// </summary>
        public string ProductID
        {
            get { return this.productID; }
            set
            {
                if (this.productID != value)
                {
                    this.productID = value;
                    lock (this.lockDataChanged)
                    {
                        this.boolDataChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Order Type
        /// </summary>
        public string OrderType
        {
            get { return this.orderType; }
            set
            {
                if (this.orderType != value)
                {
                    this.orderType = value;
                    lock (this.lockDataChanged)
                    {
                        this.boolDataChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Order Status
        /// </summary>
        public string OrderStatus
        {
            get { return this.orderStatus; }
            set
            {
                if (this.orderStatus != value)
                {
                    this.orderStatus = value;
                    lock (this.lockDataChanged)
                    {
                        this.boolDataChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Order Quantity
        /// </summary>
        public int OrderQty
        {
            get { return this.orderQty; }
            set
            {
                if (this.orderQty != value)
                {
                    this.orderQty = value;
                    lock (this.lockDataChanged)
                    {
                        this.boolDataChanged = true;
                    }
                }
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Set PLC Lamp Condition
        /// </summary>
        /// <param name="paramLampValue">PLC Lamp condition setting</param>
        /// <returns>Is Sucess</returns>
        public bool SetPlcLamp(int paramLampValue)
        {
            bool isSuccess = false;
            if (this.Parent != null)
            {
                if (this.Parent.MasterPLC != null)
                {
                    isSuccess = this.Parent.MasterPLC.SetSlavePlcLamp(this.slaveNo, paramLampValue);
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// Set PLC Motor Forbid
        /// </summary>
        /// <param name="paramLampValue">PLC Motor Forbid setting</param>
        /// <returns>Is Sucess</returns>
        public bool SetMotorForbid(bool paramForbidMotor)
        {
            bool isSuccess = false;
            if (this.Parent != null)
            {
                if (this.Parent.MasterPLC != null)
                {
                    isSuccess = this.Parent.MasterPLC.SetMotorForbid(this.slaveNo, paramForbidMotor);
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// Get Platform Object
        /// </summary>
        /// <returns>Platform Object</returns>
        public Platform GetPlatform()
        {
            return this.parent.GetPlatform(this.PlatFormID);
        }

        /// <summary>
        /// Get MPile Object
        /// </summary>
        /// <returns>MPile Object</returns>
        public MPile GetMPile()
        {
            MPile mpile = null;
            Platform platform = this.GetPlatform();
            if ((this.HoleID.Length > 0) && (platform != null))
            {
                foreach (MPile obj in platform.MPiles.Values)
                {
                    if (obj.ID.Equals(this.HoleID.Substring(0, 1)))
                    {
                        mpile = obj;
                        break;
                    }
                }
            }
            return mpile;
        }

        /// <summary>
        /// Get MHole Object
        /// </summary>
        /// <returns>MHole Object</returns>
        public MHole GetMHole()
        {
            MHole mhole = null;
            Platform platform = this.GetPlatform();
            if ((this.MHoleID.Length > 0) && (platform != null))
            {
                foreach (MHole obj in platform.MHoles.Values)
                {
                    if (obj.MachineID.Equals(this.ID))
                    {
                        mhole = obj;
                        break;
                    }
                }
            }
            return mhole;
        }
        #endregion

        #region Internal Method

        /// <summary>
        /// Start Machine Event 
        /// </summary>
        internal void Start()
        {
            if (!this.isStarted)
            {
                lock (this.timerLock)
                {
                    this.eventTimer = new System.Threading.Timer(this.EventCheck, null, EventInternal * 10, EventInternal);
                }
                this.isStarted = true;
            }
        }

        /// <summary>
        /// Stop Machine Event 
        /// </summary>
        internal void Stop()
        {
            if (this.isStarted)
            {
                try
                {
                    this.isStarted = false;
                    lock (this.timerLock)
                    {
                        if (this.eventTimer != null)
                        {
                            this.eventTimer.Change(Timeout.Infinite, Timeout.Infinite);
                            this.eventTimer.Dispose();
                            this.eventTimer = null;
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Set PLC Data and check ScanCode is Steady
        /// </summary>
        /// <param name="paramRunStatus">Run Status</param>
        /// <param name="paramForbidMotor">ForbidMotor Status</param>
        /// <param name="paramLampStatus">Lamp Status</param>
        /// <param name="paramScanCode">Scan Code</param>
        /// <param name="paramQty">Quantity</param>
        /// <param name="paramPreScanCode">Previous Scan Code</param>
        /// <param name="paramPreQty">Previous Quantity</param>
        /// <param name="paramSlaveTime">PLC Time</param>
        /// <param name="paramWeight"></param>
        /// <param name="paramWeightUnit"></param>
        /// <param name="paramElectricPower"></param>
        /// <param name="paramInstantVoltage"></param>
        /// <param name="paramInstantCurrent"></param>
        /// <param name="paramPowerFlag"></param>
        /// <param name="temperaturenumber"></param>
        /// <param name="temperaturevalue"></param>
        internal void PlcData(int paramRunStatus, int paramForbidMotor, int paramLampStatus, string paramScanCode, int paramQty,
                                                                        string paramPreScanCode, int paramPreQty, DateTime paramSlaveTime, float paramWeight, string paramWeightUnit, int paramElectricPower, int paramInstantVoltage, int paramInstantCurrent, int paramPowerFlag, int paramTemperatureNumber, string[] paramTemperatureValue)
        {
            if (this.strWrkOrderNo.Equals(paramScanCode))
            {
                this.strTempWrkOrderNo = paramScanCode;
                this.countWrkOrderNo = SteadyScanCount;
            }
            else
            {
                if (!this.strTempWrkOrderNo.Equals(paramScanCode))
                {
                    this.strTempWrkOrderNo = paramScanCode;
                    this.countWrkOrderNo = 0;
                }
                else
                {
                    this.countWrkOrderNo++;
                    if (this.countWrkOrderNo == SteadyScanCount - 1)
                    {
                        this.countWrkOrderNo = SteadyScanCount;
                        this.WorkOrderNo = paramScanCode;
                    }
                }
            }

            this.ForbidMotor = (paramForbidMotor == 1);
            this.LampStatus = paramLampStatus;
            this.RunStatus = paramRunStatus;
            this.ProdQty = paramQty;
            this.PreProdQty = paramPreQty;
            this.PreWorkOrderNo = paramPreScanCode;
            this.Weight = paramWeight;
            this.WeightUnit = paramWeightUnit;
            this.PlcTime = paramSlaveTime;
            this.ElectricPower = paramElectricPower;
            this.InstantVoltage = paramInstantVoltage;
            this.instantCurrent = paramInstantCurrent;
            this.PowerFlag = paramPowerFlag;
            this.TemperatureNumber = paramTemperatureNumber;
            this.TemperatureValue = paramTemperatureValue;
        }

        /// <summary>
        /// Set Hole ID and Check Steady
        /// </summary>
        /// <param name="paramHoleID">Hole ID</param>
        internal void SetHoleID(string paramHoleID)
        {
            if (this.strHoleID.Equals(paramHoleID))
            {
                this.strTempHoleID = paramHoleID;
                this.countHollID = SteadyHoleCount;
            }
            else
            {
                if (!this.strTempHoleID.Equals(paramHoleID))
                {
                    this.strTempHoleID = paramHoleID;
                    this.countHollID = 0;
                }
                else
                {
                    this.countHollID++;
                    if (this.countHollID == SteadyHoleCount - 1)
                    {
                        this.countHollID = SteadyHoleCount;
                        this.HoleID = paramHoleID;
                    }
                }
            }
        }

        /// <summary>
        /// Set Material Check Flag
        /// </summary>
        internal void SetMaterialCheck()
        {
            lock (this.lockMaterialCheck)
            {
                this.boolMaterialCheck = true;
            }
        }

        /// <summary>
        /// Set Test Mode
        /// </summary>
        internal void SetTestMode(bool paramTestMode)
        {
            this.testMode = paramTestMode;
        }


        #endregion

        #region Protected Method

        /// <summary>
        /// Trigger Material Check Event
        /// </summary>
        /// <param name="e">Event Argument</param>
        protected void OnMaterialCheck(MaterialCheckArgument e)
        {
            this.refreshTime = DateTime.Now;
            if (this.MaterialCheck != null)
            {
                this.MaterialCheck(this, e);
                if ((e.ForbidMotor != this.ForbidMotor) && (this.OnDuty))
                {
                    if (this.TestMode)
                    {
                        this.Parent.MasterPLC.SetMotorForbid(this.PlcSlaveNo, false);
                    }
                    else
                    {
                        this.Parent.MasterPLC.SetMotorForbid(this.PlcSlaveNo, e.ForbidMotor);
                    }
                }

                //if ((e.LampSignal != this.LampStatus) && (this.OnDuty))
                //20211103 資料有時會因開關機而導致有錯誤資料進來，因此拿掉燈號不同時才做信號發送條件。讓燈號能快速轉換。
                if (this.OnDuty)
                    this.Parent.MasterPLC.SetSlavePlcLamp(this.PlcSlaveNo, e.LampSignal);
            }
        }

        /// <summary>
        /// Trigger Data Changed Event
        /// </summary>
        /// <param name="e">Event Argument</param>
        protected void OnDataChanged(DataChangedArgument e)
        {
            this.refreshTime = DateTime.Now;
            if (this.DataChanged != null)
                this.DataChanged(this, e);
        }


        #endregion

        #region Private Method

        /// <summary>
        /// Prepared Event Argument for Material Check
        /// </summary>
        private void CheckMaterial()
        {
            MaterialCheckArgument e = new MaterialCheckArgument(this.Parent.Parent.ID, this.Parent.ID, this.PlatFormID, this.ID);
            e.ForbidMotor = this.ForbidMotor;
            this.OnMaterialCheck(e);
        }

        /// <summary>
        /// Prepared Event Argument for Data Changed
        /// </summary>
        private void ChangedData()
        {
            DataChangedArgument e = new DataChangedArgument(this.Parent.Parent.ID, this.Parent.ID, this.PlatFormID, this.ID);
            this.OnDataChanged(e);
        }

        /// <summary>
        /// Check Event
        /// </summary>
        /// <param name="state">parameter of callback function</param>
        private void EventCheck(object state)
        {
            bool bDataChanged = false;
            bool bMaterialCheck = false;
            try
            {
                lock (this.timerLock)
                {
                    if (this.eventTimer != null)
                    {
                        lock (this.lockDataChanged)
                        {
                            if (this.boolDataChanged)
                            {
                                bDataChanged = true;
                                this.boolDataChanged = false;
                            }
                        }
                        lock (this.lockMaterialCheck)
                        {
                            if (this.boolMaterialCheck)
                            {
                                bMaterialCheck = true;
                                this.boolMaterialCheck = false;
                            }
                        }
                    }
                }
                if (bMaterialCheck)
                    this.CheckMaterial();
                else if (bDataChanged)
                    this.ChangedData();
            }
            catch { }
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
                    this.lockMaterialCheck = null;
                    this.lockDataChanged = null;
                    this.eventTimer = null;
                    this.timerLock = null;
                    this.parent = null;
                    this.strID = null;
                    this.strPlatFormID = null;
                    this.strHoleID = null;
                    this.strTempHoleID = null;
                    this.strWrkOrderNo = null;
                    this.strTempWrkOrderNo = null;
                    this.strPreWorkOrderNo = null;
                    if (this.orderMaterialList != null)
                        this.orderMaterialList = null;
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
