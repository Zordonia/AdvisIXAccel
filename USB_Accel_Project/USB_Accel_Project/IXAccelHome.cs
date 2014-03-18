#define IS_BENCHMARK_DEVICE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using LibUsbDotNet.Info;
using LibUsbDotNet.LudnMonoLibUsb;
using EC = LibUsbDotNet.Main.ErrorCode;
using System.Threading;
using System.Reflection;
using ZedGraph;
using System.Collections;


namespace IXAccel
{
    public partial class IXAccelHome : Form
    {
        //Threads
        USBThreadClass USBClassObject;
        Thread USBWorkerThread;
        Thread GraphUpdaterThread;
        GraphUpdaterThreadClass GraphUpdater;

        //Data Management
        public List<Int16> accelData;
        NoDupePointList zedGraphDataList;
        LineItem zedGraphCurve;
        public UInt64 graphXIndex;
        public bool isReading = false;
        public IXAccelHome()
        {
            InitializeComponent();
            graphXIndex = 0;
            accelData = new List<Int16>();

            zedGraphDataList = new NoDupePointList();
            /*for (double i = 0; i < 36; i++)
            {
                double x = i * 10.0 + 50.0;
                double y = Math.Sin(i * Math.PI / 15.0) * 16.0;
                zedGraphDataList.Add(x, y);
            }*/

            zedGraphCurve = ZedGraphFrontPage.GraphPane.AddCurve("AccelData", zedGraphDataList, Color.Red);
            zedGraphCurve.Line.Width = 1.5F;
            zedGraphCurve.Symbol.Fill = new Fill(Color.White);
            zedGraphCurve.Symbol.Size = 5;
            zedGraphCurve.Label.IsVisible = false;
            
            ZedGraphFrontPage.GraphPane.YAxis.Scale.Min = -4096;
            ZedGraphFrontPage.GraphPane.YAxis.Scale.Max = 4096;
            ZedGraphFrontPage.GraphPane.XAxis.Scale.Min = 0;
            ZedGraphFrontPage.GraphPane.XAxis.Scale.Max = 500000;
            ZedGraphFrontPage.AxisChange();
            Refresh();
        }

        private void UsbGlobals_UsbErrorEvent(object sender, UsbError e) { Invoke(new UsbErrorEventDelegate(UsbGlobalErrorEvent), new object[] { sender, e }); }

        private void UsbGlobalErrorEvent(object sender, UsbError e) { ConsoleTextbox.AppendText(e + "\r\n"); }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            USBClassObject = new USBThreadClass(this);
            USBClassObject.OpenUSB();
            
            GraphUpdater = new GraphUpdaterThreadClass(this);
            GraphUpdaterThread = new Thread(new ThreadStart(GraphUpdater.startUpdating));

            USBWorkerThread = new Thread(new ThreadStart(USBClassObject.StartReading));
            GraphUpdaterThread = new Thread(new ThreadStart(GraphUpdater.startUpdating));
        }

        private void ReadButton_Click(object sender, EventArgs e)
        {
            isReading = true;
            if (USBWorkerThread.ThreadState == ThreadState.Stopped)
            {
                USBWorkerThread = new Thread(new ThreadStart(USBClassObject.StartReading));
                GraphUpdaterThread = new Thread(new ThreadStart(GraphUpdater.startUpdating));
            }
            USBWorkerThread.Start();
            GraphUpdaterThread.Start();
        }

        private void GetConfig_Click(object sender, EventArgs e)
        {
            /*byte bCfgValue;
            if (MyUsbDevice.GetConfiguration(out bCfgValue))
            {
                tRecv.AppendText("Configuration Value:" + bCfgValue + Environment.NewLine);
            }
            else
                tRecv.AppendText("Failed getting configuration value!");


            if (MyUsbDevice.GetAltInterfaceSetting(2, out bCfgValue))
            {
                tRecv.AppendText("Alternate Interface Setting Value:" + bCfgValue + Environment.NewLine);
            }
            else
                tRecv.AppendText("Failed getting configuration value!");*/
        }

        private delegate void UsbErrorEventDelegate(object sender, UsbError e);
        public delegate void SetConsoleTextboxDelegate(string text);

        public void SetConsoleTextboxThreadSafe(string text)
        {
            if (ConsoleTextbox.InvokeRequired)
            {
                ConsoleTextbox.Invoke(new SetConsoleTextboxDelegate(this.SetConsoleTextboxThreadSafe), text);
            } 
            else
            {
                ConsoleTextbox.AppendText(text);
            }
        }

        private void AnalyticsButton_Click(object sender, EventArgs e)
        {
            DockingAnalytics.AnalyticsController.Instance.MainDockingForm.Show();
            DockingAnalytics.AnalyticsController.Instance.MainDockingForm.BringToFront();
        }

        private void TestReadButton_Click(object sender, EventArgs e)
        {
            USBClassObject.StartTestReading();
        }

        public delegate void UpdateZedgraphDelegate();
        public void UpdateZedGraphThreadSafe()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateZedgraphDelegate(this.UpdateZedGraphThreadSafe), null);
            }
            else
            {
                zedGraphDataList = InformationHolder.Instance().zedGraphData;
                zedGraphCurve.Points = zedGraphDataList;
                zedGraphDataList.FilterData(ZedGraphFrontPage.GraphPane, ZedGraphFrontPage.GraphPane.XAxis, ZedGraphFrontPage.GraphPane.YAxis);
                //ZedGraphFrontPage.AxisChange();
                Refresh();
            }
        }


        private void StopButton_Click(object sender, EventArgs e)
        {
            USBClassObject.RequestStop();
            GraphUpdater.RequestStop();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string ADCSampleCaptureFileName = "ADCData_" + DateTime.Now.ToString("_MM_dd_yyyy_HH_mm_ss") + ".ixs";
            DockingAnalytics._xmlFile myFile = new DockingAnalytics._xmlFile(ADCSampleCaptureFileName);
            ArrayList zedDataArrayList = new ArrayList();

            for (int i = 0; i < zedGraphDataList.Count; i++)
            {
                zedDataArrayList.Add(zedGraphDataList[i].Y);
            }
            myFile.AddAccelerationValues(zedDataArrayList);
            myFile.dsSentry_data.sampling_Freq = DockingAnalytics.GlobalVars.AccelFreq; ;
            //myFile.dsSentry_data.accel_Vg_Calibration = DockingAnalytics.GlobalVars.AccelYScale;
            myFile.dsSentry_data.offset_Calibration = DockingAnalytics.GlobalVars.AccelYOffset;
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

    public class USBThreadClass
    {
        StreamWriter writer;

        List<Byte> rawAccelData;
        List<Int16> parsedData;
        IXAccelHome MainForm;

        UInt16 sinIndex = 0;

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
        private static double mTotalBytes = 0.0;
        private static int mTransferCount = 0;
        public static UsbDevice MyUsbDevice;

        static UsbEndpointReader reader;
        static UsbInterfaceInfo usbInterfaceInfo = null;
        static UsbEndpointInfo usbEndpointInfo = null;

        private bool _shouldStopUSB = false;

        public USBThreadClass(IXAccelHome mainform)
        {
            MainForm = mainform;
            rawAccelData = new List<Byte>();
            parsedData = new List<Int16>();

            //TEST
            for (int i = 0; i < 4800; i++)
            {
                short y = (short)(Math.Sin(i * Math.PI / 15.0) * 16.0);
                parsedData.Add(y);
            }
        }

        public void OpenUSB()
        {
            ErrorCode ec = ErrorCode.None;
            try
            {
                // Find and open the usb device.
                UsbRegDeviceList regList = UsbDevice.AllDevices.FindAll(MyUsbFinder);
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

                    if (!wholeUsbDevice.SetAltInterface(2))
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
                    MainForm.SetConsoleTextboxThreadSafe("Device Opened." + Environment.NewLine);
            }
            catch (Exception ex)
            {
                MainForm.SetConsoleTextboxThreadSafe((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
            }
        }

        private void closeDevice()
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

            // Wait for user input..
            Console.ReadKey();

            // Free usb resources
            UsbDevice.Exit();
        }

        public void StartReading()
        {
            ErrorCode ec = new ErrorCode();
            writer = new StreamWriter("Data.txt");
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


                int seed;
                int i;
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
                    seed = 0;
                    for (i = 0; i < handle.Data.Length; i += 2)
                    {
                        InformationHolder.Instance().zedGraphData.Add(MainForm.graphXIndex++, (Int16)((handle.Data[i]) + (handle.Data[i + 1] << 8)));
                        //writer.Write((Int16)((rawAccelData[i]) + (rawAccelData[i + 1] << 8)) + ",");

                        //Only read one channel
                        if (i == seed + 94)
                        {
                            i += 100;
                            seed = i + 2;
                        }
                    }
                } while (!_shouldStopUSB);

                //Send Parsed Data to MainForm
                //MainForm.UpdateZedGraph();

                // Cancels any oustanding transfers and free's the transfer queue handles.
                // NOTE: A transfer queue can be reused after it's freed.
                transferQeue.Free();
                rawAccelData.Clear();
                MainForm.SetConsoleTextboxThreadSafe("Done!\n");
            }
            catch (Exception ex)
            {
                MainForm.SetConsoleTextboxThreadSafe((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
            }
            writer.Close();
            _shouldStopUSB = false;
            Thread.CurrentThread.Abort();
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
                    InformationHolder.Instance().zedGraphData.Add(MainForm.graphXIndex++, parsedData[i]);
                }
                MainForm.UpdateZedGraphThreadSafe();
                //Thread.Sleep(100);
            //}
        }
    }


    /// <summary>
    /// Class for getting data from the USB Thread and giving it to the MainForm thread.
    /// </summary>
    public class GraphUpdaterThreadClass
    {

        IXAccelHome MainForm;
        private bool _shouldStopGraph = false;
        public GraphUpdaterThreadClass(IXAccelHome form)
        {
            MainForm = form;
        }
        public void Start()
        {
        }

        public void startUpdating()
        {
            while (!_shouldStopGraph)
            {
                //Give the data away
                MainForm.UpdateZedGraphThreadSafe();

                Thread.Sleep(250); //Time in ms
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
    class InformationHolder
    {
        private static InformationHolder singleton = null;
        public NoDupePointList zedGraphData;

        public InformationHolder()
        {
            zedGraphData = new NoDupePointList();
        }
        public static InformationHolder Instance()
        {
            if (singleton == null)
            {
                singleton = new InformationHolder();
            }
            return singleton;
        }
    }
}
 