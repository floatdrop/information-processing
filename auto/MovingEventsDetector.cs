using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace auto
{
    class MovingEventsDetector
    {
        public Image<Bgr, Byte> Aggregator = new Image<Bgr, byte>(640, 480);
        private Image<Bgr, Byte> _prevImage = new Image<Bgr, byte>(640, 480);
        const double PartSize = 0.2;

        public List<Event> GetMovingEvents(Image<Bgr, Byte> source)
        {
            var events = new List<Event>();

            var difference = MovingDetection(source).Convert<Gray, Byte>();
            _prevImage = source.Copy();
            difference = difference.ThresholdBinary(new Gray(80), new Gray(255));
            var distTransformed = new Image<Gray, float>(source.Width, source.Height);
            CvInvoke.cvDistTransform(difference.Ptr, distTransformed.Ptr, DIST_TYPE.CV_DIST_L2, 3, new[] { 1f, 1f }, IntPtr.Zero);
            var byteDist = distTransformed.ThresholdBinary(new Gray(1), new Gray(255)).Convert<Gray, byte>();

            Image<Gray, byte> mask = new Image<Gray, byte>(byteDist.Width + 2, byteDist.Height + 2);
            mask.ROI = new Rectangle(1, 1, byteDist.Width, byteDist.Height);
            CvInvoke.cvCopy(byteDist, mask, IntPtr.Zero);
            mask.ROI = new Rectangle(0, 0, byteDist.Width + 2, byteDist.Height + 2);
            mask = mask.Not();
            //return mask.Convert<Bgr, Byte>();

            var result = difference.Convert<Bgr, Byte>();

            for (int i = 0; i < difference.Width; i++)
            {
                for (int j = 0; j < difference.Height; j++)
                {
                    if (mask.Data[j, i, 0] == 0)
                    {
                        var comp = new MCvConnectedComp();
                        CvInvoke.cvFloodFill(
                            difference.Ptr,
                            new Point(i, j),
                            new MCvScalar(200, 200, 200, 0), // Color
                            new MCvScalar(0, 0, 0), // Lo
                            new MCvScalar(0, 0, 0),  // Up
                            out comp,
                            Emgu.CV.CvEnum.CONNECTIVITY.EIGHT_CONNECTED,
                            Emgu.CV.CvEnum.FLOODFILL_FLAG.DEFAULT,
                            mask.Ptr
                        );

                        if (comp.area > 500 && comp.area < 2500
                            && comp.rect.Size.Height > 10 && comp.rect.Size.Height < 230
                            && comp.rect.Size.Width > 10 && comp.rect.Size.Width < 230
							&& BlobDetector.IsAreaColorMousable(comp, source, mask))
                        {
                            events.Add(new Event(comp.rect, Event.EventType.SomethingIsMoving));
                        }
                    }
                }
            }
            return events.Count > 3 ? new List<Event>() : events;
        }

        public Image<Bgr, Byte> GetMoving(Image<Bgr, Byte> source)
        {
            //var sub = currentImage.AbsDiff(_prevImage).Convert<Gray, Byte>();
            var difference = MovingDetection(source).Convert<Gray, Byte>();
            _prevImage = source.Copy();
            difference = difference.ThresholdBinary(new Gray(50), new Gray(255));
            var distTransformed = new Image<Gray, float>(source.Width, source.Height);
            CvInvoke.cvDistTransform(difference.Ptr, distTransformed.Ptr, DIST_TYPE.CV_DIST_L2, 3, new[] { 1f, 1f }, IntPtr.Zero);
            var byteDist = distTransformed.ThresholdBinary(new Gray(2), new Gray(255)).Convert<Gray, byte>();

            Image<Gray, byte> mask = new Image<Gray, byte>(byteDist.Width + 2, byteDist.Height + 2);
            mask.ROI = new Rectangle(1, 1, byteDist.Width, byteDist.Height);
            CvInvoke.cvCopy(byteDist, mask, IntPtr.Zero);
            mask.ROI = new Rectangle(0, 0, byteDist.Width + 2, byteDist.Height + 2);
            mask = mask.Not();
            //return mask.Convert<Bgr, Byte>();

            var result = mask.Convert<Bgr, Byte>();

            for (int i = 0; i < difference.Width; i++)
            {
                for (int j = 0; j < difference.Height; j++)
                {
                    if (mask.Data[j, i, 0] == 0)
                    {
                        var comp = new MCvConnectedComp();
                        CvInvoke.cvFloodFill(
                            difference.Ptr,
                            new Point(i, j),
                            new MCvScalar(200, 200, 200, 0), // Color
                            new MCvScalar(0, 0, 0), // Lo
                            new MCvScalar(0, 0, 0),  // Up
                            out comp,
                            Emgu.CV.CvEnum.CONNECTIVITY.EIGHT_CONNECTED,
                            Emgu.CV.CvEnum.FLOODFILL_FLAG.DEFAULT,
                            mask.Ptr
                        );

                        if (comp.area > 500 && comp.area < 2500
                            && comp.rect.Size.Height > 10 && comp.rect.Size.Height < 230
                            && comp.rect.Size.Width > 10 && comp.rect.Size.Width < 230)
                        {
                            ReplaceColors(result, comp.rect);
                        }
                    }
                }
            }

            return result.Convert<Bgr, Byte>();
        }

        private static void ReplaceColors(Image<Bgr, byte> edges, Rectangle roi)
        {
            for (int i = roi.Left; i < roi.Right; i++)
            {
                for (int j = roi.Top; j < roi.Bottom; j++)
                {
                    if(edges.Data[j,i,0] == 255)
                    {
                        edges.Data[j, i, 0] = 255;
                        edges.Data[j, i, 1] = 0;
                        edges.Data[j, i, 2] = 0;
                    }
                }
            }
        }
        private Image<Bgr, Byte> MovingDetection(Image<Bgr, Byte> currentImage)
        {
            Image<Bgr, byte> tempImage = Aggregator;
            Image<Bgr, byte> newImage = Aggregator.Mul(1 - PartSize).Add(currentImage.Mul(PartSize));
            Aggregator = newImage;
            return newImage.AbsDiff(tempImage).Mul(10);
        }
    }
}
