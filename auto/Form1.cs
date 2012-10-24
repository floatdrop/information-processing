using System;
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
			_playTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Normal, StepRight_Click,
			                                 Dispatcher.CurrentDispatcher);
			_playTimer.Stop();
			UpdateResearchInfo();
			UpdateImage();
			ImageBox.Focus();
		}

		private void UpdateResearchInfo()
		{
			//UpdateResearchInfoByMovingDetection();
			UpdateResearchInfoByMouseColorFinding();
		}

		private void UpdateResearchInfoByMouseColorFinding()
		{
			Image<Bgr, byte> currentImage = _imgCollection.GetImage();
			_dataImage = currentImage.Erode(3); //.Convert<Bgr, byte>();
		}

		private void UpdateResearchInfoByMovingDetection()
		{
			const double partSize = 0.2;
			Image<Bgr, byte> tempImage = _oldImage;
			Image<Bgr, byte> currentImage = _imgCollection.GetImage();
			Image<Bgr, byte> newImage = _oldImage.Mul(1 - partSize).Add(currentImage.Mul(partSize));
			_dataImage = newImage.AbsDiff(tempImage).Mul(10).ThresholdBinary(new Bgr(80, 100, 100), new Bgr(255, 255, 255));
			_oldImage = newImage;
		}

		private void UpdateImage()
		{
			ImageBox.Image = _imgCollection.GetImage();
			DebugWindow.Image = _dataImage;
		}

		private void StepRight_Click(object sender, EventArgs e)
		{
			_imgCollection.MoveRight();
			UpdateResearchInfo();
			UpdateImage();
			UpdateFrameCount();
		}

		private void StepBack_Click(object sender, EventArgs e)
		{
			_imgCollection.MoveLeft();
			UpdateResearchInfo();
			UpdateImage();
			UpdateFrameCount();
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
	}
}