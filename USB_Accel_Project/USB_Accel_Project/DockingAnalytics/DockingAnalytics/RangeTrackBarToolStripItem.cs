using System;
using System.Collections.Generic;
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
    public class RangeTrackBarToolStripItem : ToolStripControlHost
    {
        public RangeTrackBarToolStripItem()
            : base(new RangeTrackBar())
        {}

    }
}
