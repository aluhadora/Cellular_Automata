using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace SeaLineSim
{
    public partial class MainForm : Form, IView
    {
        private readonly Model _model;
        private readonly Timer _timer;
        private bool _initialized;
        private readonly BigForm _form;

        public MainForm()
        {
            InitializeComponent();
            _form = new BigForm(this);
            _model = new Model(this);
            _model.Initialize();
            _timer = new Timer { Interval = 50 };
            _timer.Tick += Step;

            _form.pictureBox.Image = pictureBox.Image;
        }

        void Step(object sender, EventArgs e)
        {
            _model.Step();

            _form.pictureBox.Image = pictureBox.Image;
        }

        public Size GridSize { get { return pictureBox.Size; } }


        public long Generation
        {
            get { return Convert.ToInt64(generationLabel.Text); }
            set { generationLabel.Text = value.ToString(CultureInfo.InvariantCulture); }
        }

        public Color[,] Colors
        {
            get { return _model.FromImage((Bitmap)pictureBox.Image); }
            set { pictureBox.Image = _model.FromColors(value); }
        }

        private void PictureBoxClick(object sender, EventArgs e)
        {

        }

        private void Zoom(object sender, EventArgs e)
        {
            _form.ShowDialog();
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            zoomButton.Image = new Bitmap(Properties.Resources.Zoom_In_icon, zoomButton.Width - 5, zoomButton.Height - 5);
        }

        public void GoButtonClick(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                _initialized = true;
                _timer.Start();
                return;
            }

            _timer.Enabled = !_timer.Enabled;
        }

        private bool _mouseDown;
        private void PictureBoxMouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
        }

        private void PictureBoxMouseLeave(object sender, EventArgs e)
        {
            _mouseDown = false;
        }

        private void PictureBoxMouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void PictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (!_mouseDown) return;

            _model.DrawPoint(Cursor.Position, new Point(Top, Left));
        }
    }
}
