﻿using System;
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
						
						if (FilledAreaIsMousable(comp) && FilledAreaDimesnionsIsMousable(comp))
						{
							ReplaceColors(edges, comp.rect);
						}
                        else
						{
						    RestoreWhiteColor(edges, comp.rect);
						}
					}
				}
			}

			//TrackBlobs(edges, source);
			return edges;
		}

	    private static bool FilledAreaDimesnionsIsMousable(MCvConnectedComp comp)
	    {
	        return comp.rect.Width > 10 && comp.rect.Width < 300 && comp.rect.Height > 10 && comp.rect.Height < 300;
	    }

	    private static bool FilledAreaIsMousable(MCvConnectedComp comp)
	    {
	        return comp.area > 400 && comp.area < 2500;
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
        private static void RestoreWhiteColor(Image<Bgr, byte> edges, Rectangle roi)
        {
            for (int i = roi.Left; i < roi.Right; i++)
            {
                for (int j = roi.Top; j < roi.Bottom; j++)
                {
                    if (edges.Data[j, i, 0] == 200)
                    {
                        edges.Data[j, i, 0] = 255;
                        edges.Data[j, i, 1] = 255;
                        edges.Data[j, i, 2] = 255;
                    }
                }
            }
        }	
	}
}
