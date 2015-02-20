using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ForestFire
{
    public partial class BigFire : Form
    {
        public BigFire()
        {
            InitializeComponent();
        }

        private void BigFire_KeyPress(object sender, KeyPressEventArgs e)
        {
            Close();
        }

        private void BigFire_Load(object sender, EventArgs e)
        {
            Top = 0;
            Left = 0;
            Width = 1920;
            Height = 1080;
        }
    }
}
