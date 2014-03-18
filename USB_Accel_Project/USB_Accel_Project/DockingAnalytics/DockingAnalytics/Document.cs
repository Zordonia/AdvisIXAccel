using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using ZedGraph;
using OpenTK;
using System.Windows.Forms;

namespace DockingAnalytics
{
    public class Document : DockContent
    {

        public GraphInfo GraphInformation { get; set; }
        public String GraphName { get; set; }
        public List<LineObj> prevLines = new List<LineObj>();

        public Document()
        {
            GraphInformation = new GraphInfo();
            this.CloseButtonVisible = true;
        }

        public void AddCurve(CurveInfo ci,  String curveName)
        {
            GraphInformation.Add(ci.Curve, ci.Type, curveName);
        }

        public void InsertCurve(int index, CurveItemTypePair citp)
        {
            GraphInformation.Insert(index,citp);
        }

        public void AddCurve(CurveItem curve, String curveName)
        {
            throw new NotImplementedException();
            GraphInformation.Add(curve, curveName);
        }

        public void RemoveCurve(CurveItem curve, CurveType type, String fileName)
        {
            GraphInformation.Remove(curve,type);
        }

        public virtual void ChangeScale(int CI_index, String scale)
        {
            GraphInformation.CurveTypeItems[CI_index].ChangeScale(scale);
        }

        public virtual void ChangeSymbol(int CI_index, String symbol)
        {
            return;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Document
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Document";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Document_FormClosing);
            this.ResumeLayout(false);

        }

        private void Document_FormClosing(object sender, FormClosingEventArgs e)
        {
            double d = 0;
            d = d * 199;
        }
    }
}
