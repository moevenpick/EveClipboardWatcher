namespace EveClipboardWatcher
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.tsbtLogin = new System.Windows.Forms.ToolStripButton();
            this.btChangeShipType = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btOptions = new System.Windows.Forms.ToolStripButton();
            this.btAbout = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.dataGridViewMain = new System.Windows.Forms.DataGridView();
            this.m_icon = new System.Windows.Forms.DataGridViewImageColumn();
            this.m_nameType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_win_loss = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_pilots = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridViewLocal = new System.Windows.Forms.DataGridView();
            this.m_locIcon = new System.Windows.Forms.DataGridViewImageColumn();
            this.m_locName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_locWinLoss = new System.Windows.Forms.DataGridViewLinkColumn();
            this.m_locStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLocal)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtLogin,
            this.btChangeShipType,
            this.btRefresh,
            this.toolStripSeparator1,
            this.btOptions,
            this.btAbout});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1223, 25);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip1";
            // 
            // tsbtLogin
            // 
            this.tsbtLogin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbtLogin.Image = ((System.Drawing.Image)(resources.GetObject("tsbtLogin.Image")));
            this.tsbtLogin.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtLogin.Name = "tsbtLogin";
            this.tsbtLogin.Size = new System.Drawing.Size(23, 22);
            this.tsbtLogin.Text = "Login";
            this.tsbtLogin.Click += new System.EventHandler(this.tsbtLogin_Click);
            // 
            // btChangeShipType
            // 
            this.btChangeShipType.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btChangeShipType.Image = ((System.Drawing.Image)(resources.GetObject("btChangeShipType.Image")));
            this.btChangeShipType.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btChangeShipType.Name = "btChangeShipType";
            this.btChangeShipType.Size = new System.Drawing.Size(23, 22);
            this.btChangeShipType.Text = "chage ship type";
            this.btChangeShipType.Click += new System.EventHandler(this.btChangeShipType_Click_1);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btOptions
            // 
            this.btOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btOptions.Image = ((System.Drawing.Image)(resources.GetObject("btOptions.Image")));
            this.btOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btOptions.Name = "btOptions";
            this.btOptions.Size = new System.Drawing.Size(23, 22);
            this.btOptions.Text = "Options";
            this.btOptions.Click += new System.EventHandler(this.btOptions_Click);
            // 
            // btAbout
            // 
            this.btAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btAbout.Image = ((System.Drawing.Image)(resources.GetObject("btAbout.Image")));
            this.btAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btAbout.Name = "btAbout";
            this.btAbout.Size = new System.Drawing.Size(23, 22);
            this.btAbout.Text = "About";
            this.btAbout.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 579);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1223, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel.Text = "Status";
            // 
            // dataGridViewMain
            // 
            this.dataGridViewMain.AllowUserToAddRows = false;
            this.dataGridViewMain.AllowUserToDeleteRows = false;
            this.dataGridViewMain.AllowUserToOrderColumns = true;
            this.dataGridViewMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMain.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.m_icon,
            this.m_nameType,
            this.m_category,
            this.m_win_loss,
            this.m_pilots,
            this.m_status});
            this.dataGridViewMain.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewMain.MultiSelect = false;
            this.dataGridViewMain.Name = "dataGridViewMain";
            this.dataGridViewMain.ReadOnly = true;
            this.dataGridViewMain.RowHeadersVisible = false;
            this.dataGridViewMain.Size = new System.Drawing.Size(720, 542);
            this.dataGridViewMain.TabIndex = 4;
            // 
            // m_icon
            // 
            this.m_icon.HeaderText = "icon";
            this.m_icon.Name = "m_icon";
            this.m_icon.ReadOnly = true;
            this.m_icon.Width = 64;
            // 
            // m_nameType
            // 
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.m_nameType.DefaultCellStyle = dataGridViewCellStyle1;
            this.m_nameType.HeaderText = "name/type";
            this.m_nameType.Name = "m_nameType";
            this.m_nameType.ReadOnly = true;
            this.m_nameType.Width = 170;
            // 
            // m_category
            // 
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.m_category.DefaultCellStyle = dataGridViewCellStyle2;
            this.m_category.HeaderText = "category";
            this.m_category.Name = "m_category";
            this.m_category.ReadOnly = true;
            // 
            // m_win_loss
            // 
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.m_win_loss.DefaultCellStyle = dataGridViewCellStyle3;
            this.m_win_loss.HeaderText = "win/loss";
            this.m_win_loss.Name = "m_win_loss";
            this.m_win_loss.ReadOnly = true;
            // 
            // m_pilots
            // 
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.m_pilots.DefaultCellStyle = dataGridViewCellStyle4;
            this.m_pilots.HeaderText = "pilots";
            this.m_pilots.Name = "m_pilots";
            this.m_pilots.ReadOnly = true;
            this.m_pilots.Width = 180;
            // 
            // m_status
            // 
            this.m_status.HeaderText = "status";
            this.m_status.Name = "m_status";
            this.m_status.ReadOnly = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridViewMain);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewLocal);
            this.splitContainer1.Size = new System.Drawing.Size(1199, 548);
            this.splitContainer1.SplitterDistance = 726;
            this.splitContainer1.TabIndex = 5;
            // 
            // dataGridViewLocal
            // 
            this.dataGridViewLocal.AllowUserToAddRows = false;
            this.dataGridViewLocal.AllowUserToDeleteRows = false;
            this.dataGridViewLocal.AllowUserToOrderColumns = true;
            this.dataGridViewLocal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewLocal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLocal.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.m_locIcon,
            this.m_locName,
            this.m_locWinLoss,
            this.m_locStatus});
            this.dataGridViewLocal.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewLocal.MultiSelect = false;
            this.dataGridViewLocal.Name = "dataGridViewLocal";
            this.dataGridViewLocal.ReadOnly = true;
            this.dataGridViewLocal.RowHeadersVisible = false;
            this.dataGridViewLocal.Size = new System.Drawing.Size(463, 545);
            this.dataGridViewLocal.TabIndex = 0;
            this.dataGridViewLocal.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewLocal_CellClick);
            // 
            // m_locIcon
            // 
            this.m_locIcon.HeaderText = "icon";
            this.m_locIcon.Name = "m_locIcon";
            this.m_locIcon.ReadOnly = true;
            this.m_locIcon.Width = 64;
            // 
            // m_locName
            // 
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.m_locName.DefaultCellStyle = dataGridViewCellStyle5;
            this.m_locName.HeaderText = "name";
            this.m_locName.Name = "m_locName";
            this.m_locName.ReadOnly = true;
            this.m_locName.Width = 170;
            // 
            // m_locWinLoss
            // 
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.m_locWinLoss.DefaultCellStyle = dataGridViewCellStyle6;
            this.m_locWinLoss.HeaderText = "kills/deaths";
            this.m_locWinLoss.Name = "m_locWinLoss";
            this.m_locWinLoss.ReadOnly = true;
            this.m_locWinLoss.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.m_locWinLoss.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // m_locStatus
            // 
            this.m_locStatus.HeaderText = "status";
            this.m_locStatus.Name = "m_locStatus";
            this.m_locStatus.ReadOnly = true;
            // 
            // btRefresh
            // 
            this.btRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btRefresh.Image")));
            this.btRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btRefresh.Name = "btRefresh";
            this.btRefresh.Size = new System.Drawing.Size(23, 22);
            this.btRefresh.Text = "Refresh";
            this.btRefresh.ToolTipText = "Refresh";
            this.btRefresh.Click += new System.EventHandler(this.btRefresh_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1223, 601);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Eve Clipboardwatcher";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMain)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLocal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton tsbtLogin;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btOptions;
        public System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripButton btAbout;
        private System.Windows.Forms.DataGridView dataGridViewMain;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridViewLocal;
        private System.Windows.Forms.DataGridViewImageColumn m_icon;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_nameType;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_category;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_win_loss;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_pilots;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_status;
        private System.Windows.Forms.DataGridViewImageColumn m_locIcon;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_locName;
        private System.Windows.Forms.DataGridViewLinkColumn m_locWinLoss;
        private System.Windows.Forms.DataGridViewTextBoxColumn m_locStatus;
        private System.Windows.Forms.ToolStripButton btChangeShipType;
        private System.Windows.Forms.ToolStripButton btRefresh;
    }
}

