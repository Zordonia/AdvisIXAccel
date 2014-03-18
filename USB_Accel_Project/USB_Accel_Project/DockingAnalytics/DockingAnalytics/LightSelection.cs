using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DockingAnalytics
{
    public partial class LightSelection : Form
    {
        public LightSelection()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler ColorChanged;

        protected virtual void OnColorChanged()
        {
            if (ColorChanged != null)
            {
                ColorChanged(this, new PropertyChangedEventArgs("NON"));
            }
        }

        public float[] GetDiffuse()
        {
            float[] temp = new float[4];
            temp[0] = float.Parse(tbdv0.Text);
            temp[1] = float.Parse(tbdv1.Text);
            temp[2] = float.Parse(tbdv2.Text);
            temp[3] = float.Parse(tbdv3.Text);
            return temp;
        }

        public float[] GetAmbient()
        {
            float[] temp = new float[4];
            temp[0] = float.Parse(tbav0.Text);
            temp[1] = float.Parse(tbav1.Text);
            temp[2] = float.Parse(tbav2.Text);
            temp[3] = float.Parse(tbav3.Text);
            return temp;
        }

        public float[] GetPosition()
        {
            float[] temp = new float[4];
            temp[0] = float.Parse(tbpv0.Text);
            temp[1] = float.Parse(tbpv1.Text);
            temp[2] = float.Parse(tbpv2.Text);
            temp[3] = float.Parse(tbpv3.Text);
            return temp;
        }

        private void changeValue(TextBox tb, bool up, double increase = 0.01)
        {
            if (up)
            {
                tb.Text = (float.Parse(tb.Text) + increase).ToString();
            }
            else
            {
                tb.Text = (float.Parse(tb.Text) - increase).ToString();
            }
            OnColorChanged();
        }

        private void vScrollBar3_Scroll(object sender, ScrollEventArgs e)
        {
            changeValue(tbpv3, (e.OldValue <= e.NewValue));
        }

        private void vScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            changeValue(tbav3, (e.OldValue <= e.NewValue));
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            changeValue(tbdv3, (e.OldValue <= e.NewValue));
        }

        private void vScrollBar4_Scroll(object sender, ScrollEventArgs e)
        {
            changeValue(tbpv2, (e.OldValue <= e.NewValue), 1);
        }

        private void vScrollBar5_Scroll(object sender, ScrollEventArgs e)
        {
           changeValue(tbav2, (e.OldValue <= e.NewValue));
        }

        private void vScrollBar6_Scroll(object sender, ScrollEventArgs e)
        {
            changeValue(tbdv2, (e.OldValue <= e.NewValue));
        }

        private void vScrollBar7_Scroll(object sender, ScrollEventArgs e)
        {
            changeValue(tbpv1, (e.OldValue <= e.NewValue), 1);
        }

        private void vScrollBar8_Scroll(object sender, ScrollEventArgs e)
        {
            changeValue(tbav1, (e.OldValue <= e.NewValue));
        }

        private void vScrollBar9_Scroll(object sender, ScrollEventArgs e)
        {
            changeValue(tbdv1, (e.OldValue <= e.NewValue));
        }

        private void vScrollBar10_Scroll(object sender, ScrollEventArgs e)
        {
            changeValue(tbpv0, (e.OldValue <= e.NewValue), 1);
        }

        private void vScrollBar11_Scroll(object sender, ScrollEventArgs e)
        {
            changeValue(tbav0, (e.OldValue <= e.NewValue));
        }

        private void vScrollBar12_Scroll(object sender, ScrollEventArgs e)
        {
            changeValue(tbdv0, (e.OldValue <= e.NewValue));
        }

        private void tbdv0_TextChanged(object sender, EventArgs e)
        {
            int temp = 0;
            TextBox tb = sender as TextBox;
            if (tb.Text != "" || int.TryParse(tb.Text, out temp))
            {
                OnColorChanged();
            }
        }
    }
}
