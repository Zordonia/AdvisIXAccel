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
    public partial class UpdateForm : DockContent
    {
        private AnalyticsController Controller { get; set; }
        private GraphDock GraphDock { get; set; }

        public UpdateForm()
        {
            InitializeComponent();
        }

        public void Register(AnalyticsController cont)
        {
            Controller = cont;
            Controller.FileOpenedEvent += new AnalyticsController.FileOpened(Controller_FileOpenedEvent);
            Controller.FileClosedEvent += new AnalyticsController.FileClosed(Controller_FileClosedEvent);
            Controller.GraphCreatedEvent += new AnalyticsController.GraphCreated(Controller_GraphCreatedEvent);
        }

        public void Register(GraphDock gD)
        {
            GraphDock = gD;
        }

        bool Controller_GraphCreatedEvent(AnalyticsController aC, GraphEventArgs e)
        {
            return false;
            //throw new NotImplementedException();
        }

        bool Controller_FileClosedEvent(AnalyticsController aC, EventArgs e)
        {
            return false;
            //throw new NotImplementedException();
        }

        bool Controller_FileOpenedEvent(AnalyticsController aC, FileOpenEventArgs e)
        {
            return false;
            //throw new NotImplementedException();
        }

        private void UpdateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }
        
    }
}
