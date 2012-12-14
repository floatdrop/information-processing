using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auto
{
    class MarkovChain
    {
        private const double EmptyPenalty = 0.2;
        private const int MaxQueueSize = 6;
        private const int MaxEmptySize = 10;
        private const int OutputLength = 50;
        public Queue<List<MarkovState>> States = new Queue<List<MarkovState>>();
        public MarkovState Mouse1;
        public MarkovState Mouse2;
 
        public MarkovChain()
        {
            Mouse1 = new MarkovState
                {
                    Coords = Event.Start.EventCoords,
                    EmptyStepCount = 0,
                    CurrentEvent = Event.Start
                };
            Mouse2 = new MarkovState
                {
                    Coords = Event.Start.EventCoords,
                    EmptyStepCount = 0,
                    CurrentEvent = Event.Start
                };

            States.Enqueue(new List<MarkovState>{Mouse1, Mouse2});
        }

        public Tuple<List<MarkovState>, List<MarkovState>> NextStep(IEnumerable<Event> currentEvents)
        {
            var currentStates = States.Last();
            var newStates = new List<MarkovState>();

            foreach (var state in currentStates)
            {
                TryAddEmptyEvent(state, newStates);
            }

            foreach (var newEvent in currentEvents)
            {
                var newState = new MarkovState
                    {
                        Coords = newEvent.EventCoords,
                        CurrentEvent = newEvent,
                        EmptyStepCount = 0
                    };

                newStates.Add(newState);

                foreach (var state in currentStates)
                {
                    TryAddEvent(state, newState);
                }
            }

            States.Enqueue(newStates);

            if(States.Count <= MaxQueueSize)
                return Tuple.Create(new List<MarkovState>(), new List<MarkovState>());

            var graph = GenerateGraph();
            var ways = MaxFlowMinCost<MarkovState>.GetMinCostSize2Flow(graph.Start1, graph.Start2, graph.End);

            for (int i = 1; i < ways.Item1.Count - 1; i++)
            {
                ways.Item1[i].PreviousState = ways.Item1[i - 1];
            }
            for (int i = 1; i < ways.Item2.Count - 1; i++)
            {
                ways.Item2[i].PreviousState = ways.Item2[i - 1];
            }
            Mouse1 = ways.Item1[1];
            Mouse2 = ways.Item2[1];
            var way1 = GenerateWay(ways.Item1[ways.Item1.Count - 2]);
            var way2 = GenerateWay(ways.Item2[ways.Item2.Count - 2]);

            States.Dequeue();

            return Tuple.Create(way1, way2);
        }

        private List<MarkovState> GenerateWay(MarkovState state)
        {
            var way = new List<MarkovState>();

            for(int i = 0; i < OutputLength && state != null && state.CurrentEvent.Type != Event.EventType.StartEvent; i++)
            {
                way.Add(state);
                state = state.PreviousState;
            }

            way.Reverse();
            return way;
        }

        public GraphInfo GenerateGraph()
        {
            var graphInfo = new GraphInfo
                {
                    Start1 = new Node<MarkovState>
                        {
                            Obj = Mouse1
                        },
                    Start2 = new Node<MarkovState>
                        {
                            Obj = Mouse2
                        },
                    End = new Node<MarkovState>{Obj = new MarkovState()}
                };

            var stateToNode = new Dictionary<MarkovState, Node<MarkovState>>();
            stateToNode[Mouse1] = graphInfo.Start1;
            stateToNode[Mouse2] = graphInfo.Start2;
            var queue = new Queue<Tuple<Node<MarkovState>, int>>();
            queue.Enqueue(Tuple.Create(graphInfo.Start1, 0));
            queue.Enqueue(Tuple.Create(graphInfo.Start2, 0));

            var used = new HashSet<MarkovState>();

            while (queue.Count > 0)
            {
                var nodeLevel = queue.Dequeue();
                if(nodeLevel.Item2 == MaxQueueSize) //TODO зачекать
                {
                    nodeLevel.Item1.AddEdge(graphInfo.End, 1);
                    continue;
                }

                foreach (var stateLevel in nodeLevel.Item1.Obj.EventsProbability)
                {

                    if(!stateToNode.ContainsKey(stateLevel.Key))
                        stateToNode[stateLevel.Key] = new Node<MarkovState>
                            {
                                Obj = stateLevel.Key
                            };
                    var node = stateToNode[stateLevel.Key];

                    nodeLevel.Item1.AddEdge(node, stateLevel.Value);

                    if (used.Contains(stateLevel.Key))
                        continue;
                    used.Add(stateLevel.Key);

                    queue.Enqueue(Tuple.Create(node, nodeLevel.Item2 + 1));
                }
            }

            return graphInfo;
        }

        private void TryAddEmptyEvent(MarkovState state, List<MarkovState> outMarkovStates)
        {
            if (state.EmptyStepCount >= MaxEmptySize)
                return;

            var newState = new MarkovState
                {
                    Coords = state.Coords,
                    EmptyStepCount = state.EmptyStepCount + 1,
                    CurrentEvent = Event.Empty,
                    PreviousState = state
                };

            outMarkovStates.Add(newState);

            state.EventsProbability.Add(newState, EmptyPenalty);
        }

        private void TryAddEvent(MarkovState state, MarkovState newState)
        {
            if (state.EmptyStepCount >= MaxEmptySize)
                return;

            if (state.CurrentEvent.Type == Event.EventType.StartEvent)
            {
                state.EventsProbability.Add(newState, 1);
                return;
            }

            var prob = GetMoveProbability(state.Coords, newState.Coords);

            state.EventsProbability.Add(newState, prob);
        }

        private double GetMoveProbability(Rectangle rect1, Rectangle rect2)
        {
            var averageSize = (rect1.Size.Width + rect1.Size.Height) / 4 + (rect2.Size.Width + rect2.Size.Height) / 4; //что-то вроде суммы радиусов
            var center1 = Geometry.GetCenter(rect1);
            var center2 = Geometry.GetCenter(rect2);
            var dist = Math.Sqrt(Math.Pow(center1.X - center2.X, 2) + Math.Pow(center1.Y - center2.Y, 2));
            if (dist < averageSize)        //Если события происходят сильно рядом, штраф за расхождение идет небольшой
                return Math.Pow(1 - dist/averageSize/2, 1/4.0);
            return averageSize/dist*Math.Pow(1 - 1.0/2, 1/4.0);
        }
    }

    internal class GraphInfo
    {
        public Node<MarkovState> Start1;
        public Node<MarkovState> Start2;
        public Node<MarkovState> End;
    }

    class MarkovState
    {
        public int EmptyStepCount = 0;
        public Event CurrentEvent;
        public Dictionary<MarkovState, double> EventsProbability = new Dictionary<MarkovState, double>();
        public Rectangle Coords;
        public MarkovState PreviousState = null;
    }
}
