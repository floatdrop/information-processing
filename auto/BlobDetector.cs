using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.VideoSurveillance;

namespace auto
{
	static class BlobDetector
	{
		private static MCvFont _font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_SIMPLEX, 1.0, 1.0);
		private static FGDetector<Bgr> _detector = new FGDetector<Bgr>(FORGROUND_DETECTOR_TYPE.FGD);
		private static BlobTrackerAuto<Bgr> _tracker = new BlobTrackerAuto<Bgr>();
		private static Random Rand = new Random(100);

		private static void TrackBlobs(Image<Bgr, Byte> frame, Image<Bgr, Byte> drawOn)
		{
			frame._SmoothGaussian(3); //filter out noises
			_detector.Update(frame);
			Image<Gray, Byte> forgroundMask = _detector.ForgroundMask;
			_tracker.Process(frame, forgroundMask);
			foreach (MCvBlob blob in _tracker)
			{
				drawOn.Draw(Rectangle.Round(blob), new Bgr(255.0, 255.0, 255.0), 2);
				drawOn.Draw(blob.ID.ToString(), ref _font, Point.Round(blob.Center), new Bgr(255.0, 255.0, 255.0));
			}
		}

		public static Image<Bgr, byte> FindBlobs(Image<Bgr, byte> source)
		{
			//source._EqualizeHist();
			var edges = new Image<Bgr, byte>(source.Width, source.Height);
			// source.SmoothMedian(5);
			for (int i = 0; i < 3; i++)
				edges[i] = source[i].Canny(new Gray(100), new Gray(100));
			var distTransformed = new Image<Gray, float>(source.Width, source.Height);
			var grayEdges = edges.Convert<Gray, byte>().Not();
			CvInvoke.cvDistTransform(grayEdges.Ptr, distTransformed.Ptr, DIST_TYPE.CV_DIST_L2, 3, new[] { 1f, 1f }, IntPtr.Zero);
			var byteDist = distTransformed.ThresholdBinaryInv(new Gray(2), new Gray(255)).Convert<Gray, byte>();
			//return byteDist.Convert<Bgr, byte>();
			Image<Gray, byte> mask = new Image<Gray, byte>(byteDist.Width + 2, byteDist.Height + 2);
			mask.ROI = new Rectangle(1,1,byteDist.Width, byteDist.Height);
			CvInvoke.cvCopy(byteDist, mask, IntPtr.Zero);
			mask.ROI = new Rectangle(0, 0, byteDist.Width+2, byteDist.Height+2);
			edges = grayEdges.Convert<Bgr, byte>();
			/* Flood fill */

			var remapColorDict = new Dictionary<Bgr, Bgr>();

			for (int i = 0; i < edges.Width; i++)
			{
				for (int j = 0; j < edges.Height; j++)
				{
					if (mask.Data[j, i, 0] == 0)
					{
						var comp = new MCvConnectedComp();
						CvInvoke.cvFloodFill(
							edges.Ptr, 
							new Point(i, j),
							new MCvScalar(200, 200, 200, 0), // Color
							new MCvScalar(0, 0, 0), // Lo
							new MCvScalar(0, 0, 0),  // Up
							out comp,
							Emgu.CV.CvEnum.CONNECTIVITY.EIGHT_CONNECTED,
							Emgu.CV.CvEnum.FLOODFILL_FLAG.DEFAULT,
							mask.Ptr
						);
						
						if (comp.area > 400 && comp.area < 2500)
						{
							ReplaceColors(edges, comp.rect);
						}
					}
				}
			}

			TrackBlobs(edges, source);
			return edges;
		}

		private static void ReplaceColors(Image<Bgr, byte> edges, Rectangle roi)
		{
			for (int i = roi.Left; i < roi.Right; i++)
			{
				for (int j = roi.Top; j < roi.Bottom; j++)
				{
					if (edges.Data[j, i, 0] == 200)
					{
						edges.Data[j, i, 0] = 255;
						edges.Data[j, i, 1] = 0;
						edges.Data[j, i, 2] = 0;
					}
				}
			}
		}

		private static MCvScalar GetColor()
		{
			return new MCvScalar(Rand.Next(1, 254), Rand.Next(1, 254), Rand.Next(1, 254), 255);
		}


		private static Image<Bgr, byte> GaussEdgeDetector(Image<Bgr, byte> source)
		{
			return source.SmoothGaussian(7).AbsDiff(source.SmoothGaussian(5)).Mul(20).ThresholdToZero(new Bgr(50, 50, 50));
		}

		private static Image<Bgr, byte> MedianEdgeDetector(Image<Bgr, byte> source)
		{
			return source.SmoothMedian(7).AbsDiff(source.SmoothMedian(5)).Mul(20).ThresholdToZero(new Bgr(50, 50, 50));
		}

        private static Image<Bgr, Byte> SuperCoolEdgeMedianSmooth(Image<Bgr, Byte> source, int[,] marks)
        {
            var blobToColorSum = new Dictionary<int, BlobInfo>();

            for (int y = 0; y < marks.GetLength(0); y++)
            {
                for (int x = 0; x < marks.GetLength(1); x++)
                {
                    if(!blobToColorSum.ContainsKey(marks[y, x]))
                    {
                        blobToColorSum.Add(marks[y, x], new BlobInfo(source[y, x]));
                    }
                    else
                    {
                        blobToColorSum[marks[y, x]].AddStat(source[y, x]);
                    }
                }
            }

            var result = new Image<Bgr, byte>(source.Width, source.Height);

            var avg = new Dictionary<int, Bgr>();

            foreach (var markBlobPair in blobToColorSum)
            {
                avg[markBlobPair.Key] = markBlobPair.Value.GetAverage();
            }


            for (int y = 0; y < marks.GetLength(0); y++)
            {
                for (int x = 0; x < marks.GetLength(1); x++)
                {
                    if (marks[y, x] > 0)
                        result[y, x] = avg[marks[y, x]];
                }
            }

            return result;
        }

	    private static Image<Bgr, byte> GenerageImage(int[,] marks)
		{
			var image = new Image<Bgr, byte>(marks.GetLength(1), marks.GetLength(0));
			var colors = new Dictionary<int, Bgr>();
            colors.Add(0, new Bgr(0, 0, 0));

			for (int y = 0; y < marks.GetLength(0); y++)
			{
				for (int x = 0; x < marks.GetLength(1); x++)
				{
					if (!colors.ContainsKey(marks[y, x]))
					{
						colors.Add(marks[y, x], new Bgr((byte)Rand.Next(255), (byte)Rand.Next(255), (byte)Rand.Next(255)));
					}
					image[y, x] = colors[marks[y, x]]; 
				}
			}
			return image;
		}

		private static int[,] MarkBlobs(Image<Gray, float> distTransformed)
		{
            var marks = new int[distTransformed.Height, distTransformed.Width];
			int currentMark = 1;

            for (int x = 0; x < distTransformed.Width; x++)
			{
                for (int y = 0; y < distTransformed.Height; y++)
				{
					if (marks[y, x] == 0)
                        MarkArea(marks, distTransformed, x, y, currentMark++);
				}
			}
			return marks;
		}

		private static readonly Tuple<int, int>[] PossibleMoves = new[]
		{
			Tuple.Create(1, 0),
			Tuple.Create(0, 1),
			Tuple.Create(-1, 0),
			Tuple.Create(0, -1)
		};

	    private const float BlobThreshold = 2;

        private static void MarkArea(int[,] marks, Image<Gray, float> distTransformed, int x, int y, int currentMark)
		{
            if (distTransformed[y, x].Intensity < BlobThreshold)
                return;

			var queue = new Queue<Tuple<int, int>>();
            queue.Enqueue(Tuple.Create(x, y));
            marks[y, x] = currentMark;
				
			while (queue.Count > 0)
			{
				var xy = queue.Dequeue();
				foreach (var move in PossibleMoves)
				{
					var newX = xy.Item1 + move.Item1;
					var newY = xy.Item2 + move.Item2;
					if(newX < 0 || newX >= distTransformed.Width || newY < 0 || newY >= distTransformed.Height)
						continue;
                    if (marks[newY, newX] == 0 && distTransformed[newY, newX].Intensity >= BlobThreshold)
                    {
                        marks[newY, newX] = currentMark;
						queue.Enqueue(Tuple.Create(newX, newY));
                    }
				}
			}
		}
	}

    class BlobInfo
    {
        public int RSum;
        public int GSum;
        public int BSum;

        public int PixelCount;

        public BlobInfo(Bgr color)
        {
            RSum = (int)color.Red;
            GSum = (int)color.Green;
            BSum = (int)color.Blue;

            PixelCount = 1;
        }

        public void AddStat(Bgr color)
        {
            RSum += (int)color.Red;
            GSum += (int)color.Green;
            BSum += (int)color.Blue;

            PixelCount++;
        }

        public Bgr GetAverage()
        {
            return new Bgr(BSum*1.0/PixelCount, GSum*1.0/PixelCount, RSum*1.0/PixelCount);
        }
    }
}
