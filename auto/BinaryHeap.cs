using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace auto
{
    class BinaryHeap<T> : BinaryHeap<double, T>
    {
    }

    class BinaryHeap<K, T> where K : IComparable<K>
    {
        private readonly List<KeyValuePair<K, T>> _list = new List<KeyValuePair<K, T>> { new KeyValuePair<K, T>() };

        private void Normalize(int idx, KeyValuePair<K, T> obj)
        {
            while (idx > 1)
            {
                var next = idx >> 1;
                if (obj.Key.CompareTo(_list[next].Key) <= 0)
                    break;
                _list[idx] = _list[next];
                idx = next;
            }
            _list[idx] = obj;
        }

        public void Add(K key, T value)
        {
            int idx = _list.Count;
            var pair = new KeyValuePair<K, T>(key, value);
            _list.Add(pair);
            Normalize(idx, pair);
        }

        public T Max()
        {
            return _list[0].Value;
        }

        public T RemoveMax()
        {
            var result = _list[1];

            var temp = _list.Last();
            _list.RemoveAt(_list.Count - 1);

            int idx = 1;
            var left = idx << 1;
            var right = (idx << 1) + 1;
            bool breaked = false;

            while(right < _list.Count )
            {
                if (_list[right].Key.CompareTo(_list[left].Key) >= 0)
                {
                    if (_list[right].Key.CompareTo(temp.Key) <= 0)
                    {
                        breaked = true;
                        break;
                    }
                    _list[idx] = _list[right];
                    idx = right;
                }
                else
                {
                    if (_list[left].Key.CompareTo(temp.Key) <= 0)
                    {
                        breaked = true;
                        break;
                    }
                    _list[idx] = _list[left];
                    idx = left;
                }

                left = idx << 1;
                right = (idx << 1) + 1;
            }

            if (left < _list.Count && !breaked)
            {
                _list[idx] = _list[left];
                idx = left;
            }

            if(_list.Count > 1)
                _list[idx] = temp;

            return result.Value;
        }

        public int Count
        {
            get { return _list.Count - 1; }
        }

        public void Clear()
        {
            _list.Clear();
            _list.Add(new KeyValuePair<K, T>());
        }

        public bool Empty
        {
            get { return _list.Count == 1; }
        }
    }
}
