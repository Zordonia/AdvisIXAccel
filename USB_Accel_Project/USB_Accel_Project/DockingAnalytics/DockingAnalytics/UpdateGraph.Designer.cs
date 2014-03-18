namespace DockingAnalytics
{
    partial class UpdateGraph
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dgFNameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dbTypeCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgXCCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgYCCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgCBScaleCol = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dgBColorCol = new System.Windows.Forms.DataGridViewButtonColumn();
            this.dbSymOnCol = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgFNameCol,
            this.dbTypeCol,
            this.dgXCCol,
            this.dgYCCol,
            this.dgCBScaleCol,
            this.dgBColorCol,
            this.dbSymOnCol});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(846, 130);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.CellContentClick);
            this.dataGridView1.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView1_EditingControlShowing);
            // 
            // dgFNameCol
            // 
            this.dgFNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgFNameCol.FillWeight = 114.5283F;
            this.dgFNameCol.HeaderText = "File Name";
            this.dgFNameCol.MinimumWidth = 200;
            this.dgFNameCol.Name = "dgFNameCol";
            this.dgFNameCol.ReadOnly = true;
            this.dgFNameCol.Width = 200;
            // 
            // dbTypeCol
            // 
            this.dbTypeCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dbTypeCol.FillWeight = 1.210555F;
            this.dbTypeCol.HeaderText = "Graph Type";
            this.dbTypeCol.MinimumWidth = 95;
            this.dbTypeCol.Name = "dbTypeCol";
            this.dbTypeCol.ReadOnly = true;
            // 
            // dgXCCol
            // 
            this.dgXCCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgXCCol.FillWeight = 2.94162F;
            this.dgXCCol.HeaderText = "X-Coordinate";
            this.dgXCCol.MinimumWidth = 135;
            this.dgXCCol.Name = "dgXCCol";
            this.dgXCCol.ReadOnly = true;
            // 
            // dgYCCol
            // 
            this.dgYCCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgYCCol.FillWeight = 7.258325F;
            this.dgYCCol.HeaderText = "Y-Coordinate";
            this.dgYCCol.MinimumWidth = 135;
            this.dgYCCol.Name = "dgYCCol";
            this.dgYCCol.ReadOnly = true;
            // 
            // dgCBScaleCol
            // 
            this.dgCBScaleCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgCBScaleCol.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.dgCBScaleCol.FillWeight = 28.4367F;
            this.dgCBScaleCol.HeaderText = "Scale";
            this.dgCBScaleCol.MinimumWidth = 65;
            this.dgCBScaleCol.Name = "dgCBScaleCol";
            // 
            // dgBColorCol
            // 
            this.dgBColorCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.dgBColorCol.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgBColorCol.FillWeight = 44.95283F;
            this.dgBColorCol.HeaderText = "Color";
            this.dgBColorCol.MinimumWidth = 50;
            this.dgBColorCol.Name = "dgBColorCol";
            // 
            // dbSymOnCol
            // 
            this.dbSymOnCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dbSymOnCol.FillWeight = 111.8955F;
            this.dbSymOnCol.HeaderText = "View Symbols";
            this.dbSymOnCol.MinimumWidth = 35;
            this.dbSymOnCol.Name = "dbSymOnCol";
            // 
            // UpdateGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 130);
            this.Controls.Add(this.dataGridView1);
            this.HideOnClose = true;
            this.Name = "UpdateGraph";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockTopAutoHide;
            this.TabText = "Update Graph";
            this.Text = "UpdateGraph";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UpdateGraph_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgFNameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn dbTypeCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgXCCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgYCCol;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgCBScaleCol;
        private System.Windows.Forms.DataGridViewButtonColumn dgBColorCol;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dbSymOnCol;
    }
}