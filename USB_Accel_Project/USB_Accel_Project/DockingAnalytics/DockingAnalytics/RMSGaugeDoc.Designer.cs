namespace DockingAnalytics
{
    partial class RMSGaugeDoc
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
            this.gauge1 = new DockingAnalytics.Gauge();
            this.SuspendLayout();
            // 
            // gauge1
            // 
            this.gauge1.DeltaOKValue = 40D;
            this.gauge1.DeltaWarningLevel = 45D;
            this.gauge1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gauge1.Location = new System.Drawing.Point(0, 0);
            this.gauge1.MaxRMS = 660D;
            this.gauge1.Name = "gauge1";
            this.gauge1.PreferredValue = 130D;
            this.gauge1.Size = new System.Drawing.Size(301, 287);
            this.gauge1.Speed = 229D;
            this.gauge1.TabIndex = 0;
            // 
            // RMSGaugeDoc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 287);
            this.Controls.Add(this.gauge1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "RMSGaugeDoc";
            this.Text = "RMSGaugeDoc";
            this.ResumeLayout(false);

        }

        #endregion

        private Gauge gauge1;
    }
}