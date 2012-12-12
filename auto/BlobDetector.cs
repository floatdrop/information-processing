﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace auto
{
	static class BlobDetector
	{
		public static Image<Bgr, byte> FindBlobs(Image<Bgr, byte> source)
		{
            var graySource = source.Convert<Gray, byte>();
            var median = graySource.SmoothMedian(7);
            var edges = median.Canny(new Gray(100), new Gray(25)).Not();
            var distTransformed = new Image<Gray, float>(graySource.Width, graySource.Height);
            CvInvoke.cvDistTransform(edges.Ptr, distTransformed.Ptr, DIST_TYPE.CV_DIST_L2, 3, new[] { 1f, 1f }, IntPtr.Zero);
            var image = GenerageImage(MarkBlobs(distTransformed));
            return image;
		}

		private static Image<Bgr, byte> GaussEdgeDetector(Image<Bgr, byte> source)
		{
			return source.SmoothGaussian(7).AbsDiff(source.SmoothGaussian(5)).Mul(20).ThresholdToZero(new Bgr(50, 50, 50));
		}

		private static Image<Bgr, byte> MedianEdgeDetector(Image<Bgr, byte> source)
		{
			return source.SmoothMedian(7).AbsDiff(source.SmoothMedian(5)).Mul(20).ThresholdToZero(new Bgr(50, 50, 50));
		}

		private static Random Rand = new Random(100);

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

			var stack = new Stack<Tuple<int, int>>();
			stack.Push(Tuple.Create(x, y));
				
			while (stack.Count > 0)
			{
				var xy = stack.Pop();
				marks[xy.Item2, xy.Item1] = currentMark;
				foreach (var move in PossibleMoves)
				{
					var newX = xy.Item1 + move.Item1;
					var newY = xy.Item2 + move.Item2;
					if(newX < 0 || newX >= distTransformed.Width || newY < 0 || newY >= distTransformed.Height)
						return;
                    if (marks[newY, newX] == 0 && distTransformed[newY, newX].Intensity >= BlobThreshold)
						stack.Push(Tuple.Create(newX, newY));
				}
			}
		 }

		private static double Distance(Bgr color1, Bgr color2)
		{
			return Math.Pow(color1.Blue - color2.Blue, 2) + 
				   Math.Pow(color1.Green - color2.Green, 2) +
			       Math.Pow(color1.Red - color2.Red, 2);
		}
	}
}
