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
    public enum ExplorerType
    {
        Graph,
        File,
    }


    public struct TypeIndexPair
    {
        public CurveType GraphType { get; set; }
        public String FileName { get; set; }
        public int Index { get; set; }
    }

    public partial class ExplorerDock : DockContent
    {
        private AnalyticsController Controller { get; set; }
        public ExplorerType Type { get; private set; }
        public ExplorerDock()
        {
            InitializeComponent();
        }

        public void Register(AnalyticsController cont,ExplorerType type)
        {
            Type = type;
            this.Text = type.ToString()+" Explorer";
            if (type == ExplorerType.Graph) { InitializeGraphExplorer(); }
            Controller = cont;
            Controller.FileOpenedEvent += new AnalyticsController.FileOpened(FileOpenedEvent);
            Controller.FileClosedEvent += new AnalyticsController.FileClosed(FileClosedEvent);
            //Controller.GraphCreatedEvent += new AnalyticsController.GraphCreated(GraphCreatedEvent);
        }

        bool GraphCreatedEvent(AnalyticsController aC, GraphEventArgs e)
        {
            //GraphListComboBox.Items.Add(e.CreatedGraph.Text);
            //e.CreatedGraph.ControlRemoved += new ControlEventHandler(CreatedGraph_ControlRemoved);
            return false;
        }

        bool FileClosedEvent(AnalyticsController aC, EventArgs e)
        {
            Remove(Controller.ClosedFile);
            return false;
        }

        bool FileOpenedEvent(AnalyticsController aC, FileOpenEventArgs e)
        {
            //GraphListComboBox.Items.Add(Controller.OpenedFile.FileName);
            AddFile(Controller.OpenedFile);
            return false;
        }

        private void AddFile(File f)
        {
            String path = System.IO.Path.GetFileName(f.FileName);
            if (this.Type == ExplorerType.File)
            {
                treeView1.Nodes.Add(path);
                treeView1.Nodes[treeView1.Nodes.Count-1].Nodes.Add("Time Graphs");
                treeView1.Nodes[treeView1.Nodes.Count - 1].Nodes[0].Nodes.Add("Acceleration");
                treeView1.Nodes[treeView1.Nodes.Count - 1].Nodes[0].Nodes.Add("Velocity");
                treeView1.Nodes[treeView1.Nodes.Count - 1].Nodes.Add("Frequency Graphs");
                treeView1.Nodes[treeView1.Nodes.Count - 1].Nodes[1].Nodes.Add("FFT Magnitude");
                treeView1.Nodes[treeView1.Nodes.Count - 1].Nodes[1].Nodes.Add("FFT Power");

            }
            else if(this.Type == ExplorerType.Graph)
            {
                treeView1.Nodes[0].Nodes[0].Nodes.Add(path);
                treeView1.Nodes[0].Nodes[1].Nodes.Add(path);
                treeView1.Nodes[1].Nodes[0].Nodes.Add(path);
                treeView1.Nodes[1].Nodes[1].Nodes.Add(path);
            }
        }



      

        private void InitializeGraphExplorer()
        {
            treeView1.Nodes.Add("Time Graphs");
            treeView1.Nodes[0].Nodes.Add("Acceleration");
            treeView1.Nodes[0].Nodes.Add("Velocity");
            treeView1.Nodes.Add("Frequency Graphs");
            treeView1.Nodes[1].Nodes.Add("FFT Power");
            treeView1.Nodes[1].Nodes.Add("FFT Magnitude");
        }

        private void Remove(File f)
        {
            throw new NotImplementedException();
        }


        private void AfterCheckEvent(object sender, TreeViewEventArgs e)
        {
            CheckAllFollowing(e.Node);
        }  
        
        private void CheckAllFollowing(TreeNode node)
        {
            System.Collections.IEnumerator ienum = node.Nodes.GetEnumerator();
            TreeNode currentNode = node;
            while (ienum.MoveNext())
            {
                currentNode = (TreeNode)ienum.Current;
                currentNode.Checked = node.Checked;
            }
        }


        private List<TypeIndexPair> GetCurrentChecked()
        {
            List<TypeIndexPair> checkList = new List<TypeIndexPair>();
            TypeIndexPair tip = new TypeIndexPair();
            if (Type == ExplorerType.File)
            {
                if (treeView1.Nodes == null) { return null; }
                int i0 = -1;
                foreach (TreeNode tn in treeView1.Nodes)
                {
                    tip = new TypeIndexPair();
                    i0++;
                    TreeNode temp = treeView1.Nodes[i0].Nodes[0].Nodes[0];
                    if (temp.Checked)
                    {
                        tip.Index = i0;
                        tip.GraphType = CurveType.Acceleration;
                        tip.FileName = temp.Name;
                        checkList.Add(tip);
                    }
                    tip = new TypeIndexPair();
                    temp = treeView1.Nodes[i0].Nodes[0].Nodes[1];
                    if (temp.Checked)
                    {
                        tip.Index = i0;
                        tip.GraphType = CurveType.Velocity;
                        tip.FileName = temp.Name;
                        checkList.Add(tip);
                    }
                    tip = new TypeIndexPair();
                    temp = treeView1.Nodes[i0].Nodes[1].Nodes[0];
                    if (temp.Checked)
                    {
                        tip.Index = i0;
                        tip.GraphType = CurveType.Magnitude;
                        tip.FileName = temp.Name;
                        checkList.Add(tip);
                    } 
                    tip = new TypeIndexPair();
                    temp = treeView1.Nodes[i0].Nodes[1].Nodes[1];
                    if (temp.Checked)
                    {
                        tip.Index = i0;
                        tip.GraphType = CurveType.Power;
                        tip.FileName = temp.Name;
                        checkList.Add(tip);
                    }
                }
            }
            else if(Type == ExplorerType.Graph)
            {
                TreeNode temp;
                int typeInd = 0;
                CurveType whatType = new CurveType();
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        int ind = 0;
                        foreach (TreeNode tn in treeView1.Nodes[i].Nodes[j].Nodes)
                        {
                            whatType = (CurveType)(typeInd) ;
                            tip = new TypeIndexPair();
                            if (tn.Checked)
                            {
                                tip.Index = ind;
                                tip.GraphType = whatType;
                                tip.FileName = tn.Name;
                                checkList.Add(tip);
                            }
                            ind++;
                        }
                        typeInd++;
                    }
                }
            }
            return checkList;
       } 

        private void CreateButtonClicked(object sender, EventArgs e)
        {
            List<TypeIndexPair> tipList = GetCurrentChecked();
            Controller.Create2DDocument(tipList);
            UnCheckAllNodes();
        }

        private void CombineButtonClicked(object sender, EventArgs e)
        {
            List<TypeIndexPair> tipList = GetCurrentChecked();
            Controller.Create3DDocument(tipList);
        }

        /// <summary>
        /// Unchecks all the nodes within the treeView of the graph.
        /// </summary>
        private void UnCheckAllNodes()
        {
            foreach (TreeNode tn in treeView1.Nodes)
            {
                tn.Checked = false;
            }
        }

        private void ExplorerDock_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }
    }
}
