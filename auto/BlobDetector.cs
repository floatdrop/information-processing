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
	    public static void FindBlobs(Image<Bgr, byte> source, MouseRegions mouseRegions)
		{
	        var edges = source.Canny(new Bgr(100, 100, 100), new Bgr(100, 100, 100));
            var distTransformed = new Image<Gray, float>(source.Width, source.Height);
			var grayEdges = edges.Convert<Gray, byte>().Not();
			CvInvoke.cvDistTransform(grayEdges.Ptr, distTransformed.Ptr, DIST_TYPE.CV_DIST_L2, 3, new[] { 1f, 1f }, IntPtr.Zero);
			var byteDist = distTransformed.ThresholdBinaryInv(new Gray(2), new Gray(255)).Convert<Gray, byte>();
			var mask = new Image<Gray, byte>(byteDist.Width + 2, byteDist.Height + 2);
			mask.ROI = new Rectangle(1,1,byteDist.Width, byteDist.Height);
			CvInvoke.cvCopy(byteDist, mask, IntPtr.Zero);
			mask.ROI = new Rectangle(0, 0, byteDist.Width+2, byteDist.Height+2);
			edges = grayEdges.Convert<Bgr, byte>();

			for (int i = 0; i < edges.Width; i++)
			{
				for (int j = 0; j < edges.Height; j++)
				{
					if (mask.Data[j, i, 0] != 0)
                        continue;
					MCvConnectedComp comp;
					CvInvoke.cvFloodFill(
						edges.Ptr, 
						new Point(i, j),
						new MCvScalar(200, 200, 200, 0), // Color
						new MCvScalar(0, 0, 0), // Lo
						new MCvScalar(0, 0, 0),  // Up
						out comp,
						CONNECTIVITY.FOUR_CONNECTED,
						FLOODFILL_FLAG.DEFAULT,
						mask.Ptr
					);
					mouseRegions.Add(comp);
				}
			}
		}
	}
}
