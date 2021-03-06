﻿namespace ScpServer
{
    partial class RecordBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordBox));
            this.btnRecord = new System.Windows.Forms.Button();
            this.cBRecordDelays = new System.Windows.Forms.CheckBox();
            this.lVMacros = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.iLKeys = new System.Windows.Forms.ImageList(this.components);
            this.cBStyle = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cB360Controls = new System.Windows.Forms.ComboBox();
            this.lBHoldX360 = new System.Windows.Forms.Label();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.pnlMouseButtons = new System.Windows.Forms.Panel();
            this.btn5th = new System.Windows.Forms.Button();
            this.btn4th = new System.Windows.Forms.Button();
            this.pnlSettings.SuspendLayout();
            this.pnlMouseButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRecord
            // 
            resources.ApplyResources(this.btnRecord, "btnRecord");
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.TabStop = false;
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            this.btnRecord.KeyDown += new System.Windows.Forms.KeyEventHandler(this.anyKeyDown);
            this.btnRecord.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyKeyUp);
            // 
            // cBRecordDelays
            // 
            resources.ApplyResources(this.cBRecordDelays, "cBRecordDelays");
            this.cBRecordDelays.Name = "cBRecordDelays";
            this.cBRecordDelays.TabStop = false;
            this.cBRecordDelays.UseVisualStyleBackColor = true;
            this.cBRecordDelays.KeyDown += new System.Windows.Forms.KeyEventHandler(this.anyKeyDown);
            this.cBRecordDelays.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyKeyUp);
            this.cBRecordDelays.MouseDown += new System.Windows.Forms.MouseEventHandler(this.anyMouseDown);
            this.cBRecordDelays.MouseUp += new System.Windows.Forms.MouseEventHandler(this.anyMouseUp);
            // 
            // lVMacros
            // 
            resources.ApplyResources(this.lVMacros, "lVMacros");
            this.lVMacros.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lVMacros.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lVMacros.LargeImageList = this.iLKeys;
            this.lVMacros.Name = "lVMacros";
            this.lVMacros.SmallImageList = this.iLKeys;
            this.lVMacros.UseCompatibleStateImageBehavior = false;
            this.lVMacros.View = System.Windows.Forms.View.Details;
            this.lVMacros.KeyDown += new System.Windows.Forms.KeyEventHandler(this.anyKeyDown);
            this.lVMacros.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyKeyUp);
            this.lVMacros.MouseDown += new System.Windows.Forms.MouseEventHandler(this.anyMouseDown);
            this.lVMacros.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lVMacros_MouseMove);
            this.lVMacros.MouseUp += new System.Windows.Forms.MouseEventHandler(this.anyMouseUp);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // iLKeys
            // 
            this.iLKeys.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iLKeys.ImageStream")));
            this.iLKeys.TransparentColor = System.Drawing.Color.Transparent;
            this.iLKeys.Images.SetKeyName(0, "keydown.png");
            this.iLKeys.Images.SetKeyName(1, "keyup.png");
            this.iLKeys.Images.SetKeyName(2, "Clock.png");
            // 
            // cBStyle
            // 
            resources.ApplyResources(this.cBStyle, "cBStyle");
            this.cBStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBStyle.FormattingEnabled = true;
            this.cBStyle.Items.AddRange(new object[] {
            resources.GetString("cBStyle.Items"),
            resources.GetString("cBStyle.Items1")});
            this.cBStyle.Name = "cBStyle";
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cB360Controls
            // 
            resources.ApplyResources(this.cB360Controls, "cB360Controls");
            this.cB360Controls.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cB360Controls.FormattingEnabled = true;
            this.cB360Controls.Items.AddRange(new object[] {
            resources.GetString("cB360Controls.Items"),
            resources.GetString("cB360Controls.Items1"),
            resources.GetString("cB360Controls.Items2"),
            resources.GetString("cB360Controls.Items3"),
            resources.GetString("cB360Controls.Items4"),
            resources.GetString("cB360Controls.Items5"),
            resources.GetString("cB360Controls.Items6"),
            resources.GetString("cB360Controls.Items7"),
            resources.GetString("cB360Controls.Items8"),
            resources.GetString("cB360Controls.Items9"),
            resources.GetString("cB360Controls.Items10"),
            resources.GetString("cB360Controls.Items11"),
            resources.GetString("cB360Controls.Items12"),
            resources.GetString("cB360Controls.Items13"),
            resources.GetString("cB360Controls.Items14"),
            resources.GetString("cB360Controls.Items15"),
            resources.GetString("cB360Controls.Items16"),
            resources.GetString("cB360Controls.Items17"),
            resources.GetString("cB360Controls.Items18"),
            resources.GetString("cB360Controls.Items19"),
            resources.GetString("cB360Controls.Items20"),
            resources.GetString("cB360Controls.Items21"),
            resources.GetString("cB360Controls.Items22"),
            resources.GetString("cB360Controls.Items23"),
            resources.GetString("cB360Controls.Items24"),
            resources.GetString("cB360Controls.Items25")});
            this.cB360Controls.Name = "cB360Controls";
            // 
            // lBHoldX360
            // 
            resources.ApplyResources(this.lBHoldX360, "lBHoldX360");
            this.lBHoldX360.Name = "lBHoldX360";
            // 
            // pnlSettings
            // 
            resources.ApplyResources(this.pnlSettings, "pnlSettings");
            this.pnlSettings.Controls.Add(this.lBHoldX360);
            this.pnlSettings.Controls.Add(this.cBStyle);
            this.pnlSettings.Controls.Add(this.cB360Controls);
            this.pnlSettings.Controls.Add(this.cBRecordDelays);
            this.pnlSettings.Controls.Add(this.btnCancel);
            this.pnlSettings.Controls.Add(this.btnSave);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.MouseDown += new System.Windows.Forms.MouseEventHandler(this.anyMouseDown);
            this.pnlSettings.MouseUp += new System.Windows.Forms.MouseEventHandler(this.anyMouseUp);
            // 
            // pnlMouseButtons
            // 
            resources.ApplyResources(this.pnlMouseButtons, "pnlMouseButtons");
            this.pnlMouseButtons.Controls.Add(this.btn5th);
            this.pnlMouseButtons.Controls.Add(this.btn4th);
            this.pnlMouseButtons.Name = "pnlMouseButtons";
            this.pnlMouseButtons.MouseDown += new System.Windows.Forms.MouseEventHandler(this.anyMouseDown);
            this.pnlMouseButtons.MouseUp += new System.Windows.Forms.MouseEventHandler(this.anyMouseUp);
            // 
            // btn5th
            // 
            resources.ApplyResources(this.btn5th, "btn5th");
            this.btn5th.Name = "btn5th";
            this.btn5th.UseVisualStyleBackColor = true;
            this.btn5th.Click += new System.EventHandler(this.btn5th_Click);
            this.btn5th.KeyDown += new System.Windows.Forms.KeyEventHandler(this.anyKeyDown);
            this.btn5th.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyKeyUp);
            // 
            // btn4th
            // 
            resources.ApplyResources(this.btn4th, "btn4th");
            this.btn4th.Name = "btn4th";
            this.btn4th.UseVisualStyleBackColor = true;
            this.btn4th.Click += new System.EventHandler(this.btn4th_Click);
            this.btn4th.KeyDown += new System.Windows.Forms.KeyEventHandler(this.anyKeyDown);
            this.btn4th.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyKeyUp);
            // 
            // RecordBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.lVMacros);
            this.Controls.Add(this.pnlMouseButtons);
            this.Controls.Add(this.pnlSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "RecordBox";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RecordBox_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.anyKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.anyKeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.anyMouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.anyMouseUp);
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            this.pnlMouseButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.CheckBox cBRecordDelays;
        private System.Windows.Forms.ListView lVMacros;
        private System.Windows.Forms.ImageList iLKeys;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ComboBox cBStyle;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cB360Controls;
        private System.Windows.Forms.Label lBHoldX360;
        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.Panel pnlMouseButtons;
        private System.Windows.Forms.Button btn5th;
        private System.Windows.Forms.Button btn4th;
    }
}