using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;

namespace DockingAnalytics
{
    public class PowerInfo : CurveInfo
    {

        public PowerInfo(PointPairList ppl, double xScale, double yScale)
            :base(ppl,xScale,yScale,3)
        {
            Type = CurveType.Power;// "Power FFT vs. Frequency";
            CreateGraphComponents();
        }
    }
}
