using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Structure;

namespace auto
{
    class Event
    {
        public enum EventType
        {
            ObjectNotFound,
            SomethingIsMoving,
            ObjectIsFounded,
            StartEvent
        }

        public static Event Empty = new Event(new Rectangle(320, 240, 0, 0), EventType.ObjectNotFound);
        public static Event Start = new Event(new Rectangle(320, 240, 0, 0), EventType.StartEvent);

        public Rectangle EventCoords;
        public EventType Type;

		public Event(Rectangle coords, EventType type)
		{
			EventCoords = coords;
			AverageColor = new Bgr(255,255,255);
			Type = type;
		}

        public Event(MCvConnectedComp comp, EventType type)
        {
            EventCoords = comp.rect;
	        AverageColor = new Bgr(comp.value.v0, comp.value.v1, comp.value.v2);
            Type = type;
        }

	    public Bgr AverageColor;
    }
}
