namespace DockingAnalytics
{
    partial class RangeTrackBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.minTextBox = new System.Windows.Forms.TextBox();
            this.maxTextBox = new System.Windows.Forms.TextBox();
            this.maxTrackBar = new DockingAnalytics.TrackBarNoFocusCues();
            this.minTrackBar = new DockingAnalytics.TrackBarNoFocusCues();
            ((System.ComponentModel.ISupportInitialize)(this.maxTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // minTextBox
            // 
            this.minTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.minTextBox.Location = new System.Drawing.Point(168, 3);
            this.minTextBox.Name = "minTextBox";
            this.minTextBox.Size = new System.Drawing.Size(53, 20);
            this.minTextBox.TabIndex = 2;
            this.minTextBox.Text = "Minimum";
            this.minTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.minTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.maxTextBox_KeyDown);
            this.minTextBox.Leave += new System.EventHandler(this.maxTextBox_Leave);
            // 
            // maxTextBox
            // 
            this.maxTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.maxTextBox.Location = new System.Drawing.Point(168, 29);
            this.maxTextBox.Name = "maxTextBox";
            this.maxTextBox.Size = new System.Drawing.Size(53, 20);
            this.maxTextBox.TabIndex = 3;
            this.maxTextBox.Text = "Maximum";
            this.maxTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maxTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.maxTextBox_KeyDown);
            this.maxTextBox.Leave += new System.EventHandler(this.maxTextBox_Leave);
            // 
            // maxTrackBar
            // 
            this.maxTrackBar.BackColor = System.Drawing.SystemColors.Window;
            this.maxTrackBar.Location = new System.Drawing.Point(0, 22);
            this.maxTrackBar.Maximum = 10000;
            this.maxTrackBar.Name = "maxTrackBar";
            this.maxTrackBar.Size = new System.Drawing.Size(168, 45);
            this.maxTrackBar.TabIndex = 5;
            this.maxTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.maxTrackBar.Scroll += new System.EventHandler(this.maxTrackBar_Scroll);
            // 
            // minTrackBar
            // 
            this.minTrackBar.BackColor = System.Drawing.SystemColors.Window;
            this.minTrackBar.Location = new System.Drawing.Point(0, 2);
            this.minTrackBar.Maximum = 10000;
            this.minTrackBar.Name = "minTrackBar";
            this.minTrackBar.Size = new System.Drawing.Size(168, 45);
            this.minTrackBar.TabIndex = 4;
            this.minTrackBar.Scroll += new System.EventHandler(this.minTrackBar_Scroll);
            // 
            // RangeTrackBar
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.maxTrackBar);
            this.Controls.Add(this.minTrackBar);
            this.Controls.Add(this.maxTextBox);
            this.Controls.Add(this.minTextBox);
            this.MaximumSize = new System.Drawing.Size(224, 55);
            this.Name = "RangeTrackBar";
            this.Size = new System.Drawing.Size(224, 55);
            ((System.ComponentModel.ISupportInitialize)(this.maxTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox minTextBox;
        private System.Windows.Forms.TextBox maxTextBox;
        private TrackBarNoFocusCues minTrackBar;
        private TrackBarNoFocusCues maxTrackBar;

    }
}
