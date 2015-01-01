using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using ZedGraph;
using System.Threading;
using System.Diagnostics;
using System.Drawing;

using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using LibUsbDotNet.Info;
using LibUsbDotNet.LudnMonoLibUsb;
using EC = LibUsbDotNet.Main.ErrorCode;

using System.Threading;

namespace DockingAnalytics
{
    public enum DockType
    {
        GraphExplorer,
        FileExplorer,
        Graph2D,
        Graph3D,
        UpdateBand,
        UpdateGraph,

    }

    public class FileOpenEventArgs
    {
        public GraphDock TimeGraphDock { get; private set; }
        public GraphDock FreqGraphDock { get; private set; }
        public SpectroGraphDock ThreeDTimeDock { get; private set; }
        public SpectroGraphDock ThreeDFreqDock { get; private set; }
        public RMSGaugeDoc RMSDock { get; private set; }
        public RMSChartDoc RMSCDock { get; private set; }

        public FileOpenEventArgs(params Document[] docs)//GraphDock time, GraphDock freq, SpectroGraphDock time3d, SpectroGraphDock freq3d)
        {
            TimeGraphDock = docs[0] as GraphDock;// time;
            FreqGraphDock = docs[1] as GraphDock;//freq;
            ThreeDTimeDock = docs[2] as SpectroGraphDock;//time3d;
            ThreeDFreqDock = docs[3] as SpectroGraphDock;//freq3d;
        }

    }

    public class AnalyticsController
    {
        private static AnalyticsController instance;


        //public BaseDock MainDockingForm { get { return BaseDock.Instance; } }
        private RMSChartDoc RMSChartingDock = new RMSChartDoc();
        public BaseDock MainDockingForm = BaseDock.Instance;
        private ExplorerDock FileExplorer = new ExplorerDock();
        private ExplorerDock GraphExplorer = new ExplorerDock();
        private UpdateBand UpDateBand = new UpdateBand();
        private UpdateGraph _UpdateGraphMainView = new UpdateGraph();
        private List<Document> GraphDockList = new List<Document>();
        private USBControlDock USBController = new USBControlDock();

        public UpdateGraph UpdateGraphMainView { get { return _UpdateGraphMainView; } set { _UpdateGraphMainView = value; } }
        private GraphEventArgs GraphEventInstance = new GraphEventArgs();
        public File OpenedFile { get; set; }
        public File ClosedFile { get; set; }
        public List<File> OpenFiles { get; set; }
        public List<Document> DockList { get { return GraphDockList; } set { GraphDockList = value; } }
        public int NumFiles { get; set; }


        //USB Stuff
        USBThreadClass USBClassObject;
        Thread USBWorkerThread;
        Thread GraphUpdaterThread;
        GraphUpdaterThreadClass GraphUpdater;

        public bool isReading = false;
        public UInt64 graphXIndex;
        public int GraphListComboBoxIndex = 0;
        public int graphCurveListIndex = 0;

        public static AnalyticsController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AnalyticsController();
                    //instance.MainDockingForm.Register(instance);
                } return instance;
            }

        }

        public AnalyticsController()
        {
            MainDockingForm.Register(this);
            FileExplorer.Register(this,ExplorerType.File);
            GraphExplorer.Register(this,ExplorerType.Graph);
            UpDateBand.Register(this);
            UpdateGraphMainView.Register(this);
            USBController.Register(this);
            MainDockingForm.Update(FileExplorer, DockState.DockRight);
            FileExplorer.AllowDrop = false;
            MainDockingForm.Update(GraphExplorer, DockState.DockRight);
            GraphExplorer.AllowDrop = false;
            MainDockingForm.Update(UpDateBand, DockState.DockBottom);
            MainDockingForm.Update(UpdateGraphMainView, DockState.DockBottom);
            MainDockingForm.Update(USBController, DockState.DockBottom); 
            
            OpenFiles = new List<File>();
            //Application.Run(MainDockingForm);
            this.FileOpenedEvent += new FileOpened(AnalyticsController_FileOpenedEvent);


            //Andrew
            USBController.USBSetConsoleTextboxThreadSafe("Welcome. Begin by" + 
                Environment.NewLine + "1. Plugging in your USB device." + 
                Environment.NewLine + "2. Select Open USB Device" + 
                Environment.NewLine + "3. Select New 2D Graph." + 
                Environment.NewLine + "4. Select a graph in the drop-down. " + 
                Environment.NewLine + "5. Start/stop Reading!" + 
                Environment.NewLine + "6. Select File > Save, then File > Open and navigate to the same directory as the exe" +
                Environment.NewLine);
            USBController.USBSetConsoleTextboxThreadSafe("There Will Be Bugs! =)" + Environment.NewLine);
            
        }

        public void Register(BaseDock activeBaseDock)
        {
            activeBaseDock.Register(this);
            activeBaseDock.Update(FileExplorer, DockState.DockRight);
            activeBaseDock.Update(GraphExplorer, DockState.DockRight);
            activeBaseDock.Update(UpDateBand, DockState.DockBottom);
            activeBaseDock.Update(UpdateGraphMainView, DockState.DockBottom);
        }

        public void ActiveDocumentChanged(GraphDock activeGraphDock)
        {
            //UpdateGraphMainView.Close();
            UpdateGraphMainView.Register(activeGraphDock);
            UpdateGraphMainView.UpdateView();

            MainDockingForm.Update(UpdateGraphMainView, DockState.DockBottom);
            Console.WriteLine("Active Document Changed");
        }

        bool AnalyticsController_FileOpenedEvent(AnalyticsController aC, FileOpenEventArgs e)
        {
            return false;
            //throw new NotImplementedException();
        }

        public void ToggleView(DockType dt)
        {
            switch (dt)
            {
                case DockType.FileExplorer:
                    MainDockingForm.Update(FileExplorer, DockState.DockRight);
                    break;
                case DockType.GraphExplorer:
                    MainDockingForm.Update(GraphExplorer, DockState.DockRight);
                    break;
                case DockType.UpdateBand:
                    MainDockingForm.Update(UpDateBand,DockState.DockBottom);
                    break;
                default:
                    break;
            }
        }

        public void Open(string filename)
        {
            File nFile = new File(filename);
            Open(nFile);
        }

        private void Open(File oFile)
        {
            OpenedFile = oFile;
            OpenFiles.Add(oFile);
            GraphDock gd = new GraphDock();
            gd.GraphName = OpenedFile.FileName;
            gd.Text = "Time-" + oFile.FileName;
            gd.AddCurve(OpenedFile.AccelerationData, oFile.FileName);
            gd.AddCurve(OpenedFile.VelocityData, oFile.FileName);
            DockList.Add(gd);
            // TODO: Add from xmlFile the RMS optimum vaue and the maximum value.
            GraphEventInstance.CreatedGraph = gd;
            OnGraphCreated(GraphEventInstance);

            /*if (RMSChartingDock == null)
            {
                RMSChartingDock = new RMSChartDoc();
            }
                
            RMSChartingDock.AddRMSValues(System.IO.Path.GetFileName(OpenedFile.FileName), OpenedFile.VelocityData.RMS, true, (uint)(new Random().Next(3)));
            if (string.IsNullOrEmpty(RMSChartingDock.Text))
            {
                RMSChartingDock.Text = "RMS Values";
                DockList.Add(RMSChartingDock);
            }*/
            GraphDock gd2 = new GraphDock();
            gd2.GraphName = OpenedFile.FileName;
            gd2.Text = "FFT-" + oFile.FileName;
            gd2.AddCurve(OpenedFile.MagnitudeData,oFile.FileName);
            gd2.AddCurve(OpenedFile.PowerData,  oFile.FileName);
            // TODO: Add any bands that appear in the file to the graph
            DockList.Add(gd2);

            GraphEventInstance.CreatedGraph = gd2;
            OnGraphCreated(GraphEventInstance);

            //foreach (_xmlFile._dsSentry_data._band band in oFile.XMLFile.dsSentry_data.bandList)
            //{
            //    ZedGraph.BandObjList.Add(new BandObj(band.center_Freq - band.bandwidth / 2, band.bandwidth, OpenedFile.MagnitudeData.Curve, gd2.ZedGraphControl, band.TimeStampCreated, DateTime.Now, band.quant_Level), gd2.ZedGraphControl);
            //}
            //SpectroGraphDock sp = new SpectroGraphDock();
            //sp.GraphName = OpenedFile.FileName;
            //sp.AddCurve(OpenedFile.AccelerationData.Curve, CurveType.Acceleration, oFile.FileName);
            //sp.AddCurve(OpenedFile.VelocityData.Curve, CurveType.Velocity, oFile.FileName);
            //DockList.Add(sp);
            //SpectroGraphDock sp2 = new SpectroGraphDock();
            //sp2.GraphName = OpenedFile.FileName;
            //sp2.AddCurve(OpenedFile.MagnitudeData.Curve, CurveType.Magnitude, oFile.FileName);
            //sp2.AddCurve(OpenedFile.PowerData.Curve, CurveType.Power, oFile.FileName);
            //DockList.Add(sp2);
            //RMSGaugeDoc rmsGD = new RMSGaugeDoc(OpenedFile.VelocityData.RMSPref, OpenedFile.VelocityData.RMS, OpenedFile.VelocityData.RMSMax);
            //rmsGD.Text = System.IO.Path.GetFileName(OpenedFile.FileName);

            FileOpenEventArgs foea = new FileOpenEventArgs(gd, gd2, null, null);//sp, sp2);
            OnFileOpenEvent(foea);
        }

        public void Open()
        {
            String filename = null;
            OpenFileDialog brsw = new OpenFileDialog();
            brsw.Title = "Choose an .xml file to read the acceleration information from:";
            brsw.InitialDirectory = "C://Users//Simpson//Desktop//Vibration Test Samples";
            brsw.Filter = "IXSense Files (.ixs)|*.ixs|XML Files (.xml)|*.xml";
            if (brsw.ShowDialog() == DialogResult.OK )
            {
                filename = brsw.FileName;
            }
            else
            {
                MessageBox.Show("No file selected. Try Again.");
                return;
            }
            File nFile = null;
            try
            {
                nFile = new File(filename);
            }
            catch (Exception e)
            {
                MessageBox.Show("File selected was in an incorrect format");
            }
            if (nFile != null)
            {
                Open(nFile);
            }
            //nFile = new File(filename.Substring(0, filename.LastIndexOf('.')) + "1.xml");
            //Open(nFile);
            //nFile = new File(filename.Substring(0, filename.LastIndexOf('.')) + "2.xml");
            //Open(nFile);
            //nFile = new File(filename.Substring(0, filename.LastIndexOf('.')) + "3.xml");
            //Open(nFile);
            //nFile = new File(filename.Substring(0, filename.LastIndexOf('.')) + "4.xml");
            //Open(nFile);
            //nFile = new File(filename.Substring(0, filename.LastIndexOf('.')) + "5.xml");
            //Open(nFile);
            //nFile = new File(filename.Substring(0, filename.LastIndexOf('.')) + "6.xml");
            //Open(nFile);
        }

        /// <summary>
        /// Method for closing/exiting the application. 
        /// Since this instance of this class is waiting for the MainDockingForm
        /// to stop running before it closes, it just needs to close the 
        /// MainDockingForm in order to close itself.
        /// </summary>
        public void Exit()
        {
            MainDockingForm.Close();
        }

        public void CloseAll()
        {
            MainDockingForm.CloseAll();
        }

        public void Create2DDocument(List<TypeIndexPair> tipsList)
        {
            GraphEventInstance.GraphType = GraphType.TwoD;
            GraphDock gd = new GraphDock();
            int index = 0;
            if (tipsList.Count == 1)
            {
                switch (tipsList[0].GraphType)
                {
                    case CurveType.Acceleration:
                        GraphEventInstance.CreatedGraph = OpenFiles[tipsList[0].Index].AccelerationData.CreateGraphDock();
                        break;
                    case CurveType.Magnitude:
                        GraphEventInstance.CreatedGraph = OpenFiles[tipsList[0].Index].MagnitudeData.CreateGraphDock();
                        break;
                    case CurveType.Power:
                        GraphEventInstance.CreatedGraph = OpenFiles[tipsList[0].Index].PowerData.CreateGraphDock();
                        break;
                    case CurveType.Velocity:
                        GraphEventInstance.CreatedGraph = OpenFiles[tipsList[0].Index].VelocityData.CreateGraphDock();
                        break;
                }
                GraphEventInstance.UserGraphRemoved();
                String fN = OpenFiles[tipsList[0].Index].FileName;
                gd.Text = tipsList[0].GraphType.ToString() + " " + System.IO.Path.GetFileName(fN);
            }
            else
            {
                foreach (TypeIndexPair tip in tipsList)
                {
                    String curveName = tip.GraphType.ToString() + " " + tip.FileName;
                    Console.WriteLine(tip.GraphType.ToString() + " at index: " + tip.Index);
                    index = tip.Index;
                    switch (tip.GraphType)
                    {
                        case CurveType.Acceleration:
                            gd.AddCurve(OpenFiles[index].AccelerationData,curveName);
                            //gd.AddCurve(OpenFiles[index].AccelerationData.Curve,CurveType.Acceleration,curveName);
                            break;
                        case CurveType.Velocity:
                            gd.AddCurve(OpenFiles[index].VelocityData,curveName);
                           // gd.AddCurve(OpenFiles[index].VelocityData.Curve,CurveType.Velocity,curveName);
                            break;
                        case CurveType.Magnitude:
                            gd.AddCurve(OpenFiles[index].MagnitudeData,curveName);
                           // gd.AddCurve(OpenFiles[index].MagnitudeData.Curve,CurveType.Magnitude,curveName);
                            break;
                        case CurveType.Power:
                            gd.AddCurve(OpenFiles[index].PowerData,curveName);
                            //gd.AddCurve(OpenFiles[index].PowerData.Curve,CurveType.Power,curveName);
                            break;
                        default:
                            break;
                    }
                }


            }

            //GraphDock gdd = new GraphDock();
            //LineItem curve = new GraphPane().AddCurve("Black", new PointPairList(new double[] { 0, 1, 2, 3, 4, 5 }, new double[] { 0, 1, 2, 3, 4, 5 }), Color.Blue);
            //gdd.AddCurve(curve, CurveType.Acceleration, "Black");
            //GraphDockList.Add(gdd);

            //GraphEventInstance.CreatedGraph = gdd;
            //OnGraphCreated(GraphEventInstance);

            gd.Name = "Graph " + System.DateTime.Now.TimeOfDay;
            gd.Text = gd.Name;
            gd.GraphName = gd.Name;

            GraphDockList.Add(gd);
            GraphEventInstance.CreatedGraph = gd;
            OnGraphCreated(GraphEventInstance);
        }

        public void Create3DDocument(List<TypeIndexPair> tipsList)
        {
            GraphEventInstance.GraphType = GraphType.ThreeD;
            SpectroGraphDock sd = new SpectroGraphDock();
            int index = 0;
            double xmax = 0;
            PointPairList temp;

            foreach (TypeIndexPair tip in tipsList)
            {
                String curveName = tip.GraphType.ToString() + " " + tip.FileName;
                Console.WriteLine(tip.GraphType.ToString() + " at index: " + tip.Index);
                index = tip.Index;
                switch (tip.GraphType)
                {
                    case CurveType.Acceleration:
                        sd.AddCurve(OpenFiles[index].AccelerationData, curveName);
                        temp = OpenFiles[index].AccelerationData.PointPairList;
                        xmax = Math.Max(xmax, temp[temp.Count - 1].X);
                        break;
                    case CurveType.Velocity:
                        sd.AddCurve(OpenFiles[index].VelocityData, curveName);
                        temp = OpenFiles[index].VelocityData.PointPairList;
                        xmax = Math.Max(xmax, temp[temp.Count - 1].X);
                        break;
                    case CurveType.Magnitude:
                        sd.AddCurve(OpenFiles[index].MagnitudeData, curveName);
                        temp = OpenFiles[index].VelocityData.PointPairList;
                        xmax = Math.Max(xmax, temp[temp.Count - 1].X);
                        break;
                    case CurveType.Power:
                        sd.AddCurve(OpenFiles[index].PowerData, curveName);
                        temp = OpenFiles[index].VelocityData.PointPairList;
                        xmax = Math.Max(xmax, temp[temp.Count - 1].X);
                        break;
                    default:
                        break;
                }
            }
            GraphEventInstance.CreatedGraph = sd;
            sd.Text = "User Graph " + GraphEventInstance.NumberOfUserCreatedGraphs;
            sd.GraphName = sd.Text;
            sd.UpdateMenu();
            GraphDockList.Add(sd);
            OnGraphCreated(GraphEventInstance);
        }

        public void Create2DDocument(CurveType type)
        {
            GraphDock gd = new GraphDock();

            foreach (File f in OpenFiles)
            {
                var curveList = from curve in f.InfoList where curve.Type == type select curve;
                foreach (CurveInfo c in curveList)
                {
                    //gd.AddCurve(c.Curve, c.Type, f.FileName);
                }
            }
            gd.Name = "Graph " + System.DateTime.Now.TimeOfDay;
            gd.Text = gd.Name;
            gd.GraphName = gd.Name;

            GraphDockList.Add(gd);
            GraphEventInstance.CreatedGraph = gd;
            OnGraphCreated(GraphEventInstance);
        }

        public void Create3DDocument(CurveType type)
        {
            SpectroGraphDock sd = new SpectroGraphDock();
            double xmax = Double.MinValue;

            foreach (File f in OpenFiles)
            {
                var curveList = from curve in f.InfoList where curve.Type == type select curve;
                foreach (CurveInfo c in curveList)
                {
                    if (c.PointPairList[c.PointPairList.Count - 1].X > xmax)
                    {
                        xmax = c.PointPairList[c.PointPairList.Count - 1].X;
                    }
                    sd.AddCurve(c, f.FileName);
                    //sd.AddCurve(c.Curve, c.Type, f.FileName);
                }
            }
            GlobalVars.XMAX = xmax;
            sd.Text = type + " Graphs";
            sd.GraphName = sd.Text;
            sd.UpdateMenu();
            GraphDockList.Add(sd);
            GraphEventInstance.CreatedGraph = sd;
            OnGraphCreated(GraphEventInstance);
        }

        private void ProcessInFile(String fileName)
        {

        }

        public delegate bool GraphCreated(AnalyticsController aC, GraphEventArgs e);

        public delegate bool FileOpened(AnalyticsController aC, FileOpenEventArgs e);

        public delegate bool FileClosed(AnalyticsController aC, EventArgs e);

        [Bindable(true), Category("Events"),
         Description("Subscribe to be notified when the user creates a new Graph")]
        public event GraphCreated GraphCreatedEvent;
        
        [Bindable(true), Category("Events"),
        Description("Subscribe to be notified when the user opens a new File")]
        public event FileOpened FileOpenedEvent;

        [Bindable(true), Category("Events"),
         Description("Subscribe to be notified when the user closes an existing File.")]
        public event FileClosed FileClosedEvent;

        protected virtual void OnGraphCreated(GraphEventArgs e)
        {
            if (GraphCreatedEvent != null)
            {
                GraphCreatedEvent(this, e);
            }
        }

        protected virtual void OnFileOpenEvent(FileOpenEventArgs e)
        {
            if (FileOpenedEvent != null)
            {
                FileOpenedEvent(this, e);
            }
        }

        protected virtual void OnFileCloseEvent(EventArgs e)
        {
            if (FileClosedEvent != null)
            {
                FileClosedEvent(this, e);
            }
        }

        public string GetFileNameFromPath(String path)
        {
            string filename = "";
            string[] patharray = path.Split('/');
            filename = patharray[patharray.Length - 1];
            return filename;
        }

        public bool USBAnalytics_OpenUSB()
        {
            USBClassObject = new USBThreadClass(this);

            if (!USBClassObject.OpenUSB())
                return false;
            GraphUpdater = new GraphUpdaterThreadClass(this);
            GraphUpdaterThread = new Thread(new ThreadStart(GraphUpdater.startUpdating));

            USBWorkerThread = new Thread(new ThreadStart(USBClassObject.StartReading));
            GraphUpdaterThread = new Thread(new ThreadStart(GraphUpdater.startUpdating));
            return true;
        }

        public void USBAnalytics_CloseUSB()
        {
            USBClassObject.closeDevice();
        }

        public void USBAnalytics_ReadUSB()
        {
            if (USBClassObject == null)
            {
                USBAnalytics_OpenUSB();
            }
            if (!USBClassObject.isOpen)
            {
                MessageBox.Show("USB must be opened first");
                return;
            }

            //first make sure there is a curve to add data to
            GraphDock dock = (GraphDock)GraphDockList[GraphListComboBoxIndex];
            graphCurveListIndex = 0;
            dock.ZedGraphControl.GraphPane.XAxis.Title.Text = "Sample Number";
            dock.ZedGraphControl.GraphPane.YAxis.Title.Text = "Acceleration";
            if (dock.ZedGraphControl.GraphPane.CurveList.Count > 0)
            {
                foreach (CurveItem curve in dock.ZedGraphControl.GraphPane.CurveList)
                {
                    if (curve.Tag == "USB") //use the first curve we find that is tagged with USB
                    {
                        break;
                    }
                    graphCurveListIndex++;
                }
            }
            
            if(dock.ZedGraphControl.GraphPane.CurveList.Count == 0 || graphCurveListIndex > dock.ZedGraphControl.GraphPane.CurveList.Count)
            {
                dock.ZedGraphControl.GraphPane.CurveList.Clear(); //TODO start fresh?

                InformationHolder.HighGainContainer().zedGraphData.Add(new PointPairList());
                InformationHolder.LowGainContainer().zedGraphData.Add(new PointPairList());

                //Have "low" = index 0 and "high" = index 1
                LineItem lowGainZedGraphCurve = dock.ZedGraphControl.GraphPane.AddCurve("USB Low Gain Accel", InformationHolder.LowGainContainer().zedGraphData[GraphListComboBoxIndex], Color.Red, SymbolType.None);
                lowGainZedGraphCurve.Tag = "USB Low Gain";
                lowGainZedGraphCurve.Line.Width = 1.5F;
                lowGainZedGraphCurve.Symbol.Fill = new Fill(Color.White);
                lowGainZedGraphCurve.Symbol.Size = 5;
                lowGainZedGraphCurve.Label.IsVisible = false;

                // Set up high gain curve
                LineItem highGainZedGraphCurve = dock.ZedGraphControl.GraphPane.AddCurve("USB High Gain Accel", InformationHolder.HighGainContainer().zedGraphData[GraphListComboBoxIndex], Color.DeepSkyBlue, SymbolType.None);
                highGainZedGraphCurve.Tag = "USB High Gain";
                highGainZedGraphCurve.Line.Width = 1.5F;
                highGainZedGraphCurve.Symbol.Fill = new Fill(Color.White);
                highGainZedGraphCurve.Symbol.Size = 5;
                highGainZedGraphCurve.Label.IsVisible = false;

                //dock.ZedGraphControl.GraphPane.YAxis.Scale.Min = -66000;
                //dock.ZedGraphControl.GraphPane.YAxis.Scale.Max = 66000;
                dock.ZedGraphControl.GraphPane.XAxis.Scale.MinAuto = true;
                dock.ZedGraphControl.GraphPane.XAxis.Scale.MaxAuto = true;
                dock.ZedGraphControl.AxisChange();
            }

            dock.UpdateZedGraphViewThreadSafe(USBController.gain);
            isReading = true;

            if (USBWorkerThread.ThreadState == System.Threading.ThreadState.Stopped)
            {
                USBWorkerThread = new Thread(new ThreadStart(USBClassObject.StartReading));
                GraphUpdaterThread = new Thread(new ThreadStart(GraphUpdater.startUpdating));
            }
            USBWorkerThread.Start();
            GraphUpdaterThread.Start();
        }

        public void USBAnalytics_StopUSB()
        {
            isReading = false;
            USBClassObject.RequestStop();
            if (GraphUpdater != null)
            {
                GraphUpdater.RequestStop();
            }
        }

        public void AnalyticsSetConsoleTextboxThreadSafe(string text)
        {
            USBController.USBSetConsoleTextboxThreadSafe(text);
        }

        public void UpdateUSBGraph()
        {

            if (isReading)
            {
                GraphDock dock = (GraphDock)GraphDockList[GraphListComboBoxIndex];
                PointPairList highGainZedGraphDataList = InformationHolder.HighGainContainer().zedGraphData[GraphListComboBoxIndex];
                PointPairList lowGainZedGraphDataList = InformationHolder.LowGainContainer().zedGraphData[GraphListComboBoxIndex];
                //dock.ZedGraphControl.GraphPane.CurveList[graphCurveListIndex].Points = zedGraphData[graphCurveListIndex];
                // zedGraphDataList.FilterData(dock.ZedGraphControl.GraphPane, dock.ZedGraphControl.GraphPane.XAxis, dock.ZedGraphControl.GraphPane.YAxis);

                //dock.UpdateZedGraphThreadSafe(InformationHolder.HighGainContainer().zedGraphData[GraphListComboBoxIndex]);
                //dock.UpdateZedGraphThreadSafe(InformationHolder.LowGainContainer().zedGraphData[GraphListComboBoxIndex]);

                //Update which data is viewed based on the radio buttons
                if (this.USBController.gain == USBControlDock.ChannelGain.Both)
                {
                    dock.UpdateZedGraphThreadSafe(InformationHolder.HighGainContainer().zedGraphData[GraphListComboBoxIndex],
                                                  USBControlDock.ChannelGain.High);
                    dock.UpdateZedGraphThreadSafe(InformationHolder.LowGainContainer().zedGraphData[GraphListComboBoxIndex],
                                                  USBControlDock.ChannelGain.Low);
                }
                else if (this.USBController.gain == USBControlDock.ChannelGain.High)
                {
                    dock.UpdateZedGraphThreadSafe(InformationHolder.HighGainContainer().zedGraphData[GraphListComboBoxIndex],
                                                    USBControlDock.ChannelGain.High);
                }
                else if (this.USBController.gain == USBControlDock.ChannelGain.Low)
                {
                    dock.UpdateZedGraphThreadSafe(InformationHolder.LowGainContainer().zedGraphData[GraphListComboBoxIndex],
                                                    USBControlDock.ChannelGain.Low);
                }
            }
        }

        public void UpdateUSBGraphGainView(USBControlDock.ChannelGain gainView)
        {
            if (GraphDockList.Count > 0)
            {
                GraphDock dock = (GraphDock)GraphDockList[GraphListComboBoxIndex];
                if(dock.ZedGraphControl.GraphPane.CurveList.Count > 0)
                    dock.UpdateZedGraphViewThreadSafe(gainView);
            }
        }
    }




    /************************************************************************************
     * 
     * USB Classes
     * 
     * 
     * **********************************************************************************/
    public class USBThreadClass
    {

        List<Byte> rawAccelData;
        List<Int16> parsedData;
        AnalyticsController Controller;

        public static UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(2235, 10688);

        /// <summary>Use the first read endpoint</summary>
        public static readonly byte TRANFER_ENDPOINT = UsbConstants.ENDPOINT_DIR_MASK;

        /// <summary>Number of transfers to sumbit before waiting begins</summary>
        public static readonly int TRANFER_MAX_OUTSTANDING_IO = 3;

        /// <summary>Number of transfers before terminating the test</summary>
        public static readonly int TRANSFER_COUNT = 1;

        /// <summary>Size of each transfer</summary>
        public static int TRANFER_SIZE = 20480;

        private static DateTime mStartTime = DateTime.MinValue;
        public static UsbDevice MyUsbDevice;

        static UsbEndpointReader reader;
        static UsbInterfaceInfo usbInterfaceInfo = null;
        static UsbEndpointInfo usbEndpointInfo = null;
        private static int mTransferCount = 0;
        private bool _shouldStopUSB = false;
        public bool isOpen = false;
        public USBThreadClass(AnalyticsController controller)
        {
            Controller = controller;
            rawAccelData = new List<Byte>();
            parsedData = new List<Int16>();

            //TEST
            for (int i = 0; i < 4800; i++)
            {
                short y = (short)(Math.Sin(i * Math.PI / 15.0) * 16.0);
                parsedData.Add(y);
            }
        }

        public bool OpenUSB()
        {
            
            ErrorCode ec = ErrorCode.None;
            try
            {
                // Find and open the usb device.
                UsbRegDeviceList regList =
                    UsbDevice.AllDevices.FindAll(MyUsbFinder);
                if (regList.Count == 0) throw new Exception("Device Not Found.");

                // Look through all conected devices with this vid and pid until
                // one is found that has and and endpoint that matches TRANFER_ENDPOINT.
                //
                foreach (UsbRegistry regDevice in regList)
                {
                    if (regDevice.Open(out MyUsbDevice))
                    {
                        if (MyUsbDevice.Configs.Count > 0)
                        {
                            // if TRANFER_ENDPOINT is 0x80 or 0x00, LookupEndpointInfo will return the 
                            // first read or write (respectively).
                            if (UsbEndpointBase.LookupEndpointInfo(MyUsbDevice.Configs[0], TRANFER_ENDPOINT,
                                out usbInterfaceInfo, out usbEndpointInfo))
                                break;

                            MyUsbDevice.Close();
                            MyUsbDevice = null;
                        }
                    }
                }

                // If the device is open and ready
                if (MyUsbDevice == null) throw new Exception("Device Not Found.");

                // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                // it exposes an IUsbDevice interface. If not (WinUSB) the 
                // 'wholeUsbDevice' variable will be null indicating this is 
                // an interface of a device; it does not require or support 
                // configuration and interface selection.
                IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    // This is a "whole" USB device. Before it can be used, 
                    // the desired configuration and interface must be selected.

                    // Select config #1
                    if (!wholeUsbDevice.SetConfiguration(1))
                        throw new Exception("Failed to set configuration.");

                    // Claim interface.
                    if (!wholeUsbDevice.ClaimInterface(usbInterfaceInfo.Descriptor.InterfaceID))
                        throw new Exception("Failed to claim interface 2.");

                    if (!wholeUsbDevice.SetAltInterface(1))
                        throw new Exception("Failed to set alternate interface.");
                }

                // open read endpoint.
                reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep04,
                    //(ReadEndpointID)usbEndpointInfo.Descriptor.EndpointID,
                0,
                (EndpointType)(usbEndpointInfo.Descriptor.Attributes & 0x3));

                if (ReferenceEquals(reader, null))
                {
                    throw new Exception("Failed locating read endpoint.");
                }
                else
                    Controller.AnalyticsSetConsoleTextboxThreadSafe("Device Opened." + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Controller.AnalyticsSetConsoleTextboxThreadSafe((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
                return false;
            }
            isOpen = true;
            return true;
        }

        public void closeDevice()
        {
            if (MyUsbDevice != null)
            {
                if (MyUsbDevice.IsOpen)
                {
                    // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                    // it exposes an IUsbDevice interface. If not (WinUSB) the 
                    // 'wholeUsbDevice' variable will be null indicating this is 
                    // an interface of a device; it does not require or support 
                    // configuration and interface selection.
                    IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // Release interface #0.
                        wholeUsbDevice.ReleaseInterface(0);

                        wholeUsbDevice.ReleaseInterface(1);
                    }

                    MyUsbDevice.Close();
                }
                MyUsbDevice = null;
            }
            isOpen = false;

            // Free usb resources
            UsbDevice.Exit();
        }


        int ch1SampleCounter = 0, ch2SampleCounter = 0;
        public void StartReading()
        {
            bool channelOne;
            ErrorCode ec = new ErrorCode();
            try
            {
                reader.Reset();
                    // The benchmark device firmware works with this example but it must be put into PC read mode.
#if IS_BENCHMARK_DEVICE
                int transferred;
                byte[] ctrlData = new byte[1];
                UsbSetupPacket setTestTypePacket =
                    new UsbSetupPacket((byte)(UsbCtrlFlags.Direction_In | UsbCtrlFlags.Recipient_Device | UsbCtrlFlags.RequestType_Vendor),
                        0x0E, 0x01, usbInterfaceInfo.Descriptor.InterfaceID, 1);
                MyUsbDevice.ControlTransfer(ref setTestTypePacket, ctrlData, 1, out transferred);
#endif
                TRANFER_SIZE -= (TRANFER_SIZE % usbEndpointInfo.Descriptor.MaxPacketSize);

                mTransferCount = 0;
                UsbTransferQueue transferQeue = new UsbTransferQueue(reader,
                                                                        TRANFER_MAX_OUTSTANDING_IO,
                                                                        TRANFER_SIZE,
                                                                        5000,
                                                                        usbEndpointInfo.Descriptor.MaxPacketSize);


                do
                {
                    UsbTransferQueue.Handle handle;

                    // Begin submitting transfers until TRANFER_MAX_OUTSTANDING_IO has benn reached.
                    // then wait for the oldest outstanding transfer to complete.
                    //
                    ec = transferQeue.Transfer(out handle);
                    if (ec != ErrorCode.Success)
                        throw new Exception("Failed getting async result: " + ec.ToString());

                    // Show some information on the completed transfer.

                    int seed;
                    int i = AppSettings.InitialHeader;
                    seed = 0;
                    channelOne = true;

                    int footerCheck = 0;
                    for (int j = 192; j < handle.Data.Length + 3; j+=196)
                    {
                        footerCheck += handle.Data[j] + handle.Data[j + 1] + handle.Data[j + 2] + handle.Data[j + 3];
                    }
                    // We've seen the four byte 0 delimiters start at byte 48 and at byte 96. This should reasonably detect where.
                    bool delimAt96 = footerCheck == 0;


                    int ch1Counter = 0, ch2Counter = 0;
                    for (i = AppSettings.InitialHeader; i < handle.Data.Length; i += 2)
                    {
                        if (i != 0 && ((delimAt96 && i % 192 == 0) || (!delimAt96 && (i + 48 * 2) % 192 == 0) || (!delimAt96 && i == 48 * 2)))
                        {
                            i += 4;
                            break;
                        }

                        if (channelOne)
                        {
                            InformationHolder.HighGainContainer().Add(Controller.GraphListComboBoxIndex, ch1SampleCounter++, (Int16)((handle.Data[i]) + (handle.Data[i + 1] << 8)));
                            ch1Counter+=2;
                        }
                        else
                        {
                            InformationHolder.LowGainContainer().Add(Controller.GraphListComboBoxIndex, ch2SampleCounter++, (Int16)((handle.Data[i]) + (handle.Data[i + 1] << 8)));
                            ch2Counter+=2;
                        }

                        if (ch1Counter == AppSettings.ChannelOneOffset)
                        {
                            ch1Counter = 0;
                            channelOne = !channelOne;
                            i += AppSettings.ChannelTwoHeader;
                        }
                        if (ch2Counter == AppSettings.ChannelTwoOffset)
                        {
                            ch2Counter = 0;
                            channelOne = !channelOne;
                            i += AppSettings.ChannelOneHeader;
                        }
                    }
                    //_shouldStopUSB = true;


                } while (!_shouldStopUSB);

                //Send Parsed Data to MainForm
                //MainForm.UpdateZedGraph();

                // Cancels any oustanding transfers and free's the transfer queue handles.
                // NOTE: A transfer queue can be reused after it's freed.
                transferQeue.Free();
                rawAccelData.Clear();
                Controller.AnalyticsSetConsoleTextboxThreadSafe("Done!" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Controller.AnalyticsSetConsoleTextboxThreadSafe((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
            }
            _shouldStopUSB = false;
            Thread.CurrentThread.Abort();
        }

        public void ProcessUSBData()
        {

        }

        public void RequestStop()
        {
            if (_shouldStopUSB)
            {
                //error
            }
            else
                _shouldStopUSB = true;
        }

        public void StartTestReading()
        {
            //while (true)
            //{
                for (int i = 0; i < parsedData.Count; i++)
                {
                    //InformationHolder.Instance().zedGraphData.Add(Controller.graphXIndex++, parsedData[i]);
                }
                //Controller.UpdateZedGraphThreadSafe();
                Controller.UpdateUSBGraph();
                //Thread.Sleep(100);
            //}
        }
    }


    /// <summary>
    /// Class for getting data from the USB Thread and giving it to the MainForm thread.
    /// </summary>
    public class GraphUpdaterThreadClass
    {

        AnalyticsController Controller;
        private bool _shouldStopGraph = false;
        public GraphUpdaterThreadClass(AnalyticsController controller)
        {
            Controller = controller;
        }
        public void Start()
        {
        }

        public void startUpdating()
        {
            while (!_shouldStopGraph)
            {
                //Give the data away
                //zedGraphDataList = InformationHolder.Instance().zedGraphData;
                //zedGraphCurve.Points = zedGraphDataList;
                //zedGraphDataList.FilterData(ZedGraphFrontPage.GraphPane, ZedGraphFrontPage.GraphPane.XAxis, ZedGraphFrontPage.GraphPane.YAxis);

                 

                Thread.Sleep(250); //Time in ms
                Controller.UpdateUSBGraph();
                
            }
            _shouldStopGraph = false;
            Thread.CurrentThread.Abort();
        }

        public void RequestStop()
        {
            if (_shouldStopGraph)
            {
                //error
            }
            else
            _shouldStopGraph = true;
        }
    }

    /// <summary>
    /// Class for holding onto the graph data for the MainForm.
    /// </summary>
    public class InformationHolder
    {
        public enum GainType
        {
            HighGain,
            LowGain
        }

        public GainType Gain {get;set;}

        private static InformationHolder highGainSingleton = null;

        private static InformationHolder lowGainSingleton = null;

        public List<PointPairList> zedGraphData;
        public List<int> Data { get; set; }

        int totalCount = 0;

        public InformationHolder()
        {
            zedGraphData = new List<PointPairList>();
            Data = new List<int>();
        }
        public static InformationHolder HighGainContainer()
        {
            if (highGainSingleton == null)
            {
                highGainSingleton = new InformationHolder();
                highGainSingleton.Gain = GainType.HighGain;
            }
            return highGainSingleton;
        }

        public static InformationHolder LowGainContainer()
        {
            if (lowGainSingleton == null)
            {
                lowGainSingleton = new InformationHolder();
                lowGainSingleton.Gain = GainType.LowGain;
            }
            return lowGainSingleton;
        }

        public void Add(int graphIndex, int x, int y)
        {
            if (AppSettings.ResolutionOfGraph <= 0) { AppSettings.ResolutionOfGraph = 1; }
            double value = y.ConvertToGs(this.Gain);
            if (totalCount % AppSettings.ResolutionOfGraph == 0)
            {
                zedGraphData[graphIndex].Add(x, value);
            }

            // If we have more data points than desired, remove them by rolling the window forward.
            if (zedGraphData[graphIndex].Count * AppSettings.ResolutionOfGraph > AppSettings.SecondsToGraphData * AppSettings.SamplingFrequency)
            {
                // Changes
                zedGraphData[graphIndex].RemoveAt(0);
            }

            // Increment the total count and add to the data that we will save.
            totalCount++;
            Data.Add(y);

            // If we have more recored more data than we want, roll the window forward by removing stale data.
            if (Data.Count > AppSettings.SecondsToSaveData * AppSettings.SamplingFrequency)
            {
                Data.RemoveAt(0);
                totalCount = 0;
            }
        }
    }
}
