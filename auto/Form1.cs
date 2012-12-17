using System;
using System.Collections.Generic;
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
		private static readonly ImageFileGallery _imgCollection = new ImageFileGallery(DataFolder);
		private readonly DispatcherTimer _playTimer;
		private Image<Bgr, byte> _dataImage = new Image<Bgr, byte>(640, 480);
        private MarkovChain _chain = new MarkovChain();
        private MovingEventsDetector _moveDetector = new MovingEventsDetector();
		private bool _playing;
		private VideoWriter videoWriter;

		public Form1()
		{
            _imgCollection.GoTo(0);
			videoWriter = new VideoWriter("video.avi", 20, 640, 480, true);
			InitializeComponent();
			_playTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 1), DispatcherPriority.Normal, StepRight_Click,
			                                 Dispatcher.CurrentDispatcher);
			_playTimer.Stop();
			UpdateResearchInfo();
			UpdateImage();
			UpdateFrameCount();
			ImageBox.Focus();
		}

		private void UpdateResearchInfo()
		{
			//UpdateResearchInfoByMovingDetection();
			//EdgeDetect();
		    MatchEvents();
			//videoWriter.WriteFrame(_dataImage);
			//DebugMoving();
		}

        private void MatchEvents()
        {
            Image<Bgr, byte> currentImage = _imgCollection.GetImage();
            var events = BlobDetector.GetBlobEvents(currentImage);
            var movingEvents = _moveDetector.GetMovingEvents(currentImage);
            _dataImage = currentImage;
            DrawEvents(_dataImage, events, new Bgr(Color.Green));
            DrawEvents(_dataImage, movingEvents, new Bgr(Color.Yellow));
            events.AddRange(movingEvents);
            var ways = _chain.NextStep(events);
            DrawPolyline(_dataImage, ways.Item1, new Bgr(Color.Blue));
            DrawPolyline(_dataImage, ways.Item2, new Bgr(Color.Red));
        }

        private void DrawPolyline(Image<Bgr, Byte> image, List<MarkovState> way, Bgr color)
        {
            for(int i = 1; i < way.Count; i++)
            {
                image.Draw(new LineSegment2D(Geometry.GetCenter(way[i].Coords), Geometry.GetCenter(way[i-1].Coords)), color, 3);
            }
        }

        private void DrawEvents(Image<Bgr, Byte> image, List<Event> events, Bgr color)
        {
            foreach(var ev in events)
            {
                image.Draw(ev.EventCoords, color, 1);
            }
        }

	    private void UpdateResearchInfoByMouseColorFinding()
		{
			Image<Bgr, byte> currentImage = _imgCollection.GetImage();
			_dataImage = currentImage.Erode(3); //.Convert<Bgr, byte>();
		}

        private void EdgeDetect()
        {
            Image<Bgr, byte> currentImage = _imgCollection.GetImage();
            _dataImage = BlobDetector.FindBlobs(currentImage);
        }

        private void DebugMoving()
        {
            Image<Bgr, byte> currentImage = _imgCollection.GetImage();
            _dataImage = _moveDetector.GetMoving(currentImage);
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
				videoWriter.Dispose();
				_playTimer.Stop();
			}
			else
			{
				PlayStop.Text = "Stop";
				videoWriter = new VideoWriter("video.avi", 20, 640, 480, true);
            	_playTimer.Start();
			}
			_playing = !_playing;
		}
	}
}