namespace JeromeTerry.PokerHandHistory
{
    partial class AboutPokerHandHistory
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
            this.label1 = new System.Windows.Forms.Label();
            this._lnkCodePlex = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this._lnkLicense = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this._lblVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(80, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(251, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Poker Hand History";
            // 
            // _lnkCodePlex
            // 
            this._lnkCodePlex.AutoSize = true;
            this._lnkCodePlex.Location = new System.Drawing.Point(161, 92);
            this._lnkCodePlex.Name = "_lnkCodePlex";
            this._lnkCodePlex.Size = new System.Drawing.Size(88, 13);
            this._lnkCodePlex.TabIndex = 1;
            this._lnkCodePlex.TabStop = true;
            this._lnkCodePlex.Text = "CodePlex Project";
            this._lnkCodePlex.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._lnkCodePlex_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(79, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(253, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Copyright © Jerome Terry 2012. All Rights Reserved";
            // 
            // _lnkLicense
            // 
            this._lnkLicense.AutoSize = true;
            this._lnkLicense.Location = new System.Drawing.Point(172, 121);
            this._lnkLicense.Name = "_lnkLicense";
            this._lnkLicense.Size = new System.Drawing.Size(66, 13);
            this._lnkLicense.TabIndex = 3;
            this._lnkLicense.TabStop = true;
            this._lnkLicense.Text = "MIT License";
            this._lnkLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._lnkLicense_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(131, 248);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "Version";
            // 
            // _lblVersion
            // 
            this._lblVersion.AutoSize = true;
            this._lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblVersion.Location = new System.Drawing.Point(215, 248);
            this._lblVersion.Name = "_lblVersion";
            this._lblVersion.Size = new System.Drawing.Size(65, 24);
            this._lblVersion.TabIndex = 5;
            this._lblVersion.Text = "1.0.0.0";
            // 
            // AboutPokerHandHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 320);
            this.Controls.Add(this._lblVersion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._lnkLicense);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._lnkCodePlex);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutPokerHandHistory";
            this.Text = "About Poker Hand History";
            this.Load += new System.EventHandler(this.AboutPokerHandHistory_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel _lnkCodePlex;
        private System.Windows.Forms.Label label2;
        protected internal System.Windows.Forms.LinkLabel _lnkLicense;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label _lblVersion;
    }
}