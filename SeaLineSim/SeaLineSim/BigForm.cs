using System;
using System.Windows.Forms;

namespace SeaLineSim
{
    public partial class BigForm : Form
    {
        private MainForm _mainForm;

        public BigForm(MainForm mainForm)
        {
            InitializeComponent();

            _mainForm = mainForm;
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

        private void pictureBox_Click(object sender, EventArgs e)
        {
            _mainForm.GoButtonClick(sender, e);
        }
    }
}
