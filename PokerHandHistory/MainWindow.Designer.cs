namespace JeromeTerry.PokerHandHistory
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this._miFile = new System.Windows.Forms.ToolStripMenuItem();
            this._miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._miViewLog = new System.Windows.Forms.ToolStripMenuItem();
            this._miDataDir = new System.Windows.Forms.ToolStripMenuItem();
            this._miHandAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._miAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this._lblTotalHands = new System.Windows.Forms.Label();
            this._statsRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this._fileWatchTimer = new System.Windows.Forms.Timer(this.components);
            this._lblTotalPlayers = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._playersDataGrid = new System.Windows.Forms.DataGridView();
            this._playersDataSet = new JeromeTerry.PokerHandHistory.Players();
            this._playerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.handCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateAddedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._playersDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._playersDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._playerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miFile,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(911, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // _miFile
            // 
            this._miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miExit});
            this._miFile.Name = "_miFile";
            this._miFile.Size = new System.Drawing.Size(37, 20);
            this._miFile.Text = "&File";
            this._miFile.DropDownOpening += new System.EventHandler(this._miFile_DropDownOpening);
            // 
            // _miExit
            // 
            this._miExit.Name = "_miExit";
            this._miExit.Size = new System.Drawing.Size(92, 22);
            this._miExit.Text = "E&xit";
            this._miExit.Click += new System.EventHandler(this._miExit_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miViewLog,
            this._miDataDir,
            this._miHandAnalysis});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // _miViewLog
            // 
            this._miViewLog.Name = "_miViewLog";
            this._miViewLog.Size = new System.Drawing.Size(149, 22);
            this._miViewLog.Text = "View &Log";
            this._miViewLog.Click += new System.EventHandler(this._miViewLog_Click);
            // 
            // _miDataDir
            // 
            this._miDataDir.Name = "_miDataDir";
            this._miDataDir.Size = new System.Drawing.Size(149, 22);
            this._miDataDir.Text = "Open &Data Dir";
            this._miDataDir.Click += new System.EventHandler(this._miDataDir_Click);
            // 
            // _miHandAnalysis
            // 
            this._miHandAnalysis.Name = "_miHandAnalysis";
            this._miHandAnalysis.Size = new System.Drawing.Size(149, 22);
            this._miHandAnalysis.Text = "Hand Analysis";
            this._miHandAnalysis.Click += new System.EventHandler(this._miHandAnalysis_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // _miAbout
            // 
            this._miAbout.Name = "_miAbout";
            this._miAbout.Size = new System.Drawing.Size(107, 22);
            this._miAbout.Text = "&About";
            this._miAbout.Click += new System.EventHandler(this._miAbout_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "Total Hands:";
            // 
            // _lblTotalHands
            // 
            this._lblTotalHands.AutoSize = true;
            this._lblTotalHands.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblTotalHands.Location = new System.Drawing.Point(139, 38);
            this._lblTotalHands.Name = "_lblTotalHands";
            this._lblTotalHands.Size = new System.Drawing.Size(20, 23);
            this._lblTotalHands.TabIndex = 5;
            this._lblTotalHands.Text = "0";
            // 
            // _statsRefreshTimer
            // 
            this._statsRefreshTimer.Interval = 1000;
            this._statsRefreshTimer.Tick += new System.EventHandler(this._statsRefreshTimer_Tick);
            // 
            // _fileWatchTimer
            // 
            this._fileWatchTimer.Interval = 5000;
            this._fileWatchTimer.Tick += new System.EventHandler(this._fileWatchTimer_Tick);
            // 
            // _lblTotalPlayers
            // 
            this._lblTotalPlayers.AutoSize = true;
            this._lblTotalPlayers.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblTotalPlayers.Location = new System.Drawing.Point(336, 38);
            this._lblTotalPlayers.Name = "_lblTotalPlayers";
            this._lblTotalPlayers.Size = new System.Drawing.Size(20, 23);
            this._lblTotalPlayers.TabIndex = 7;
            this._lblTotalPlayers.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(209, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 23);
            this.label3.TabIndex = 6;
            this.label3.Text = "Total Players:";
            // 
            // _playersDataGrid
            // 
            this._playersDataGrid.AllowUserToAddRows = false;
            this._playersDataGrid.AllowUserToDeleteRows = false;
            this._playersDataGrid.AllowUserToOrderColumns = true;
            this._playersDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._playersDataGrid.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._playersDataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this._playersDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._playersDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.handCountDataGridViewTextBoxColumn,
            this.dateAddedDataGridViewTextBoxColumn});
            this._playersDataGrid.DataSource = this._playerBindingSource;
            this._playersDataGrid.Location = new System.Drawing.Point(16, 80);
            this._playersDataGrid.Name = "_playersDataGrid";
            this._playersDataGrid.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._playersDataGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._playersDataGrid.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this._playersDataGrid.Size = new System.Drawing.Size(872, 413);
            this._playersDataGrid.TabIndex = 8;
            // 
            // _playersDataSet
            // 
            this._playersDataSet.DataSetName = "Players";
            this._playersDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // _playerBindingSource
            // 
            this._playerBindingSource.DataMember = "Player";
            this._playerBindingSource.DataSource = this._playersDataSet;
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            this.idDataGridViewTextBoxColumn.HeaderText = "Id";
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            this.idDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            this.nameDataGridViewTextBoxColumn.Width = 200;
            // 
            // handCountDataGridViewTextBoxColumn
            // 
            this.handCountDataGridViewTextBoxColumn.DataPropertyName = "HandCount";
            this.handCountDataGridViewTextBoxColumn.HeaderText = "Hands";
            this.handCountDataGridViewTextBoxColumn.Name = "handCountDataGridViewTextBoxColumn";
            this.handCountDataGridViewTextBoxColumn.ReadOnly = true;
            this.handCountDataGridViewTextBoxColumn.Width = 200;
            // 
            // dateAddedDataGridViewTextBoxColumn
            // 
            this.dateAddedDataGridViewTextBoxColumn.DataPropertyName = "DateAdded";
            this.dateAddedDataGridViewTextBoxColumn.HeaderText = "First Seen";
            this.dateAddedDataGridViewTextBoxColumn.Name = "dateAddedDataGridViewTextBoxColumn";
            this.dateAddedDataGridViewTextBoxColumn.ReadOnly = true;
            this.dateAddedDataGridViewTextBoxColumn.Width = 300;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(911, 514);
            this.Controls.Add(this._playersDataGrid);
            this.Controls.Add(this._lblTotalPlayers);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._lblTotalHands);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "Poker Hand History";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._playersDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._playersDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._playerBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem _miFile;
        private System.Windows.Forms.ToolStripMenuItem _miExit;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _miViewLog;
        private System.Windows.Forms.ToolStripMenuItem _miDataDir;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _miAbout;
        private System.Windows.Forms.ToolStripMenuItem _miHandAnalysis;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _lblTotalHands;
        private System.Windows.Forms.Timer _statsRefreshTimer;
        private System.Windows.Forms.Timer _fileWatchTimer;
        private System.Windows.Forms.Label _lblTotalPlayers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView _playersDataGrid;
        private System.Windows.Forms.BindingSource _playerBindingSource;
        private Players _playersDataSet;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn handCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateAddedDataGridViewTextBoxColumn;
    }
}

