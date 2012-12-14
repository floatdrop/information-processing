using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace auto
{
    public class MouseRegions
    {
        public readonly List<MouseRegion> Regions = new List<MouseRegion>();

        public void Add(MCvConnectedComp comp, Image<Bgr, byte> img)
        {
            if (!IsConCompMousabe(comp))
                return;

            var foundedRegion = FindRegionFor(comp);
            if (foundedRegion == null)
                Regions.Add(new MouseRegion(comp));
            else
                foundedRegion.Update(comp);

        }

        private MouseRegion FindRegionFor(MCvConnectedComp comp)
        {
            double match = 0;
            MouseRegion result = null;
            foreach (var mouseRegion in Regions)
            {
                if (comp.rect.IntersectsWith(mouseRegion.Rectangle))
                {
                    Rectangle intersection = comp.rect;
                    intersection.Intersect(mouseRegion.Rectangle);
                    var newMatch = intersection.Width*intersection.Height;
                    if (newMatch > match)
                    {
                        result = mouseRegion;
                        match = newMatch;
                    }
                }
            }
            if (match > 0)
                return result;

            // TODO: Iterate all regions and try to found previous by optical flow ?
            // Maybe in that ^^^^ foreach

            return null;
        }

        private static MCvFont _font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_SIMPLEX, 0.5, 0.5);

        public void DrawRegions(Image<Bgr, byte> dataImage)
        {

            foreach (var region in Regions.Where(r=>!r.IsUpdated))
            {
                // TODO: Try to find, where that region is gone with optical flow
            }

            var candidatesToDelete = new List<MouseRegion>();
            
            foreach (var region in Regions)
            {
                region.Probability -= 0.1;
                if (region.Probability < 0.2)
                    candidatesToDelete.Add(region);
                if (region.Probability < 0.4)
                    continue;
                dataImage.DrawPolyline(region.Trajectory.ToArray(), false, new Bgr(0, 255, 0), 2);
                dataImage.Draw(region.Rectangle, new Bgr(0, 0, 255), 2);
                dataImage.Draw(region.Id, ref _font, region.Center, new Bgr(0, 0, 255));
                region.IsUpdated = false;
            }
            
            foreach (var d in candidatesToDelete)
                Regions.Remove(d);
            
        }

        private bool IsConCompMousabe(MCvConnectedComp comp)
        {
            return
                comp.rect.Width > 25 &&
                comp.rect.Width < 300 &&
                comp.rect.Height > 25 &&
                comp.rect.Height < 300 &&
                comp.area > 400 &&
                comp.area < 2500;
        }

    }

    public class MouseRegion
    {
        public MCvConnectedComp Component;
        public double Probability = 0.5;
        public string Id = GetId();
        public List<Point> Trajectory = new List<Point>(); 

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(Component.rect.Left, Component.rect.Top, Component.rect.Width,
                                     Component.rect.Height);
            }
        }

        public Point Center
        {
            get { return new Point(Component.rect.Left + Component.rect.Width / 2, Component.rect.Top + Component.rect.Height / 2); }
        }

        public bool IsUpdated { get; set; }

        private static string GetId()
        {
            StaticIdCounter += 1;
            return StaticIdCounter.ToString(CultureInfo.InvariantCulture);
        }

        protected static int StaticIdCounter;

        public MouseRegion(MCvConnectedComp comp)
        {
            Component = comp;
            Trajectory.Add(Center);
        }

        public void Update(MCvConnectedComp comp)
        {
            if (Probability < 1)
                Probability += 0.2;
            Component = comp;
            Trajectory.Add(Center);
            IsUpdated = true;
        }
    }

}