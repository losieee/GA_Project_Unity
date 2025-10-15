using System;
using System.Collections.Generic;

public class SimplePriorityQueue<T>
{
    private List<QueueNode> heap = new List<QueueNode>();

    private class QueueNode
    {
        public T item;
        public float priority;

        public QueueNode(T item, float priority)
        {
            this.item = item;
            this.priority = priority;
        }
    }

    public int Count
    {
        get { return heap.Count; }
    }

    
    public void Enqueue(T item, float priority)
    {
        heap.Add(new QueueNode(item, priority));
        HeapifyUp(heap.Count - 1);
    }

    
    public T Dequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        T rootItem = heap[0].item;
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        HeapifyDown(0);
        return rootItem;
    }

  
    private void HeapifyUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (heap[i].priority >= heap[parent].priority)
                break;

            QueueNode temp = heap[i];
            heap[i] = heap[parent];
            heap[parent] = temp;

            i = parent;
        }
    }

    
    private void HeapifyDown(int i)
    {
        int last = heap.Count - 1;
        while (true)
        {
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            int smallest = i;

            if (left <= last && heap[left].priority < heap[smallest].priority)
                smallest = left;
            if (right <= last && heap[right].priority < heap[smallest].priority)
                smallest = right;

            if (smallest == i)
                break;

            QueueNode temp = heap[i];
            heap[i] = heap[smallest];
            heap[smallest] = temp;

            i = smallest;
        }
    }
}
