using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Collections;

namespace DockingAnalytics
{
    public partial class BaseDock : Form
    {
        private static BaseDock instance;
        private AnalyticsController Controller { get; set; }
        private List<Document> DockList { get; set; }
        private Form rmsDockForm;
        private DockPanel rmsDockPanel { get; set; }

        public static BaseDock Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new BaseDock();
                    return instance;
                }
                else
                {
                    return instance;
                }
            }
        }

        public BaseDock()
        {
            InitializeComponent();
            DockList = new List<Document>();
            dockPanel1.ActiveDocumentChanged += new EventHandler(ActiveDocumentChanged);
        }

        public void Register(AnalyticsController cont)
        {
            Controller = cont;
            Controller.FileOpenedEvent += new AnalyticsController.FileOpened(FileOpenedEvent);
            Controller.FileClosedEvent += new AnalyticsController.FileClosed(FileClosedEvent);
            Controller.GraphCreatedEvent += new AnalyticsController.GraphCreated(GraphCreatedEvent);
            Controller.UpdateGraphMainView.DockStateChanged += new EventHandler(UpdateGraphMainView_DockStateChanged);
        }

        bool once = false;
        void UpdateGraphMainView_DockStateChanged(object sender, EventArgs e)
        {
             //9889
            UpdateGraph ug = sender as UpdateGraph;
            //int i = -1;
            if (ug != null && !once)// && i > 1)
            {
                if (ug.DockState == DockState.Hidden) { return; }
                var topLeft = dockPanel1.Location;
                int ugWidth = ug.Pane.Size.Width;
                int ugHeight = ug.Pane.Size.Height;
                topLeft.X += (dockPanel1.Size.Width / 2 - ugWidth / 2);
                topLeft.Y += (dockPanel1.Size.Height / 2 - ugHeight / 2);
                ug.Show(dockPanel1, new Rectangle(topLeft, ug.Size));
                ug.Update();
                
            }
            once = true;
            //i++;
        }

        bool GraphCreatedEvent(AnalyticsController aC, GraphEventArgs e)
        {
            //e.CreatedGraph.Show(dockPanel1, DockState.Document);
            e.CreatedGraph.Show(dockPanel1);
            DockList.Add(e.CreatedGraph);
            //if (e.GraphType == GraphType.TwoD)
            //{
            //    UpdateGraph ug = new UpdateGraph();
            //    ug.DockTo(e.CreatedGraph.PanelPane.DockPanel, DockStyle.Bottom);
            //    ug.Show();
            //    ug.DockStateChanged += new EventHandler(UpdateGraphMainView_DockStateChanged);
            //}
            e.CreatedGraph.DockStateChanged += new EventHandler(CreatedGraph_DockStateChanged);
            return false;
        }


        /// <summary>
        /// TODO:Debug:::The DockStateChanged event occurs when the graph is either floated, or docked to the 
        /// original panel, it is not firing when it docks to a subsequent (floated) panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CreatedGraph_DockStateChanged(object sender, EventArgs e)
        {
            GraphDock gd = sender as GraphDock;
            //Console.WriteLine("Dock state changed" + gd.Text);
            //if (gd.DockPanel.Container == null)
            //{
            //    Console.WriteLine("Alone");
            //}
            //else
            //{
            //    Console.WriteLine(gd.DockPanel.Container.Components.Count);
            //}
            if (gd != null)
            {
                if (true)//(gd.DockState == DockState.Float)
                {
                    var topLeft = dockPanel1.Location;
                    topLeft.X += (dockPanel1.Size.Width / 2 - gd.Size.Width / 2);
                    topLeft.Y += (dockPanel1.Size.Height / 2 - gd.Size.Height / 2);
                    gd.Show(dockPanel1, new Rectangle(topLeft, gd.Size));
                    gd.Update(this);
                }
                else
                {
                    Controller.ActiveDocumentChanged(gd);
                }
            }
        }

        public void Update(DockContent dc,DockState dS)
        {
            dc.Show(dockPanel1, dS);
        }


        void ActiveDocumentChanged(object sender, EventArgs e)
        {
            String replacement = "File";

            GraphDock gp = dockPanel1.ActiveDocument as GraphDock;
            if (gp != null )
            {
                replacement = gp.GraphName;
                Controller.ActiveDocumentChanged(gp);

                Console.WriteLine("New active document:" + gp.GraphName);
            }
            //fileToolStripMenuItem.Text = System.IO.Path.GetFileName(replacement);
        }

        bool FileClosedEvent(AnalyticsController aC, EventArgs e)
        {
            throw new NotImplementedException();
        }


        private Document[] rmsDocs = new Document[4];
        private Document[] rmscDocs = new Document[4];
        int docks = 0;
        int rcdocks = 0;
        
        bool FileOpenedEvent(AnalyticsController aC, FileOpenEventArgs e)
        {
            dockPanel1.SuspendLayout(true);
            //if (rmsDockPanel == null)
            //{
            //    rmsDockPanel = new DockPanel();
            //    rmsDockPanel.Dock = DockStyle.Fill;
            //    rmsDockPanel.Show();
            //    if (rmsDockForm == null)
            //    {
            //        rmsDockForm = new Form();
            //        this.Controls.Add(rmsDockPanel);
            //        rmsDockForm.Controls.Add(rmsDockPanel);
            //        rmsDockPanel.Dock = DockStyle.Fill;
            //        rmsDockForm.Show();
            //    }
            //}

            graphsToolStripMenuItem.Enabled = true;
            tsmiAccelGraphs.Enabled = true;
            tsmiMagGraphs.Enabled = true;
            tsmiPowerGraphs.Enabled = true;
            tsmiVelocGraphs.Enabled = true;
            //e.FreqGraphDock.HideOnClose = true;

            if (e.RMSDock != null)
            {
                e.RMSDock.DockPanel = dockPanel1;
                if (e.RMSDock.DockPanel.DocumentStyle == DocumentStyle.DockingMdi)
                {
                    e.RMSDock.MdiParent = this;
                    //e.RMSDock.Show(dockPanel1, new Rectangle(98,133,200,383));
                }
                else
                {
                    //e.RMSDock.Show(dockPanel1, new Rectangle(98,133,200,383));
                }
                e.RMSDock.DockState = DockState.DockRight;
                e.RMSDock.Dock = DockStyle.None;
                //e.RMSDock.DockTo(rmsFP.Pane, DockStyle.Fill, 1);
                if (docks < 4)
                {
                    rmsDocs[docks] = e.RMSDock;
                    switch (docks)
                    {
                        case 0:
                            e.RMSDock.DockTo(rmsDocs[docks].Pane, DockStyle.Fill, 1);
                            break;
                        case 1:
                            e.RMSDock.DockTo(rmsDocs[0].Pane, DockStyle.Right, 1);
                            break;
                        case 2:
                            e.RMSDock.DockTo(rmsDocs[0].Pane, DockStyle.Bottom, 1);
                            break;
                        case 3:
                            e.RMSDock.DockTo(rmsDocs[1].Pane, DockStyle.Bottom, 1);
                            break;
                    }
                }
                else
                {
                    e.RMSDock.DockTo(rmsDocs[docks % 4].Pane, DockStyle.Fill, 1);
                }
                docks++;
            }




            if (e.RMSCDock != null && !e.RMSCDock.Visible)
            {
                e.RMSCDock.DockPanel = dockPanel1;
                if (e.RMSCDock.DockPanel.DocumentStyle == DocumentStyle.DockingMdi)
                {
                    e.RMSCDock.MdiParent = this;
                    e.RMSCDock.DockState = DockState.DockRight;
                    e.RMSCDock.Dock = DockStyle.None;
                }
            }

            

            e.FreqGraphDock.DockPanel = dockPanel1;
            if (e.FreqGraphDock.DockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                e.FreqGraphDock.MdiParent = this;
                e.FreqGraphDock.Show();
            }
            else
            {
                e.FreqGraphDock.Show(dockPanel1);
            }
            //e.FreqGraphDock.Show(this.dockPanel1, DockState.Document);
            e.FreqGraphDock.DockStateChanged += new EventHandler(FreqGraphDock_DockStateChanged);
            DockList.Add(e.FreqGraphDock);
            //e.TimeGraphDock.HideOnClose = true;
            e.TimeGraphDock.DockPanel = dockPanel1;
            if (e.TimeGraphDock.DockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                e.TimeGraphDock.MdiParent = this;
                e.TimeGraphDock.Show();
            }
            else
            {
                e.TimeGraphDock.Show();
            }
            //e.TimeGraphDock.Show(this.dockPanel1, DockState.Document);
            e.TimeGraphDock.DockStateChanged += new EventHandler(TimeGraphDock_DockStateChanged);
            DockList.Add(e.TimeGraphDock);
            DockList.Add(e.ThreeDFreqDock);
            DockList.Add(e.ThreeDTimeDock);
            
            //e.ThreeDTimeDock.Show(this.dockPanel1, DockState.Document);
            //e.ThreeDTimeDock.DockStateChanged += new EventHandler(ThreeDTimeDock_DockStateChanged);
            //e.ThreeDFreqDock.Show(this.dockPanel1, DockState.Document);
            //e.ThreeDFreqDock.DockStateChanged += new EventHandler(ThreeDFreqDock_DockStateChanged);

            dockPanel1.ResumeLayout(true,true);
            return false;
        }

        void GraphUpdateView_DockStateChanged(object sender, EventArgs e)
        {
            this.CreatedGraph_DockStateChanged(sender, e);
        }

        void ThreeDFreqDock_DockStateChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void ThreeDTimeDock_DockStateChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }


        void TimeGraphDock_DockStateChanged(object sender, EventArgs e)
        {
            this.CreatedGraph_DockStateChanged(sender, e);
        }

        void FreqGraphDock_DockStateChanged(object sender, EventArgs e)
        {
            this.CreatedGraph_DockStateChanged(sender, e);
        }

        private void OpenFileClick(object sender, EventArgs e)
        {
            Controller.Open();
        }

        private void closeAllTab(object sender, EventArgs e)
        {
            Controller.Exit();
        }

        private void CloseAllTabsEvent(object sender, EventArgs e)
        {
            this.CloseAll();
        }

        public void CloseAll()
        {
            if (dockPanel1.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                {
                    form.Close();
                }
            }
            else
            {
                for (int index = dockPanel1.Contents.Count - 1; index >= 0; index--)
                {
                    if (dockPanel1.Contents[index] is Document)
                    {
                        Document content = (Document)dockPanel1.Contents[index];
                        content.DockHandler.Close();
                        //content.Close();
                    }
                }
            }

            DockList.Clear();
            //foreach (DockWindow dw in dockPanel1.DockWindows)
            //{
            //    Remove
            ////}
            //foreach (Document gd in DockList)
            //{
            //    gd.Close();
            //    //this.
            //    //gd.Visible = false;
            //    //gd.Close();
            //}
            //DockList.Clear();
        }

        private void ViewGraphExplorer(object sender, EventArgs e)
        {
            Controller.ToggleView(DockType.GraphExplorer);
        }

        private void ViewFileExplorer(object sender, EventArgs e)
        {
            Controller.ToggleView(DockType.FileExplorer);
        }

        private void ViewBandInformation(object sender, EventArgs e)
        {
            Controller.ToggleView(DockType.UpdateBand);
        }

        private void ColorChangeClick(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            tsmi = tsmi.OwnerItem as ToolStripMenuItem;
            CurveType? type;
            if (tsmi.Text.Contains("Acceleration"))
            {
                type = CurveType.Acceleration;
            }
            else if (tsmi.Text.Contains("Velocity"))
            {
                type = CurveType.Velocity;
            }
            else if (tsmi.Text.Contains("Power"))
            {
                type = CurveType.Power;
            }
            else if (tsmi.Text.Contains("Magnitude"))
            {
                type = CurveType.Magnitude;
            }
            else
            {
                type = null;
                MessageBox.Show("Invalid Graph Type for Color Change", "Menu Error",
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
            System.Windows.Forms.ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                ChangeAllColors(type, cd.Color);
            }

        }

        private void ChangeAllColors(CurveType? type, Color color)
        {
            if (type == null)
            {
                return;
            }
            else
            {
                foreach (Document doc in Controller.DockList)
                {
                    GraphDock gd = doc as GraphDock;
                    if (gd != null)
                    {
                        gd.ChangeCurveTypeColor(type, color);
                    }

                }
            }
        }

        private void ScaleChangeClick(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            String scale = tsmi.Text;
            tsmi = tsmi.OwnerItem.OwnerItem as ToolStripMenuItem;
            CurveType? type;
            if (tsmi.Text.Contains("Acceleration"))
            {
                type = CurveType.Acceleration;
            }
            else if (tsmi.Text.Contains("Velocity"))
            {
                type = CurveType.Velocity;
            }
            else
            {
                type = null;
                MessageBox.Show("Invalid Graph Type for Scale Change", "Menu Error",
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
            ChangeAllScales(type, scale);
        }

        private void ChangeAllScales(CurveType? type, String scale)
        {
            if (type == null)
            {
                return;
            }
            else
            {
                foreach (Document doc in Controller.DockList)
                {
                    for (int i = 0; i < doc.GraphInformation.CurveTypeItems.Count; i++)
                    {
                        if (doc.GraphInformation.CurveTypeItems[i].Type == type)
                        {
                            doc.ChangeScale(i, scale);
                        }
                    }
                }
            }
        }

        private void SymbolChangeClick(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            String symbol = tsmi.Text;
            tsmi = tsmi.OwnerItem.OwnerItem as ToolStripMenuItem;
            CurveType? type; 
            if (tsmi.Text.Contains("Acceleration"))
            {
                type = CurveType.Acceleration;
            }
            else if (tsmi.Text.Contains("Velocity"))
            {
                type = CurveType.Velocity;
            }
            else if(tsmi.Text.Contains("Magnitude"))
            {
                type = CurveType.Magnitude;
            }
            else if (tsmi.Text.Contains("Power"))
            {
                type = CurveType.Power;
            }
            else
            {
                type = null;
                MessageBox.Show("Invalid Graph Type for Scale Change", "Menu Error",
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
            ChangeAllSymbols(type, symbol);
        }

        private void ChangeAllSymbols(CurveType? type, String symbol)
        {
            if (type == null)
            {
                return;
            }
            else
            {
                foreach (Document doc in Controller.DockList)
                {
                    for (int i = 0; i < doc.GraphInformation.CurveTypeItems.Count; i++)
                    {
                        if (doc.GraphInformation.CurveTypeItems[i].Type == type)
                        {
                            doc.ChangeSymbol(i, symbol);
                        }
                    }
                }
            }
        }

        private void ViewAllXClick(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi;
            GraphType dimSel;
            if (null != (tsmi = sender as ToolStripMenuItem))
            {
                String dS = tsmi.Text;
                if (dS.Contains("3"))
                {
                    dimSel = GraphType.ThreeD;
                }
                else
                {
                    dimSel = GraphType.TwoD;
                }
            }
            else
            {
                MessageBox.Show("Error in Viewing");
                return;
            }

            if (null != (tsmi = tsmi.OwnerItem as ToolStripMenuItem))
            {
                switch (tsmi.Text)
                {
                    case "Acceleration Graphs":
                        ShowAllGraphs(CurveType.Acceleration, dimSel);
                        break;
                    case "Velocity Graphs":
                        ShowAllGraphs(CurveType.Velocity, dimSel);
                        break;
                    case "FFT Magnitude Graphs":
                        ShowAllGraphs(CurveType.Magnitude, dimSel);
                        break;
                    case "FFT Power Graphs":
                        ShowAllGraphs(CurveType.Power, dimSel);
                        break;
                }
            }
        }

        private void ShowAllGraphs(CurveType type, GraphType dim)
        {
            if (dim == GraphType.ThreeD)
            {
                Controller.Create3DDocument(type);
            }
            else if (dim == GraphType.TwoD)
            {
                Controller.Create2DDocument(type);
            }
        }

        private void BaseDock_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphDock gp = dockPanel1.ActiveDocument as GraphDock;

            string adcSampleCaptureFileName = "{0}_GraphData_" + DateTime.Now.ToString("_MM_dd_yyyy_HH_mm_ss") + ".ixs";
            saveFile(InformationHolder.HighGainContainer().Data, String.Format(adcSampleCaptureFileName, "HighGain_"), InformationHolder.HighGainContainer().Gain);
            saveFile(InformationHolder.LowGainContainer().Data, String.Format(adcSampleCaptureFileName, "LowGain_"), InformationHolder.LowGainContainer().Gain);
        }

        private void saveFile(List<int> data, String filename, InformationHolder.GainType gain)
        {
            DockingAnalytics._xmlFile myFile = new DockingAnalytics._xmlFile(filename);
            ArrayList zedDataArrayList = new ArrayList();

            // TODO: For now, this is only adding the data that was collected by the recording to the file.
            // This will break existing expected functionality with existing graph saves (They are expected to 
            // save all data points that are currently graphed).
            // Probable solution: Separate save method for a graph without recorded data, and one for a graph with recorded data
            foreach (int datapoint in data)
            {
                double val = datapoint.ConvertToGs(gain);
                zedDataArrayList.Add(datapoint);
            }
            //for (int i = 0; i < gp.ZedGraphControl.GraphPane.CurveList[Controller.graphCurveListIndex].Points.Count; i++)
            //{
            //    zedDataArrayList.Add(gp.ZedGraphControl.GraphPane.CurveList[Controller.graphCurveListIndex].Points[i].Y);
            //}
            myFile.AddAccelerationValues(zedDataArrayList);
            myFile.dsSentry_data.sampling_Freq = DockingAnalytics.GlobalVars.AccelFreq; ;
            //myFile.dsSentry_data.accel_Vg_Calibration = DockingAnalytics.GlobalVars.AccelYScale;
            myFile.dsSentry_data.offset_Calibration = 1;
            //myFile.dsSentry_data.max_Freq_Res = DockingAnalytics.GlobalVars.MaxResFreq;
            //myFile.dsSentry_data.settings_Timestamp_Created = tss;
            //myFile.dsSentry_data.settings_Timestamp_LastEdit = DateTime.Now;
            //USB ADC Capture Write
            myFile.dsSentry_data.capture_type = "Manual";
            //myFile.dsSentry_data.node_Serial = GetAlphaNum();
            myFile.dsSentry_data.node_Serial = "12345";
            //myFile.dsSentry_data.gateway_Serial = GetAlphaNum();
            myFile.dsSentry_data.manufacturer_Name = "ADVIS";
            myFile.dsSentry_data.owner_Name = "Company ABC";
            myFile.dsSentry_data.location_Name_1 = "Factory A";
            //myFile.dsSentry_data.timestamp_Start = tss;
            myFile.dsSentry_data.timestamp_End = DateTime.Now;
            myFile.XmlWrite();
        }
    }
}
