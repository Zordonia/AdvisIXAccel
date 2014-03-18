using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;

namespace DockingAnalytics
{
    public class MagnitudeInfo : CurveInfo
    {
        
        public MagnitudeInfo(PointPairList ppl, double xScale, double yScale)
            :base(ppl,xScale,yScale,2)
        {
            Type = CurveType.Magnitude;// "Magnituge FFT vs. Frequency";
            CreateGraphComponents();
        }
    }
}
