//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using TYG.HoleDetect;
//using System.Data;

//namespace tygPLCAPI
//{
//    public class _Daq
//    {
//        public void daqinit(DAQ daq)
//        {
//            daq.Area_ConnectChanged += Daq_ConnectChanged;
//            daq.Platform_ConnectChanged += Daq_ConnectChanged;
//            daq.AlarmHappened += Daq_AlarmHappened;
//            daq.Machine_DataChanged += Daq_Machine_DataChanged;
//            daq.Machine_MaterialCheck += Daq_Machine_MaterialCheck;
//            daq.Platform_HoleChanged += Daq_Platform_HoleChanged;
//        }

//        private void Daq_Platform_HoleChanged(object Sender, HoleChangedArgument e)
//        {
//            string strMessage = "";
//            Platform platform = (Platform)Sender;
//            string[] holeIDs = platform.ChannelDetection;
//            strMessage = "Hole Changed (" + platform.Parent.Parent.ID + "." + platform.Parent.ID + "." + platform.ID + ") [";
//            for (int index = 0; index < holeIDs.Length; index++)
//            {
//                strMessage += holeIDs[index];
//                if (index < holeIDs.Length - 1)
//                    strMessage += ",";
//                else
//                    strMessage += "]";
//            }
//            //this.Invoke(this.methodRichTextBoxAdd, new object[] { this.rtbHoleDetect, strMessage });
//        }

//        private void Daq_Machine_MaterialCheck(object Sender, MaterialCheckArgument e)
//        {
//            string strMessage = "";
//            Machine machine = (Machine)Sender;
//            DataRow row = null;
//            strMessage = "Material Check (" + e.FactoryID + "." + e.AreaID + "." + e.PlatFormID + "." + e.MachineID + ")";
//            //if ((this.dgvMachine.Visible) && (this.Contains(this.dsView.Machine, e.MachineID, ref row)))
//            //    this.Invoke(this.methodMachineSubItemModify, new object[] { row, machine });

//            //this.Invoke(this.methodRichTextBoxAdd, new object[] { this.rtbDataChange, strMessage });

//            //e.ForbidMotor = MaterialCheck(e.FlatFormID, e.WorkOrderNo, e.HoleID);
//        }

//        private void Daq_Machine_DataChanged(object Sender, DataChangedArgument e)
//        {
//            string strMessage = "";
//            Machine machine = (Machine)Sender;
//            DataRow row = null;
//            strMessage = "Data Changed (" + e.FactoryID + "." + e.AreaID + "." + e.PlatFormID + "." + e.MachineID + ")";
//            //if ((this.dgvMachine.Visible) && (this.Contains(this.dsView.Machine, e.MachineID, ref row)))
//            //    this.Invoke(this.methodMachineSubItemModify, new object[] { row, machine });

//            //this.Invoke(this.methodRichTextBoxAdd, new object[] { this.rtbDataChange, strMessage });
//            //((Machine)Sender).

//        }

//        private void Daq_AlarmHappened(object Sender, ModbusAlarmHappenedArgument e)
//        {

//        }

//        private void Daq_ConnectChanged(object Sender, ModbusConnectChangedArgument e)
//        {
//            string strMessage = "";
//            Area area;
//            Platform platform;
//            DataRow row = null;
//            if (e.ModbusType == DeviceType.Area)
//            {
//                area = (Area)Sender;
//                strMessage = "Connect Changed (" + e.IsConnected.ToString() + ") (" + area.Parent.ID + "." + area.ID + ")";
//                //if ((this.dgvArea.Visible) && (this.Contains(this.dsView.Area, area.ID, ref row)))
//                //{
//                //    if (area.Parent.Equals(this.dgvArea.Tag))
//                //        //this.Invoke(this.methodAreaSubItemModify, new object[] { row, area });
//                //}
//            }
//            else if (e.ModbusType == DeviceType.Platform)
//            {
//                platform = (Platform)Sender;
//                strMessage = "Connect Changed (" + e.IsConnected.ToString() + ") (" + platform.Parent.Parent.ID + "." + platform.Parent.ID + "." + platform.ID + ")";
//                //if ((this.dgvPlatform.Visible) && (this.Contains(this.dsView.Platform, platform.ID, ref row)))
//                //{
//                //    if (platform.Parent.Equals(this.dgvPlatform.Tag))
//                //        //this.Invoke(this.methodPlatformSubItemModify, new object[] { row, platform });
//                //}
//            }
//            //this.Invoke(this.methodRichTextBoxAdd, new object[] { this.rtbConnect, strMessage });
//        }

//    }
//}