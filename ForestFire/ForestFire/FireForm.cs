using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ForestFire
{
    public partial class FireForm : Form, IView
    {
        private readonly Model _model;
        private readonly Timer _timer;
        private bool _initialized;

        public FireForm()
        {
            InitializeComponent();
            _model = new Model(this);
            _model.Initialize();
            _timer = new Timer {Interval = 50};
            _timer.Tick += Step;
        }

        void Step(object sender, EventArgs e)
        {
            _model.Step();

            if (_form == null) return;

            _form.pictureBox.Image = pictureBox.Image;
        }

        public Size FireSize { get { return pictureBox.Size; } }

        private double _lastKnownP;
        public double P
        {
            get
            {
                double d;
                try
                {
                    d = Convert.ToDouble(pTextBox.Text);
                    _lastKnownP = d;
                }
                catch (Exception)
                {
                    d = _lastKnownP;
                }

                return d;
            }
            set { pTextBox.Text = value.ToString("0.000"); }
        }

        private double _lastKnownF;
        private BigFire _form;

        public double F
        {
            get
            {
                double d;
                try
                {
                    d = Convert.ToDouble(fTextBox.Text);
                    _lastKnownF = d;
                }
                catch (Exception)
                {
                    d = _lastKnownF;
                }

                return d;
            }
            set { fTextBox.Text = value.ToString("0.000"); }
        }

        public long Generation
        {
            get { return Convert.ToInt64(generationLabel.Text); }
            set { generationLabel.Text = value.ToString(CultureInfo.InvariantCulture); }
        }

        public Color[,] Colors
        {
            get { return _model.FromImage((Bitmap) pictureBox.Image); }
            set { pictureBox.Image = _model.FromColors(value); }
        }

        private void PictureBoxClick(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                _initialized = true;
                _timer.Start();
                return;
            }

            _timer.Enabled = !_timer.Enabled;
        }

        private void Zoom(object sender, EventArgs e)
        {
            _form = new BigFire();

            _form.ShowDialog();
        }

        private void FireForm_Load(object sender, EventArgs e)
        {
            zoomButton.Image = new Bitmap(Properties.Resources.Zoom_In_icon, zoomButton.Width - 5, zoomButton.Height - 5);
        }
    }
}
