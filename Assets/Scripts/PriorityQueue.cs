using System;
using System.Collections.Generic;

public class PriorityQueue<TPriority, TValue> where TPriority : IComparable<TPriority>
{
    private List<KeyValuePair<TPriority, TValue>> _baseHeap;
    private IComparer<TPriority> _comparer;

    public PriorityQueue()
    {
        _baseHeap = new List<KeyValuePair<TPriority, TValue>>();
        _comparer = Comparer<TPriority>.Default;
    }

    public int Count
    {
        get { return _baseHeap.Count; }
    }

    public void Enqueue(TPriority priority, TValue value)
    {
        Insert(priority, value);
    }

    public KeyValuePair<TPriority, TValue> Dequeue()
    {
        if (!IsEmpty)
        {
            KeyValuePair<TPriority, TValue> result = _baseHeap[0];
            DeleteRoot();
            return result;
        }
        else
        {
            throw new InvalidOperationException("Priority queue is empty");
        }
    }

    public bool IsEmpty
    {
        get { return _baseHeap.Count == 0; }
    }

    private void Insert(TPriority priority, TValue value)
    {
        KeyValuePair<TPriority, TValue> val = new KeyValuePair<TPriority, TValue>(priority, value);
        _baseHeap.Add(val);

        int pos = _baseHeap.Count - 1;
        int parentPos = (pos - 1) / 2;

        while (pos > 0 && _comparer.Compare(_baseHeap[parentPos].Key, _baseHeap[pos].Key) > 0)
        {
            Swap(parentPos, pos);
            pos = parentPos;
            parentPos = (pos - 1) / 2;
        }
    }

    private void DeleteRoot()
    {
        if (_baseHeap.Count <= 1)
        {
            _baseHeap.Clear();
            return;
        }

        _baseHeap[0] = _baseHeap[_baseHeap.Count - 1];
        _baseHeap.RemoveAt(_baseHeap.Count - 1);

        int pos = 0;
        while (true)
        {
            int childPos = pos * 2 + 1;
            if (childPos >= _baseHeap.Count) break;

            int rightChildPos = childPos + 1;
            if (rightChildPos < _baseHeap.Count && _comparer.Compare(_baseHeap[rightChildPos].Key, _baseHeap[childPos].Key) < 0)
            {
                childPos = rightChildPos;
            }

            if (_comparer.Compare(_baseHeap[pos].Key, _baseHeap[childPos].Key) <= 0) break;

            Swap(pos, childPos);
            pos = childPos;
        }
    }

    private void Swap(int i, int j)
    {
        KeyValuePair<TPriority, TValue> tmp = _baseHeap[i];
        _baseHeap[i] = _baseHeap[j];
        _baseHeap[j] = tmp;
    }
}
