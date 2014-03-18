namespace IXAccel
{
    partial class IXAccelHome
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
            this.OpenButton = new System.Windows.Forms.Button();
            this.ReadButton = new System.Windows.Forms.Button();
            this.ConsoleTextbox = new System.Windows.Forms.TextBox();
            this.ckShowAsHex = new System.Windows.Forms.CheckBox();
            this.GetConfig = new System.Windows.Forms.Button();
            this.AnalyticsButton = new System.Windows.Forms.Button();
            this.ZedGraphFrontPage = new ZedGraph.ZedGraphControl();
            this.TestReadButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // OpenButton
            // 
            this.OpenButton.Location = new System.Drawing.Point(2, 26);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(98, 23);
            this.OpenButton.TabIndex = 0;
            this.OpenButton.Text = "Open Device";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // ReadButton
            // 
            this.ReadButton.Location = new System.Drawing.Point(2, 55);
            this.ReadButton.Name = "ReadButton";
            this.ReadButton.Size = new System.Drawing.Size(98, 23);
            this.ReadButton.TabIndex = 2;
            this.ReadButton.Text = "Start Reading";
            this.ReadButton.UseVisualStyleBackColor = true;
            this.ReadButton.Click += new System.EventHandler(this.ReadButton_Click);
            // 
            // ConsoleTextbox
            // 
            this.ConsoleTextbox.Location = new System.Drawing.Point(314, 26);
            this.ConsoleTextbox.Multiline = true;
            this.ConsoleTextbox.Name = "ConsoleTextbox";
            this.ConsoleTextbox.Size = new System.Drawing.Size(291, 88);
            this.ConsoleTextbox.TabIndex = 3;
            // 
            // ckShowAsHex
            // 
            this.ckShowAsHex.AutoSize = true;
            this.ckShowAsHex.Location = new System.Drawing.Point(219, 90);
            this.ckShowAsHex.Name = "ckShowAsHex";
            this.ckShowAsHex.Size = new System.Drawing.Size(68, 17);
            this.ckShowAsHex.TabIndex = 4;
            this.ckShowAsHex.Text = "HexView";
            this.ckShowAsHex.UseVisualStyleBackColor = true;
            // 
            // GetConfig
            // 
            this.GetConfig.Location = new System.Drawing.Point(210, 26);
            this.GetConfig.Name = "GetConfig";
            this.GetConfig.Size = new System.Drawing.Size(98, 23);
            this.GetConfig.TabIndex = 5;
            this.GetConfig.Text = "Get USB Config";
            this.GetConfig.UseVisualStyleBackColor = true;
            this.GetConfig.Click += new System.EventHandler(this.GetConfig_Click);
            // 
            // AnalyticsButton
            // 
            this.AnalyticsButton.Location = new System.Drawing.Point(106, 84);
            this.AnalyticsButton.Name = "AnalyticsButton";
            this.AnalyticsButton.Size = new System.Drawing.Size(98, 23);
            this.AnalyticsButton.TabIndex = 6;
            this.AnalyticsButton.Text = "Analytics";
            this.AnalyticsButton.UseVisualStyleBackColor = true;
            this.AnalyticsButton.Click += new System.EventHandler(this.AnalyticsButton_Click);
            // 
            // ZedGraphFrontPage
            // 
            this.ZedGraphFrontPage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ZedGraphFrontPage.IsEnableBandAdd = false;
            this.ZedGraphFrontPage.Location = new System.Drawing.Point(2, 218);
            this.ZedGraphFrontPage.Name = "ZedGraphFrontPage";
            this.ZedGraphFrontPage.ScrollGrace = 0D;
            this.ZedGraphFrontPage.ScrollMaxX = 0D;
            this.ZedGraphFrontPage.ScrollMaxY = 0D;
            this.ZedGraphFrontPage.ScrollMaxY2 = 0D;
            this.ZedGraphFrontPage.ScrollMinX = 0D;
            this.ZedGraphFrontPage.ScrollMinY = 0D;
            this.ZedGraphFrontPage.ScrollMinY2 = 0D;
            this.ZedGraphFrontPage.Size = new System.Drawing.Size(620, 220);
            this.ZedGraphFrontPage.TabIndex = 7;
            // 
            // TestReadButton
            // 
            this.TestReadButton.Location = new System.Drawing.Point(210, 55);
            this.TestReadButton.Name = "TestReadButton";
            this.TestReadButton.Size = new System.Drawing.Size(98, 23);
            this.TestReadButton.TabIndex = 8;
            this.TestReadButton.Text = "Test Read";
            this.TestReadButton.UseVisualStyleBackColor = true;
            this.TestReadButton.Click += new System.EventHandler(this.TestReadButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(2, 84);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(98, 23);
            this.StopButton.TabIndex = 9;
            this.StopButton.Text = "Stop Reading";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(106, 26);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(98, 23);
            this.SaveButton.TabIndex = 10;
            this.SaveButton.Text = "Save Data";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(106, 55);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(98, 23);
            this.ClearButton.TabIndex = 11;
            this.ClearButton.Text = "Clear Data";
            this.ClearButton.UseVisualStyleBackColor = true;
            // 
            // IXAccelHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.TestReadButton);
            this.Controls.Add(this.ZedGraphFrontPage);
            this.Controls.Add(this.AnalyticsButton);
            this.Controls.Add(this.GetConfig);
            this.Controls.Add(this.ckShowAsHex);
            this.Controls.Add(this.ConsoleTextbox);
            this.Controls.Add(this.ReadButton);
            this.Controls.Add(this.OpenButton);
            this.Name = "IXAccelHome";
            this.Text = "IX Sense";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.Button ReadButton;
        private System.Windows.Forms.TextBox ConsoleTextbox;
        private System.Windows.Forms.CheckBox ckShowAsHex;
        private System.Windows.Forms.Button GetConfig;
        private System.Windows.Forms.Button AnalyticsButton;
        private ZedGraph.ZedGraphControl ZedGraphFrontPage;
        private System.Windows.Forms.Button TestReadButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button ClearButton;
    }
}

