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
    public partial class RMSGaugeDoc : Document
    {
        public RMSGaugeDoc() : base()
        {
            InitializeComponent();
        }

        public RMSGaugeDoc(double value)
            : base()
        {
            InitializeComponent();
            gauge1.PreferredValue = 630D;
            gauge1.Speed = value;
        }

        public RMSGaugeDoc(double prefValue, double speed)
            :base()
        {
            InitializeComponent();
            gauge1.PreferredValue = prefValue;
            gauge1.Speed = speed;
        }

        public RMSGaugeDoc(double prefValue, double speed, double max)
            : base()
        {
            InitializeComponent();
            gauge1.PreferredValue = prefValue;
            gauge1.Speed = speed;
            gauge1.MaxRMS = max;
        }
    }
}
