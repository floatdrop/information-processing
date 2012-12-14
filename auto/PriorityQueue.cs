using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace auto
{
    class PriorityQueue<T>
    {
        private readonly BinaryHeap<T> _queue = new BinaryHeap<T>();

        public void Enqueue(double priority, T element)
        {
            _queue.Add(priority, element);
        }

        public T Dequeue()
        {
            return _queue.RemoveMax();
        }

        public T Peek()
        {
            return _queue.Max();
        }

        public bool Empty
        {
            get { return _queue.Count == 0; }
        }

        public int Count
        {
            get { return _queue.Count; }
        }

        public void Clear()
        {
            _queue.Clear();
        }
    }
}
