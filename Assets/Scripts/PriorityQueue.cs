using System;
using System.Collections.Generic;

public class PriorityQueue<TElement, TPriority>
{
    private readonly List<(TElement element, TPriority priority)> items;
    private readonly IComparer<TPriority> comparer;

    public PriorityQueue() : this(null) { }

    public PriorityQueue(IComparer<TPriority> comparer)
    {
        this.comparer = comparer ?? Comparer<TPriority>.Default;
        items = new List<(TElement, TPriority)>();
    }

    public int Count => items.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        items.Add((element, priority));
        int childIndex = items.Count - 1;

        while (childIndex > 0)
        {
            int parentIndex = (childIndex - 1) / 2;

            if (comparer.Compare(items[childIndex].priority, items[parentIndex].priority) >= 0)
                break;

            Swap(childIndex, parentIndex);
            childIndex = parentIndex;
        }
    }

    public TElement Dequeue()
    {
        if (items.Count == 0)
            throw new InvalidOperationException("PriorityQueue is empty.");

        var dequeuedItem = items[0].element;
        int lastIndex = items.Count - 1;
        items[0] = items[lastIndex];
        items.RemoveAt(lastIndex);

        int parentIndex = 0;
        int childIndex = 1;

        while (childIndex < lastIndex)
        {
            int rightChildIndex = childIndex + 1;

            if (rightChildIndex < lastIndex && comparer.Compare(items[rightChildIndex].priority, items[childIndex].priority) < 0)
                childIndex = rightChildIndex;

            if (comparer.Compare(items[childIndex].priority, items[parentIndex].priority) >= 0)
                break;

            Swap(childIndex, parentIndex);
            parentIndex = childIndex;
            childIndex = 2 * parentIndex + 1;
        }

        return dequeuedItem;
    }

    public TElement Peek()
    {
        if (items.Count == 0)
            throw new InvalidOperationException("PriorityQueue is empty.");

        return items[0].element;
    }

    private void Swap(int index1, int index2)
    {
        var temp = items[index1];
        items[index1] = items[index2];
        items[index2] = temp;
    }
}
