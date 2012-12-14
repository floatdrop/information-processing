using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auto
{
    class MaxFlowMinCost<T>
    {
        public static Tuple<List<T>, List<T>> GetMinCostSize2Flow(Node<T> input1, Node<T> input2, Node<T> output)
        {
            var queue = new PriorityQueue<State<T>>();
            queue.Enqueue(1, new State<T>(input1, input2, 1, null));
            var used = new HashSet<State<T>>();

            while (!queue.Empty)
            {
                var state = queue.Dequeue();
                if(used.Contains(state))
                    continue;
                used.Add(state);

                if (state.FirstObjState == output && state.SecondObjState == output)
                    return GetWays(state);

                foreach (var edge in state.FirstObjState.Edges.Where(edge => edge.To != state.SecondObjState || edge.To == output))
                {
                    var newState = new State<T>(edge.To, state.SecondObjState, state.Probability * edge.Weight, state);

                    if (used.Contains(newState))
                        continue;

                    queue.Enqueue(newState.Probability, newState);
                }

                foreach (var edge in state.SecondObjState.Edges.Where(edge => edge.To != state.FirstObjState || edge.To == output))
                {
                    var newState = new State<T>(state.FirstObjState, edge.To, state.Probability * edge.Weight, state);

                    if (used.Contains(newState))
                        continue;

                    queue.Enqueue(newState.Probability, newState);
                }
            }

            return Tuple.Create(new List<T>(), new List<T>());
        }

        private static Tuple<List<T>, List<T>> GetWays(State<T> state)
        {
            var way1 = new List<T>();
            var way2 = new List<T>();

            way1.Add(state.FirstObjState.Obj);
            way2.Add(state.SecondObjState.Obj);

            while(state != null)
            {
                if (!way1[way1.Count - 1].Equals(state.FirstObjState.Obj))
                    way1.Add(state.FirstObjState.Obj);
                if (!way2[way2.Count - 1].Equals(state.SecondObjState.Obj))
                    way2.Add(state.SecondObjState.Obj);
                state = state.Dad;
            }

            way1.Reverse();
            way2.Reverse();

            return Tuple.Create(way1, way2);
        }
    }

    public class Node<T>
    {
        public List<Edge<T>> Edges = new List<Edge<T>>();
        public T Obj;

        public void AddEdge(Node<T> node, double weight)
        {
            Edges.Add(new Edge<T> { To = node, Weight = weight });
        }
    }

    public class Edge<T>
    {
        public bool Used = false;
        public double Weight;
        public Node<T> To;
    }

    class State<T>
    {
        public readonly Node<T> FirstObjState;
        public readonly Node<T> SecondObjState;
        public readonly State<T> Dad;
        public readonly double Probability;

        public State(Node<T> state1, Node<T> state2, double probability, State<T> dad)
        {
            FirstObjState = state1;
            SecondObjState = state2;
            Probability = probability;
            Dad = dad;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is State<T>))
                return false;
            var state = (State<T>) obj;
            return state.FirstObjState == FirstObjState && state.SecondObjState == SecondObjState;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(FirstObjState, SecondObjState).GetHashCode();
        }
    }
}
