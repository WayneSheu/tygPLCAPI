using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TYG.HoleDetect;

namespace MaterialErrorProofing
{
    public partial class FormDetection : Form
    {
        private BackgroundWorker myWorker = new BackgroundWorker();

        private delegate void delegateMachineSubItemModify(DataSetView.MachineRow paramMachineRow, Machine paramMachine);
        private delegate void delegateAreaSubItemModify(DataSetView.AreaRow paramAreaRow, Area paramArea);
        private delegate void delegatePlatformSubItemModify(DataSetView.PlatformRow paramPlatformRow, Platform paramPlatforme);
        private delegate void delegateRichTextBoxAdd(RichTextBox paramRichTextBox, string paramMessage);
        private delegateMachineSubItemModify methodMachineSubItemModify;
        private delegateAreaSubItemModify methodAreaSubItemModify;
        private delegatePlatformSubItemModify methodPlatformSubItemModify;
        private delegateRichTextBoxAdd methodRichTextBoxAdd;

        //private string xmlDirectory = ".\\XML\\";
        //private DataTable tbFactory;
        //private DataTable tbArea;
        //private DataTable tbPlatform;
        //private DataTable tbMachine;

        private DAQ daq;

        public FormDetection()
        {
            InitializeComponent();
            myWorker.DoWork += new DoWorkEventHandler(myWorker_DoWork);
            myWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(myWorker_RunWorkerCompleted);
            myWorker.ProgressChanged += new ProgressChangedEventHandler(myWorker_ProgressChanged);
            myWorker.WorkerReportsProgress = true;
            myWorker.WorkerSupportsCancellation = true;
            //myWorker.RunWorkerAsync();
        }
        protected void myWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (myWorker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                this.daq.Start();
            }
        }
        protected void myWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }
        protected void myWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }
            private void FormDetection_Load(object sender, EventArgs e)
        {
            this.dgvArea.Dock = DockStyle.Fill;
            this.dgvPlatform.Dock = DockStyle.Fill;
            this.dgvMachine.Dock = DockStyle.Fill;

            this.methodMachineSubItemModify = new delegateMachineSubItemModify(this.MachineSubItemModify);
            this.methodAreaSubItemModify = new delegateAreaSubItemModify(this.AreaSubItemModify);
            this.methodPlatformSubItemModify = new delegatePlatformSubItemModify(this.PlatformSubItemModify);
            this.methodRichTextBoxAdd = new delegateRichTextBoxAdd(this.RichTextBoxAdd);

            this.daq = DAQCollection.GetInstance(false, false).DAQObject;

            this.SetTreeView();
            this.daq.Area_ConnectChanged += Daq_ConnectChanged;
            this.daq.Platform_ConnectChanged += Daq_ConnectChanged;
            this.daq.MHole_ConnectChanged += Daq_ConnectChanged;
            this.daq.AlarmHappened += Daq_AlarmHappened;
            this.daq.Machine_DataChanged += Daq_Machine_DataChanged;
            this.daq.Machine_MaterialCheck += Daq_Machine_MaterialCheck;
            this.daq.Platform_HoleChanged += Daq_Platform_HoleChanged;
            this.daq.MHole_HoleChanged += Daq_MHole_HoleChanged;
            //this.daq.Start();
            if (!myWorker.IsBusy)
            {
                myWorker.RunWorkerAsync();
            }
        }

        private void Daq_Platform_HoleChanged(object Sender, HoleChangedArgument e)
        {
            string strMessage = "";
            Platform platform = (Platform)Sender;
            string[] holeIDs = platform.ChannelDetection;
            //strMessage = "Hole Changed (" + platform.Parent.Parent.ID + "." + platform.Parent.ID + "." + platform.ID + ") [";
            //for (int index=0; index < holeIDs.Length; index++)
            //{
            //    strMessage += holeIDs[index];
            //    if (index < holeIDs.Length - 1)
            //        strMessage += ",";
            //    else
            //        strMessage += "]";
            //}
            MHole mhole = (MHole)Sender;
            //string[] holeIDs = mhole.ChannelDetection;
            strMessage = "Hole Changed (" + mhole.Parent.Parent.Parent.ID + "." + mhole.Parent.Parent.ID + "." + mhole.Parent.ID + "." + mhole.ID + ") [";
            //for (int index = 0; index < holeIDs.Length; index++)
            //{
            //    strMessage += holeIDs[index];
            //    if (index < holeIDs.Length - 1)
            //        strMessage += ",";
            //    else
            //        strMessage += "]";
            //}
            this.Invoke(this.methodRichTextBoxAdd, new object[] { this.rtbHoleDetect, strMessage });
        }

        private void Daq_MHole_HoleChanged(object Sender, HoleChangedArgument e)
        {
            string strMessage = "";
            MHole mhole = (MHole)Sender;
            bool[] holeStatus = mhole.ChannelStatus;
            strMessage = "Hole Changed (" + mhole.Parent.Parent.Parent.ID + "." + mhole.Parent.Parent.ID + "." + mhole.Parent.ID + "." + mhole.ID + ") [";
            for (int index = 0; index < holeStatus.Length; index++)
            {
                strMessage += Convert.ToInt16(holeStatus[index]).ToString();
                if (index < holeStatus.Length - 1)
                    strMessage += ",";
                else
                    strMessage += "]";
            }
            this.Invoke(this.methodRichTextBoxAdd, new object[] { this.rtbHoleDetect, strMessage });
        }

        private void Daq_Machine_MaterialCheck(object Sender, MaterialCheckArgument e)
        {
            string strMessage = "";
            Machine machine = (Machine)Sender;
            DataRow row = null;
            strMessage = "Material Check (" + e.FactoryID + "." + e.AreaID + "." + e.PlatFormID + "." + e.MachineID + ")";

            if (e.ForbidMotor != false)
            {
                strMessage += "=> Motor Forbidden";
            }
            
            if ((this.dgvMachine.Visible) && (this.Contains(this.dsView.Machine, e.MachineID, ref row)))
                this.Invoke(this.methodMachineSubItemModify, new object[] { row, machine });

            this.Invoke(this.methodRichTextBoxAdd, new object[] { this.rtbDataChange, strMessage });

            //e.ForbidMotor = MaterialCheck(e.FlatFormID, e.WorkOrderNo, e.HoleID);
        }

        private void Daq_Machine_DataChanged(object Sender, DataChangedArgument e)
        {
            string strMessage = ""; 
            Machine machine = (Machine)Sender;
            DataRow row = null;
            strMessage = "Data Changed (" + e.FactoryID + "." + e.AreaID + "." + e.PlatFormID + "." + e.MachineID + ")";
            if ((this.dgvMachine.Visible) && (this.Contains(this.dsView.Machine, e.MachineID, ref row)))
                this.Invoke(this.methodMachineSubItemModify, new object[] { row, machine });

            this.Invoke(this.methodRichTextBoxAdd, new object[] { this.rtbDataChange, strMessage });
            //((Machine)Sender).

        }

        private void Daq_AlarmHappened(object Sender, ModbusAlarmHappenedArgument e)
        {

        }

        private void Daq_ConnectChanged(object Sender, ModbusConnectChangedArgument e)
        {
            string strMessage = "";
            Area area;
            Platform platform;
            MHole mhole;
            DataRow row = null;
            if (e.ModbusType == DeviceType.Area)
            {
                area = (Area)Sender;
                strMessage = "Connect Changed (" + e.IsConnected.ToString() + ") (" + area.Parent.ID + "." + area.ID + ")";
                if ((this.dgvArea.Visible) && (this.Contains(this.dsView.Area, area.ID, ref row)))
                {
                    if (area.Parent.Equals(this.dgvArea.Tag))
                        this.Invoke(this.methodAreaSubItemModify, new object[] { row, area }); 
                }
            }
            else if (e.ModbusType == DeviceType.Platform)
            {
                platform = (Platform)Sender;
                strMessage = "Connect Changed (" + e.IsConnected.ToString() + ") (" + platform.Parent.Parent.ID + "." + platform.Parent.ID + "." + platform.ID + ")";
                if ((this.dgvPlatform.Visible) && (this.Contains(this.dsView.Platform, platform.ID, ref row)))
                {
                    if (platform.Parent.Equals(this.dgvPlatform.Tag))
                        this.Invoke(this.methodPlatformSubItemModify, new object[] { row, platform });
                }
            }
            else if (e.ModbusType == DeviceType.MHole)
            {
                mhole = (MHole)Sender;
                strMessage = "Connect Changed (" + e.IsConnected.ToString() + ") (" + mhole.Parent.Parent.Parent.ID + "." + mhole.Parent.Parent.ID + "." +mhole.Parent.ID + "." + mhole.ID + "." + mhole.IPAddress + ")";
                //if ((this.dgvPlatform.Visible) && (this.Contains(this.dsView., platform.ID, ref row)))
                //{
                //    if (platform.Parent.Equals(this.dgvPlatform.Tag))
                //        this.Invoke(this.methodPlatformSubItemModify, new object[] { row, platform });
                //}
            }
            this.Invoke(this.methodRichTextBoxAdd, new object[] { this.rtbConnect, strMessage });
        }

        private bool Contains(DataTable paramTable, string paramName, ref DataRow paramDataRow)
        {
            bool bContain = false;
            foreach(DataRow row in paramTable.Rows)
            {
                if (row[0].ToString() == paramName)
                {
                    bContain = true;
                    paramDataRow = row;
                    break;
                }
            }
            return bContain;
        }

        private void RichTextBoxAdd(RichTextBox paramRichTextBox, string paramMessage)
        {
            if (paramRichTextBox.Lines.Length > 500)
                paramRichTextBox.Clear();
            if (paramRichTextBox.Text.Length > 0)
                paramRichTextBox.Text += Environment.NewLine;
            paramRichTextBox.Text += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff ") + "---> " + paramMessage;
            paramRichTextBox.SelectionStart = paramRichTextBox.Text.Length;
            paramRichTextBox.ScrollToCaret();
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            ////this.tbFactory = this.GetDataTableData("tbFactory.xml");
            ////this.tbArea = this.GetDataTableData("tbArea.xml");
            ////this.tbPlatform = this.GetDataTableData("tbPlatform.xml");
            ////this.tbMachine = this.GetDataTableData("tbMachine.xml");
            ////this.SetDAQ();
            //this.daq = DAQCollection.GetInstance().DAQObject;

            //this.SetTreeView();
            //this.daq.Area_ConnectChanged += Daq_ConnectChanged;
            //this.daq.Platform_ConnectChanged += Daq_ConnectChanged;
            //this.daq.AlarmHappened += Daq_AlarmHappened;
            //this.daq.Machine_DataChanged += Daq_Machine_DataChanged;
            //this.daq.Machine_MaterialCheck += Daq_Machine_MaterialCheck;
            //this.daq.Platform_HoleChanged += Daq_Platform_HoleChanged;

            ////this.btnStart.Enabled = true;
        }

        private void SetTreeView()
        {
            Factory factory;
            Area area;
            Platform platform;
            Machine machine;
            TreeNode Fnode,Anode,Pnode,Mnode, AnodeCollection,PnodeCollection, MnodeCollection;

            foreach (string Fkey in this.daq.FactoryKeys)
            {
                factory = this.daq.GetFactory(Fkey);
                Fnode = new TreeNode(factory.ID + "(" + factory.Name + ")");
                Fnode.Name = factory.ID;
                Fnode.Tag = "Factory";
                AnodeCollection = new TreeNode("Areas");
                AnodeCollection.Name = "Areas";
                AnodeCollection.Tag = factory.ID;
                foreach (string Akey in factory.AreaKeys)
                {
                    area = factory.GetArea(Akey);
                    Anode = new TreeNode(area.ID);
                    Anode.Name = area.ID;
                    Anode.Tag = "Area";
                    MnodeCollection = new TreeNode("Machines");
                    MnodeCollection.Name = "Machines";
                    MnodeCollection.Tag = factory.ID + "," + area.ID;
                    foreach (int Mkey in area.MachineKeys)
                    {
                        machine = area.GetMachine(Mkey);
                        Mnode = new TreeNode(machine.ID);
                        Mnode.Name = machine.ID;
                        Mnode.Tag = "Machine";
                        MnodeCollection.Nodes.Add(Mnode);
                    }
                    Anode.Nodes.Add(MnodeCollection);
                    PnodeCollection = new TreeNode("Platforms");
                    PnodeCollection.Name = "Platforms";
                    PnodeCollection.Tag = factory.ID + "," + area.ID;
                    foreach (string Pkey in area.PlatformKeys)
                    {
                        platform = area.GetPlatform(Pkey);
                        Pnode = new TreeNode(platform.ID);
                        Pnode.Name = platform.ID;
                        Pnode.Tag = "Platform";
                        MnodeCollection = new TreeNode("Machines");
                        MnodeCollection.Name = "Machines";
                        MnodeCollection.Tag = factory.ID + "," + area.ID + "," + platform.ID;
                        foreach (int Mkey in platform.MachineKeys)
                        {
                            machine = platform.GetMachine(Mkey);
                            Mnode = new TreeNode(machine.ID);
                            Mnode.Name = machine.ID;
                            Mnode.Tag = "Machine";
                            MnodeCollection.Nodes.Add(Mnode);
                        }
                        Pnode.Nodes.Add(MnodeCollection);
                        PnodeCollection.Nodes.Add(Pnode);
                    }
                    Anode.Nodes.Add(PnodeCollection);
                    AnodeCollection.Nodes.Add(Anode);
                }
                Fnode.Nodes.Add(AnodeCollection);
                this.trvDevice.Nodes.Add(Fnode);
            }
        }

        //private void SetDAQ()
        //{
        //    Factory factory;
        //    Area area;
           
        //    foreach (DataRow row in this.tbFactory.Rows)
        //    {
        //        this.daq.AddFactory((string)row["ID"], (string)row["Name"]);
        //    }
        //    foreach (DataRow row in this.tbArea.Rows)
        //    {
        //        factory = this.daq.GetFactory((string)row["FactoryID"]);
        //        factory.AddArea((string)row["ID"], (string)row["Name"], (string)row["IPAddress"], int.Parse((string)row["Port"]));
        //    }
        //    foreach (DataRow row in this.tbPlatform.Rows)
        //    {
        //        area = this.daq.GetFactory((string)row["FactoryID"]).GetArea((string)row["AreaID"]);
        //        area.AddPlatForm((string)row["ID"], (string)row["Name"], (string)row["IPAddress"], int.Parse((string)row["Port"]));
        //    }
        //    foreach (DataRow row in this.tbMachine.Rows)
        //    {
        //        area = this.daq.GetFactory((string)row["FactoryID"]).GetArea((string)row["AreaID"]);
        //        area.AddMachine((string)row["ID"], int.Parse((string)row["SlaveNo"]), (string)row["PlatformID"], int.Parse((string)row["ChannelNO"]), false);
        //    }
        //}

        //private DataTable GetDataTableData(string paramFileName)
        //{
        //    DataTable dataTable = null;
        //    DataSet dsTemp = new DataSet();
        //    string strFileName = xmlDirectory + paramFileName;
        //    dsTemp.ReadXml(strFileName);
        //    dataTable = dsTemp.Tables[0].Copy();

        //    return dataTable;
        //}

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.btnStart.Enabled = false;
            this.daq.Start();
            this.btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.btnStop.Enabled = false;
            this.daq.Stop();
            this.btnStart.Enabled = true;
        }

        private void trvDevice_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {

            if (this.trvDevice.SelectedNode == null)
                return;
            if (this.trvDevice.SelectedNode.Name == "Areas")
            {
                this.dgvArea.Visible = false;
                this.dsView.Area.Clear();
            }
            else if (this.trvDevice.SelectedNode.Name == "Platforms")
            {
                this.dgvPlatform.Visible = false;
                this.dsView.Platform.Clear();
            }
            else if (this.trvDevice.SelectedNode.Name == "Machines")
            {
                this.dgvMachine.Visible = false;
                this.dsView.Machine.Clear();
            }
        }

        private void trvDevice_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name == "Areas")
            {
                this.SetListViewAreas(e.Node);
                this.dgvArea.Visible = true;
            }
            else if (e.Node.Name == "Platforms")
            {
                this.SetListViewPlatforms(e.Node);
                this.dgvPlatform.Visible = true;
            }
            else if (e.Node.Name == "Machines")
            {
                this.SetListViewMachines(e.Node);
                this.dgvMachine.Visible = true;
            }
        }

        private void SetListViewAreas(TreeNode node)
        {
            Factory factory = this.daq.GetFactory(node.Tag.ToString());
            Area area;
            DataSetView.AreaRow row; 
            this.dsView.Area.Clear();
            this.dgvArea.Tag = factory;
            foreach (string Akey in factory.AreaKeys)
            {
                area = factory.GetArea(Akey);
                row = this.dsView.Area.NewAreaRow();
                row.AreaID = area.ID;
                row.AreaName = area.Name;
                row.MasterPlcIP = area.IPAddress;
                row.MasterPlcPort = area.Port;
                this.AreaSubItemModify(row, area);
                this.dsView.Area.Rows.Add(row);
            }
        }

        private void AreaSubItemModify(DataSetView.AreaRow paramAreaRow, Area paramArea)
        {
            paramAreaRow.MasterPlcConnected = paramArea.Connected;
            if (paramArea.ConnectedTime != null)
                paramAreaRow.PlcConnectedTime = paramArea.ConnectedTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
            else
                paramAreaRow.PlcConnectedTime = "";
            if (paramArea.DisconnectedTime != null)
                paramAreaRow.PlcDisconnectedTime = paramArea.DisconnectedTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
            else
                paramAreaRow.PlcDisconnectedTime = "";
        }

        private void SetListViewPlatforms(TreeNode node)
        {
            string[] path = node.Tag.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Area area = this.daq.GetFactory(path[0]).GetArea(path[1]);
            Platform platform;
            DataSetView.PlatformRow row;
            this.dsView.Platform.Clear();
            this.dgvPlatform.Tag = area;
            foreach (string Pkey in area.PlatformKeys)
            {
                platform = area.GetPlatform(Pkey);
                row = this.dsView.Platform.NewPlatformRow();
                row.PlatformID = platform.ID;
                row.PlatformName = platform.Name;
                row.RemoteIOIP = platform.IPAddress;
                row.RemoteIOPort = platform.Port;
                this.PlatformSubItemModify(row, platform);
                this.dsView.Platform.Rows.Add(row);
            }
        }

        private void PlatformSubItemModify(DataSetView.PlatformRow paramPlatformRow, Platform paramPlatform)
        {
            paramPlatformRow.RemoteIOConnected = paramPlatform.Connected;
            if (paramPlatform.ConnectedTime != null)
                paramPlatformRow.IOConnectedTime = paramPlatform.ConnectedTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
            else
                paramPlatformRow.IOConnectedTime = "";
            if (paramPlatform.DisconnectedTime != null)
                paramPlatformRow.IODisconnectedTime = paramPlatform.DisconnectedTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
            else
                paramPlatformRow.IODisconnectedTime = "";
        }


        private void SetListViewMachines(TreeNode node)
        {
            string[] path = node.Tag.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Area area = this.daq.GetFactory(path[0]).GetArea(path[1]);
            DataSetView.MachineRow row;
            Platform platform;
            Machine machine;
            this.dsView.Machine.Clear();
            if (path.Length > 2)
            {
                platform = area.GetPlatform(path[2]);
                this.dgvMachine.Tag = platform;
                foreach (int Mkey in platform.MachineKeys)
                {
                    machine = platform.GetMachine(Mkey);
                    row = this.dsView.Machine.NewMachineRow();
                    row.MachineID = machine.ID;
                    row.PlcSlaveNo = machine.PlcSlaveNo;
                    row.PlatformID = machine.PlatFormID;
                    row.IOChannelNo = machine.IOChannelNo;
                    this.MachineSubItemModify(row, machine);
                    this.dsView.Machine.Rows.Add(row);
                }
            }
            else
            {
                this.dgvMachine.Tag = area;
                foreach (int Mkey in area.MachineKeys)
                {
                    machine = area.GetMachine(Mkey);
                    row = this.dsView.Machine.NewMachineRow();
                    row.MachineID = machine.ID;
                    row.PlcSlaveNo = machine.PlcSlaveNo;
                    row.PlatformID = machine.PlatFormID;
                    row.IOChannelNo = machine.IOChannelNo;
                    this.MachineSubItemModify(row, machine);
                    this.dsView.Machine.Rows.Add(row);
                }
            }
        }

        private void MachineSubItemModify(DataSetView.MachineRow paramMachineRow, Machine paramMachine)
        {
            paramMachineRow.PlcConnected = paramMachine.PlcConnected;
            paramMachineRow.IOConnected = paramMachine.IOConnected;
            paramMachineRow.RunStatus = paramMachine.RunStatus;
            paramMachineRow.ForbidMotor = paramMachine.ForbidMotor;
            paramMachineRow.LampStatus = paramMachine.LampStatus;
            paramMachineRow.WorkOrderNo = paramMachine.WorkOrderNo;
            paramMachineRow.ProdQty = paramMachine.ProdQty;
            paramMachineRow.HoleID = paramMachine.HoleID;
        }

        private void btnClearMessage_Click(object sender, EventArgs e)
        {
            this.rtbConnect.Text = "";
            this.rtbDataChange.Text = "";
            this.rtbHoleDetect.Text = "";
        }
    }
}
