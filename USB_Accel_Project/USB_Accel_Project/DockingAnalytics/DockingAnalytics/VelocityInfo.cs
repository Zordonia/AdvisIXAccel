using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;

namespace DockingAnalytics
{
    public class VelocityInfo : CurveInfo
    {

        public VelocityInfo(PointPairList ppl, double xScale, double yScale)
            :base(ppl,xScale,yScale,1)
        {
            Type = CurveType.Velocity;// "Velocity vs. Frequency";
            CreateGraphComponents();
        }
    }
}
