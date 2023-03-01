using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
//using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TYG.HoleDetect
{
    public class DAQCollection
    {
        private static string dta = "proddta";
        private volatile static DAQCollection instance = null;
        private static object lockInstance = new object();
        private bool onDuty = false;
        string sqlconn = "Data Source=172.17.3.106;Database=PMI;Uid=pmi;Pwd=pmidb!;";

        private DAQ daq = new DAQ();

        public static DAQCollection GetInstance(bool OnDuty, bool AutoStart)
        {
            lock (lockInstance)
            {
                if (instance == null)
                {
                    instance = new DAQCollection(OnDuty, AutoStart);
                }
            }
            return instance;
        }

        #region Constructor

        private DAQCollection(bool OnDuty, bool AutoStart)
        {
            this.onDuty = OnDuty;
            InitialDaq();
            DaqEventHandler();
            if (AutoStart)
                this.daq.Start();
        }

        #endregion

        #region Data Acquistion Initialize

        private void InitialDaq()
        {
            SetFactories();
            SetAreas();
            SetPlatforms();
            SetMPiles();
            SetMHoles();
            SetMachines();
        }

        private void SetFactories()
        {
            string getFactories = string.Format("SELECT FactoryID, FactoryName FROM Factory");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getFactories);
            foreach (DataRow dr in dt.Rows)
            {
                daq.AddFactory(dr.Field<String>("FactoryID").Trim(), dr.Field<String>("FactoryName"));
            }
        }

        private void SetAreas()
        {
            Factory factory;
            string getAreas = string.Format("SELECT FactoryID, AreaID, AreaName, AreaIP, AreaPort, TestMode FROM Area");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getAreas);
            foreach (DataRow dr in dt.Rows)
            {
                bool isTestMode = (dr.Field<int>("TestMode") == 0 ? false : true);
                factory = daq.GetFactory(dr.Field<String>("FactoryID").Trim());
                factory.AddArea(dr.Field<String>("AreaID").Trim(), dr.Field<String>("AreaName").Trim(), dr.Field<String>("AreaIP").Trim(), dr.Field<int>("AreaPort"), isTestMode);
                factory.GetArea(dr.Field<String>("AreaID").Trim()).TestMode = (dr.Field<int>("TestMode") == 0 ? false : true);
            }
        }

        private void SetPlatforms()
        {
            Area area;
            string getPlatforms = string.Format("SELECT FactoryID, AreaID, PlatformID, PlatformName, PlatformIP, PlatformPort FROM Platform");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatforms);
            foreach (DataRow dr in dt.Rows)
            {
                area = daq.GetFactory(dr.Field<String>("FactoryID").Trim()).GetArea(dr.Field<String>("AreaID").Trim());
                area.AddPlatForm(dr.Field<String>("PlatformID").Trim(), dr.Field<String>("PlatformName").Trim(), dr.Field<String>("PlatformIP").Trim(), dr.Field<int>("PlatformPort"));
            }
        }

        private void SetMPiles()
        {
            Platform platform;
            string getMPiless = string.Format("SELECT b.FactoryID, b.AreaID, a.PlatformID, a.MPileSort, MPileSerial1, MPileSerial2, MPileSerial3, MRatio " +
                                                "FROM MPile AS a " +
                                                "LEFT JOIN Platform AS b " +
                                                "ON a.PlatformID = b.PlatformID " +
                                                "ORDER BY a.PlatformID, MPileSort;");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMPiless);
            foreach (DataRow dr in dt.Rows)
            {
                platform = daq.GetFactory(dr.Field<String>("FactoryID")).GetArea(dr.Field<String>("AreaID")).GetPlatform(dr.Field<String>("PlatformID"));
                platform.AddMPile(dr.Field<int>("MPileSort"));
                platform.SetMPile(dr.Field<int>("MPileSort"), dr.Field<String>("MPileSerial1"), dr.Field<String>("MPileSerial2"), dr.Field<String>("MPileSerial3"), dr.Field<String>("MRatio"));
            }
        }

        private void SetMHoles()
        {
            Platform platform;
            string getMHoles = string.Format("SELECT b.FactoryID, b.AreaID, a.mh_PlatformID PlatformID, a.mh_HoleID HoleID, a.mh_MHoleSerial1 MHoleSerial1, a.mh_MHoleSerial2 MHoleSerial2, a.mh_MHoleSerial3 MHoleSerial3, a.mh_MRatio MRatio,a.mh_IP1 IP,a.mh_Port1 Port, ISNULL(c.MachineID,'') MachineID " +
                                                "FROM MHole AS a " +
                                                "LEFT JOIN Platform AS b " +
                                                "ON a.mh_PlatformID = b.PlatformID " +
                                                "LEFT JOIN Machine AS c " +
                                                "ON a.mh_HoleID = c.MHoleID " +
                                                "ORDER BY a.mh_PlatformID, mh_HoleID;");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMHoles);
            foreach (DataRow dr in dt.Rows)
            {
                platform = daq.GetFactory(dr.Field<string>("FactoryID").Trim()).GetArea(dr.Field<string>("AreaID").Trim()).GetPlatform(dr.Field<string>("PlatformID").Trim());
                platform.AddMHole(dr.Field<string>("HoleID"), dr.Field<string>("IP"), dr.Field<int>("Port"), dr.Field<string>("MachineID"));
                platform.SetMHole(dr.Field<string>("HoleID"), dr.Field<string>("MHoleSerial1"), dr.Field<string>("MHoleSerial2"), dr.Field<string>("MHoleSerial3"), dr.Field<string>("MRatio"));
            }
        }

        private void SetMachines()
        {
            Area area;
            string getMachines = string.Format("SELECT FactoryID, AreaID, PlatformID, MachineID, SlaveNo, Channel, ISNULL(MHoleID,'') MHoleID FROM Machine");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMachines);
            foreach (DataRow dr in dt.Rows)
            {
                area = daq.GetFactory(dr.Field<String>("FactoryID").Trim()).GetArea(dr.Field<String>("AreaID").Trim());
                area.AddMachine(dr.Field<String>("MachineID").Trim(), dr.Field<int>("SlaveNo"), dr.Field<String>("PlatformID").Trim(), dr.Field<int>("Channel"), area.TestMode, dr.Field<string>("MHoleID"));
                area.GetMachine(dr.Field<String>("MachineID").Trim()).OnDuty = this.onDuty;
            }
        }

        #endregion

        #region Data Acquistion Event Handler

        private void DaqEventHandler()
        {
            this.daq.Area_ConnectChanged += Daq_AreaConnectChanged;
            this.daq.Platform_ConnectChanged += Daq_PlatformConnectChanged;
            this.daq.Machine_DataChanged += Daq_Machine_DataChanged;
            this.daq.Machine_MaterialCheck += Daq_Machine_MaterialCheck;
            this.daq.Platform_HoleChanged += Daq_Platform_HoleChanged;
            this.daq.MHole_HoleChanged += Daq_MHole_HoleChanged;
            this.daq.AlarmHappened += Daq_AlarmHappened;
        }

        private void Daq_MHole_HoleChanged(object Sender, HoleChangedArgument e)
        {
            if (!this.onDuty)
                return;
            MHole m = (MHole)Sender;
            Machine machine = m.Parent.GetMachine(m.MachineID);
            if( machine != null)
            {
                MaterialCheckArgument ee = new MaterialCheckArgument(e.FactoryID, e.AreaID, e.PlatFormID, e.MachineID);
                this.MaterialCheck(machine, ref ee);
            }
            

        }

        private void Daq_Platform_HoleChanged(object Sender, HoleChangedArgument e)
        {
            if (!this.onDuty)
                return;

        }

        private void Daq_Machine_MaterialCheck(object Sender, MaterialCheckArgument e)
        {
            if (!this.onDuty)
                return;

            this.MaterialCheck(Sender, ref e);
            this.UpdateMachineData(Sender, e);
        }

        private void Daq_Machine_DataChanged(object Sender, DataChangedArgument e)
        {
            if (!this.onDuty)
                return;
            this.UpdateMachineData(Sender, e);      
        }

        private void Daq_AreaConnectChanged(object Sender, ModbusConnectChangedArgument e)
        {
            if (!this.onDuty)
                return;

            Area area = (Area)Sender;
            string ConnectedTime = e.ConnectedTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
            string DisconnectedTime = (e.DisconnectedTime == null ? "" : e.DisconnectedTime.Value.ToString("yyyy/MM/dd HH:mm:ss"));
            string updateArea = string.Format("UPDATE Area SET ConnectionStatus = '{0}', ConnectedTime = '{1}', DisconnectedTime = '{2}' WHERE AreaID = '{3}';",
                                                (e.IsConnected ? 1 : 0), ConnectedTime, DisconnectedTime, area.ID);
            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, updateArea);
                if (!e.IsConnected)
                {
                    string DisconnectedReasson = (e.DisconnectedTime == null ? "" : e.DisconnectReason);
                    string insertLog = string.Format("insert into DisconnectLog (DeviceID, DeviceName, ConnectedTime, DisconnectedTime, DisconnectedReasson) values ('{0}','{1}','{2}','{3}','{4}')", area.ID, area.Name, ConnectedTime, DisconnectedTime, DisconnectedReasson);
                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertLog);
                }
            }
            catch (Exception) { }
        }

        private void Daq_PlatformConnectChanged(object Sender, ModbusConnectChangedArgument e)
        {
            if (!this.onDuty)
                return;
            Platform platform = (Platform)Sender;
            string ConnectedTime = e.ConnectedTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
            string DisconnectedTime = (e.DisconnectedTime == null ? "" : e.DisconnectedTime.Value.ToString("yyyy/MM/dd HH:mm:ss"));
            string updatePlatform = string.Format("UPDATE Platform SET ConnectionStatus = '{0}', ConnectedTime = '{1}', DisconnectedTime = '{2}' WHERE PlatformID = '{3}';",
                                                (e.IsConnected ? 1 : 0), ConnectedTime, DisconnectedTime, platform.ID);
            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, updatePlatform);
                if (!e.IsConnected)
                {
                    string DisconnectedReasson = (e.DisconnectedTime == null ? "" : e.DisconnectReason);
                    string insertLog = string.Format("insert into DisconnectLog (DeviceID, DeviceName, ConnectedTime, DisconnectedTime, DisconnectedReasson) values ('{0}','{1}','{2}','{3}','{4}')", platform.ID, platform.Name, ConnectedTime, DisconnectedTime, DisconnectedReasson);
                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertLog);
                }
            }
            catch (Exception) { }

        }

        private void Daq_AlarmHappened(object Sender, ModbusAlarmHappenedArgument e)
        {
            if (!this.onDuty)
                return;

        }

        private void MaterialCheck(object Sender, ref MaterialCheckArgument e)
        {
            bool orderFlag = false;
            bool IgnoreMaterialFlag = false;
            List<string> orderMaterialList = null;
            Machine machine = (Machine)Sender;
            string orderID = machine.WorkOrderNo.Replace("\r\n", "");
            string preorderID = machine.PreWorkOrderNo.Replace("\r\n", "");
            string productID = "";
            string orderType = "";
            string orderStatus = "";
            string orderMRatio = "";
            string specMaterial = "";
            int orderQty = 0;
            bool yellowLamp = true;
            bool redLamp = true;

            MPile mpile = machine.GetMPile();
            MHole mhole = machine.GetMHole();
            
            if ((machine.HoleID.Length > 0) && (machine.HoleID != "XX") && (machine.HoleID != "  "))
            {
                redLamp = false;
            }

            //if(preorderID != orderID && preorderID.Trim() !="")
            //{
            //    bool clearPassFlag = ByPassClear(machine.ID, preorderID);
            //}

            orderFlag = this.GetOrderData(Sender, orderID, ref productID, ref orderQty, ref orderType, ref orderStatus);

            if (orderFlag)
            {
                orderMaterialList = this.GetMaterialList(orderID, ref specMaterial, ref orderMRatio);
                yellowLamp = false;
            }
            
            //20211105 判定機台有沒有設定忽略斷料
            IgnoreMaterialFlag = this.CheckIgnoreMaterial(Sender);

            if (orderFlag && (!redLamp) && (machine.HoleID != "**"))
            {
                //if (mpile != null)
                //{
                //    foreach (string material in mpile.MaterialList)
                //    {
                //        if (!orderMaterialList.Contains(material))
                //        {
                //            redLamp = true;
                //            break;
                //        }
                //    }
                //    //if (mpile.MaterialRatio != orderMRatio)
                //    //    redLamp = true;
                //}
                if(mhole != null)
                {
                    foreach(string material in mhole.MaterialList)
                    {
                        if(!orderMaterialList.Contains(material))
                        {
                            redLamp = true;
                            break;
                        }
                    }
                }
            }
            
            if(machine.HoleID == "00" && !redLamp)//20211124 HoleID=00情況有：斷開電阻、REMOTE IO不存在或損壞。
            {
                machine.HoleID = "X";
            }

            if(orderType == "WT")//20210816 新增當工單為試作工單WT時，只呈現綠燈。
            {
                redLamp = false;
                yellowLamp = false;
            }

            if ((!redLamp) && (!yellowLamp))
            {
                e.ForbidMotor = false;
                e.LampSignal = 2;
            }
            else
            {
                if(IgnoreMaterialFlag)
                    e.ForbidMotor = false;
                else
                    e.ForbidMotor = true;

                if ((redLamp) && (yellowLamp))
                    e.LampSignal = 4;
                else if (yellowLamp)
                    e.LampSignal = 3;
                else
                    e.LampSignal = 1;
            }
            machine.ProductID = productID;
            machine.OrderType = orderType;
            machine.OrderQty = orderQty;
            machine.OrderStatus = orderStatus;
            machine.OrderMaterialList = orderMaterialList;
            machine.SpecMaterial = specMaterial;
            machine.OrderMRatio = orderMRatio;
        }

        private bool GetOrderData(object Sender, string orderID, ref string productID, ref int orderQty, ref string orderType, ref string orderStatus)
        {
            Machine machine = (Machine)Sender;
            bool result = false;
            DataTable dt = null;
            OracleParameter orderIDPara = new OracleParameter("ORDERID", OracleType.VarChar);
            orderIDPara.Value = orderID;
            OracleParameter[] paras = new OracleParameter[] { orderIDPara };


            string selectCmd = "Select F4801.WADCTO,wasrst,WADOCO,F4801.WALITM AS ProductID, F4801.WAUORG / 100 AS Quantity " +
                                "FROM "+ dta +".F4801 WHERE  F4801.WADOCO = :ORDERID";
            try
            {
                dt = OracleHelper.ExecuteNonQuery(selectCmd, paras);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    orderStatus = row.Field<String>("wasrst").Trim(); //工單狀態
                    orderType = row.Field<String>("WADCTO").Trim(); //工單類型
                    productID = row.Field<String>("ProductID").Trim();
                    orderQty = Convert.ToInt32(row.Field<decimal>("Quantity"));

                    if ((orderType == "WO" || orderType == "WT") && (this.CheckOrderStatus(orderStatus)))
                        result = true;
                }
            }
            catch (Exception) { }
            return result;
        }

        private bool CheckOrderStatus(string orderStatus)
        {
            bool result = true;
            if (orderStatus.Length != 2)
                result = false;
            else
            {
                bool orderConvert = Int32.TryParse(orderStatus, out int iorderStatus);

                if(!orderConvert)
                {
                    result = false;
                }
                else
                {
                    if (iorderStatus >= 30 && iorderStatus <= 94)
                        result = true;
                    else
                        result = false;
                }
                //char[] chArray = orderStatus.ToCharArray();
                //if (chArray[0] < '3' && chArray[0] > '9')
                //    result = false;
                //else if (chArray[0] == '9' && chArray[1] > '4')
                //    result = false;
            }
            return result;
        }

        /// <summary>
        /// 20211105檢查忽略斷料功能
        /// </summary>
        /// <param name="Sender"></param>
        /// <returns></returns>
        public bool CheckIgnoreMaterial(object Sender)
        {
            Machine machine = (Machine)Sender;
            string MachineID = machine.ID;
            string getMachines = string.Empty;
            bool result = false;
            DataTable dt = new DataTable();
            try
            {
                getMachines = string.Format("SELECT *  FROM [PMI].[dbo].[Machine] where MachineID = '{0}' and IgnoreMaterial = '{1}'", MachineID, "1");
                dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMachines);
            }
            catch (Exception) { }

            if (dt.Rows.Count != 0)
                result = true;

            return result;
        }

        private List<string> GetSpecMaterialList(string orderID)
        {
            List<string> materialList = new List<string>();

            DataTable dt = null;
            OracleParameter paraORDERID = new OracleParameter("ORDERID", OracleType.VarChar);
            OracleParameter paraIBG1PT = new OracleParameter("IBG1PT", OracleType.VarChar);
            OracleParameter paraWMITC = new OracleParameter("WMITC", OracleType.VarChar);
            paraORDERID.Value = orderID;
            paraIBG1PT.Value = "IN06";
            paraWMITC.Value = "B";

            OracleParameter[] paras = new OracleParameter[] { paraORDERID, paraIBG1PT, paraWMITC };

            string selectCmd = "Select T3111.wmcpil AS SpecMaterial " +
                                "From " + dta + ".f3111 T3111 " +
                                "LEFT JOIN " + dta + ".f4102 T4102 ON T3111.wmcpil=T4102.IBLITM And T3111.wmcmcu=T4102.IBMCU " +
                                "LEFT JOIN " + dta + ".f4101 T4101 ON T3111.wmcpil=T4101.IMLITM " +
                                "Where wmdoco = :ORDERID And T4102.ibglpt = :IBG1PT And  T3111.wmITC = :WMITC";

            try
            {
                dt = OracleHelper.ExecuteNonQuery(selectCmd, paras);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string specMaterial = row.Field<String>("SpecMaterial").Trim(); //同級料號
                        if (!materialList.Contains(specMaterial))
                            materialList.Add(specMaterial);
                    }
                }
            }
            catch (Exception) { }

            return materialList;
        }

        private List<string> GetMaterialList(string orderID, ref string specMaterial, ref string orderMRatio)
        {
            List<string> materialList = new List<string>();

            materialList = this.GetSpecMaterialList(orderID);
            if (materialList.Count > 0)
                specMaterial = materialList[0];
            DataTable dtMain = null;
            OracleParameter paraORDERID = new OracleParameter("ORDERID", OracleType.VarChar);
            OracleParameter paraIBG1PT = new OracleParameter("IBG1PT", OracleType.VarChar);
            OracleParameter paraWMITC = new OracleParameter("WMITC", OracleType.VarChar);
            OracleParameter paraIBSTKT = new OracleParameter("IBSTKT", OracleType.VarChar);
            OracleParameter paraIMSRTX = new OracleParameter("IMSRTX", OracleType.VarChar);
            paraORDERID.Value = orderID;
            paraIBG1PT.Value = "IN06";
            paraWMITC.Value = "F";
            paraIBSTKT.Value = "U";
            paraIMSRTX.Value = "01";

            OracleParameter[] parasMain = new OracleParameter[] { paraORDERID, paraIBG1PT, paraWMITC, paraIBSTKT };
            /*string selectCmdMain = "Select T3111.WMCPIT,T3111.wmcpil AS MainMaterial From "+ dta +".f3111 T3111 " +
                                   "LEFT JOIN "+ dta +".f4102 T4102 ON T3111.wmcpil = T4102.IBLITM And T3111.wmcmcu = T4102.IBMCU " +
                                   "LEFT JOIN "+ dta +".f4101 T4101 ON T3111.wmcpil = T4101.IMLITM " +
                                   "Where wmdoco = :ORDERID And T3111.WMCMCU = :WMCMCU  And T4102.ibglpt = :IBG1PT And T3111.wmITC = :WMITC";*/

            string selectCmdMain = "Select T3111.WMCPIT, T3111.WMCPIL From " + dta + ".f3111 T3111 " +
                                   "LEFT JOIN " + dta + ".f4102 T4102 ON T3111.wmcpil = T4102.IBLITM And T3111.wmcmcu = T4102.IBMCU " +
                                   "LEFT JOIN " + dta + ".f4101 T4101 ON T3111.wmcpil = T4101.IMLITM " +
                                   "Where wmdoco = :ORDERID And T4102.ibglpt = :IBG1PT And T3111.wmITC = :WMITC And T4102.IBSTKT = :IBSTKT And T4101.IMSRTX = '01'";

            try
            {
                dtMain = OracleHelper.ExecuteNonQuery(selectCmdMain, parasMain);
                if (dtMain.Rows.Count > 0)
                {
                    foreach (DataRow rowMain in dtMain.Rows)
                    { 
                        OracleParameter paraIXTBM = new OracleParameter("IXTBM", OracleType.Char);
                        OracleParameter paraIXKIT = new OracleParameter("IXKIT", OracleType.Int32);
                        OracleParameter paraIXMMCU = new OracleParameter("IXMMCU", OracleType.VarChar);
                        int specCode = Convert.ToInt32(rowMain.Field<Decimal>("WMCPIT")); //規格瑪
                        paraIXTBM.Value = 'M';
                        paraIXKIT.Value = specCode;
                        paraIXMMCU.Value = "      01-111";
                        //string mainMaterial = rowMain.Field<String>("MainMaterial").Trim(); //專用料號

                        //if (!materialList.Contains(mainMaterial))
                        //    materialList.Add(mainMaterial);

                        DataTable dtSame = null;
                        OracleParameter[] parasSame = new OracleParameter[] { paraIXTBM, paraIXKIT, paraIXMMCU };
                        string selectCmdSame = "Select IXLITM AS SameMaterial From " + dta + ".F3002 " +
                                               "Where F3002.IXTBM = :IXTBM and " +
                                               "F3002.IXKIT = :IXKIT and " +
                                               "F3002.IXMMCU = :IXMMCU and " +
                                               "F3002.IXEFFF <= OBT_IF.TYG_D2J(SYSDATE) and F3002.IXEFFT >= OBT_IF.TYG_D2J(SYSDATE)";

                        try
                        {
                            dtSame = OracleHelper.ExecuteNonQuery(selectCmdSame, parasSame);
                            if (dtSame.Rows.Count > 0)
                            {
                                foreach (DataRow rowSame in dtSame.Rows)
                                {
                                    string sameMaterial = rowSame.Field<String>("SameMaterial").Trim(); //同級料號
                                    if (!materialList.Contains(sameMaterial))
                                        materialList.Add(sameMaterial);
                                }
                            }
                        }
                        catch (Exception) { }
                    }
                }
                if (dtMain.Rows.Count > 1)
                {
                    orderMRatio = GetOrderMRatio(dtMain.Rows[0].Field<String>("WMCPIL").Trim(), dtMain.Rows[1].Field<String>("WMCPIL").Trim());
                    specMaterial = materialList[0] + "," + materialList[1] + ",(" + orderMRatio + ")";
                }
            }
            catch (Exception) { }

            return materialList;
        }

        private string GetOrderMRatio(string paramMixA, string paramMixB)
        {
            string orderMRatio = "";
            try
            {
                string getRatio = string.Format("select Ratio from Mix where (MixA = '{0}' and MixB = '{1}') or (MixA = '{1}' and MixB = '{0}')", paramMixA, paramMixB);
                DataTable dtRatio = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getRatio);
                if (dtRatio.Rows.Count > 0)
                    orderMRatio = dtRatio.Rows[0]["Ratio"].ToString();
            }
            catch (Exception)
            { }

            return orderMRatio;
        }

        /// <summary>
        /// 20211124 將BYPASS換工單清除改到這邊執行
        /// </summary>
        /// <param name="pMachineID"></param>
        /// <param name="preOrderID"></param>
        /// <returns></returns>
        public bool ByPassClear(string pMachineID, string preOrderID)
        {
            bool result = false;
            string serrMsg = string.Empty;
            string FactoryID = string.Empty;
            string AreaID = string.Empty;
            string PlatformID = string.Empty;
            bool AddByPassFlag = false;
            try
            {
                string getMachines = string.Format("Select FactoryID, AreaID, PlatformID from Machine where  MachineID = '{0}'", pMachineID);
                DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMachines);
                if (dt.Rows.Count > 0)
                {
                    FactoryID = dt.Rows[0]["FactoryID"].ToString();
                    AreaID = dt.Rows[0]["AreaID"].ToString();
                    PlatformID = dt.Rows[0]["PlatformID"].ToString();
                }

                string getPassRecord = string.Format("Select TOP (1) * from ByPassMachineRecord where  MachineID = '{0}' and OrderID = '{1}' and ByPassState = 1  Order by PassRecordNo desc", pMachineID, preOrderID);
                dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPassRecord);
                if (dt.Rows.Count == 1)
                    AddByPassFlag = true;

                if (AddByPassFlag)
                {
                    var Description = "換工單自動關閉";
                    var RecordDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string insertPassRecord = string.Format("insert ByPassMachineRecord (RecordDate, FactoryID, AreaID, PlatformID, MachineID, ByPassState, USERID, Description, OrderID) values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')",RecordDate, FactoryID, AreaID, PlatformID, pMachineID, "0", "System", Description, preOrderID);
                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertPassRecord);
                    result = true;
                }
            }
            catch (Exception)
            { }

            return result;
        }

        private void UpdateMachineData(object Sender, DataChangedArgument e)
        {
            if (!this.onDuty)
                return;

            //Machine machine = (Machine)Sender;
            //using (SqlConnection MyConnection = new SqlConnection(sqlconn))
            //{
            //    var CommandString = @"Select TOP (1) * from ByPassMachineRecord where  MachineID = @MachineID  Order by PassRecordNo desc";
            //    DataTable ds = new DataTable();
            //    using (SqlCommand command = new SqlCommand(CommandString, MyConnection))
            //    {
            //        command.Parameters.AddWithValue("@MachineID", machine.ID);
            //        MyConnection.Open();
            //        using (SqlDataAdapter myAdapter = new SqlDataAdapter(command))
            //        {
            //            myAdapter.Fill(ds);
            //            if (ds.Rows.Count > 0)
            //            {
            //                foreach (DataRow row in ds.Rows)
            //                {
            //                    if (row["ByPassState"].ToString().Trim() == "1" && machine.HoleID != "**")
            //                    {
            //                        machine.ByPassState = machine.HoleID;
            //                        machine.HoleID = "**";
            //                    }
            //                    else if (row["ByPassState"].ToString().Trim() == "0" && machine.HoleID == "**")
            //                    {
            //                        machine.HoleID = machine.ByPassState;
            //                        machine.ByPassState ="";
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            this.saveMachineRecord(Sender);
            this.saveOrderData(Sender);

        }

        public bool checkMachineRecord(object Sender)
        {
            Machine machine = (Machine)Sender;
            string MachineID = machine.ID;
            int MachineState = machine.RunStatus;
            string PrevOrder = machine.PreWorkOrderNo;
            string NowOrder = machine.WorkOrderNo;
            string getRecord = null;
            DataTable dtRecord = null;
            int recordCount = 0;
            try
            {
                getRecord = string.Format("select count(*) as count from MachineRecord where MachineID = '{0}' and MachineState = '{1}' and PrevOrder = '{2}' and NowOrder = '{3}'", MachineID, MachineState, PrevOrder, NowOrder);
                dtRecord = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getRecord);
                recordCount = Convert.ToInt32(dtRecord.Rows[0]["count"].ToString());
            }
            catch { }

            return (recordCount > 0);
        }

        public void saveMachineRecord(object Sender)
        {
            Machine machine = (Machine)Sender;
            MPile mpile = machine.GetMPile();
            MHole mhole = machine.GetMHole();

            string RecordDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            int RecordType = ((machine.PlcConnected) ? 1 : 0);
            string FactoryID = machine.FactoryID;
            string AreaID = machine.AreaID;
            string PlatformID = machine.PlatFormID;
            string MachineID = machine.ID;
            int MachineState = machine.RunStatus;
            int LampState = machine.LampStatus;
            string HoleID = machine.HoleID;
            string MPlileList = "";
            string MHoleList = "";

            //if (mpile != null)
            //{
            //    MPlileList += ((mpile.MaterialList.Count > 0) ? mpile.MaterialList[0] : "");
            //    MPlileList += ((mpile.MaterialList.Count > 1) ? "<br>" + mpile.MaterialList[1] : "");
            //    MPlileList += ((mpile.MaterialList.Count > 2) ? "<br>" + mpile.MaterialList[2] : "");
            //    MPlileList += ((mpile.MaterialRatio != "") ? "<br>" + mpile.MaterialRatio : "");
            //}
            if (mhole != null)
            {
                MHoleList += ((mhole.MaterialList.Count > 0) ? mhole.MaterialList[0] : "");
                MHoleList += ((mhole.MaterialList.Count > 1) ? "<br>" + mhole.MaterialList[1] : "");
                MHoleList += ((mhole.MaterialList.Count > 2) ? "<br>" + mhole.MaterialList[2] : "");
                MHoleList += ((mhole.MaterialRatio != "") ? "<br>" + mhole.MaterialRatio : "");
            }

            float Weight = machine.Weight;
            string WeightUnit = machine.WeightUnit;
            string PrevOrder = machine.PreWorkOrderNo;
            int PrevQuantity = machine.PreProdQty;
            string NowOrder = machine.WorkOrderNo;
            int NowQuantity = machine.ProdQty;
            string NowOrderType = machine.OrderType;
            int ElectricPower = machine.ElectricPower;
            int PowerFlag = machine.PowerFlag;
            int InstantVoltage = machine.InstantVoltage;
            int InstantCurrent = machine.InstantCurrent;
            int TemperatureNumber = machine.TemperatureNumber;
            string[] TemperatureValue = machine.TemperatureValue;
            string PlcTime = machine.PlcTime.ToString("yyyy-MM-dd HH:mm:ss");

            string insertRecord = string.Format("insert MachineRecord (RecordDate, RecordType, FactoryID, AreaID, PlatformID, MachineID, MachineState, LampState, HoleID, MHoleList, Weight, WeightUnit, PrevOrder, PrevQuantity, NowOrder, NowQuantity, NowOrderType, ElectricPower, InstantVoltage, InstantCurrent, PowerFlag, TempNumber, TempS1, TempS2, TempS3, TempS4, TempS5, TempElectricity, TempAmbient, ElectricPowerTime) values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}')",
            RecordDate, RecordType, FactoryID, AreaID, PlatformID, MachineID, MachineState, LampState, HoleID, MHoleList, Weight, WeightUnit, PrevOrder, PrevQuantity, NowOrder, NowQuantity, NowOrderType, ElectricPower, InstantVoltage, InstantCurrent, PowerFlag, TemperatureNumber, TemperatureValue[0], TemperatureValue[1], TemperatureValue[2], TemperatureValue[3], TemperatureValue[4], TemperatureValue[5], TemperatureValue[6], PlcTime);

            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertRecord);
            }
            catch (Exception ex) { }
        }

        public bool checkOrderData(object Sender, out int nowQuantity, out int plcQuantity)
        {
            Machine machine = (Machine)Sender;
            string MachineID = machine.ID;
            string OrderID = machine.WorkOrderNo;
            string getOrders = null;
            DataTable dtOrders = null;
            int ordersCount = 0;
            nowQuantity = 0;
            plcQuantity = 0;
            try
            {
                getOrders = string.Format("select * from Orders where MachineID = '{0}' and OrderID = '{1}'", MachineID, OrderID);
                dtOrders = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getOrders);
                ordersCount = dtOrders.Rows.Count;

                if(ordersCount > 0)
                {
                    nowQuantity = Convert.ToInt32(dtOrders.Rows[0]["NowQuantity"].ToString());
                    plcQuantity = Convert.ToInt32(dtOrders.Rows[0]["PLCQuantity"].ToString());
                }

            }
            catch (Exception) { }

            return (ordersCount > 0);
        }

        public bool checkMachineOrderByDateData(object Sender, out int iniQuantity, out int endQuantity, out string DateShift, out DateTime recordDate)
        {
            bool result = false;
            Machine machine = (Machine)Sender;
            string MachineID = machine.ID;
            string OrderID = machine.WorkOrderNo;
            int NowQuantity = machine.ProdQty;
            string getOrders = null;
            DataTable dtOrders = null;
            int ordersCount = 0;
            iniQuantity = 0;
            endQuantity = 0;
            DateShift = string.Empty;
            recordDate = DateTime.Now;

            try
            {
                //先判定目前時間班次
                DateShift = getDateShift(recordDate, out bool lastNight);

                if(lastNight)
                    recordDate = recordDate.AddDays(-1);

                getOrders = string.Format("SELECT * FROM [PMI].[dbo].[OrdersByDate] Where MachineID = '{0}' and [Date] = '{1}' and DateShift = '{2}' and OrderID = '{3}' and ActiveFlag = 'Y' ", MachineID, recordDate.Date.ToString("yyyy-MM-dd"), DateShift, OrderID);
                dtOrders = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getOrders);
                ordersCount = dtOrders.Rows.Count;

                if (ordersCount > 0)
                {         
                    iniQuantity = Convert.ToInt32(dtOrders.Rows[0]["IniQuantity"].ToString());
                    endQuantity = Convert.ToInt32(dtOrders.Rows[0]["EndQuantity"].ToString());

                    if(NowQuantity >= endQuantity)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception) { }

            return result;
        }

        public void saveOrderData(object Sender)
        {
            Machine machine = (Machine)Sender;
            string ERPDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string FactoryID = machine.FactoryID;
            string AreaID = machine.AreaID;
            string PlatformID = machine.PlatFormID;
            string MachineID = machine.ID;
            int MachineState = machine.RunStatus;
            string OrderID = machine.WorkOrderNo;
            string OrderType = machine.OrderType;
            string ProductID = machine.ProductID;
            string specMaterial = "";
            if (machine.OrderMaterialList != null)
            {
                if (machine.OrderMaterialList.Count > 0)
                    specMaterial = machine.OrderMaterialList[0];
            }
            int OrdQuantity = machine.OrderQty;
            int NowQuantity = machine.ProdQty;

            try
            {
                if (OrderID != "")
                {
                    if (!this.checkOrderData(Sender, out int nowQuantity, out int plcQuantity))
                    {
                        //該機台的工單號不存在
                        var connStr = string.Format("select * from Machine where MachineID = '{0}' and AutoReportOrders = '{1}'", MachineID, 1);
                        DataTable dtMachine = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, connStr);

                        if(dtMachine.Rows.Count > 0 /*&& getTimeSpan(DateTime.Now.ToString())*/)
                        {       
                            //機台支援自動報工
                            connStr = string.Format("select * from Orders where OrderID = '{0}'", OrderID);
                            DataTable dtOrders = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, connStr);
                        
                            if (dtOrders.Rows.Count  > 0)
                            {
                                //有找到另一機台有該工單
                                var reportValue = Convert.ToInt32(dtOrders.Rows[0]["isReport"].ToString());
                                if(reportValue == 0)
                                {
                                    //還沒報完工，新增工單判定可報工。
                                    var isReport = 0;
                                    string insertOrders = string.Format("insert into Orders (ERPDate, ModDate, FactoryID, AreaID, PlatformID, MachineID, MachineState, OrderID, OrderType, ProductID, SpecMaterial, OrdQuantity, NowQuantity, IsReport, PLCQuantity) values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');",
                                            ERPDate, ERPDate, FactoryID, AreaID, PlatformID, MachineID, MachineState, OrderID, OrderType, ProductID, specMaterial, OrdQuantity, NowQuantity, isReport, NowQuantity);
                                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertOrders);
                                }
                                else if (reportValue == 1)
                                {
                                    //已報完工，新增工單判定需報完工。
                                    var isReport = 1;
                                    string insertOrders = string.Format("insert into Orders (ERPDate, ModDate, FactoryID, AreaID, PlatformID, MachineID, MachineState, OrderID, OrderType, ProductID, SpecMaterial, OrdQuantity, NowQuantity, IsReport, PLCQuantity) values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');",
                                            ERPDate, ERPDate, FactoryID, AreaID, PlatformID, MachineID, MachineState, OrderID, OrderType, ProductID, specMaterial, OrdQuantity, NowQuantity, isReport, NowQuantity);
                                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertOrders);
                                }
                                else if (reportValue == 2)
                                {
                                    //不支援報工，新增工單判定可報工
                                    var isReport = 0;
                                    string insertOrders = string.Format("insert into Orders (ERPDate, ModDate, FactoryID, AreaID, PlatformID, MachineID, MachineState, OrderID, OrderType, ProductID, SpecMaterial, OrdQuantity, NowQuantity, IsReport, PLCQuantity) values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');",
                                            ERPDate, ERPDate, FactoryID, AreaID, PlatformID, MachineID, MachineState, OrderID, OrderType, ProductID, specMaterial, OrdQuantity, NowQuantity, isReport, NowQuantity);
                                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertOrders);
                                }
                            }
                            else
                            {
                                //其他機台沒有做過這張工單，判定需要報工
                                var isReport = 0;
                                string insertOrders = string.Format("insert into Orders (ERPDate, ModDate, FactoryID, AreaID, PlatformID, MachineID, MachineState, OrderID, OrderType, ProductID, SpecMaterial, OrdQuantity, NowQuantity, IsReport, PLCQuantity) values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');",
                                        ERPDate, ERPDate, FactoryID, AreaID, PlatformID, MachineID, MachineState, OrderID, OrderType, ProductID, specMaterial, OrdQuantity, NowQuantity, isReport, NowQuantity);
                                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertOrders);
                            }
                        }
                        else
                        {
                            //機台不支援自動報工
                            var isReport = 2;
                            string insertOrders = string.Format("insert into Orders (ERPDate, ModDate, FactoryID, AreaID, PlatformID, MachineID, MachineState, OrderID, OrderType, ProductID, SpecMaterial, OrdQuantity, NowQuantity, IsReport, PLCQuantity) values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');",
                                    ERPDate, ERPDate, FactoryID, AreaID, PlatformID, MachineID, MachineState, OrderID, OrderType, ProductID, specMaterial, OrdQuantity, NowQuantity, isReport, NowQuantity);
                            SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertOrders);
                        }

                    }
                    else
                    {
                        //該機台的工單號已存在
                        if (nowQuantity > NowQuantity)
                        {
                            if(NowQuantity != 0 && NowQuantity != plcQuantity)
                            {
                                string updateOrders = string.Format("update Orders set ModDate = '{0}', MachineState = '{1}', NowQuantity = '{2}', PLCQuantity = '{3}' where MachineID = '{4}' and OrderID = '{5}';", ERPDate, MachineState, nowQuantity + 1, NowQuantity, MachineID, OrderID);
                                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, updateOrders);
                            }
                        }
                        else
                        {
                            string updateOrders = string.Format("update Orders set ModDate = '{0}', MachineState = '{1}', NowQuantity = '{2}', PLCQuantity = '{3}' where MachineID = '{4}' and OrderID = '{5}';", ERPDate, MachineState, NowQuantity, NowQuantity, MachineID, OrderID );
                            SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, updateOrders);
                        }

                        //20220907同步資料至ERP   
                        var connStr = string.Format("Select Sum(NowQuantity) AS NowQuantity from[PMI].[dbo].[Orders] where OrderID = '{0}'", OrderID);
                        DataTable dt_OrderSum = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, connStr);
                        if(dt_OrderSum.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt_OrderSum.Rows)//只會有一行
                            {
                                if(Int32.TryParse(row["NowQuantity"].ToString().Trim(), out int iOrderSum))
                                {
                                    OracleParameter paraORDERID = new OracleParameter("ORDERID", OracleType.VarChar);
                                    paraORDERID.Value = OrderID;
                                    OracleParameter[] paras = new OracleParameter[] { paraORDERID};
                                    string selectCmd = "Select WE1DT3,WE1S108,WE1NS132,WEUSER,WEPID,WEJOBN ,WEUPMJ,WETDAY,WEDOCO From " + dta + ".F5948017 where WEDOCO=:ORDERID ";

                                    try
                                    {
                                        DataTable dt = OracleHelper.ExecuteNonQuery(selectCmd, paras);
                                        if (dt.Rows.Count > 0)
                                        {
                                            foreach (DataRow rowA in dt.Rows)
                                            {
                                                DateTime now = DateTime.Now;

                                                OracleParameter paraWE1DT3 = new OracleParameter("WE1DT3", OracleType.Int32);
                                                paraWE1DT3.Value = GetJulianDate(now);
                                                OracleParameter paraWE1S108 = new OracleParameter("WE1S108", OracleType.Int32);
                                                paraWE1S108.Value = GetJulianTime(now);
                                                OracleParameter paraWE1NS132 = new OracleParameter("WE1NS132", OracleType.Int32);
                                                paraWE1NS132.Value = iOrderSum * 100;
                                                OracleParameter paraWEPID = new OracleParameter("WEPID", OracleType.VarChar);
                                                paraWEPID.Value = "PMI0001";
                                                OracleParameter paraWEUPMJ = new OracleParameter("WEUPMJ", OracleType.Int32);
                                                paraWEUPMJ.Value = GetJulianDate(now);
                                                OracleParameter paraWETDAY = new OracleParameter("WETDAY", OracleType.Int32);
                                                paraWETDAY.Value = GetJulianTime(now);
                                                paraORDERID = new OracleParameter("ORDERID", OracleType.VarChar);
                                                paraORDERID.Value = OrderID;

                                                if (rowA["WE1DT3"].ToString() == "0" && rowA["WE1S108"].ToString().Trim() == "")
                                                {
                                                    //第一次更新
                                                    paras = new OracleParameter[] { paraWE1DT3, paraWE1S108, paraWE1NS132, paraWEPID, paraWEUPMJ, paraWETDAY, paraORDERID };
                                                    selectCmd = "update " + dta + ".F5948017 set WE1DT3 =:WE1DT3, WE1S108 =:WE1S108, WE1NS132 =:WE1NS132, WEPID =:WEPID, WEUPMJ =:WEUPMJ, WETDAY =:WETDAY where WEDOCO =:ORDERID";
                                                    var affected = OracleHelper.UpdateNonQuery(selectCmd, paras);
                                                }
                                                else
                                                {
                                                    //後續更新
                                                    paras = new OracleParameter[] { paraWE1NS132, paraWEPID, paraWEUPMJ, paraWETDAY, paraORDERID };
                                                    selectCmd = "update " + dta + ".F5948017 set WE1NS132 =:WE1NS132, WEPID =:WEPID, WEUPMJ =:WEUPMJ, WETDAY =:WETDAY where WEDOCO =:ORDERID ";
                                                    var affected = OracleHelper.UpdateNonQuery(selectCmd, paras);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception) { }
                                }
                            }
                        }


                    }

                    //20220919簡化效率收集作業，將BY日期工單資料整理至MachineOrderByDate Table中
                    if (!this.checkMachineOrderByDateData(Sender, out int iniQuantity, out int endQuantity, out string DateShift, out DateTime recordDate))
                    {
                        //Insert，工單在該機台當班的目前資料找不到，Insert前先設定該機台所有ActiveFlag="Y"的工單為N
                        string updateSql = string.Format("update [OrdersByDate] set ActiveFlag = 'N' where MachineID = '{0}' and ActiveFlag = 'Y'", MachineID);
                        SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, updateSql);

                        string insertSql = string.Format("insert into [OrdersByDate] ([MachineID],[Date],[DateShift],[OrderID],[OrderType],[IniTime],[EndTime],[IniQuantity],[EndQuantity],[ActiveFlag]) values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}')",
                                                            MachineID, recordDate.Date.ToString("yyyy-MM-dd"), DateShift, OrderID, OrderType, ERPDate, ERPDate, NowQuantity, NowQuantity,"Y");
                        SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertSql);
                    }
                    else
                    {
                        //Update，工單為該機台目前正在生產的工單
                        if(NowQuantity != 0)// 應不存在要回頭更新0的狀況
                        {
                            string updateSql = string.Format("update [OrdersByDate] set EndTime = '{0}', EndQuantity = '{1}' Where MachineID = '{2}' and [Date] = '{3}' and DateShift = '{4}' and OrderID = '{5}' and ActiveFlag = 'Y' ", ERPDate, NowQuantity, MachineID, recordDate.Date.ToString("yyyy-MM-dd"), DateShift, OrderID);
                            SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, updateSql);
                        }
                    }

                    //20220524將目前該機台Order的資料同步至一模多穴資料表中，單張工單也需更新
                    if (NowQuantity > 0)
                    { 
                        var connStr = string.Format("SELECT * FROM MachineOrder where NowOrderP = '{0}' and MachineID = '{1}' and ActiveYN = 'Y';", OrderID, MachineID);
                        DataTable dtMachineOrder = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, connStr);

                        if (dtMachineOrder.Rows.Count > 0)
                        {
                            foreach (DataRow row in dtMachineOrder.Rows)
                            {
                                var diff = Convert.ToInt32(row["OrderQuantity"]) - Convert.ToInt32(row["ProdQuantity"]);
                                var NowOrderC = row["NowOrderC"].ToString();

                                if (NowQuantity > diff)
                                {
                                    //原超過數量不在更新，後續以數量超過欄位註記。
                                    var updateMachineOrder = string.Format("Update MachineOrder set OverQuantity = 'Y' Where NowOrderP = '{0}' and NowOrderC ='{1}' and MachineID = '{2}' and ActiveYN = 'Y' and OverQuantity != 'Y';", OrderID, NowOrderC, MachineID);
                                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, updateMachineOrder);
                                }
                                else
                                {
                                    //更新
                                    var updateMachineOrder = string.Format("Update MachineOrder set NowQuantity = '{0}' Where NowOrderP = '{1}' and NowOrderC ='{2}' and MachineID = '{3}' and ActiveYN = 'Y';", NowQuantity, OrderID, NowOrderC, MachineID);
                                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, updateMachineOrder);
                                }
                            }
                        }
                    }       
                }
            }
            catch (Exception) { }
        }
        #endregion

        #region Public Property

        /// <summary>
        /// Data Acquistion Object
        /// </summary>
        public DAQ DAQObject
        {
            get { return this.daq; }
        }

        #endregion

        /// <summary>
        /// 判定班別
        /// </summary>
        /// <param name="timeStr"></param>
        /// <param name="lastNight">前晚註記(24:00~08:30)</param>
        /// <returns></returns>
        protected string getDateShift(DateTime timeStr, out bool lastNight)
        {
            lastNight = false;
            string _strWorkingDayAM = "08:30";
            string _strWorkingDayPM = "20:30";
            TimeSpan dspWorkingDayAM = DateTime.Parse(_strWorkingDayAM).TimeOfDay;
            TimeSpan dspWorkingDayPM = DateTime.Parse(_strWorkingDayPM).TimeOfDay;
            TimeSpan dspNow = timeStr.TimeOfDay;

            if(dspNow < dspWorkingDayAM)
            {
                lastNight = true;
            }

            if(dspNow > dspWorkingDayAM && dspNow < dspWorkingDayPM)
            {
                return "morning";
            }
            else
            {
                return "night";
            }
        }

        /// <summary>取得 Julian Date</summary>
        /// <param name="date">西元日期</param>
        /// <returns>Julian Date</returns>
        private static int GetJulianDate(DateTime date)
        {
            string century = Convert.ToString((date.Year - 1900) / 100);
            string year = date.Year.ToString();
            year = year.Substring(2, 2);
            string day = string.Format("{0:D3}", date.DayOfYear);
            int julianDate = Convert.ToInt32(century + year + day);
            return julianDate;
        }

        /// <summary>取得 Julian Time</summary>
        /// <param name="time">西元時間</param>
        /// <returns>Julian Time</returns>
        private static int GetJulianTime(DateTime time)
        {
            string timeStr = time.Hour.ToString() + time.Minute.ToString("D2") + time.Second.ToString("D2");
            return Convert.ToInt32(timeStr);
        }

    }
}
