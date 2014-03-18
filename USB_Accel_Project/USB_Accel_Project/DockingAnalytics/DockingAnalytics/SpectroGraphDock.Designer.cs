namespace DockingAnalytics
{
    partial class SpectroGraphDock
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
            this.glControl1 = new OpenTK.GLControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.zoomMenuStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.rangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meshGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl1.Location = new System.Drawing.Point(0, 0);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(704, 641);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = false;
            this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintThreeD);
            this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyDown);
            this.glControl1.Leave += new System.EventHandler(this.glControl1_Leave);
            this.glControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseClick);
            this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
            this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
            this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
            this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomMenuStrip,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(704, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // zoomMenuStrip
            // 
            this.zoomMenuStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rangeToolStripMenuItem});
            this.zoomMenuStrip.Name = "zoomMenuStrip";
            this.zoomMenuStrip.Size = new System.Drawing.Size(51, 20);
            this.zoomMenuStrip.Text = "Zoom";
            // 
            // rangeToolStripMenuItem
            // 
            this.rangeToolStripMenuItem.Name = "rangeToolStripMenuItem";
            this.rangeToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.rangeToolStripMenuItem.Text = "Range";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lineGraphToolStripMenuItem,
            this.barGraphToolStripMenuItem,
            this.meshGraphToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.viewToolStripMenuItem.Text = "3D View";
            // 
            // lineGraphToolStripMenuItem
            // 
            this.lineGraphToolStripMenuItem.Checked = true;
            this.lineGraphToolStripMenuItem.CheckOnClick = true;
            this.lineGraphToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.lineGraphToolStripMenuItem.Name = "lineGraphToolStripMenuItem";
            this.lineGraphToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.lineGraphToolStripMenuItem.Text = "Line Graph";
            this.lineGraphToolStripMenuItem.CheckedChanged += new System.EventHandler(this.lineGraphToolStripMenuItem_CheckedChanged);
            // 
            // barGraphToolStripMenuItem
            // 
            this.barGraphToolStripMenuItem.Checked = true;
            this.barGraphToolStripMenuItem.CheckOnClick = true;
            this.barGraphToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.barGraphToolStripMenuItem.Name = "barGraphToolStripMenuItem";
            this.barGraphToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.barGraphToolStripMenuItem.Text = "Bar Graph";
            this.barGraphToolStripMenuItem.CheckedChanged += new System.EventHandler(this.lineGraphToolStripMenuItem_CheckedChanged);
            // 
            // meshGraphToolStripMenuItem
            // 
            this.meshGraphToolStripMenuItem.Checked = true;
            this.meshGraphToolStripMenuItem.CheckOnClick = true;
            this.meshGraphToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.meshGraphToolStripMenuItem.Name = "meshGraphToolStripMenuItem";
            this.meshGraphToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.meshGraphToolStripMenuItem.Text = "Mesh Graph";
            this.meshGraphToolStripMenuItem.CheckedChanged += new System.EventHandler(this.lineGraphToolStripMenuItem_CheckedChanged);
            // 
            // SpectroGraphDock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 641);
            this.Controls.Add(this.glControl1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpectroGraphDock";
            this.Text = "SpectroGraphDock";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem zoomMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lineGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem barGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meshGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rangeToolStripMenuItem;
    }
}