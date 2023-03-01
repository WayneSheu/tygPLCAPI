using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Data.OracleClient;
//using Oracle.ManagedDataAccess.Client;

namespace tygPLCAPI
{
    /// <summary>
    /// Hole Detection for raw material supply of forming machine
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class IUDService : System.Web.Services.WebService
    {
        public static string mainPicURL = "http://172.17.3.85/tyg/FactoryPic/";
        private static string ctl = "prodctl";
        private static string dta = "proddta";
        public static string OracleConnection = "Data Source=//172.17.3.131:1521/erp92.tyg.com.tw;Persist Security Info=True;User ID=POCAD;Password=TEAAB7121973;Unicode=True";
        public static string ComputerName = "tyg.com.tw";

        HoleDetectServiceReference.HoleDetectService hds = null;

        #region Factory

        private string FactoryJson(DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int FactoryNo = dr.Field<Int32>("FactoryNo");
                    string CreateDate = dr.Field<DateTime>("CreateDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string AlterDate = dr.Field<DateTime>("AlterDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string FactoryID = dr.Field<String>("FactoryID").Trim();
                    string FactoryName = dr.Field<String>("FactoryName");
                    string PicFileName = mainPicURL + dr.Field<String>("PictureFileName").Trim();
                    string Note = dr.IsNull("Note") ? "": dr.Field<String>("Note").Trim();
                    string Founder = dr.Field<String>("Founder").Trim();
                    string Updater = dr.Field<String>("Updater").Trim();
                    
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("FactoryNo", FactoryNo),
                            new JProperty("CreateDate", CreateDate),
                            new JProperty("AlterDate", AlterDate), 
                            new JProperty("FactoryID", FactoryID),
                            new JProperty("FactoryName", FactoryName),
                            new JProperty("PictureFileName", PicFileName),
                            new JProperty("Note", Note),
                            new JProperty("Founder", Founder),
                            new JProperty("Updater", Updater)
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            return _jsonObject.ToString();
        }

        [WebMethod]
        public string GetAllFactory()
        {
            string getFactory = string.Format("select * from Factory order by FactoryID");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getFactory);

            return FactoryJson(dt);
        }

        [WebMethod]
        public string GetFactoryByID(string factoryID)
        {
            string getFactory = string.Format("select * from Factory where FactoryID = '{0}'", factoryID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getFactory);
            return FactoryJson(dt);
        }

        [WebMethod]
        public string AddFactory(string factoryID, string factoryName, string bgImage, string note, string founder)
        {
            string rslt = "";

            //Save File
            string filepath = Server.MapPath(@"\FactoryPic\");            
            string filename = factoryID + ".png";

            if (!System.IO.Directory.Exists(filepath))
                System.IO.Directory.CreateDirectory(filepath);

            string getFactory = string.Format("select count(*) as count from Factory where FactoryID = '{0}'", factoryID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getFactory);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            if (rowcount > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "資料重複")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            else
            {
                Base64ToImage(filepath + filename, bgImage);
                CopyImage(filename);

                string bgFileName = factoryID + ".png";

                try
                {                
                    //Insert
                    string insertFactory = string.Format("insert into Factory (CreateDate, AlterDate, FactoryID, FactoryName, PictureFileName, Note, Founder, Updater) values ('{0}','{0}','{1}',N'{2}','{3}','{4}','{5}','{5}')",
                                                                                 System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), factoryID, factoryName, bgFileName, note, founder);
                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertFactory);

                    foreach (DataRow dr in dt.Rows)
                    {
                        //Json Data
                        JObject jsonObject =
                            new JObject(
                                new JProperty("Result", "新增成功")
                            );
                        jsonarr.Add(jsonObject);
                    }
                }
                catch
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "新增失敗")
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string ModFactory(string factoryID, string factoryName, string bgImage, string note, string updater)
        {
            string rslt = "";

            //Save File
            string filepath = Server.MapPath(@"\FactoryPic\");            
            string filename = factoryID + ".png";

            if (!System.IO.Directory.Exists(filepath))
                System.IO.Directory.CreateDirectory(filepath);

            string getFactory = string.Format("select count(*) as count from Factory where FactoryID = '{0}'", factoryID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getFactory);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();


            Base64ToImage(filepath + filename, bgImage);
            CopyImage(filename);

            string bgFileName = factoryID + ".png";

            try
            {
                //Insert
                string modFactory = "";
                if (bgImage.Equals(""))
                    modFactory = string.Format("update Factory set AlterDate = '{0}', FactoryName = N'{1}', Note ='{2}', Updater = '{3}' WHERE FactoryID = '{4}'",
                                                                            System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), factoryName, note, updater, factoryID);
                else
                    modFactory = string.Format("update Factory set AlterDate = '{0}', FactoryName = N'{1}', PictureFileName = '{2}', Note = '{3}', Updater ='{4}' WHERE FactoryID = '{5}'",
                                                                            System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), factoryName, bgFileName, note, updater, factoryID);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, modFactory);

                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "修改成功")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            catch
            {
                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "修改失敗")
                    );
                jsonarr.Add(jsonObject);
            }            

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string DelFactory(string factoryID)
        {
            string rslt = "";

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                string delFactory = string.Format("delete from Factory where FactoryID = '{0}'", factoryID);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, delFactory);

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除成功")
                    );
                jsonarr.Add(jsonObject);
            }
            catch
            {
                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除失敗")
                    );
                jsonarr.Add(jsonObject);

            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }
        #endregion

        #region Area

        private string AreaJson(DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();

            if (dt.Rows.Count > 0)
            {
              

                foreach (DataRow dr in dt.Rows)
                {
                    Byte AreaNo = dr.Field<Byte>("AreaNo");
                    string CreateDate = dr.Field<DateTime>("CreateDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string AlterDate = dr.Field<DateTime>("AlterDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string FactoryID = dr.Field<String>("FactoryID").Trim();
                    string AreaID = dr.Field<String>("AreaID").Trim();
                    string AreaName = dr.Field<String>("AreaName").Trim();
                    string AreaIP = dr.Field<String>("AreaIP").Trim();
                    int AreaPort = dr.Field<Int32>("AreaPort");
                    int TestMode = dr.Field<Int32>("TestMode");
                    string Note = dr.IsNull("Note") ? "": dr.Field<String>("Note").Trim();
                    string Founder = dr.Field<String>("Founder").Trim();
                    string Updater = dr.Field<String>("Updater").Trim();
                    int MNum = dr.Field<Int32>("MNum");

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("AreaNo", AreaNo),
                            new JProperty("CreateDate", CreateDate),
                            new JProperty("AlterDate", AlterDate),
                            new JProperty("FactoryID", FactoryID),
                            new JProperty("AreaID", AreaID),
                            new JProperty("AreaName", AreaName),
                            new JProperty("AreaIP", AreaIP),
                            new JProperty("AreaPort", AreaPort),
                            new JProperty("TestMode", TestMode),
                            new JProperty("Note", Note),
                            new JProperty("Founder", Founder),
                            new JProperty("Updater", Updater),
                            new JProperty("MNum", MNum)
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            return _jsonObject.ToString();
        }

        [WebMethod]
        public string GetAllArea()
        {
            string getAreas = string.Format("select *, (select count(*) from Machine where Machine.AreaID = Area.AreaID) as MNum from Area Order by FactoryID");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getAreas);

            return AreaJson(dt);
        }

        [WebMethod]
        public string GetAreaByID(string areaID)
        {
            string getArea = string.Format("select *, (select count(*) from Machine where Machine.AreaID = Area.AreaID) as MNum from Area where AreaID = '{0}'", areaID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getArea);

            return AreaJson(dt);
        }

        [WebMethod]
        public string GetAreaByFactoryID(string factoryID)
        {
            string getAreas = string.Format("select *, (select count(*) from Machine where Machine.AreaID = Area.AreaID) as MNum from Area where FactoryID = '{0}'", factoryID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getAreas);

            return AreaJson(dt);
        }

        [WebMethod]
        public string GetAreaOption(string factoryID)
        {
            string getAreas = "";
            if (factoryID == "")
                getAreas = string.Format("select *, (select count(*) from Machine where Machine.AreaID = Area.AreaID) as MNum from Area Order by FactoryID");
            else
                getAreas = string.Format("select *, (select count(*) from Machine where Machine.AreaID = Area.AreaID) as MNum from Area where FactoryID = '{0}'", factoryID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getAreas);

            return AreaJson(dt);
        }

        [WebMethod]
        public string AddArea(string factoryID, string areaID, string areaName, string areaIP, int areaPort, int testMode, string note, string founder)
        {
            string rslt = "";

            string getArea = string.Format("select count(*) as count from Area where AreaID = '{0}'", areaID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getArea);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            if (rowcount > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "資料重複")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            else
            {
                try
                {
                    //Insert
                    string insertArea = string.Format("insert into Area (CreateDate, AlterDate, FactoryID, AreaID, AreaName, AreaIP, AreaPort, TestMode, Note, Founder, Updater) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{9}')",
                                                                                System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), factoryID, areaID, areaName, areaIP, areaPort, testMode, note, founder);
                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertArea);

                    if (hds == null)
                        hds = new HoleDetectServiceReference.HoleDetectService();

                    hds.SetTestMode(factoryID, areaID, testMode);

                    foreach (DataRow dr in dt.Rows)
                    {
                        //Json Data
                        JObject jsonObject =
                            new JObject(
                                new JProperty("Result", "新增成功")
                            );
                        jsonarr.Add(jsonObject);
                    }
                }
                catch
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "新增失敗")
                        );
                    jsonarr.Add(jsonObject);
                }                
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string ModArea(string factoryID, string areaID, string areaName, string areaIP, int areaPort, int testMode, string note, string updater)
        {
            string rslt = "";

            string getArea = string.Format("select count(*) as count from Area where AreaID = '{0}'", areaID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getArea);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                //Insert
                string modArea = string.Format("update Area set AlterDate = '{0}', FactoryID = '{1}', AreaName = '{2}', AreaIP = '{3}', AreaPort = '{4}', TestMode = '{5}', Note = '{6}', Updater = '{7}' where AreaID = '{8}'",
                                                                            System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), factoryID, areaName, areaIP, areaPort, testMode, note, updater, areaID);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, modArea);

                if (hds == null)
                    hds = new HoleDetectServiceReference.HoleDetectService();

                //hds.SetTestMode(factoryID, areaID, testMode);

                if(hds.SetTestMode(factoryID, areaID, testMode) == "true")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        //Json Data
                        JObject jsonObject =
                            new JObject(
                                new JProperty("Result", "修改成功")
                            );
                        jsonarr.Add(jsonObject);
                    }
                }
            }
            catch
            {
                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "修改失敗")
                    );
                jsonarr.Add(jsonObject);
            }
            
            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string DelArea(string areaID)
        {
            string rslt = "";

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                string delArea = string.Format("delete from Area where AreaID = '{0}'", areaID);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, delArea);

                string delMachine = string.Format("delete from Machine where AreaID = '{0}'", areaID);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, delMachine);

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除成功")
                    );
                jsonarr.Add(jsonObject);

            }
            catch
            {
                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除失敗")
                    );
                jsonarr.Add(jsonObject);
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        #endregion

        #region Platform

        private string PlatformJson(DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int PlatformNo = dr.Field<Int32>("PlatformNo");
                    string CreateDate = dr.Field<DateTime>("CreateDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string AlterDate = dr.Field<DateTime>("AlterDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string FactoryID = dr["FactoryID"].ToString().Trim();
                    string AreaID = dr.Field<String>("AreaID").Trim();
                    string PlatformID = dr.Field<String>("PlatformID").Trim();
                    string PlatformName = dr.Field<String>("PlatformName").Trim();
                    string PlatformIP = dr.Field<String>("PlatformIP").Trim();
                    int PlatformPort = dr.Field<Int32>("PlatformPort");
                    int MNum = dr.Field<Int32>("MNum");
                    int DNum = dr.Field<Int32>("DNum");
                    int ConnectionStatus = dr.Field<Int32>("ConnectionStatus");
                    int VoltageState = dr.Field<Int32>("VoltageState");
                    string Note = dr.Field<String>("Note").Trim();
                    string Founder = dr["Founder"].ToString().Trim();
                    string Updater = dr.Field<String>("Updater").Trim();

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("PlatformNo", PlatformNo),
                            new JProperty("CreateDate", CreateDate),
                            new JProperty("AlterDate", AlterDate),
                            new JProperty("FactoryID", FactoryID),
                            new JProperty("AreaID", AreaID),
                            new JProperty("PlatformID", PlatformID),
                            new JProperty("PlatformName", PlatformName),
                            new JProperty("PlatformIP", PlatformIP),
                            new JProperty("PlatformPort", PlatformPort),
                            new JProperty("MNum", MNum),
                            new JProperty("DNum", DNum),
                            new JProperty("ConnectionStatus", ConnectionStatus),
                            new JProperty("VoltageState", VoltageState),
                            new JProperty("Note", Note),
                            new JProperty("Founder", Founder),
                            new JProperty("Updater", Updater)
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            return _jsonObject.ToString();
        }

        private string PlatformJson_New(DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int PlatformNo = dr.Field<Int32>("pn_PlatformNo");
                    string CreateDate = dr.Field<DateTime>("pn_CreateDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string AlterDate = dr.Field<DateTime>("pn_AlterDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string FactoryID = dr["pn_FactoryID"].ToString().Trim();
                    string AreaID = dr.Field<String>("pn_AreaID").Trim();
                    string PlatformID = dr.Field<String>("pn_PlatformID").Trim();
                    string PlatformName = dr.Field<String>("pn_PlatformName").Trim();
                    string PlatformIP = dr.Field<String>("pn_PlatformIP").Trim();
                    int PlatformPort = dr.Field<Int32>("pn_PlatformPort");
                    string GroupNo = dr.IsNull("pn_GroupNo") ? "" : dr.Field<string>("pn_GroupNo");
                    int XNum = dr.Field<Int32>("pn_XNum");
                    int YNum = dr.Field<Int32>("pn_YNum");
                    int ConnectionStatus = dr.Field<Int32>("pn_ConnectionStatus");
                    int VoltageState = dr.Field<Int32>("pn_VoltageState");
                    string Note = dr.IsNull("pn_Note") ? "" : dr.Field<String>("pn_Note").Trim();
                    string Founder = dr["pn_Founder"].ToString().Trim();
                    string Updater = dr.Field<String>("pn_Updater").Trim();

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("PlatformNo", PlatformNo),
                            new JProperty("CreateDate", CreateDate),
                            new JProperty("AlterDate", AlterDate),
                            new JProperty("FactoryID", FactoryID),
                            new JProperty("AreaID", AreaID),
                            new JProperty("PlatformID", PlatformID),
                            new JProperty("PlatformName", PlatformName),
                            new JProperty("PlatformIP", PlatformIP),
                            new JProperty("PlatformPort", PlatformPort),
                            new JProperty("GroupNo", GroupNo),
                            new JProperty("XNum", XNum),
                            new JProperty("YNum", YNum),
                            new JProperty("ConnectionStatus", ConnectionStatus),
                            new JProperty("VoltageState", VoltageState),
                            new JProperty("Note", Note),
                            new JProperty("Founder", Founder),
                            new JProperty("Updater", Updater)
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            return _jsonObject.ToString();
        }

        [WebMethod]
        public string GetAllPlatform()
        {
            string getPlatforms = string.Format("select * from Platform order by pn_PlatformID");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatforms);

            return PlatformJson(dt);
        }

        [WebMethod]
        public string GetAllPlatform_New()
        {
            string getPlatforms = string.Format("select * from Platform_New order by pn_PlatformID");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatforms);

            return PlatformJson_New(dt);
        }

        [WebMethod]
        public string GetAllowPlatform(string allow)
        {
            string getPlatforms = string.Format("select * from Platform where FactoryID in ('{0}') order by PlatformID", allow.Replace(",", "','"));
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatforms);

            return PlatformJson(dt);
        }

        [WebMethod]
        public string GetAllowPlatform_New(string allow)
        {
            string getPlatforms = string.Format("select * from Platform_New where pn_FactoryID in ('{0}') order by pn_PlatformID", allow.Replace(",", "','"));
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatforms);

            return PlatformJson_New(dt);
        }

        [WebMethod]
        public string GetThePlatform(string factoryID, string areaID)
        {
            string where = "";
            if (factoryID != "")
                where += string.Format(" and FactoryID = '{0}'", factoryID);
            if (areaID != "")
                where += string.Format(" and AreaID = '{0}'", areaID);
            string getPlatforms = string.Format("select * from Platform where 1 = 1{0} order by PlatformID", where);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatforms);

            return PlatformJson(dt);
        }

        [WebMethod]
        public string GetThePlatform_New(string factoryID, string areaID)
        {
            string where = "";
            if (factoryID != "")
                where += string.Format(" and pn_FactoryID = '{0}'", factoryID);
            if (areaID != "")
                where += string.Format(" and pn_AreaID = '{0}'", areaID);
            string getPlatforms = string.Format("select * from Platform_New where 1 = 1{0} order by pn_PlatformID", where);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatforms);

            return PlatformJson_New(dt);
        }

        [WebMethod]
        public string GetPlatformByID(string platformID)
        {
            string getPlatform = string.Format("select * from Platform where PlatformID = '{0}'", platformID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatform);

            return PlatformJson(dt);
        }

        [WebMethod]
        public string GetPlatformByID_New(string platformID)
        {
            string getPlatform = string.Format("select * from Platform_New where pn_PlatformID = '{0}'", platformID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatform);

            return PlatformJson_New(dt);
        }

        [WebMethod]
        public string GetMHoleListByGroupNo(string platformID, string groupNo)
        {
            string getHoleList = string.Format("select * from Platform_New  where pn_PlatformID = '{0}' and pn_GroupNo = '{1}'", platformID,groupNo);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getHoleList);

            return PlatformJson_New(dt);
        }

        [WebMethod]
        public string GetPlatformByAreaID(string areaID)
        {
            string getPlatforms = string.Format("select * from Platform where AreaID = '{0}' order by PlatformID", areaID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatforms);

            return PlatformJson(dt);
        }

        [WebMethod]
        public string GetPlatformByFactoryID(string factoryID)
        {
            string getPlatforms = string.Format("select * from Platform where FactoryID = '{0}' order by PlatformID", factoryID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatforms);

            return PlatformJson(dt);
        }

        [WebMethod]
        public string GetPlatformOption(string factoryID, string areaID)
        {
            string where = "";
            if (factoryID != "")
                where += string.Format(" and FactoryID = '{0}'", factoryID);
            if (areaID != "")
                where += string.Format(" and AreaID = '{0}'", areaID);
            string getPlatforms = string.Format("select * from Platform where 1 = 1{0} order by PlatformID", where);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatforms);

            return PlatformJson(dt);
        }

        [WebMethod]
        public string GetPlatformOption_New(string factoryID, string areaID)
        {
            string where = "";
            if (factoryID != "")
                where += string.Format(" and pn_FactoryID = '{0}'", factoryID);
            if (areaID != "")
                where += string.Format(" and pn_AreaID = '{0}'", areaID);
            string getPlatforms = string.Format("select * from Platform_New where 1 = 1{0} order by pn_PlatformID", where);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatforms);

            return PlatformJson_New(dt);
        }

        [WebMethod]
        public string AddPlatform(string factoryID, string areaID, string platformID, string platformName, string platformIP, int platformPort, int mNum, int dNum, string note, string founder)
        {
            string rslt = "";

            string getPlatform = string.Format("select count(*) as count from Platform where PlatformID = '{0}'", platformID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatform);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            if (rowcount > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "資料重複")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            else
            {
                try
                {
                    //Insert
                    string insertPlatform = string.Format("insert into Platform (CreateDate, AlterDate, FactoryID, AreaID, PlatformID, PlatformName, PlatformIP, PlatformPort, MNum, DNum, ConnectionStatus, VoltageState, Note, Founder, Updater) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}', '0', '0','{10}','{11}','{11}')",
                                                                                 System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), factoryID, areaID, platformID, platformName, platformIP, platformPort, mNum, dNum, note, founder);
                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertPlatform);

                    foreach (DataRow dr in dt.Rows)
                    {
                        //Json Data
                        JObject jsonObject =
                            new JObject(
                                new JProperty("Result", "新增成功")
                            );
                        jsonarr.Add(jsonObject);
                    }
                }
                catch
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "新增失敗")
                        );
                    jsonarr.Add(jsonObject);
                }                
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }
        [WebMethod]
        public string AddPlatform_New(string factoryID, string areaID, string platformID, string platformName, string platformIP, int platformPort, int groupNo,int xNum, int yNum, string note, string founder)
        {
            string rslt = "";

            string getPlatform = string.Format("select count(*) as count from Platform_New where pn_PlatformID = '{0}'", platformID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatform);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            if (rowcount > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "資料重複")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            else
            {
                try
                {
                    //Insert
                    string insertPlatform = string.Format("insert into Platform_New (pn_CreateDate, pn_AlterDate, pn_FactoryID, pn_AreaID, pn_PlatformID, pn_PlatformName, pn_PlatformIP, pn_PlatformPort, pn_GroupNo,pn_XNum, pn_YNum, pn_ConnectionStatus, pn_VoltageState, pn_Note, pn_Founder, pn_Updater) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}', '0', '0','{11}','{12}','{12}')",
                                                                                 System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), factoryID, areaID, platformID, platformName, platformIP, platformPort, groupNo,xNum, yNum, note, founder);
                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertPlatform);

                    foreach (DataRow dr in dt.Rows)
                    {
                        //Json Data
                        JObject jsonObject =
                            new JObject(
                                new JProperty("Result", "新增成功")
                            );
                        jsonarr.Add(jsonObject);
                    }
                }
                catch
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "新增失敗")
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string ModPlatform(string factoryID, string areaID, string platformID, string platformName, string platformIP, int platformPort, int mNum, int dNum, string note, string updater)
        {
            string rslt = "";

            string getPlatform = string.Format("select count(*) as count from Platform where PlatformID = '{0}'", platformID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatform);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                //Insert
                string modPlatform = string.Format("update Platform set AlterDate = '{0}', FactoryID = '{1}', AreaID = '{2}', PlatformName = '{3}', PlatformIP = '{4}', PlatformPort = '{5}', MNum = '{6}', DNum = '{7}', Note = '{8}', Updater = '{9}' where PlatformID = '{10}'",
                                                                            System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), factoryID, areaID, platformName, platformIP, platformPort, mNum, dNum, note, updater, platformID);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, modPlatform);

                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "修改成功")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            catch
            {
                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "修改失敗")
                    );
                jsonarr.Add(jsonObject);
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string ModPlatform_New(string factoryID, string areaID, string platformID, string platformName, string platformIP, int platformPort, int groupNo, int xNum, int yNum, string note, string updater)
        {
            string rslt = "";

            string getPlatform = string.Format("select count(*) as count from Platform_New where pn_PlatformID = '{0}'", platformID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPlatform);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                //Insert
                string modPlatform = string.Format("update Platform_New set pn_AlterDate = '{0}', pn_FactoryID = '{1}', pn_AreaID = '{2}', pn_PlatformName = '{3}', pn_PlatformIP = '{4}', pn_PlatformPort = '{5}', pn_GroupNo = '{6}',pn_XNum = '{7}', pn_YNum = '{8}', pn_Note = '{9}', pn_Updater = '{10}' where pn_PlatformID = '{11}'",
                                                                            System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), factoryID, areaID, platformName, platformIP, platformPort, groupNo, xNum,yNum, note, updater, platformID);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, modPlatform);

                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "修改成功")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            catch(Exception ex)
            {
                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "修改失敗," + ex.ToString())
                    );
                jsonarr.Add(jsonObject);
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }


        [WebMethod]
        public string DelPlatform(string platformID)
        {
            string rslt = "";

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                string delPlatform = string.Format("delete from Platform where PlatformID = '{0}'", platformID);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, delPlatform);

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除成功")
                    );
                jsonarr.Add(jsonObject);

            }
            catch
            {

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除失敗")
                    );
                jsonarr.Add(jsonObject);

            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        #endregion

        #region Machine

        private string MachineJson(DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int MachineNo = dr.Field<Int32>("MachineNo");
                    string CreateDate = dr.Field<DateTime>("CreateDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string AlterDate = dr.Field<DateTime>("AlterDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string FactoryID = dr.Field<String>("FactoryID").Trim();
                    string FactoryName = dr.Field<String>("FactoryName").Trim();
                    string AreaID = dr.Field<String>("AreaID").Trim();
                    string AreaName = dr.Field<String>("AreaName").Trim();
                    string PlatformID = dr.Field<String>("PlatformID").Trim();
                    string MachineID = dr.Field<String>("MachineID").Trim();
                    int SlaveNo = dr.Field<Int32>("SlaveNo");
                    int Channel = dr.Field<Int32>("Channel");
                    int CoordinateX = dr.Field<Int32>("CoordinateX");
                    int CoordinateY = dr.Field<Int32>("CoordinateY");
                    int ConnectionStatus = dr.Field<Int32>("ConnectionStatus");
                    int MachineState = dr.Field<Int32>("MachineState");
                    int LampState = dr.Field<Int32>("LampState");
                    string HoleID = dr.Field<String>("HoleID").Trim();
                    string OrderID = dr.Field<String>("OrderID").Trim();
                    string Founder = dr.Field<String>("Founder").Trim();
                    string Updater = dr.Field<String>("Updater").Trim();

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("MachineNo", MachineNo),
                            new JProperty("CreateDate", CreateDate),
                            new JProperty("AlterDate", AlterDate),
                            new JProperty("FactoryID", FactoryID),
                            new JProperty("FactoryName", FactoryName),
                            new JProperty("AreaID", AreaID),
                            new JProperty("AreaName", AreaName), 
                            new JProperty("PlatformID", PlatformID),
                            new JProperty("MachineID", MachineID),
                            new JProperty("SlaveNo", SlaveNo),
                            new JProperty("Channel", Channel),
                            new JProperty("CoordinateX", CoordinateX),
                            new JProperty("CoordinateY", CoordinateY),
                            new JProperty("ConnectionStatus", ConnectionStatus),
                            new JProperty("MachineState", MachineState),
                            new JProperty("LampState", LampState),
                            new JProperty("HoleID", HoleID),
                            new JProperty("OrderID", OrderID),
                            new JProperty("Founder", Founder),
                            new JProperty("Updater", Updater)
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));


            return _jsonObject.ToString();
        }


        [WebMethod]
        public string GetAllMachine()
        {
            string getMachines = string.Format("select Machine.*, Factory.FactoryName, Area.AreaName from Machine inner join Factory on Machine.FactoryID = Factory.FactoryID inner join Area on Machine.AreaID = Area.AreaID Order by Machine.FactoryID, Machine.AreaID desc, Machine.SlaveNo");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMachines);

            return MachineJson(dt);
        }

        [WebMethod]
        public string GetTheMachine(string factoryID, string areaID)
        {            
            string where = "";
            if (factoryID != "")
                where += string.Format(" and Machine.FactoryID = '{0}'", factoryID);
            if (areaID != "")
                where += string.Format(" and Machine.AreaID = '{0}'", areaID);
            string getMachines = string.Format("select Machine.*, Factory.FactoryName, Area.AreaName from Machine inner join Factory on Machine.FactoryID = Factory.FactoryID inner join Area on Machine.AreaID = Area.AreaID where 1 = 1{0} Order by Machine.FactoryID, Machine.AreaID desc, Machine.SlaveNo", where);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMachines);

            return MachineJson(dt);
        }

        [WebMethod]
        public string GetMachineByID(string machineID)
        {
            string getMachine = string.Format("select Machine.*, Factory.FactoryName, Area.AreaName from Machine inner join Factory on Machine.FactoryID = Factory.FactoryID inner join Area on Machine.AreaID = Area.AreaID where Machine.MachineID = '{0}' Order by Machine.FactoryID, Machine.AreaID desc, Machine.SlaveNo", machineID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMachine);

            return MachineJson(dt);
        }

        [WebMethod]
        public string GetMachineByPlatformID(string platformID)
        {
            string getMachines = string.Format("select Machine.*, Factory.FactoryName, Area.AreaName from Machine inner join Factory on Machine.FactoryID = Factory.FactoryID inner join Area on Machine.AreaID = Area.AreaID where Machine.PlatformID = '{0}' Order by Machine.FactoryID, Machine.AreaID desc, Machine.SlaveNo", platformID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMachines);

            return MachineJson(dt);
        }

        [WebMethod]
        public string GetMachineByAreaID(string areaID)
        {
            string getMachines = string.Format("select Machine.*, Factory.FactoryName, Area.AreaName from Machine inner join Factory on Machine.FactoryID = Factory.FactoryID inner join Area on Machine.AreaID = Area.AreaID where Machine.AreaID = '{0}' Order by Machine.FactoryID, Machine.AreaID desc, Machine.SlaveNo", areaID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMachines);

            return MachineJson(dt);
        }

        [WebMethod]
        public string GetMachineByFactoryID(string factoryID)
        {
            string getMachines = string.Format("select Machine.*, Factory.FactoryName, Area.AreaName from Machine inner join Factory on Machine.FactoryID = Factory.FactoryID inner join Area on Machine.AreaID = Area.AreaID where Machine.FactoryID = '{0}' Order by Machine.FactoryID, Machine.AreaID desc, Machine.SlaveNo", factoryID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMachines);

            return MachineJson(dt);
        }

        [WebMethod]
        public string GetMachineOption(string factoryID, string areaID, string platformID)
        {
            string where = "";
            if (factoryID != "")
                where += string.Format(" and Machine.FactoryID = '{0}'", factoryID);
            if (areaID != "")
                where += string.Format(" and Machine.AreaID = '{0}'", areaID);
            if (platformID != "")
                where += string.Format(" and Machine.PlatformID = '{0}'", platformID);
            string getMachines = string.Format("select Machine.*, Factory.FactoryName, Area.AreaName from Machine inner join Factory on Machine.FactoryID = Factory.FactoryID inner join Area on Machine.AreaID = Area.AreaID where 1 = 1{0} Order by Machine.FactoryID, Machine.AreaID desc, Machine.SlaveNo", where);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMachines);

            return MachineJson(dt);
        }

        [WebMethod]
        public string AddMachine(string factoryID, string areaID, string platformID, string machineID, string slaveNo, string channel, string coordinateX, string coordinateY, string founder)
        {
            string rslt = "";

            string getMachine = string.Format("select count(*) as count from Machine where MachineID = '{0}'", machineID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMachine);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            if (rowcount > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "資料重複")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            else
            {
                try
                {
                    //Insert
                    string insertMachine = string.Format("insert into Machine (CreateDate, AlterDate, FactoryID, AreaID, PlatformID, MachineID, SlaveNo, Channel, CoordinateX, CoordinateY, ConnectionStatus, MachineState, LampState, HoleID, OrderID, Founder, Updater) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}', '0', '0', '0', '00', '','{10}','{10}')",
                                                                                System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), factoryID, areaID, platformID, machineID, slaveNo, channel, coordinateX, coordinateY, founder);
                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertMachine);

                    foreach (DataRow dr in dt.Rows)
                    {
                        //Json Data
                        JObject jsonObject =
                            new JObject(
                                new JProperty("Result", "新增成功")
                            );
                        jsonarr.Add(jsonObject);
                    }
                }
                catch
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "新增失敗")
                        );
                    jsonarr.Add(jsonObject);
                }                
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string ModMachine(string factoryID, string areaID, string platformID, string machineID, string slaveNo, string channel, string coordinateX, string coordinateY, string updater)
        {
            string rslt = "";

            string getMachine = string.Format("select count(*) as count from Machine where MachineID = '{0}'", machineID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMachine);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                //Insert
                string modMachine = string.Format("update Machine set AlterDate = '{0}', FactoryID = '{1}', AreaID = '{2}', PlatformID = '{3}', SlaveNo = '{4}', Channel = '{5}', CoordinateX = '{6}', CoordinateY = '{7}', Updater = '{8}' where MachineID = '{9}'",
                                                                            System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), factoryID, areaID, platformID, slaveNo, channel, coordinateX, coordinateY, updater, machineID);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, modMachine);

                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "修改成功")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            catch
            {
                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "修改失敗")
                    );
                jsonarr.Add(jsonObject);
            }            

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string DelMachine(string machineID)
        {
            string rslt = "";

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                string delMachine = string.Format("delete from Machine where MachineID = '{0}'", machineID);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, delMachine);

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除成功")
                    );
                jsonarr.Add(jsonObject);

            }
            catch
            {

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除失敗")
                    );
                jsonarr.Add(jsonObject);

            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        #endregion

        #region Oracle

        private string OracleJson(DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string WADCTO = dr.Field<String>("WADCTO").Trim();

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("WADCTO", WADCTO)
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            return _jsonObject.ToString();
        }        

        #endregion

        #region MPile

        private string OracleMPileJson(DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string MPile = dr.Field<String>("MPile").Trim();

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("MPile", MPile)
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            return _jsonObject.ToString();
        }

        private string MPileJson(DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int MPileNo = dr.Field<Int32>("MPileNo");
                    string CreateDate = dr.Field<DateTime>("CreateDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string AlterDate = dr.Field<DateTime>("AlterDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string PlatformID = dr.Field<String>("PlatformID").Trim();
                    int MPileSort = dr.Field<Int32>("MPileSort");                    
                    string MPileSerial1 = dr.Field<String>("MPileSerial1").Trim();
                    string MPileSerial2 = dr.Field<String>("MPileSerial2").Trim();
                    string MPileSerial3 = dr.Field<String>("MPileSerial3").Trim();
                    string MRatio = dr.Field<String>("MRatio").Trim();
                    string Founder = dr.Field<String>("Founder").Trim();
                    string Updater = dr.Field<String>("Updater").Trim();

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("MPileNo", MPileNo),
                            new JProperty("CreateDate", CreateDate),
                            new JProperty("AlterDate", AlterDate),
                            new JProperty("PlatformID", PlatformID),
                            new JProperty("MPileSort", MPileSort),
                            new JProperty("MPileSerial1", MPileSerial1),
                            new JProperty("MPileSerial2", MPileSerial2),
                            new JProperty("MPileSerial3", MPileSerial3),
                            new JProperty("MRatio", MRatio),
                            new JProperty("Founder", Founder),
                            new JProperty("Updater", Updater)
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            return _jsonObject.ToString();
        }

        private string MHoleJson(DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int MHoleNo = dr.Field<Int32>("mh_MHoleNo");
                    string CreateDate = dr.Field<DateTime>("mh_CreateDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string AlterDate = dr.Field<DateTime>("mh_AlterDate").ToString("yyyy/MM/dd HH:mm:ss");
                    string PlatformID = dr.Field<String>("mh_PlatformID");
                    string GroupNo = dr.Field<string>("mh_GroupID");
                    string MHoleID = dr.Field<string>("mh_HoleID");
                    string MHoleSerial1 = dr.Field<String>("mh_MHoleSerial1");
                    string MHoleSerial2 = dr.Field<String>("mh_MHoleSerial2");
                    string MHoleSerial3 = dr.Field<String>("mh_MHoleSerial3");
                    string MRatio = dr.Field<String>("mh_MRatio");
                    string Founder = dr.Field<String>("mh_Founder");
                    string Updater = dr.Field<String>("mh_Updater");

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("MHoleNo", MHoleNo),
                            new JProperty("CreateDate", CreateDate),
                            new JProperty("AlterDate", AlterDate),
                            new JProperty("PlatformID", PlatformID),
                            new JProperty("GroupNo", GroupNo),
                            new JProperty("MHoleID", MHoleID),
                            new JProperty("MHoleSerial1", MHoleSerial1),
                            new JProperty("MHoleSerial2", MHoleSerial2),
                            new JProperty("MHoleSerial3", MHoleSerial3),
                            new JProperty("MRatio", MRatio),
                            new JProperty("Founder", Founder),
                            new JProperty("Updater", Updater)
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            return _jsonObject.ToString();
        }

        [WebMethod]
        public string GetOracleMPile(string keyword)
        {
            DataTable dt = new DataTable();//建立datatable接收query的資料
            using (OracleConnection conn = new OracleConnection(OracleConnection))
            {
                conn.Open();//開始連線
                //建立od物件接收select結果
                string commend = string.Format("select IMLITM as MPile from " + dta + ".F4101 where IMLITM like '{0}%' and IMGLPT = 'IN06' and IMPRP1 = '1'", keyword);
                OracleDataAdapter OD = new OracleDataAdapter(commend, conn);

                OD.Fill(dt);
                //指定datagridview的datasource

                conn.Close();//結束連線
            }

            return OracleMPileJson(dt);
        }

        [WebMethod]
        public string GetAllMPile()
        {
            string getMPiles = string.Format("select * from MPile Order by PlatformID,MPileSort");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMPiles);

            return MPileJson(dt);
        }

        [WebMethod]
        public string GetAllMHole()
        {
            string getMHoles = string.Format("select * from MHole Order by mh_PlatformID,mh_HoleID");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMHoles);

            return MHoleJson(dt);
        }

        [WebMethod]
        public string GetAllPlatformMPile(string platformID)
        {
            string getMPiles = string.Format("select * from MPile where PlatformID = '{0}' Order by PlatformID,MPileSort", platformID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMPiles);

            return MPileJson(dt);
        }

        [WebMethod]
        public string GetAllPlatformMHole(string platformID)
        {
            string getMPiles = string.Format("select * from MHole where mh_PlatformID = '{0}' Order by mh_PlatformID,mh_HoleID", platformID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMPiles);

            return MHoleJson(dt);
        }

        [WebMethod]
        public string GetAllMPileByPlatformID(string platformID)
        {
            string where = "";
            if (platformID != "")
            {
                where += string.Format(" where PlatformID = '{0}'", platformID);
            }
            string getMPiles = string.Format("select * from MPile{0}  Order by PlatformID,MPileSort", where);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMPiles);

            return MPileJson(dt);
        }

        [WebMethod]
        public string GetAllMHoleByPlatformID(string platformID)
        {
            string where = "";
            if (platformID != "")
            {
                where += string.Format(" where mh_PlatformID = '{0}'", platformID);
            }
            string getMHoles = string.Format("select * from MHole{0}  Order by mh_PlatformID,mh_HoleID", where);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMHoles);

            return MHoleJson(dt);
        }

        [WebMethod]
        public string GetMPileByPlatformID(string platformID, string MPileSort)
        {
            string getMPiles = string.Format("select * from MPile where PlatformID = '{0}' and MPileSort = '{1}'", platformID, MPileSort);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMPiles);

            return MPileJson(dt);
        }

        [WebMethod]
        public string GetMHoleByPlatformID(string platformID, string groupNo, string holeID)
        {
            string getMHoles = string.Format("select * from MHole where mh_PlatformID = '{0}' and mh_GroupID = '{1}' and mh_holeID = '{2}'", platformID, groupNo,holeID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMHoles);

            return MHoleJson(dt);
        }

        [WebMethod]
        public string AddMPile(string platformID, string MPileSort, string MPileSerial1, string MPileSerial2, string MPileSerial3, string MRatio, string founder)
        {
            string rslt = "";

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                //Insert
                string insertMPile = string.Format("insert into MPile (CreateDate, AlterDate, PlatformID, MPileSort, MPileSerial1, MPileSerial2, MPileSerial3, MRatio, Founder, Updater) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{8}')",
                                                                            System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), platformID, MPileSort, MPileSerial1, MPileSerial2, MPileSerial3, MRatio, founder);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertMPile);

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "新增成功")
                    );
                jsonarr.Add(jsonObject);
            }
            catch
            {
                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "新增失敗")
                    );
                jsonarr.Add(jsonObject);
            }            

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string ModMPile(string factoryID, string areaID, string platformID, int MPileSort, string MPileSerial1, string MPileSerial2, string MPileSerial3, string MRatio, string updater)
        {
            string rslt = "";

            string getMPile = string.Format("select count(*) as count from MPile where PlatformID = '{0}' and MPileSort = '{1}'", platformID, MPileSort);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMPile);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                //Insert
                string modMPile = string.Format("update MPile set AlterDate = '{0}', MPileSerial1 = '{1}', MPileSerial2 = '{2}', MPileSerial3 = '{3}', MRatio = '{4}', Updater = '{5}' where PlatformID = '{6}' and MPileSort = '{7}'",
                                                                            System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), MPileSerial1, MPileSerial2, MPileSerial3, MRatio, updater, platformID, MPileSort);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, modMPile);

                if (hds == null)
                    hds = new HoleDetectServiceReference.HoleDetectService();

                string[] MPileSerial = new string[] { MPileSerial1, MPileSerial2, MPileSerial3 };

                hds.SetMaterialData(factoryID, areaID, platformID, MPileSort, MPileSerial, MRatio);

                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "設定成功")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            catch
            {
                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "設定失敗")
                    );
                jsonarr.Add(jsonObject);
            }
            
            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string ModMHole(string factoryID, string areaID, string platformID, string MHoleID, string MHoleSerial1, string MHoleSerial2, string MHoleSerial3, string MRatio, string updater)
        {
            string rslt = "";

            string getMHole = string.Format("select count(*) as count from MHole where mh_PlatformID = '{0}' and mh_HoleID = '{1}'", platformID, MHoleID);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMHole);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                //Insert
                string modMPile = string.Format("update MHole set mh_AlterDate = '{0}', mh_MHoleSerial1 = '{1}', mh_MHoleSerial2 = '{2}', mh_MHoleSerial3 = '{3}', mh_MRatio = '{4}', mh_Updater = '{5}' where mh_PlatformID = '{6}' and mh_HoleID = '{7}'",
                                                                            System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), MHoleSerial1, MHoleSerial2, MHoleSerial3, MRatio, updater, platformID, MHoleID);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, modMPile);

                if (hds == null)
                    hds = new HoleDetectServiceReference.HoleDetectService();

                string[] MHoleSerial = new string[] { MHoleSerial1, MHoleSerial2, MHoleSerial3 };

                //hds.SetMaterialData_New(factoryID, areaID, platformID, MHoleID, MHoleSerial, MRatio);

                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "設定成功")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            catch
            {
                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "設定失敗")
                    );
                jsonarr.Add(jsonObject);
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }


        [WebMethod]
        public string ModBatchMPile(string factoryID, string areaID, string platformID, string MPileSort, string MPileSerial1, string MPileSerial2, string MPileSerial3, string MRatio, string updater)
        {
            string rslt = "";

            string where = "";
            if (factoryID != "")
                where += string.Format(" and Platform.FactoryID = '{0}'", factoryID);
            if (areaID != "")
                where += string.Format(" and Platform.AreaID = '{0}'", areaID);
            if (platformID != "")
                where += string.Format(" and MPile.PlatformID = '{0}'", platformID);
            char[] p1 = { ';' };
            char[] p2 = { ',' };

            string[] MPileSet = MPileSort.Split(p1);
            string oldMRatio = "";
            if (MPileSet.Length > 1)
                oldMRatio = MPileSet[1];

            string[] oldSerialSet = MPileSet[0].Split(p2);
            string oldSerial1 = "";
            string oldSerial2 = "";
            string oldSerial3 = "";
            if (oldSerialSet.Length > 0)
                oldSerial1 = oldSerialSet[0];
            if (oldSerialSet.Length > 1)
                oldSerial2 = oldSerialSet[1];
            if (oldSerialSet.Length > 2)
                oldSerial3 = oldSerialSet[2];
            where += string.Format(" and MPile.MPileSerial1 = '{0}' and MPile.MPileSerial2 = '{1}' and MPile.MPileSerial3 = '{2}' and MPile.MRatio = '{3}'", oldSerial1, oldSerial2, oldSerial3, oldMRatio);            

            string getMPile = string.Format("select MPile.*, Platform.FactoryID, Platform.AreaID from MPile inner join Platform on Platform.PlatformID = MPile.PlatformID where 1 = 1{0}", where);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMPile);

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                if (hds == null)
                    hds = new HoleDetectServiceReference.HoleDetectService();

                string[] MPileSerial = new string[] { MPileSerial1, MPileSerial2, MPileSerial3 };

                foreach (DataRow dr in dt.Rows)
                {
                    //Insert
                    string modMPile = string.Format("update MPile set AlterDate = '{0}', MPileSerial1 = '{1}', MPileSerial2 = '{2}', MPileSerial3 = '{3}', MRatio = '{4}', Updater = '{5}' where PlatformID = '{6}' and MPileSort = '{7}'",
                                                                            System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), MPileSerial1, MPileSerial2, MPileSerial3, MRatio, updater, dr.Field<String>("PlatformID").Trim(), dr.Field<Int32>("MPileSort"));
                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, modMPile);

                    hds.SetMaterialData(dr.Field<String>("FactoryID").Trim(), dr.Field<String>("AreaID").Trim(), dr.Field<String>("PlatformID").Trim(), dr.Field<Int32>("MPileSort"), MPileSerial, MRatio);
                }

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "設定成功")
                    );
                jsonarr.Add(jsonObject);
            }
            catch
            {
                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "設定失敗")
                    );
                jsonarr.Add(jsonObject);
            }                    
            

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string ModBatchMHole(string factoryID, string areaID, string platformID, string MHoleID, string MHoleSerial1, string MHoleSerial2, string MHoleSerial3, string MRatio, string updater)
        {
            string rslt = "";

            string where = "";
            if (factoryID != "")
                where += string.Format(" and Platform.FactoryID = '{0}'", factoryID);
            if (areaID != "")
                where += string.Format(" and Platform.AreaID = '{0}'", areaID);
            if (platformID != "")
                where += string.Format(" and MPile.PlatformID = '{0}'", platformID);
            char[] p1 = { ';' };
            char[] p2 = { ',' };

            string[] MHoleSet = MHoleID.Split(p1);
            string oldMRatio = "";
            if (MHoleSet.Length > 1)
                oldMRatio = MHoleSet[1];

            string[] oldSerialSet = MHoleSet[0].Split(p2);
            string oldSerial1 = "";
            string oldSerial2 = "";
            string oldSerial3 = "";
            if (oldSerialSet.Length > 0)
                oldSerial1 = oldSerialSet[0];
            if (oldSerialSet.Length > 1)
                oldSerial2 = oldSerialSet[1];
            if (oldSerialSet.Length > 2)
                oldSerial3 = oldSerialSet[2];
            where += string.Format(" and m.MHoleSerial1 = '{0}' and m.mh_MHoleSerial2 = '{1}' and m.mh_MHoleSerial3 = '{2}' and m.mh_MRatio = '{3}'", oldSerial1, oldSerial2, oldSerial3, oldMRatio);

            string getMPile = string.Format("select m.*, p.pn_FactoryID, p.pn_AreaID from MHole m inner join Platform_New p on p.pn_PlatformID = m.pn_PlatformID where 1 = 1{0}", where);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getMPile);

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                if (hds == null)
                    hds = new HoleDetectServiceReference.HoleDetectService();

                string[] MHoleSerial = new string[] { MHoleSerial1, MHoleSerial2, MHoleSerial3 };

                foreach (DataRow dr in dt.Rows)
                {
                    //Insert
                    string modMHole = string.Format("update MHole set mh_AlterDate = '{0}', mh_MHoleSerial1 = '{1}', mh_MHoleSerial2 = '{2}', mh_MHoleSerial3 = '{3}', mh_MRatio = '{4}', mh_Updater = '{5}' where mh_PlatformID = '{6}' and mh_MHoleID = '{7}'",
                                                                            System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), MHoleSerial1, MHoleSerial2, MHoleSerial3, MRatio, updater, dr.Field<String>("PlatformID").Trim(), dr.Field<string>("MHoleID"));
                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, modMHole);

                    hds.SetMaterialData_New(dr.Field<String>("FactoryID").Trim(), dr.Field<String>("AreaID").Trim(), dr.Field<String>("PlatformID").Trim(), dr.Field<string>("MHoleID"), MHoleSerial, MRatio);
                }

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "設定成功")
                    );
                jsonarr.Add(jsonObject);
            }
            catch
            {
                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "設定失敗")
                    );
                jsonarr.Add(jsonObject);
            }


            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }


        [WebMethod]
        public string DeleteMPileByPlatformID(string platformID)
        {
            string rslt = "";

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                string delMPile = string.Format("delete from MPile where PlatformID = '{0}'", platformID);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, delMPile);

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除成功")
                    );
                jsonarr.Add(jsonObject);

            }
            catch
            {

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除失敗")
                    );
                jsonarr.Add(jsonObject);

            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string DeleteMPileByMPileSort(string platformID, int MPileSort)
        {
            string rslt = "";

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                string delMPile = string.Format("delete from MPile where PlatformID = '{0}' and MPileSort = '{1}'", platformID, MPileSort);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, delMPile);

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除成功")
                    );
                jsonarr.Add(jsonObject);

            }
            catch
            {

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除失敗")
                    );
                jsonarr.Add(jsonObject);

            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }
        #endregion

        #region MHole


        #endregion

        #region Status
        private string StatusJson(int total, int num, DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();
            JArray infoarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string FactoryID = dr.Field<String>("FactoryID").Trim();
                    string FactoryName = dr.Field<String>("FactoryName").Trim();
                    string AreaID = dr.Field<String>("AreaID").Trim();
                    string AreaName = dr.Field<String>("AreaName").Trim();
                    string MachineID = dr.Field<String>("MachineID").Trim();
                    string OrderType = dr.Field<String>("OrderType").Trim();
                    string OrderID = dr.Field<String>("OrderID").Trim();
                    int RedCount = dr.Field<Int32>("RedCount");
                    int GreenCount = dr.Field<Int32>("GreenCount");
                    int BlueCount = dr.Field<Int32>("BlueCount");
                    int OrangeCount = dr.Field<Int32>("OrangeCount");
                    int WhiteCount = dr.Field<Int32>("WhiteCount");

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("FactoryID", FactoryID),
                            new JProperty("FactoryName", FactoryName),
                            new JProperty("AreaID", AreaID),
                            new JProperty("AreaName", AreaName),
                            new JProperty("MachineID", MachineID),
                            new JProperty("OrderType", OrderType),
                            new JProperty("OrderID", OrderID),
                            new JProperty("RedCount", RedCount),
                            new JProperty("GreenCount", GreenCount),
                            new JProperty("BlueCount", BlueCount),
                            new JProperty("OrangeCount", OrangeCount),
                            new JProperty("WhiteCount", WhiteCount)
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            JObject jsonInfo =
                new JObject(
                    new JProperty("Total", total),
                    new JProperty("Pages", Convert.ToInt16(Math.Ceiling(Convert.ToDecimal(total) / Convert.ToDecimal(num)))),
                    new JProperty("Num", num)
                );
            infoarr.Add(jsonInfo);

            //Json Main object
            JObject _jsonObject =
                new JObject(
                    new JProperty("Info", infoarr),
                    new JProperty("Data", jsonarr)
                );

            return _jsonObject.ToString();
        }

        [WebMethod]
        public string GetPageStatus(string factoryID, string areaID, string platformID, string machineID, string startDate, string endDate, string orderID, int pg)
        {
            int num = 10;
            string where = "";
            if (factoryID != "")
                where += string.Format(" and mr.FactoryID = '{0}'", factoryID);
            if (areaID != "")
                where += string.Format(" and mr.AreaID = '{0}'", areaID);
            if (platformID != "")
                where += string.Format(" and mr.PlatformID = '{0}'", platformID);
            if (machineID != "")
                where += string.Format(" and mr.MachineID = '{0}'", machineID);
            if (startDate != "" && endDate != "") {
                where += string.Format(" and DATEDIFF(second, mr.RecordDate, '{0} 00:00:00') < 0 and DATEDIFF(second, mr.RecordDate, '{1} 23:59:59') > 0", startDate, endDate);
            }
            if (orderID != "")
                where += string.Format(" and mr.NowOrder like '%{0}%'", orderID);

            string countStatus = string.Format("select count(*) as count from (select mr.FactoryID, mr.AreaID, mr.PlatformID, mr.MachineID, mr.NowOrderType as OrderType, mr.NowOrder as OrderID from MachineRecord as mr where 1=1{0} group by mr.FactoryID, mr.AreaID, mr.PlatformID, mr.MachineID, mr.NowOrderType, mr.NowOrder) as targetTable", where);
            DataTable dtCount = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, countStatus);
            int total = Convert.ToInt32(dtCount.Rows[0]["count"].ToString());

            string getStatus = string.Format("select * from (select ROW_NUMBER() over (order by mr.FactoryID, mr.AreaID, mr.PlatformID, mr.MachineID, mr.NowOrder) as rn, mr.FactoryID, mr.AreaID, mr.MachineID, mr.NowOrderType as OrderType, mr.NowOrder as OrderID, (select count(*) from MachineRecord where MachineID = mr.MachineID and NowOrder = mr.NowOrder and MachineState = '0') as RedCount, (select count(*) from MachineRecord where MachineID = mr.MachineID and NowOrder = mr.NowOrder and MachineState = '1') as GreenCount, (select count(*) from MachineRecord where MachineID = mr.MachineID and NowOrder = mr.NowOrder and MachineState = '2') as BlueCount, (select count(*) from MachineRecord where MachineID = mr.MachineID and NowOrder = mr.NowOrder and MachineState = '3') as OrangeCount, (select count(*) from MachineRecord where MachineID = mr.MachineID and NowOrder = mr.NowOrder and MachineState = '4') as WhiteCount, Factory.FactoryName, Area.AreaName from MachineRecord as mr inner join Factory on mr.FactoryID = Factory.FactoryID inner join Area on mr.AreaID = Area.AreaID where 1=1{0} group by mr.FactoryID, Factory.FactoryName, mr.AreaID, Area.AreaName, mr.PlatformID, mr.NowOrderType, mr.MachineID, mr.NowOrder) as targetTable where targetTable.rn > {1} and targetTable.rn <= {2}", where, (pg - 1) * num, pg * num);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getStatus);

            return StatusJson(total, num, dt);
        }

        #endregion

        #region History
        private string HistoryJson(int total, int num, DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();
            JArray infoarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string FactoryID = dr.Field<String>("FactoryID").Trim();
                    string FactoryName = dr.Field<String>("FactoryName").Trim();
                    string AreaID = dr.Field<String>("AreaID").Trim();
                    string AreaName = dr.Field<String>("AreaName").Trim();
                    string PlatformID = dr.Field<String>("PlatformID").Trim();
                    string MachineID = dr.Field<String>("MachineID").Trim();
                    int MachineState = dr.Field<Int32>("MachineState");
                    string OrderID = dr.Field<String>("OrderID").Trim().Trim();
                    string OrderType = dr.Field<String>("OrderType").Trim();
                    string ProductID = dr.Field<String>("ProductID").Trim();
                    string SpecMaterial = dr.Field<String>("SpecMaterial").Trim();
                    int OrdQuantity = dr.Field<Int32>("OrdQuantity");
                    int NowQuantity = dr.Field<Int32>("NowQuantity");

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("FactoryID", FactoryID),
                            new JProperty("FactoryName", FactoryName),
                            new JProperty("AreaID", AreaID),
                            new JProperty("AreaName", AreaName),
                            new JProperty("PlatformID", PlatformID),
                            new JProperty("MachineID", MachineID),
                            new JProperty("MachineState", MachineState),
                            new JProperty("OrderID", OrderID),
                            new JProperty("OrderType", OrderType),
                            new JProperty("ProductID", ProductID),
                            new JProperty("SpecMaterial", SpecMaterial),
                            new JProperty("OrdQuantity", OrdQuantity),
                            new JProperty("NowQuantity", NowQuantity)
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            JObject jsonInfo =
                new JObject(
                    new JProperty("Total", total),
                    new JProperty("Pages", Convert.ToInt16(Math.Ceiling(Convert.ToDecimal(total) / Convert.ToDecimal(num)))),
                    new JProperty("Num", num)
                );
            infoarr.Add(jsonInfo);

            //Json Main object
            JObject _jsonObject = 
                new JObject(
                    new JProperty("Info", infoarr),
                    new JProperty("Data", jsonarr)
                );

            return _jsonObject.ToString();
        }

        [WebMethod]
        public string GetPageHistory(string factoryID, string areaID, string platformID, string machineID, string startDate, string endDate, string orderID, int pg)
        {
            int num = 10;
            string where = "";
            if (factoryID != "")
                where += string.Format(" and o.FactoryID = '{0}'", factoryID);
            if (areaID != "")
                where += string.Format(" and o.AreaID = '{0}'", areaID);
            if (platformID != "")
                where += string.Format(" and o.PlatformID = '{0}'", platformID);
            if (machineID != "")
                where += string.Format(" and o.MachineID = '{0}'", machineID);
            if (startDate != "" && endDate != "")
            {
                where += string.Format(" and DATEDIFF(second, o.ModDate, '{0} 00:00:00') < 0 and DATEDIFF(second, o.ModDate, '{1} 23:59:59') > 0", startDate, endDate);
            }
            if (orderID != "")
                where += string.Format(" and OrderID like '%{0}%'", orderID);

            string countHistory = string.Format("select count(*) as count from Orders as o where 1 = 1{0}", where);
            DataTable dtCount = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, countHistory);
            int total = Convert.ToInt32(dtCount.Rows[0]["count"].ToString());

            string getHistory = string.Format("select * from (select ROW_NUMBER() over (order by o.FactoryID asc, o.AreaID asc, o.PlatformID asc, o.MachineID asc, o.OrderID asc) as rn, o.*, Factory.FactoryName, Area.AreaName from Orders as o inner join Factory on o.FactoryID = Factory.FactoryID inner join Area on o.AreaID = Area.AreaID where 1 = 1{0}) as targetTable where targetTable.rn > {1} and targetTable.rn <= {2}", where, (pg - 1) * num, pg * num);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getHistory);

            return HistoryJson(total, num, dt);
        }

        #endregion

        #region Detail

        private string DetailJson(int total, int num, DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();
            JArray infoarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string RecordDate = dr.Field<DateTime>("RecordDate").ToString("yyyy-MM-dd HH:mm:ss");
                    string FactoryID = dr.Field<String>("FactoryID").Trim();
                    string FactoryName = dr.Field<String>("FactoryName").Trim();
                    string AreaID = dr.Field<String>("AreaID").Trim();
                    string AreaName = dr.Field<String>("AreaName").Trim();
                    string PlatformID = dr.Field<String>("PlatformID").Trim();
                    string MachineID = dr.Field<String>("MachineID").Trim();
                    int MachineState = dr.Field<Int32>("MachineState");
                    int LampState = dr.Field<Int32>("LampState");
                    string HoleID = dr.Field<String>("HoleID").Trim();
                    string MPlileList = dr.Field<String>("MPlileList").Trim();
                    double Weight = dr.Field<Double>("Weight");
                    string WeightUnit = dr.Field<String>("WeightUnit").Trim();
                    string OrderID = dr.Field<String>("NowOrder").Trim();
                    string OrderType = dr.Field<String>("NowOrderType").Trim();
                    int NowQuantity = dr.Field<Int32>("NowQuantity");

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("RecordDate", RecordDate),
                            new JProperty("FactoryID", FactoryID),
                            new JProperty("FactoryName", FactoryName),
                            new JProperty("AreaID", AreaID),
                            new JProperty("AreaName", AreaName),
                            new JProperty("PlatformID", PlatformID),
                            new JProperty("MachineID", MachineID),
                            new JProperty("MachineState", MachineState),
                            new JProperty("LampState", LampState),
                            new JProperty("HoleID", HoleID),
                            new JProperty("MPlileList", MPlileList),
                            new JProperty("Weight", Weight),
                            new JProperty("WeightUnit", WeightUnit),
                            new JProperty("OrderID", OrderID),
                            new JProperty("OrderType", OrderType),
                            new JProperty("NowQuantity", NowQuantity)
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            JObject jsonInfo =
                new JObject(
                    new JProperty("Total", total),
                    new JProperty("Pages", Convert.ToInt16(Math.Ceiling(Convert.ToDecimal(total) / Convert.ToDecimal(num)))),
                    new JProperty("Num", num)
                );
            infoarr.Add(jsonInfo);

            //Json Main object
            JObject _jsonObject =
                new JObject(
                    new JProperty("Info", infoarr),
                    new JProperty("Data", jsonarr)
                );

            return _jsonObject.ToString();
        }

        [WebMethod]
        public string GetPageDetail(string machineID, string orderID, int pg)
        {
            int num = 10;
            string where = "";

            where += string.Format(" and MachineID = '{0}'", machineID);
            where += string.Format(" and NowOrder like '%{0}%'", orderID);

            string countDetail = string.Format("select count(*) as total from (select ROW_NUMBER() over (order by RecordDate desc) as rn1, * from MachineRecord where 1 = 1{0}) as mr1 left join (select ROW_NUMBER() over (order by RecordDate desc)+1 as rn2, * from MachineRecord where 1 = 1{0}) as mr2 on mr1.rn1 = mr2.rn2 where (mr1.MachineState <> mr2.MachineState) or (mr1.LampState <> mr2.LampState) or (mr1.Weight <> mr2.Weight) or (mr1.NowQuantity <> mr2.NowQuantity)", where);
            DataTable dtCount = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, countDetail);
            int total = Convert.ToInt32(dtCount.Rows[0]["total"].ToString());

            string getDetail = string.Format("select * from (select ROW_NUMBER() over (order by mr1.RecordDate desc) as rn, mr1.*, Factory.FactoryName, Area.AreaName from (select ROW_NUMBER() over (order by RecordDate desc) as rn1, * from MachineRecord where 1 = 1{0}) as mr1 left join (select ROW_NUMBER() over (order by RecordDate desc)-1 as rn2, * from MachineRecord where 1 = 1{0}) as mr2 on mr1.rn1 = mr2.rn2 inner join Factory on mr1.FactoryID = Factory.FactoryID inner join Area on mr1.AreaID = Area.AreaID where (mr1.MachineState <> mr2.MachineState) or (mr1.LampState <> mr2.LampState) or (mr1.Weight <> mr2.Weight) or (mr1.NowQuantity <> mr2.NowQuantity)) as targetTable where targetTable.rn > {1} and targetTable.rn <= {2}", where, (pg - 1) * num, pg * num);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getDetail);
            
            return DetailJson(total, num, dt);
        }

        #endregion

        #region Order
        private string OracleOrderJson(DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string WADCTO = dr.Field<String>("WADCTO").Trim();
                    string wasrst = dr.Field<String>("wasrst").Trim();
                    string WADOCO = dr.Field<String>("WADOCO").Trim();
                    string ProductID = dr.Field<String>("ProductID").Trim();
                    int Quantity = dr.Field<Int32>("Quantity");

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("WADCTO", WADCTO),
                            new JProperty("wasrst", wasrst),
                            new JProperty("WADOCO", WADOCO),
                            new JProperty("ProductID", ProductID),
                            new JProperty("Quantity", Quantity)
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            return _jsonObject.ToString();
        }

        #endregion

        #region Login
        [WebMethod]
        public string ValidateUser(string UserName, string Password)
        {
            // 如果 ping 不到 host name
            // 要加 ip 與 host name 對應
            // C:\WINDOWS\system32\drivers\etc\hosts.
            // 例: 10.101.107.150	lab_evta.lab.corp 

            string strPath;

            if (ComputerName.IndexOf('.') != -1)
                strPath = string.Format(@"LDAP://{0}", ComputerName);
            else
                strPath = string.Format(@"WinNT://{0}/{1}, user", ComputerName, UserName);

            System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(strPath, UserName, Password);
            string value = "";

            try
            {
                string objectSid = (new System.Security.Principal.SecurityIdentifier((byte[])entry.Properties["objectSid"].Value, 0).Value);

                return objectSid;
            }
            catch (Exception)// (DirectoryServicesCOMException)
            {
                return "驗證失敗";
            }
            finally
            {
                entry.Dispose();
            }
        }        
        #endregion

        #region Group

        private string GroupJson(DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int GroupNo = dr.Field<Int32>("GroupNo");
                    string CreateDate = dr.Field<DateTime>("CreateDate").ToString("yyyy-MM-dd HH:mm:ss");
                    string AlterDate = dr.Field<DateTime>("AlterDate").ToString("yyyy-MM-dd HH:mm:ss");
                    string GroupName = dr.Field<String>("GroupName").Trim();
                    int DetailAuth = dr.Field<Int32>("DetailAuth");
                    int InfoAuth = dr.Field<Int32>("InfoAuth");
                    string AllowAuth = dr.Field<String>("AllowAuth");
                    int NewsAuth = dr.Field<Int32>("NewsAuth");
                    int HistoryAuth = dr.Field<Int32>("HistoryAuth");
                    int StatusAuth = dr.Field<Int32>("StatusAuth");
                    int FactoryAuth = dr.Field<Int32>("FactoryAuth");
                    int AreaAuth = dr.Field<Int32>("AreaAuth");
                    int PlatformAuth = dr.Field<Int32>("PlatformAuth");
                    int GroupAuth = dr.Field<Int32>("GroupAuth");
                    string Founder = dr.Field<String>("Founder").Trim();
                    string Updater = dr.Field<String>("Updater").Trim();

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("GroupNo", GroupNo),
                            new JProperty("CreateDate", CreateDate),
                            new JProperty("AlterDate", AlterDate),
                            new JProperty("GroupName", GroupName),
                            new JProperty("DetailAuth", DetailAuth),
                            new JProperty("InfoAuth", InfoAuth),
                            new JProperty("AllowAuth", AllowAuth),
                            new JProperty("NewsAuth", NewsAuth),
                            new JProperty("HistoryAuth", HistoryAuth),
                            new JProperty("StatusAuth", StatusAuth),
                            new JProperty("FactoryAuth", FactoryAuth),
                            new JProperty("AreaAuth", AreaAuth),
                            new JProperty("PlatformAuth", PlatformAuth),
                            new JProperty("GroupAuth", GroupAuth),
                            new JProperty("Founder", Founder),
                            new JProperty("Updater", Updater)
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));


            return _jsonObject.ToString();
        }

        private string GroupPermissionJson(DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int PermissionNo = dr.Field<Int32>("PermissionNo");
                    string CreateDate = dr.Field<DateTime>("CreateDate").ToString("yyyy-MM-dd HH:mm:ss");
                    int GroupNo = dr.Field<Int32>("GroupNo");
                    string Username = dr.Field<String>("Username").Trim();
                    string Founder = dr.Field<String>("Founder").Trim();

                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("PermissionNo", PermissionNo),
                            new JProperty("CreateDate", CreateDate),
                            new JProperty("GroupNo", GroupNo),
                            new JProperty("Username", Username),                            
                            new JProperty("Founder", Founder)
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));


            return _jsonObject.ToString();
        }

        [WebMethod]
        public string GetAllGroups()
        {
            string getGroups = string.Format("select * from Groups");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getGroups);

            return GroupJson(dt);
        }

        [WebMethod]
        public string GetAllPermission(int groupNo)
        {
            string getPermissions = string.Format("select * from GroupPermission where GroupNo = '{0}'", groupNo);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPermissions);

            return GroupPermissionJson(dt);
        }

        [WebMethod]
        public string GetGroupsByNo(string groupNo)
        {
            string getGroups = string.Format("select * from Groups where groupNo = '{0}'", groupNo);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getGroups);

            return GroupJson(dt);
        }

        [WebMethod]
        public string AddGroups(string groupName, int detailAuth, int infoAuth, string allowAuth, int newsAuth, int historyAuth, int statusAuth, int factoryAuth, int areaAuth, int platformAuth, int groupAuth, string founder)
        {
            string rslt = "";

            string getGroup = string.Format("select count(*) as count from Groups where GroupName = '{0}'", groupName);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getGroup);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            if (rowcount > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "資料重複")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            else
            {
                try
                {
                    //Insert
                    string insertGroups = string.Format("insert into Groups (CreateDate, AlterDate, GroupName, DetailAuth, InfoAuth, AllowAuth, NewsAuth, HistoryAuth, StatusAuth, FactoryAuth, AreaAuth, PlatformAuth, GroupAuth, Founder, Updater) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{13}')",
                                                                                System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), groupName, detailAuth, infoAuth, allowAuth, newsAuth, historyAuth, statusAuth, factoryAuth, areaAuth, platformAuth, groupAuth, founder);
                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertGroups);

                    foreach (DataRow dr in dt.Rows)
                    {
                        //Json Data
                        JObject jsonObject =
                            new JObject(
                                new JProperty("Result", "新增成功")
                            );
                        jsonarr.Add(jsonObject);
                    }
                }
                catch
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "新增失敗")
                        );
                    jsonarr.Add(jsonObject);
                }

                
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string ModGroups(int groupNo, string groupName, int detailAuth, int infoAuth, string allowAuth, int newsAuth, int historyAuth, int statusAuth, int factoryAuth, int areaAuth, int platformAuth, int groupAuth, string updater)
        {
            string rslt = "";

            string getGroup = string.Format("select count(*) as count from Groups where GroupNo = '{0}'", groupNo);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getGroup);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                //Insert
                string modGroups = string.Format("update Groups set AlterDate = '{0}', GroupName = '{1}', DetailAuth = '{2}', InfoAuth = '{3}', AllowAuth = '{4}', NewsAuth = '{5}', HistoryAuth = '{6}', StatusAuth = '{7}', FactoryAuth = '{8}', AreaAuth = '{9}', PlatformAuth = '{10}', GroupAuth = '{11}', Updater = '{12}' where GroupNo = '{13}'",
                                                                            System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), groupName, detailAuth, infoAuth, allowAuth, newsAuth, historyAuth, statusAuth, factoryAuth, areaAuth, platformAuth, groupAuth, updater, groupNo);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, modGroups);

                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "修改成功")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            catch
            {
                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "修改失敗")
                    );
                jsonarr.Add(jsonObject);
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string DelGroups(int groupNo)
        {
            string rslt = "";

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                string delGroups = string.Format("delete from Groups where GroupNo = '{0}'", groupNo);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, delGroups);

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除成功")
                    );
                jsonarr.Add(jsonObject);
            }
            catch
            {

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除失敗")
                    );
                jsonarr.Add(jsonObject);

            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string AddPermission(int groupNo, string username, string founder)
        {
            string rslt = "";

            string getPermissions = string.Format("select count(*) as count from GroupPermission where Username = '{0}'", username);
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getPermissions);

            int rowcount = Convert.ToInt32(dt.Rows[0]["count"].ToString());

            //Json Data array
            JArray jsonarr = new JArray();

            if (rowcount > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "該使用者已存在於其他組別")
                        );
                    jsonarr.Add(jsonObject);
                }
            }
            else
            {
                try
                {
                    //Insert
                    string insertPermission = string.Format("insert into GroupPermission (CreateDate, GroupNo, Username, Founder) values ('{0}','{1}','{2}','{3}')",
                                                                                System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), groupNo, username, founder);
                    SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, insertPermission);

                    foreach (DataRow dr in dt.Rows)
                    {
                        //Json Data
                        JObject jsonObject =
                            new JObject(
                                new JProperty("Result", "新增成功")
                            );
                        jsonarr.Add(jsonObject);
                    }
                }
                catch
                {
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("Result", "新增成功")
                        );
                    jsonarr.Add(jsonObject);
                }
                
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        [WebMethod]
        public string DelPermission(int permissionNo)
        {
            string rslt = "";

            //Json Data array
            JArray jsonarr = new JArray();

            try
            {
                string delPermission = string.Format("delete from GroupPermission where PermissionNo = '{0}'", permissionNo);
                SqlHelper.ExecuteNonQuery(SqlHelper.MSSQLConnection, CommandType.Text, delPermission);

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除成功")
                    );
                jsonarr.Add(jsonObject);
            }
            catch
            {

                //Json Data
                JObject jsonObject =
                    new JObject(
                        new JProperty("Result", "刪除失敗")
                    );
                jsonarr.Add(jsonObject);

            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            rslt = _jsonObject.ToString();

            return rslt;
        }

        #endregion

        #region Ratio

        private string RatioJson(DataTable dt)
        {
            //Json Data array
            JArray jsonarr = new JArray();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int RatioNo = dr.Field<Int32>("RatioNo");
                    string Option = dr.Field<String>("Description").Trim();
                    
                    //Json Data
                    JObject jsonObject =
                        new JObject(
                            new JProperty("RatioNo", RatioNo),
                            new JProperty("Option", Option)
                        );
                    jsonarr.Add(jsonObject);
                }
            }

            //Json Main object
            JObject _jsonObject = new JObject(new JProperty("Data", jsonarr));

            return _jsonObject.ToString();
        }

        [WebMethod]
        public string GetAllRatio()
        {
            string getRatio = string.Format("select * from Ratio");
            DataTable dt = SqlHelper.ExecuteDataTable(SqlHelper.MSSQLConnection, CommandType.Text, getRatio);

            return RatioJson(dt);
        }

        #endregion

        private void CopyImage(string filename)
        {
            System.IO.File.Copy("C:\\tyg_webservice\\FactoryPic\\" + filename, "C:\\tyg_web\\FactoryPic\\" + filename, true);
        }

        private bool Base64ToImage(string dest, string base64)
        {
            try
            {
                if (base64.Length > 0)
                {
                    var bytes = Convert.FromBase64String(base64.Replace("data:image/jpeg;base64,", ""));
                    using (var imageFile = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                }
            }
            catch { return false; }

            return true;
        }        
    }
}
