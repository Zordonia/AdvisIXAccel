using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DockingAnalytics
{
    public class BaseObject
    {
        public uint[] m_colorID = new uint[3];
        public static uint[] gColorID = new uint[3];
        private bool selected = false;

        public BaseObject()
        {

            gColorID[0]++;
            if (gColorID[0] > 255)
            {
                gColorID[0] = 0;
                gColorID[1]++;
                if (gColorID[1] > 255)
                {
                    gColorID[2]++;
                }
            }
            gColorID[0] = 220;
            gColorID[1] = 0;
            gColorID[2] = 0;
            m_colorID[0] = gColorID[0];
            m_colorID[1] = gColorID[1];
            m_colorID[2] = gColorID[2];
        }

        public bool isSelected() { return selected; }

        public void setSelected(bool sel) { selected = sel; }

        public void Render(bool picking) { }

    }
}
