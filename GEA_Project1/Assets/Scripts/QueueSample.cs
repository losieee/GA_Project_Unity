using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueSample : MonoBehaviour
{
    
    void Start()
    {
        Queue<string> queue = new Queue<string>();

        queue.Enqueue("첫번째");
        queue.Enqueue("두번째");
        queue.Enqueue("세번째");

        Debug.Log("====== Queue 1 ======");
        foreach (string item in queue)
            Debug.Log(item);
        Debug.Log("======================");

        Debug.Log("Peek: " + queue.Peek());

        Debug.Log("Dequeue: " + queue.Dequeue());
        Debug.Log("Dequeue: " + queue.Dequeue());

        Debug.Log("남은 데이터 수: " + queue.Count);

        foreach (string item in queue)
            Debug.Log(item);
    }

    
}
