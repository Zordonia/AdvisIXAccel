namespace DockingAnalytics
{
    partial class USBControlDock
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
            this.ConsoleTextbox = new System.Windows.Forms.TextBox();
            this.GraphListComboBox = new System.Windows.Forms.ComboBox();
            this.StopUSBButton = new System.Windows.Forms.Button();
            this.ReadUSBButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.ChannelGroupBox = new System.Windows.Forms.GroupBox();
            this.BothGainsRadioButton = new System.Windows.Forms.RadioButton();
            this.LowGainRadioButton = new System.Windows.Forms.RadioButton();
            this.HighGainRadioButton = new System.Windows.Forms.RadioButton();
            this.SensitivityGroupBox = new System.Windows.Forms.GroupBox();
            this.SenseUnitsComboBox = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.OpenUSBButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.ChannelGroupBox.SuspendLayout();
            this.SensitivityGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(117, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "Select Graph";
            // 
            // ConsoleTextbox
            // 
            this.ConsoleTextbox.BackColor = System.Drawing.SystemColors.InfoText;
            this.ConsoleTextbox.ForeColor = System.Drawing.SystemColors.Info;
            this.ConsoleTextbox.Location = new System.Drawing.Point(52, 19);
            this.ConsoleTextbox.Multiline = true;
            this.ConsoleTextbox.Name = "ConsoleTextbox";
            this.ConsoleTextbox.Size = new System.Drawing.Size(289, 161);
            this.ConsoleTextbox.TabIndex = 15;
            // 
            // GraphListComboBox
            // 
            this.GraphListComboBox.FormattingEnabled = true;
            this.GraphListComboBox.Location = new System.Drawing.Point(120, 139);
            this.GraphListComboBox.Name = "GraphListComboBox";
            this.GraphListComboBox.Size = new System.Drawing.Size(92, 25);
            this.GraphListComboBox.TabIndex = 14;
            this.GraphListComboBox.SelectedIndexChanged += new System.EventHandler(this.GraphListComboBox_SelectedIndexChanged);
            // 
            // StopUSBButton
            // 
            this.StopUSBButton.Location = new System.Drawing.Point(284, 53);
            this.StopUSBButton.Name = "StopUSBButton";
            this.StopUSBButton.Size = new System.Drawing.Size(95, 37);
            this.StopUSBButton.TabIndex = 16;
            this.StopUSBButton.Text = "Stop Reading";
            this.StopUSBButton.UseVisualStyleBackColor = true;
            this.StopUSBButton.Click += new System.EventHandler(this.StopUSBButton_Click);
            // 
            // ReadUSBButton
            // 
            this.ReadUSBButton.Location = new System.Drawing.Point(284, 10);
            this.ReadUSBButton.Name = "ReadUSBButton";
            this.ReadUSBButton.Size = new System.Drawing.Size(95, 37);
            this.ReadUSBButton.TabIndex = 13;
            this.ReadUSBButton.Text = "Start Reading";
            this.ReadUSBButton.UseVisualStyleBackColor = true;
            this.ReadUSBButton.Click += new System.EventHandler(this.ReadUSBButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(2, 2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.ChannelGroupBox);
            this.splitContainer1.Panel1.Controls.Add(this.SensitivityGroupBox);
            this.splitContainer1.Panel1.Controls.Add(this.OpenUSBButton);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.GraphListComboBox);
            this.splitContainer1.Panel1.Controls.Add(this.ReadUSBButton);
            this.splitContainer1.Panel1.Controls.Add(this.StopUSBButton);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ConsoleTextbox);
            this.splitContainer1.Size = new System.Drawing.Size(777, 190);
            this.splitContainer1.SplitterDistance = 382;
            this.splitContainer1.TabIndex = 18;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Location = new System.Drawing.Point(116, 60);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(156, 50);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sensor Capacitance";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "mV/g"});
            this.comboBox1.Location = new System.Drawing.Point(70, 19);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(80, 25);
            this.comboBox1.TabIndex = 19;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(7, 19);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(57, 23);
            this.textBox2.TabIndex = 18;
            this.textBox2.TextChanged += new System.EventHandler(this.sensorCapacitanceTextBoxChanged);
            // 
            // ChannelGroupBox
            // 
            this.ChannelGroupBox.Controls.Add(this.BothGainsRadioButton);
            this.ChannelGroupBox.Controls.Add(this.LowGainRadioButton);
            this.ChannelGroupBox.Controls.Add(this.HighGainRadioButton);
            this.ChannelGroupBox.Location = new System.Drawing.Point(10, 10);
            this.ChannelGroupBox.Name = "ChannelGroupBox";
            this.ChannelGroupBox.Size = new System.Drawing.Size(97, 92);
            this.ChannelGroupBox.TabIndex = 20;
            this.ChannelGroupBox.TabStop = false;
            this.ChannelGroupBox.Text = "Channel Select";
            // 
            // BothGainsRadioButton
            // 
            this.BothGainsRadioButton.AutoSize = true;
            this.BothGainsRadioButton.Location = new System.Drawing.Point(7, 65);
            this.BothGainsRadioButton.Name = "BothGainsRadioButton";
            this.BothGainsRadioButton.Size = new System.Drawing.Size(58, 21);
            this.BothGainsRadioButton.TabIndex = 2;
            this.BothGainsRadioButton.TabStop = true;
            this.BothGainsRadioButton.Text = "Both";
            this.BothGainsRadioButton.UseVisualStyleBackColor = true;
            this.BothGainsRadioButton.CheckedChanged += new System.EventHandler(this.GainRadioButton_CheckedChanged);
            // 
            // LowGainRadioButton
            // 
            this.LowGainRadioButton.AutoSize = true;
            this.LowGainRadioButton.Location = new System.Drawing.Point(7, 43);
            this.LowGainRadioButton.Name = "LowGainRadioButton";
            this.LowGainRadioButton.Size = new System.Drawing.Size(88, 21);
            this.LowGainRadioButton.TabIndex = 1;
            this.LowGainRadioButton.TabStop = true;
            this.LowGainRadioButton.Text = "Low Gain";
            this.LowGainRadioButton.UseVisualStyleBackColor = true;
            this.LowGainRadioButton.CheckedChanged += new System.EventHandler(this.GainRadioButton_CheckedChanged);
            // 
            // HighGainRadioButton
            // 
            this.HighGainRadioButton.AutoSize = true;
            this.HighGainRadioButton.Location = new System.Drawing.Point(7, 20);
            this.HighGainRadioButton.Name = "HighGainRadioButton";
            this.HighGainRadioButton.Size = new System.Drawing.Size(92, 21);
            this.HighGainRadioButton.TabIndex = 0;
            this.HighGainRadioButton.TabStop = true;
            this.HighGainRadioButton.Text = "High Gain";
            this.HighGainRadioButton.UseVisualStyleBackColor = true;
            this.HighGainRadioButton.CheckedChanged += new System.EventHandler(this.GainRadioButton_CheckedChanged);
            // 
            // SensitivityGroupBox
            // 
            this.SensitivityGroupBox.Controls.Add(this.SenseUnitsComboBox);
            this.SensitivityGroupBox.Controls.Add(this.textBox1);
            this.SensitivityGroupBox.Location = new System.Drawing.Point(113, 10);
            this.SensitivityGroupBox.Name = "SensitivityGroupBox";
            this.SensitivityGroupBox.Size = new System.Drawing.Size(156, 50);
            this.SensitivityGroupBox.TabIndex = 19;
            this.SensitivityGroupBox.TabStop = false;
            this.SensitivityGroupBox.Text = "Sensor Sensitivity";
            // 
            // SenseUnitsComboBox
            // 
            this.SenseUnitsComboBox.FormattingEnabled = true;
            this.SenseUnitsComboBox.Items.AddRange(new object[] {
            "mV/g"});
            this.SenseUnitsComboBox.Location = new System.Drawing.Point(70, 19);
            this.SenseUnitsComboBox.Name = "SenseUnitsComboBox";
            this.SenseUnitsComboBox.Size = new System.Drawing.Size(80, 25);
            this.SenseUnitsComboBox.TabIndex = 19;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(7, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(57, 23);
            this.textBox1.TabIndex = 18;
            this.textBox1.TextChanged += new System.EventHandler(this.sensorSensitivityTextBoxChanged);
            // 
            // OpenUSBButton
            // 
            this.OpenUSBButton.BackColor = System.Drawing.Color.LightYellow;
            this.OpenUSBButton.Location = new System.Drawing.Point(10, 119);
            this.OpenUSBButton.Name = "OpenUSBButton";
            this.OpenUSBButton.Size = new System.Drawing.Size(97, 37);
            this.OpenUSBButton.TabIndex = 12;
            this.OpenUSBButton.Text = "Open USB Device";
            this.OpenUSBButton.UseVisualStyleBackColor = false;
            this.OpenUSBButton.Visible = false;
            this.OpenUSBButton.Click += new System.EventHandler(this.OpenUSBButton_Click);
            // 
            // USBControlDock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 194);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "USBControlDock";
            this.Text = "USB";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ChannelGroupBox.ResumeLayout(false);
            this.ChannelGroupBox.PerformLayout();
            this.SensitivityGroupBox.ResumeLayout(false);
            this.SensitivityGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ConsoleTextbox;
        private System.Windows.Forms.ComboBox GraphListComboBox;
        private System.Windows.Forms.Button StopUSBButton;
        private System.Windows.Forms.Button ReadUSBButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox SensitivityGroupBox;
        private System.Windows.Forms.ComboBox SenseUnitsComboBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox ChannelGroupBox;
        private System.Windows.Forms.RadioButton BothGainsRadioButton;
        private System.Windows.Forms.RadioButton LowGainRadioButton;
        private System.Windows.Forms.RadioButton HighGainRadioButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button OpenUSBButton;

    }
}