using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.Structure;

namespace auto
{
	public partial class Form1 : Form
	{
		private const string DataFolder = "rats";
		private static readonly ImageFileGallery ImgCollection = new ImageFileGallery(DataFolder);
		private readonly DispatcherTimer _playTimer;
		private Image<Bgr, byte> _dataImage = new Image<Bgr, byte>(640, 480);
		private bool _playing;

		public Form1()
		{
			InitializeComponent();
		    FrameBar.Minimum = 0;
            FrameBar.Maximum = ImgCollection.Count();
            _playTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 10), DispatcherPriority.Normal, StepRightClick,
			                                 Dispatcher.CurrentDispatcher);
			_playTimer.Stop();
		    FrameBarValueChanged(this, null);
			ImageBox.Focus();
		}

		private void UpdateResearchInfo()
		{
            Image<Bgr, byte> currentImage = ImgCollection.GetImage();
            _sw.Restart();
            BlobDetector.FindBlobs(currentImage, _mouseRegions);
            DrawRegions(currentImage, _mouseRegions);
		    _dataImage = currentImage;
            _sw.Stop();
            DelayLabel.Text = String.Format("{0}ms", _sw.ElapsedMilliseconds);
		}

        private readonly Stopwatch _sw = new Stopwatch();
	    private readonly MouseRegions _mouseRegions = new MouseRegions();
        private static MCvFont _font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_SIMPLEX, 0.5, 0.5);
      
	    private void DrawRegions(Image<Bgr, byte> dataImage, MouseRegions mouseRegions)
	    {
	        foreach (var region in _mouseRegions.Regions)
	        {
                dataImage.Draw(region.Rectangle, new Bgr(0,0,255), 2);
                dataImage.Draw(region.Id, ref _font, GetCenter(region.Component.rect), new Bgr(0,0,255));
	        }
	    }

	    private Point GetCenter(Rectangle rect)
	    {
	        return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
	    }

	    private void UpdateImage()
		{
			ImageBox.Image = ImgCollection.GetImage();
			DebugWindow.Image = _dataImage;
		}

		private void StepRightClick(object sender, EventArgs e)
		{
			ImgCollection.MoveRight();
			FrameBar.Value = ImgCollection.CurrentIndex();
		}

		private void StepBackClick(object sender, EventArgs e)
		{
			ImgCollection.MoveLeft();
			FrameBar.Value = ImgCollection.CurrentIndex();
		}


		private void UpdateFrameCount()
		{
			FrameCount.Text = string.Format("{0} / {1}", ImgCollection.CurrentIndex(), ImgCollection.Count());
		}

		private void PlayStopClick(object sender, EventArgs e)
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

        private void FrameBarValueChanged(object sender, EventArgs e)
        {
            ImgCollection.GoTo(FrameBar.Value);
            UpdateResearchInfo();
            UpdateImage();
            UpdateFrameCount();
        }
	}
}