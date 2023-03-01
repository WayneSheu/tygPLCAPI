namespace MaterialErrorProofing
{
    partial class FormDetection
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClearMessage = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rtbDataChange = new System.Windows.Forms.RichTextBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rtbHoleDetect = new System.Windows.Forms.RichTextBox();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.panel4 = new System.Windows.Forms.Panel();
            this.rtbConnect = new System.Windows.Forms.RichTextBox();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.panel5 = new System.Windows.Forms.Panel();
            this.trvDevice = new System.Windows.Forms.TreeView();
            this.splitter5 = new System.Windows.Forms.Splitter();
            this.panel6 = new System.Windows.Forms.Panel();
            this.dgvMachine = new System.Windows.Forms.DataGridView();
            this.dgvPlatform = new System.Windows.Forms.DataGridView();
            this.dgvArea = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMachine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlatform)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArea)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.btnClearMessage);
            this.panel1.Controls.Add(this.btnStop);
            this.panel1.Controls.Add(this.btnStart);
            this.panel1.Controls.Add(this.btnLoad);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1154, 66);
            this.panel1.TabIndex = 0;
            // 
            // btnClearMessage
            // 
            this.btnClearMessage.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnClearMessage.Location = new System.Drawing.Point(32, 21);
            this.btnClearMessage.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnClearMessage.Name = "btnClearMessage";
            this.btnClearMessage.Size = new System.Drawing.Size(120, 30);
            this.btnClearMessage.TabIndex = 3;
            this.btnClearMessage.Text = "Clear Message";
            this.btnClearMessage.UseVisualStyleBackColor = true;
            this.btnClearMessage.Click += new System.EventHandler(this.btnClearMessage_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnStop.Location = new System.Drawing.Point(980, 21);
            this.btnStop.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(90, 30);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnStart.Location = new System.Drawing.Point(834, 21);
            this.btnStart.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(90, 30);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLoad.Location = new System.Drawing.Point(665, 21);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(90, 30);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 66);
            this.splitter1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1154, 3);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.rtbDataChange);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 551);
            this.panel2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1154, 120);
            this.panel2.TabIndex = 2;
            // 
            // rtbDataChange
            // 
            this.rtbDataChange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbDataChange.Location = new System.Drawing.Point(0, 0);
            this.rtbDataChange.Name = "rtbDataChange";
            this.rtbDataChange.Size = new System.Drawing.Size(1150, 116);
            this.rtbDataChange.TabIndex = 1;
            this.rtbDataChange.Text = "";
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 548);
            this.splitter2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(1154, 3);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.rtbHoleDetect);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 428);
            this.panel3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1154, 120);
            this.panel3.TabIndex = 4;
            // 
            // rtbHoleDetect
            // 
            this.rtbHoleDetect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbHoleDetect.Location = new System.Drawing.Point(0, 0);
            this.rtbHoleDetect.Name = "rtbHoleDetect";
            this.rtbHoleDetect.Size = new System.Drawing.Size(1150, 116);
            this.rtbHoleDetect.TabIndex = 1;
            this.rtbHoleDetect.Text = "";
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter3.Location = new System.Drawing.Point(0, 425);
            this.splitter3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(1154, 3);
            this.splitter3.TabIndex = 5;
            this.splitter3.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.rtbConnect);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 305);
            this.panel4.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1154, 120);
            this.panel4.TabIndex = 6;
            // 
            // rtbConnect
            // 
            this.rtbConnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbConnect.Location = new System.Drawing.Point(0, 0);
            this.rtbConnect.Name = "rtbConnect";
            this.rtbConnect.Size = new System.Drawing.Size(1150, 116);
            this.rtbConnect.TabIndex = 0;
            this.rtbConnect.Text = "";
            // 
            // splitter4
            // 
            this.splitter4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter4.Location = new System.Drawing.Point(0, 302);
            this.splitter4.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(1154, 3);
            this.splitter4.TabIndex = 7;
            this.splitter4.TabStop = false;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.trvDevice);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 69);
            this.panel5.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(292, 233);
            this.panel5.TabIndex = 8;
            // 
            // trvDevice
            // 
            this.trvDevice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvDevice.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.trvDevice.Location = new System.Drawing.Point(0, 0);
            this.trvDevice.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.trvDevice.Name = "trvDevice";
            this.trvDevice.Size = new System.Drawing.Size(292, 233);
            this.trvDevice.TabIndex = 0;
            this.trvDevice.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.trvDevice_BeforeSelect);
            this.trvDevice.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvDevice_AfterSelect);
            // 
            // splitter5
            // 
            this.splitter5.Location = new System.Drawing.Point(292, 69);
            this.splitter5.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.splitter5.Name = "splitter5";
            this.splitter5.Size = new System.Drawing.Size(3, 233);
            this.splitter5.TabIndex = 9;
            this.splitter5.TabStop = false;
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel6.Controls.Add(this.dgvMachine);
            this.panel6.Controls.Add(this.dgvPlatform);
            this.panel6.Controls.Add(this.dgvArea);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(295, 69);
            this.panel6.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(859, 233);
            this.panel6.TabIndex = 10;
            // 
            // dgvMachine
            // 
            this.dgvMachine.AllowUserToAddRows = false;
            this.dgvMachine.AllowUserToDeleteRows = false;
            this.dgvMachine.AllowUserToOrderColumns = true;
            this.dgvMachine.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMachine.Enabled = false;
            this.dgvMachine.Location = new System.Drawing.Point(567, 39);
            this.dgvMachine.Name = "dgvMachine";
            this.dgvMachine.ReadOnly = true;
            this.dgvMachine.RowHeadersVisible = false;
            this.dgvMachine.RowTemplate.Height = 24;
            this.dgvMachine.Size = new System.Drawing.Size(209, 167);
            this.dgvMachine.TabIndex = 2;
            this.dgvMachine.Visible = false;
            // 
            // dgvPlatform
            // 
            this.dgvPlatform.AllowUserToAddRows = false;
            this.dgvPlatform.AllowUserToDeleteRows = false;
            this.dgvPlatform.AllowUserToOrderColumns = true;
            this.dgvPlatform.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPlatform.Enabled = false;
            this.dgvPlatform.Location = new System.Drawing.Point(323, 31);
            this.dgvPlatform.Name = "dgvPlatform";
            this.dgvPlatform.ReadOnly = true;
            this.dgvPlatform.RowHeadersVisible = false;
            this.dgvPlatform.RowTemplate.Height = 24;
            this.dgvPlatform.Size = new System.Drawing.Size(209, 167);
            this.dgvPlatform.TabIndex = 1;
            this.dgvPlatform.Visible = false;
            // 
            // dgvArea
            // 
            this.dgvArea.AllowUserToAddRows = false;
            this.dgvArea.AllowUserToDeleteRows = false;
            this.dgvArea.AllowUserToOrderColumns = true;
            this.dgvArea.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvArea.Enabled = false;
            this.dgvArea.Location = new System.Drawing.Point(0, 0);
            this.dgvArea.Name = "dgvArea";
            this.dgvArea.ReadOnly = true;
            this.dgvArea.RowHeadersVisible = false;
            this.dgvArea.RowTemplate.Height = 24;
            this.dgvArea.Size = new System.Drawing.Size(209, 167);
            this.dgvArea.TabIndex = 0;
            this.dgvArea.Visible = false;
            // 
            // FormDetection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 671);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.splitter5);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.splitter4);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.splitter3);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "FormDetection";
            this.Text = "MaterialErrorProofing 0.9.1";
            this.Load += new System.EventHandler(this.FormDetection_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMachine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlatform)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArea)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Splitter splitter4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Splitter splitter5;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TreeView trvDevice;
        private System.Windows.Forms.RichTextBox rtbDataChange;
        private System.Windows.Forms.RichTextBox rtbHoleDetect;
        private System.Windows.Forms.RichTextBox rtbConnect;
        private System.Windows.Forms.Button btnClearMessage;
        private System.Windows.Forms.DataGridView dgvArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn areaIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn areaNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn masterPlcIPDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn masterPlcPortDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn masterPlcConnectedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn plcConnectedTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn plcDisconnectedTimeDataGridViewTextBoxColumn;
        private DataSetView dsView;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.DataGridView dgvMachine;
        private System.Windows.Forms.DataGridView dgvPlatform;
        private System.Windows.Forms.DataGridView dgvMHole;
        private System.Windows.Forms.DataGridViewTextBoxColumn mholeIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mholeNameDataGridViewTextBoxColumn;

        private System.Windows.Forms.DataGridViewTextBoxColumn platformIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn platformNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn remoteIOIPDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn remoteIOPortDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn remoteIOConnectedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iOConnectedTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iODisconnectedTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn machineIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn plcSlaveNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn platformIDDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn iOChannelNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn plcConnectedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn iOConnectedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn runStatusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn forbidMotorDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lampStatusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn workOrderNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn prodQtyDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn holeIDDataGridViewTextBoxColumn;
    }
}

