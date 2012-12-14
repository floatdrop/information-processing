using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static Event Empty = new Event(new Rectangle(620, 240, 0, 0), EventType.ObjectNotFound);
        public static Event Start = new Event(new Rectangle(620, 240, 0, 0), EventType.StartEvent);

        public Rectangle EventCoords;
        public EventType Type;

        public Event(Rectangle coords, EventType type)
        {
            EventCoords = coords;
            Type = type;
        }
    }
}
