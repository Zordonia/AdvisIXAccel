using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DockingAnalytics
{
    [
    ToolStripItemDesignerAvailability
      (ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)
    ]
    public partial class RangeTrackBar : UserControl
    {
        private double XMin = 0, XMax = 0;
        public double XMinPercent
        {
            get
            {
                return
                    (double)minTrackBar.Value / minTrackBar.Maximum;
            }
            private set { }
        }
        public double XMaxPercent
        {
            get
            {
                return
                    (double)maxTrackBar.Value / maxTrackBar.Maximum;
            }
            private set { }
        }
        public RangeTrackBar()
        {
            InitializeComponent();
        }

        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
        public void SetXMax(double value)
        {
            maxTextBox.Text = ((int)Math.Round(value)).ToString();
            XMax = value;
            int deltav = (int)Math.Round(value);
            maxTrackBar.Value = deltav <= maxTrackBar.Maximum ? deltav >= 0 ? deltav : 0 : maxTrackBar.Maximum;
        }

        public void SetXMin(double value)
        {
            minTextBox.Text =((int)Math.Round(value)).ToString();
            XMin = value;
            int deltav = (int)Math.Round(value);
            minTrackBar.Value = deltav <= minTrackBar.Maximum ? deltav >= 0 ? deltav : 0 : minTrackBar.Maximum;
        }

        private void maxTrackBar_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = sender as TrackBar;
            if (tb != null)
            {
                if (tb.Value < minTrackBar.Value) { minTrackBar.Value = tb.Value - 40; }
                OnScaleChanged(EventArgs.Empty);
            }
        }

        private void minTrackBar_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = sender as TrackBar;
            if (tb != null)
            {
                if (tb.Value > maxTrackBar.Value) { maxTrackBar.Value = tb.Value + 40; }
                OnScaleChanged(EventArgs.Empty);
            }
        }


        public delegate bool ScaleChanged(object sender, EventArgs e);

        [Bindable(true), Category("Events"),
         Description("Subscribe to be notified when the scale has been changed")]
        public event ScaleChanged ScaleChangedEvent;

        protected virtual void OnScaleChanged(EventArgs e)
        {
            if (ScaleChangedEvent != null)
            {
                ScaleChangedEvent(this, e);
            }
        }

        private void updateMaxMinValues()
        {
            double val;
            int deltav;
            if (Double.TryParse(minTextBox.Text, out val))
            {
                deltav = (int)(val * minTrackBar.Maximum / GlobalVars.XMAX);
                this.SetXMin(deltav);
                if (minTrackBar.Value > maxTrackBar.Value) { maxTrackBar.Value = minTrackBar.Value + 40; }
            }
            if (Double.TryParse(maxTextBox.Text, out val))
            {
                deltav = (int)(val * maxTrackBar.Maximum / GlobalVars.XMAX);
                this.SetXMax(deltav);
                if (maxTrackBar.Value < minTrackBar.Value) { minTrackBar.Value = maxTrackBar.Value - 40; }
            }
            OnScaleChanged(EventArgs.Empty);
        }

        private void maxTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                updateMaxMinValues();
            }
        }

        private void maxTextBox_Leave(object sender, EventArgs e)
        {
            updateMaxMinValues();
        }


        

    }

    internal class TrackBarNoFocusCues : System.Windows.Forms.TrackBar
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        private static int MakeParam(int loWord, int hiWord)
        {
            return (hiWord << 16) | (loWord & 0xffff);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            SendMessage(this.Handle, 0x0128, MakeParam(1, 0x1), 0);
        }
    }

}
