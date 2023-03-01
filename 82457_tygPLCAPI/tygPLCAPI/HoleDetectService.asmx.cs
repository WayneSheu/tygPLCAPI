using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using Newtonsoft.Json.Linq;
using TYG.HoleDetect;

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
    public class HoleDetectService : System.Web.Services.WebService
    {
        private static DAQ daq = DAQCollection.GetInstance(true, true).DAQObject;

        #region Page Refresh

        [WebMethod]
        public string CheckAreaReflash(string lastReflash, string factoryID, string areaID)
        {
            string rslt = "false";
            Factory factory = null;
            Area area = null;
            try
            {
                //DateTime lastTime = DateTime.Parse(lastReflash);
                factory = daq.GetFactory(factoryID);
                if (factory != null)
                {
                    area = factory.GetArea(areaID);
                    if (area != null)
                    {
                        //if (area.RefreshTime > lastTime)
                        {
                            rslt = "true";
                        }
                    }
                }
            }
            catch { }
            return rslt;
        }

        [WebMethod]
        public string CheckPlatformReflash(string lastReflash, string factoryID, string areaID, string PlatformID)
        {
            string rslt = "false";
            Factory factory = null;
            Area area = null;
            Platform platform = null;
            try
            {
                DateTime lastTime = DateTime.Parse(lastReflash);
                factory = daq.GetFactory(factoryID);
                if (factory != null)
                {
                    area = factory.GetArea(areaID);
                    if (area != null)
                    {
                        platform = area.GetPlatform(PlatformID);
                        if (platform != null)
                        {
                            if (platform.RefreshTime > lastTime)
                            {
                                rslt = "true";
                            }
                        }
                    }
                }
            }
            catch { }
            return rslt;
        }

        [WebMethod]
        public string CheckPlatformReflash_New(string lastReflash, string factoryID, string areaID, string PlatformID)
        {
            string rslt = "false";
            Factory factory = null;
            Area area = null;
            Platform platform = null;
            try
            {
                DateTime lastTime = DateTime.Parse(lastReflash);
                factory = daq.GetFactory(factoryID);
                if (factory != null)
                {
                    area = factory.GetArea(areaID);
                    if (area != null)
                    {
                        platform = area.GetPlatform(PlatformID);
                        if (platform != null)
                        {
                            if (platform.RefreshTime > lastTime)
                            {
                                rslt = "true";
                            }
                        }
                    }
                }
            }
            catch { }
            return rslt;
        }

        [WebMethod]
        public string CheckMachineReflash(string lastReflash, string factoryID, string areaID, string machineID)
        {
            string rslt = "false";
            Factory factory = null;
            Area area = null;
            Machine machine = null;
            try
            {
                DateTime lastTime = DateTime.Parse(lastReflash);
                factory = daq.GetFactory(factoryID);
                if (factory != null)
                {
                    area = factory.GetArea(areaID);
                    if (area != null)
                    {
                        machine = area.GetMachine(machineID);
                        if (machine != null)
                        {
                            if (machine.RefreshTime > lastTime)
                            {
                                rslt = "true";
                            }
                        }
                    }
                }
            }
            catch { }
            return rslt;
        }

        #endregion

        #region Page Data

        [WebMethod]
        public string GetkAreaData(string factoryID, string areaID)
        {
            string rslt = "";
            Factory factory = null;
            Area area = null;

            try
            {
                factory = daq.GetFactory(factoryID);
                if (factory != null)
                {
                    area = factory.GetArea(areaID);
                    if (area != null)
                    {
                        JObject jsonObject =
                            new JObject(
                                new JProperty("ConnectionStauts", (area.Connected ? 1 : 0)),
                                new JProperty("ReflashTime", area.RefreshTime.ToString("yyyy/MM/dd HH:mm:ss"))
                            );
                        rslt = jsonObject.ToString();
                    }
                }
            }
            catch { }
            return rslt;
        }

        [WebMethod]
        public string GetPlatformData(string factoryID, string areaID, string platformID)
        {
            string rslt = "";
            Factory factory = null;
            Area area = null;
            Platform platform = null;
            MPile mpile;
            try
            {
                JArray holeArray = new JArray();
                JArray mpileArray = new JArray();
                factory = daq.GetFactory(factoryID);
                if (factory != null)
                {
                    area = factory.GetArea(areaID);
                    if (area != null)
                    {
                        platform = area.GetPlatform(platformID);
                        if (platform != null)
                        {
                            if (platform.ChannelDetection.Length > 1)
                            {
                                Machine machine;
                                for (int index = 1; index < platform.ChannelDetection.Length; index++)
                                {
                                    machine = platform.GetMachine(index);
                                    if (machine != null)
                                    {
                                        JObject holeObject =
                                            new JObject(
                                                new JProperty("MachineID", machine.ID),
                                                new JProperty("HoleID", (platform.ChannelDetection[index].Trim().Length == 0 ? "00" : platform.ChannelDetection[index])),
                                                new JProperty("MachineState", machine.RunStatus),
                                                new JProperty("LampState", machine.LampStatus)
                                            );
                                        holeArray.Add(holeObject);
                                    }
                                }
                            }

                            foreach (int mpilekey in platform.MPileDictKeys)
                            {
                                mpile = platform.GetMPile(mpilekey);
                                string pileSerial1 = "", pileSerial2 = "", pileSerial3 = "";
                                if (mpile.MaterialList.Count > 0)
                                    pileSerial1 = mpile.MaterialList[0];
                                if (mpile.MaterialList.Count > 1)
                                    pileSerial2 = mpile.MaterialList[1];
                                if (mpile.MaterialList.Count > 2)
                                    pileSerial3 = mpile.MaterialList[2];
                                JObject mpileObject =
                                        new JObject(
                                            new JProperty("PileSerial1", pileSerial1),
                                            new JProperty("PileSerial2", pileSerial2),
                                            new JProperty("PileSerial3", pileSerial3),
                                            new JProperty("MRatio", mpile.MaterialRatio)
                                        );
                                mpileArray.Add(mpileObject);
                            }

                            JObject jsonObject =
                                new JObject(
                                    new JProperty("ConnectionStauts", (platform.Connected ? 1 : 0)),
                                    new JProperty("VoltageState", (platform.ChannelDetection[0] == "MB" ? 1 : 0)),
                                    new JProperty("Holes", holeArray),
                                    new JProperty("MPiles", mpileArray),
                                    new JProperty("ReflashTime", platform.RefreshTime.ToString("yyyy/MM/dd HH:mm:ss"))
                                );
                            rslt = jsonObject.ToString();
                        }
                    }
                }

            }
            catch { }
            return rslt;
        }

        [WebMethod]
        public string GetPlatformData_New(string factoryID, string areaID, string platformID)
        {
            string rslt = "";
            Factory factory = null;
            Area area = null;
            Platform platform = null;
            MPile mpile;
            MHole mhole;
            try
            {
                JArray holeArray = new JArray();
                JArray mpileArray = new JArray();
                JArray mholeArray = new JArray();

                factory = daq.GetFactory(factoryID);
                if (factory != null)
                {
                    area = factory.GetArea(areaID);
                    if (area != null)
                    {
                        platform = area.GetPlatform(platformID);
                        if (platform != null)
                        {
                            if (platform.ChannelDetection.Length > 1)
                            {
                                Machine machine;
                                for (int index = 1; index < platform.ChannelDetection.Length; index++)
                                {
                                    machine = platform.GetMachine(index);
                                    if (machine != null)
                                    {
                                        JObject holeObject =
                                            new JObject(
                                                new JProperty("MachineID", machine.ID),
                                                new JProperty("HoleID", (platform.ChannelDetection[index].Trim().Length == 0 ? "00" : platform.ChannelDetection[index])),
                                                new JProperty("MachineState", machine.RunStatus),
                                                new JProperty("LampState", machine.LampStatus),
                                                new JProperty("MHoleID", machine.MHoleID)
                                            );
                                        holeArray.Add(holeObject);
                                    }
                                }
                            }

                            //foreach (int mpilekey in platform.MPileDictKeys)
                            //{
                            //    mpile = platform.GetMPile(mpilekey);
                            //    string pileSerial1 = "", pileSerial2 = "", pileSerial3 = "";
                            //    if (mpile.MaterialList.Count > 0)
                            //        pileSerial1 = mpile.MaterialList[0];
                            //    if (mpile.MaterialList.Count > 1)
                            //        pileSerial2 = mpile.MaterialList[1];
                            //    if (mpile.MaterialList.Count > 2)
                            //        pileSerial3 = mpile.MaterialList[2];
                            //    JObject mpileObject =
                            //            new JObject(
                            //                new JProperty("PileSerial1", pileSerial1),
                            //                new JProperty("PileSerial2", pileSerial2),
                            //                new JProperty("PileSerial3", pileSerial3),
                            //                new JProperty("MRatio", mpile.MaterialRatio)
                            //            );
                            //    mpileArray.Add(mpileObject);
                            //}

                            foreach (string mholekey in platform.MHoleDictKeys)
                            {
                                mhole = platform.GetMHole(mholekey);
                                string holeSerial1 = "", holeSerial2 = "", holeSerial3 = "";
                                if (mhole.MaterialList.Count > 0)
                                    holeSerial1 = mhole.MaterialList[0];
                                if (mhole.MaterialList.Count > 1)
                                    holeSerial2 = mhole.MaterialList[1];
                                if (mhole.MaterialList.Count > 2)
                                    holeSerial3 = mhole.MaterialList[2];
                                JObject mholeObject =
                                        new JObject(
                                            new JProperty("HoleSerial1", holeSerial1),
                                            new JProperty("HoleSerial2", holeSerial2),
                                            new JProperty("HoleSerial3", holeSerial3),
                                            new JProperty("MRatio", mhole.MaterialRatio)
                                        );
                                mholeArray.Add(mholeObject);
                            }

                            JObject jsonObject =
                                new JObject(
                                    new JProperty("ConnectionStatus", (platform.Connected ? 1 : 0)),
                                    new JProperty("VoltageState", (platform.ChannelDetection[0] == "MB" ? 1 : 0)),
                                    new JProperty("Holes", holeArray),
                                    new JProperty("MHoles", mholeArray),
                                    new JProperty("ReflashTime", platform.RefreshTime.ToString("yyyy/MM/dd HH:mm:ss"))
                                );
                            rslt = jsonObject.ToString();
                        }
                    }
                }

            }
            catch { }
            return rslt;
        }

        [WebMethod]
        public string GetMachineData(string factoryID, string areaID, string machineID)
        {
            string rslt = "";
            Factory factory = null;
            Area area = null;
            Machine machine = null;
            try
            {
                factory = daq.GetFactory(factoryID);
                if (factory != null)
                {
                    area = factory.GetArea(areaID);
                    if (area != null)
                    {
                        machine = area.GetMachine(machineID);
                        if (machine != null)
                        {
                            MPile mpile = machine.GetMPile();
                            string MPlileSerial1 = "";
                            string MPlileSerial2 = "";
                            string MPlileSerial3 = "";
                            string MaterialRatio = "";
                            if (mpile != null)
                            {
                                if (mpile.MaterialList.Count > 0)
                                    MPlileSerial1 = mpile.MaterialList[0];
                                if (mpile.MaterialList.Count > 1)
                                    MPlileSerial2 = mpile.MaterialList[1];
                                if (mpile.MaterialList.Count > 2)
                                    MPlileSerial3 = mpile.MaterialList[2];
                                MaterialRatio = mpile.MaterialRatio;
                            }
                            JObject productDetail = new JObject(
                                    new JProperty("ProductID", machine.ProductID),
                                    new JProperty("SpecMaterial", machine.SpecMaterial),
                                    new JProperty("OrderType", machine.OrderType),
                                    new JProperty("OrderStatus", machine.OrderStatus),
                                    new JProperty("OrderQty", machine.OrderQty),
                                    new JProperty("ProdQty", machine.ProdQty),
                                    new JProperty("Weight", machine.Weight),
                                    new JProperty("WeightUnit", machine.WeightUnit),
                                    new JProperty("MPlileSerial1", MPlileSerial1),
                                    new JProperty("MPlileSerial2", MPlileSerial2),
                                    new JProperty("MPlileSerial3", MPlileSerial3),
                                    new JProperty("MRatio", MaterialRatio)
                                );

                            JObject jsonObject =
                                new JObject(
                                    new JProperty("PlcConnectionStauts", (machine.PlcConnected ? 1 : 0)),
                                    new JProperty("IOConnectionStauts", (machine.IOConnected ? 1 : 0)),
                                    new JProperty("MachineState", machine.RunStatus),
                                    new JProperty("LampState", machine.LampStatus),
                                    new JProperty("HoleID", (machine.HoleID.Trim().Length == 0 ? "00" : machine.HoleID)),
                                    new JProperty("OrderID", machine.WorkOrderNo),
                                    new JProperty("ProductDetail", productDetail),
                                    new JProperty("ReflashTime", machine.RefreshTime.ToString("yyyy/MM/dd HH:mm:ss"))
                                );

                            rslt = jsonObject.ToString();
                        }
                    }
                }
            }
            catch { }
            return rslt;
        }

        [WebMethod]
        public string GetMachineData_New(string factoryID, string areaID, string machineID)
        {
            string rslt = "";
            Factory factory = null;
            Area area = null;
            Machine machine = null;
            try
            {
                factory = daq.GetFactory(factoryID);
                if (factory != null)
                {
                    area = factory.GetArea(areaID);
                    if (area != null)
                    {
                        machine = area.GetMachine(machineID);
                        if (machine != null)
                        {
                            MHole mhole = machine.GetMHole();
                            string MHoleSerial1 = "";
                            string MHoleSerial2 = "";
                            string MHoleSerial3 = "";
                            string MaterialRatio = "";
                            if (mhole != null)
                            {
                                if (mhole.MaterialList.Count > 0)
                                    MHoleSerial1 = mhole.MaterialList[0];
                                if (mhole.MaterialList.Count > 1)
                                    MHoleSerial2 = mhole.MaterialList[1];
                                if (mhole.MaterialList.Count > 2)
                                    MHoleSerial3 = mhole.MaterialList[2];
                                MaterialRatio = mhole.MaterialRatio;
                            }
                            JObject productDetail = new JObject(
                                    new JProperty("ProductID", machine.ProductID),
                                    new JProperty("SpecMaterial", machine.SpecMaterial),
                                    new JProperty("OrderType", machine.OrderType),
                                    new JProperty("OrderStatus", machine.OrderStatus),
                                    new JProperty("OrderQty", machine.OrderQty),
                                    new JProperty("ProdQty", machine.ProdQty),
                                    new JProperty("Weight", machine.Weight),
                                    new JProperty("WeightUnit", machine.WeightUnit),
                                    new JProperty("MHoleSerial1", MHoleSerial1),
                                    new JProperty("MHoleSerial2", MHoleSerial2),
                                    new JProperty("MHoleSerial3", MHoleSerial3),
                                    new JProperty("MRatio", MaterialRatio)
                                );

                            JObject jsonObject =
                                new JObject(
                                    new JProperty("PlcConnectionStauts", (machine.PlcConnected ? 1 : 0)),
                                    new JProperty("IOConnectionStauts", (machine.IOConnected ? 1 : 0)),
                                    new JProperty("MachineState", machine.RunStatus),
                                    new JProperty("LampState", machine.LampStatus),
                                    new JProperty("HoleID", (machine.HoleID.Trim().Length == 0 ? "00" : machine.HoleID)),
                                    new JProperty("OrderID", machine.WorkOrderNo),
                                    new JProperty("ProductDetail", productDetail),
                                    new JProperty("ReflashTime", machine.RefreshTime.ToString("yyyy/MM/dd HH:mm:ss")),
                                    new JProperty("MHoleID", machine.MHoleID)
                                );

                            rslt = jsonObject.ToString();
                        }
                    }
                }
            }
            catch { }
            return rslt;
        }
        #endregion

        #region Setting Data

        [WebMethod]
        public string SetMaterialData(string factoryID, string areaID, string platformID, int mpileSort, string[] mpileSerial, string mRatio)
        {
            Factory factory = null;
            Area area = null;
            Platform platform = null;
            string result = "false";
            string mpileSerial1 = "";
            string mpileSerial2 = "";
            string mpileSerial3 = "";

            try
            {
                factory = daq.GetFactory(factoryID);
                if (factory != null)
                {
                    area = factory.GetArea(areaID);
                    if (area != null)
                    {
                        platform = area.GetPlatform(platformID);
                        if (platform != null)
                        {
                            if (mpileSerial.Length > 0)
                                mpileSerial1 = mpileSerial[0];
                            if (mpileSerial.Length > 1)
                                mpileSerial2 = mpileSerial[1];
                            if (mpileSerial.Length > 2)
                                mpileSerial3 = mpileSerial[2];

                            if (platform.SetMPile(mpileSort, mpileSerial1, mpileSerial2, mpileSerial3, mRatio))
                                result = "true";
                        }
                    }
                }
            }
            catch { }
            return result;
        }

        [WebMethod]
        public string SetMaterialData_New(string factoryID, string areaID, string platformID, string mholeID, string[] mholeSerial, string mRatio)
        {
            Factory factory = null;
            Area area = null;
            Platform platform = null;
            string result = "false";
            string mholeSerial1 = "";
            string mholeSerial2 = "";
            string mholeSerial3 = "";

            try
            {
                factory = daq.GetFactory(factoryID);
                if (factory != null)
                {
                    area = factory.GetArea(areaID);
                    if (area != null)
                    {
                        platform = area.GetPlatform(platformID);
                        if (platform != null)
                        {
                            if (mholeSerial.Length > 0)
                                mholeSerial1 = mholeSerial[0];
                            if (mholeSerial.Length > 1)
                                mholeSerial2 = mholeSerial[1];
                            if (mholeSerial.Length > 2)
                                mholeSerial3 = mholeSerial[2];

                            if (platform.SetMHole(mholeID, mholeSerial1, mholeSerial2, mholeSerial3, mRatio))
                                result = "true";
                        }
                    }
                }
            }
            catch { }
            return result;
        }

        [WebMethod]
        public string UpdateMaterialData(string factoryID, string areaID, string platformID, string[] oldMpileSerial, string oldMRatio, string[] newMpileSerial, string newMRatio)
        {
            string result = "false";
            Factory factory = null;
            Area area = null;
            Platform platform = null;
            try
            {
                if (factoryID.Trim().Length > 0)
                {
                    factory = daq.GetFactory(factoryID);
                    if (factory != null)
                    {
                        if (areaID.Trim().Length == 0)
                        {
                            factory.UpdateMaterialData(oldMpileSerial, oldMRatio, newMpileSerial, newMRatio);
                            result = "true";
                        }
                        else
                        {
                            area = factory.GetArea(areaID);
                            if (area != null)
                            {
                                if (platformID.Trim().Length == 0)
                                {
                                    area.UpdateMaterialData(oldMpileSerial, oldMRatio, newMpileSerial, newMRatio);
                                    result = "true";
                                }
                                else
                                {
                                    platform = area.GetPlatform(platformID);
                                    if (platform != null)
                                    {
                                        platform.UpdateMaterialData(oldMpileSerial, oldMRatio, newMpileSerial, newMRatio);
                                        result = "true";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return result;
        }

        [WebMethod]
        public string SetTestMode(string factoryID, string areaID, int testMode)
        {
            string result = "false";
            Factory factory = null;
            Area area = null;
            try
            {
                factory = daq.GetFactory(factoryID);
                if (factory != null)
                {
                    area = factory.GetArea(areaID);
                    if (area != null)
                    {
                        area.TestMode = (testMode == 0 ? false : true);
                        result = "true";
                    }
                }
            }
            catch { }
            return result;
        }

        [WebMethod]
        public void RestartDAQ()
        {
        }


        #endregion

    }
}
