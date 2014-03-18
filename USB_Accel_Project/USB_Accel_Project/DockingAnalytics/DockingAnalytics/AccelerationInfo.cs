using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;

namespace DockingAnalytics
{
    public class AccelerationInfo : CurveInfo
    {

        public AccelerationInfo(PointPairList ppl,double xScale,double yScale, double yOff)
            :base(ppl,xScale,yScale,0, yOff)
        {
            Type = CurveType.Acceleration; 
            CreateGraphComponents();
        }
    }
}
