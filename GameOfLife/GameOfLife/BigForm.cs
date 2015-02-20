using System;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class BigForm : Form
    {
        public BigForm()
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
