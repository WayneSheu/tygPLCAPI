using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace HoleDetector
{
    // 注意: 您可以使用 [重構] 功能表上的 [重新命名] 命令同時變更程式碼和組態檔中的介面名稱 "IHoleDetectorService"。
    [ServiceContract]
    public interface IHoleDetectService
    {
        [OperationContract]
        string WcfCheckAreaReflash(string lastReflash, string factoryID, string areaID);

        [OperationContract]
        string WcfCheckPlatfromReflash(string lastReflash, string factoryID, string areaID, string platfromID);

        [OperationContract]
        string WcfCheckMachineReflash(string lastReflash, string factoryID, string areaID, string machineID);

        [OperationContract]
        string WcfGetkAreaData(string factoryID, string areaID);

        [OperationContract]
        string WcfGetPlatfromData(string factoryID, string areaID, string platfromID);

        [OperationContract]
        string WcfGetMachineData(string factoryID, string areaID, string machineID);

    }
}
