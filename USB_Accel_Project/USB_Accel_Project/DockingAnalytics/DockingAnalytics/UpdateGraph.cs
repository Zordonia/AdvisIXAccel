using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using WeifenLuo.WinFormsUI.Docking;

namespace DockingAnalytics
{
    public partial class UpdateGraph : UpdateForm
    {
        private GraphDock GraphDock { get; set; }
        private ComboBox myComboBox = new ComboBox();

        public UpdateGraph()
        {
            InitializeComponent();
            InitializeDataGrid();
            this.Text = GlobalVars.GraphNum.ToString();
            this.TabText = GlobalVars.GraphNum.ToString();
            GlobalVars.GraphNum++;
        }

        public void Register(GraphDock gd)
        {
            GraphDock = gd;
            this.Invalidate();
        }

        private void InitializeDataGrid()
        {

        }

        public void UpdateView()
        {
            //Console.WriteLine(this.Text);
            int index = 1;
            foreach (CurveItemTypePair citp in GraphDock.GraphInformation.CurveTypeItems)
            {
                if (dataGridView1.Rows.Count < index)
                {
                    dataGridView1.Rows.Add();
                }
                dataGridView1.Rows[index - 1].Cells[0].Value = System.IO.Path.GetFileName(GraphDock.GraphName);

                dataGridView1.Rows[index - 1].Cells[1].Value = citp.Type;
                dataGridView1.Rows[index - 1].Cells[2].Value = citp.CurrCoordinates.X;
                dataGridView1.Rows[index - 1].Cells[3].Value = citp.CurrCoordinates.Y;
                DataGridViewComboBoxCell dbc = UpdateScaleValues(index - 1, citp.Type);
                dataGridView1.Rows[index - 1].Cells[4] = dbc;
                dataGridView1.Rows[index - 1].Cells[4].Value = citp.Scale.ScaleString;
                dataGridView1.Rows[index - 1].Cells[5].Style.BackColor = citp.Curve.Color;
                dataGridView1.Rows[index - 1].Cells[5].Value = "...";
                dataGridView1.Rows[index - 1].Cells[5].Style.SelectionBackColor = citp.Curve.Color;
                dataGridView1.Rows[index - 1].Cells[6].Value = citp.SymbolsOn;

                index++;
            }
            this.Update();
        }


        private DataGridViewComboBoxCell UpdateScaleValues(int rowValue, CurveType type)
        {
            DataGridViewComboBoxCell dbc = new DataGridViewComboBoxCell();
            switch (type)
            {
                case CurveType.Acceleration:
                    dbc.Items.Clear();
                    dbc.Items.AddRange(GlobalVars.AccelerationScales);
                    break;
                case CurveType.Velocity:
                    dbc.Items.Clear();
                    dbc.Items.AddRange(GlobalVars.VelocityScales);
                    break;
                case CurveType.Magnitude:
                    dbc.Items.Clear();
                    dbc.Items.AddRange(GlobalVars.MagnitudeScales);
                    break;
                case CurveType.Power:
                    dbc.Items.Clear();
                    dbc.Items.AddRange(GlobalVars.PowerScales);
                    break;
                default:
                    break;
            }
            return dbc;
        }

        private void UpdateGraph_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                if (dataGridView1.Columns.Contains(this.dbSymOnCol))
                {
                    dataGridView1.Columns.Remove(this.dbSymOnCol);
                }
                else
                {
                    dataGridView1.Columns.Add(this.dbSymOnCol);
                }
            }
        }

        private void CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            switch (c)
            {
                case 5:
                    System.Windows.Forms.ColorDialog cd = new ColorDialog();
                    if (cd.ShowDialog() == DialogResult.OK)
                    {
                        GraphDock.GraphInformation.CurveTypeItems[r].Curve.Color = cd.Color;
                    }
                    break;
                case 6:
                    if (dataGridView1.Rows[r].Cells[c].Value.Equals(true))
                    {
                        GraphDock.GraphInformation.CurveTypeItems[r].SymbolsOn = false;
                        ((LineItem)GraphDock.GraphInformation.CurveTypeItems[r].Curve).Symbol.IsVisible = false;
                    }
                    else
                    {
                        GraphDock.GraphInformation.CurveTypeItems[r].SymbolsOn = true;
                        ((LineItem)GraphDock.GraphInformation.CurveTypeItems[r].Curve).Symbol.IsVisible = true;
                    }
                    GraphDock.Refresh();
                    break;
            }
            UpdateView();
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ComboBox combo = e.Control as ComboBox;
            if (combo != null)
            {
                combo.SelectedIndexChanged -= new EventHandler(combo_SelectedIndexChanged);
                combo.SelectedIndexChanged += new EventHandler(combo_SelectedIndexChanged);
            }
        }

        void combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            GraphDock.ChangeScale(dataGridView1.CurrentCellAddress.Y, ((ComboBox)sender).Text);
        }
    }
}
