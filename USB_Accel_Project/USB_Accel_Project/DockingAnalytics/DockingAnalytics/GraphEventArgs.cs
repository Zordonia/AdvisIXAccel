using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DockingAnalytics
{

    public class GraphEventArgs
    {
        public GraphType GraphType { get; set; }

        private Document _createdGraph = new Document();
        public Document CreatedGraph
        {
            get { return _createdGraph; }
            set
            {
                _createdGraph = value;
                UserGraphCreated();
            }
        }
        public int NumberOfUserCreatedGraphs { get; private set; }

        private void UserGraphCreated()
        {
            NumberOfUserCreatedGraphs++;
        }

        public void UserGraphRemoved()
        {
            NumberOfUserCreatedGraphs--;
        }
    }
}
