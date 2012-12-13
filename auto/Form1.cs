using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.Structure;

namespace auto
{
	public partial class Form1 : Form
	{
		private const string DataFolder = "rats";
		private static readonly ImageFileGallery _imgCollection = new ImageFileGallery(DataFolder);
		private readonly DispatcherTimer _playTimer;
		private Image<Bgr, byte> _dataImage = new Image<Bgr, byte>(640, 480);
		private Image<Bgr, byte> _oldImage = new Image<Bgr, byte>(640, 480);
		private bool _playing;

		public Form1()
		{
			InitializeComponent();
		    FrameBar.Minimum = 0;
            FrameBar.Maximum = _imgCollection.Count();
            _playTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 10), DispatcherPriority.Normal, StepRight_Click,
			                                 Dispatcher.CurrentDispatcher);
			_playTimer.Stop();
		    FrameBar_ValueChanged(this, null);
			ImageBox.Focus();
		}

		private void UpdateResearchInfo()
		{
			//UpdateResearchInfoByMovingDetection();
			EdgeDetect();
		}

		private void UpdateResearchInfoByMouseColorFinding()
		{
			Image<Bgr, byte> currentImage = _imgCollection.GetImage();
			_dataImage = currentImage.Erode(3); //.Convert<Bgr, byte>();
		}

        private readonly Stopwatch _sw = new Stopwatch();
        private void EdgeDetect()
		{
            Image<Bgr, byte> currentImage = _imgCollection.GetImage();
            _sw.Restart();
			_dataImage = BlobDetector.FindBlobs(currentImage);
            _sw.Stop();
		    DelayLabel.Text = String.Format("{0}ms", _sw.ElapsedMilliseconds);
		}

		private void UpdateImage()
		{
			ImageBox.Image = _imgCollection.GetImage();
			DebugWindow.Image = _dataImage;
		}

		private void StepRight_Click(object sender, EventArgs e)
		{
			_imgCollection.MoveRight();
			FrameBar.Value = _imgCollection.CurrentIndex();
		}

		private void StepBack_Click(object sender, EventArgs e)
		{
			_imgCollection.MoveLeft();
			FrameBar.Value = _imgCollection.CurrentIndex();
		}


		private void UpdateFrameCount()
		{
			FrameCount.Text = string.Format("{0} / {1}", _imgCollection.CurrentIndex(), _imgCollection.Count());
		}

		private void PlayStop_Click(object sender, EventArgs e)
		{
			if (_playing)
			{
				PlayStop.Text = "Play";
				_playTimer.Stop();
			}
			else
			{
				PlayStop.Text = "Stop";
				_playTimer.Start();
			}
			_playing = !_playing;
		}

        private void FrameBar_ValueChanged(object sender, EventArgs e)
        {
            _imgCollection.GoTo(FrameBar.Value);
            UpdateResearchInfo();
            UpdateImage();
            UpdateFrameCount();
        }
	}
}