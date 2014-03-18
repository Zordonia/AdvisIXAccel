using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DockingAnalytics
{
    public partial class USBControlDock : DockContent
    {
        private AnalyticsController Controller { get; set; }
        
        public enum ChannelGain
        {
            Low,
            High,
            Both
        }

        public ChannelGain gain;
        public USBControlDock()
        {
            InitializeComponent();
            SenseUnitsComboBox.SelectedIndex = 0;
            this.gain = ChannelGain.High;
            HighGainRadioButton.Checked = true;
            LowGainRadioButton.Checked = false;
            BothGainsRadioButton.Checked = false;
        }

        public void Register(AnalyticsController cont)
        {
            //if (type == ExplorerType.Graph) { InitializeGraphExplorer(); }
            Controller = cont;
            //Controller.FileOpenedEvent += new AnalyticsController.FileOpened(FileOpenedEvent);
            //Controller.FileClosedEvent += new AnalyticsController.FileClosed(FileClosedEvent);
            Controller.GraphCreatedEvent += new AnalyticsController.GraphCreated(GraphCreatedEvent);
        }

        bool GraphCreatedEvent(AnalyticsController aC, GraphEventArgs e)
        {
            GraphListComboBox.Items.Add(e.CreatedGraph.Text);
            e.CreatedGraph.ControlRemoved += new ControlEventHandler(CreatedGraph_ControlRemoved);
            return false;
        }

        void CreatedGraph_ControlRemoved(Object sender, ControlEventArgs e)
        {
            for (int i = 0; i < GraphListComboBox.Items.Count; i++)
            {
                if (e.Control.Name == GraphListComboBox.Items[i])
                {
                    GraphListComboBox.Items.RemoveAt(i);
                    return;
                }
            }
        }

        public delegate void SetConsoleTextboxDelegate(string text);

        public void USBSetConsoleTextboxThreadSafe(string text)
        {
            if (ConsoleTextbox.InvokeRequired)
            {
                ConsoleTextbox.Invoke(new SetConsoleTextboxDelegate(this.USBSetConsoleTextboxThreadSafe), text);
            }
            else
            {
                ConsoleTextbox.AppendText(text);
            }
        }

        private void OpenUSBButton_Click(object sender, EventArgs e)
        {
            if (OpenUSBButton.BackColor == Color.LightYellow)
            {
                if (Controller.USBAnalytics_OpenUSB())
                {
                    OpenUSBButton.BackColor = Color.LightGreen;
                    OpenUSBButton.Text = "Close USB Device";
                }
            }
            else
            {
                Controller.USBAnalytics_CloseUSB();
                OpenUSBButton.BackColor = Color.LightYellow;
                OpenUSBButton.Text = "Open USB Device";
            }

        }

        private void ReadUSBButton_Click(object sender, EventArgs e)
        {
            Controller.USBAnalytics_ReadUSB();
        }

        private void StopUSBButton_Click(object sender, EventArgs e)
        {
            Controller.USBAnalytics_StopUSB();
        }

        private void GraphListComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Controller.GraphListComboBoxIndex = GraphListComboBox.SelectedIndex;
        }

        private void GainRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if(HighGainRadioButton.Checked)
            {
                this.gain = ChannelGain.High;
            }
            else if (LowGainRadioButton.Checked)
            {
                this.gain = ChannelGain.Low;
            }
            else
            {
                this.gain = ChannelGain.Both;
            }
        }
    }
}
