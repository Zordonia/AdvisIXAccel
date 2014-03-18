using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DockingAnalytics
{
    public partial class RMSChartDoc : Document
    {
        public RMSChartDoc() : base()
        {
            InitializeComponent();
        }

        public RMSChartDoc(double value, uint mclass, bool MMS) : base()
        {
            InitializeComponent();
            if (MMS)
            {
                vrmsChartControl1.RMSList.Add(new VRMSChartControl.MachineClassRMSValues { MMPS = value, Class = (VRMSChartControl.MachineClass)mclass });
            }
            else
            {
                vrmsChartControl1.RMSList.Add(new VRMSChartControl.MachineClassRMSValues { INPS= value, Class = (VRMSChartControl.MachineClass)mclass });
            }
        }

        public void AddRMSValues(String filename, double RMS, bool MMS, uint mclass)
        {
            if (MMS)
            {
                vrmsChartControl1.RMSList.Add(new VRMSChartControl.MachineClassRMSValues { MMPS = RMS, ISMMS = MMS, Class = (VRMSChartControl.MachineClass)mclass, FileName = filename });
            }
            else
            {
                vrmsChartControl1.RMSList.Add(new VRMSChartControl.MachineClassRMSValues { INPS = RMS, ISMMS = MMS, Class = (VRMSChartControl.MachineClass)mclass, FileName = filename });
            }
        }

        private void RMSChartDoc_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }
    }
}
