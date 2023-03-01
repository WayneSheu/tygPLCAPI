using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Newtonsoft.Json.Linq;
using TYG.HoleDetect;

namespace HoleDetector
{
    // 注意: 您可以使用 [重構] 功能表上的 [重新命名] 命令同時變更程式碼和組態檔中的類別名稱 "HoleDetectService"。
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class HoleDetectService : IHoleDetectService
    {
        DAQ daq = DAQCollection.GetInstance().DAQObject;

        public string WcfCheckAreaReflash(string lastReflash, string factoryID, string areaID)
        {
            string rslt = "false";
            Area area;
            try
            {
                DateTime lastTime = DateTime.Parse(lastReflash);
                area = daq.GetFactory(factoryID).GetArea(areaID);
                if (area.RefreshTime > lastTime)
                {
                    rslt = "true";
                }
            }
            catch { }
            return rslt;
        }

        public string WcfCheckPlatfromReflash(string lastReflash, string factoryID, string areaID, string platfromID)
        {
            string rslt = "false";
            Platform platform;
            try
            {
                DateTime lastTime = DateTime.Parse(lastReflash);
                platform = daq.GetFactory(factoryID).GetArea(areaID).GetPlatform(platfromID);
                if (platform.RefreshTime > lastTime)
                {
                    rslt = "true";
                }
            }
            catch { }
            return rslt;
        }

        public string WcfCheckMachineReflash(string lastReflash, string factoryID, string areaID, string machineID)
        {
            string rslt = "false";
            Machine machine;
            try
            {
                DateTime lastTime = DateTime.Parse(lastReflash);
                machine = daq.GetFactory(factoryID).GetArea(areaID).GetMachine(machineID);
                if (machine.RefreshTime > lastTime)
                {
                    rslt = "true";
                }
            }
            catch { }
            return rslt;
        }

        public string WcfGetkAreaData(string factoryID, string areaID)
        {
            string rslt = "";
            Area area;

            try
            {
                area = daq.GetFactory(factoryID).GetArea(areaID);
                JObject jsonObject =
                    new JObject(
                        new JProperty("ConnectionStauts", (area.Connected ? 1 : 0)),
                        new JProperty("ReflashTime", area.RefreshTime.ToString("yyyy/MM/dd HH:mm:ss"))
                    );
                rslt = jsonObject.ToString();
            }
            catch { }
            return rslt;
        }

        public string WcfGetPlatfromData(string factoryID, string areaID, string platfromID)
        {
            string rslt = "";
            Platform platform;

            try
            {
                JArray holeArray = new JArray();

                platform = daq.GetFactory(factoryID).GetArea(areaID).GetPlatform(platfromID);
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
                                    new JProperty("HoleID", (platform.ChannelDetection[index].Trim().Length == 0 ? "00" : platform.ChannelDetection[index]))
                                );
                            holeArray.Add(holeObject);
                        }
                    }
                }

                JObject jsonObject =
                    new JObject(
                        new JProperty("ConnectionStauts", (platform.Connected ? 1 : 0)),
                        new JProperty("VoltageState", (platform.ChannelDetection[0] == "MB" ? 1 : 0)),
                        new JProperty("Holes", holeArray.ToString()),
                        new JProperty("ReflashTime", platform.RefreshTime.ToString("yyyy/MM/dd HH:mm:ss"))
                    );
                rslt = jsonObject.ToString();
            }
            catch { }
            return rslt;
        }

        public string WcfGetMachineData(string factoryID, string areaID, string machineID)
        {
            string rslt = "";
            Machine machine;
            try
            {
                machine = daq.GetFactory(factoryID).GetArea(areaID).GetMachine(machineID);

                JObject productDetail = new JObject(
                        new JProperty("ProductID", ""),
                        new JProperty("ProductName", ""),
                        new JProperty("WAUORG", 0),
                        new JProperty("WADCTO", ""),
                        new JProperty("Quantity", machine.ProdQty),
                        new JProperty("MPlileSerial1", ""),
                        new JProperty("MPlileSerial2", ""),
                        new JProperty("MPlileSerial3", "")
                    );

                JObject jsonObject =
                    new JObject(
                        new JProperty("PlcConnectionStauts", (machine.PlcConnected ? 1 : 0)),
                        new JProperty("IOConnectionStauts", (machine.IOConnected ? 1 : 0)),
                        new JProperty("MachineState", machine.RunStatus),
                        new JProperty("LampState", machine.LampStatus),
                        new JProperty("HoleID", (machine.HoleID.Trim().Length == 0 ? "00" : machine.HoleID)),
                        new JProperty("OrderID", machine.WorkOrderNo),
                        new JProperty("ProductDetail", productDetail.ToString()),
                        new JProperty("ReflashTime", machine.RefreshTime.ToString("yyyy/MM/dd HH:mm:ss"))
                    );

                rslt = jsonObject.ToString();
            }
            catch { }
            return rslt;
        }
    }
}
