using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandRecorder : MonoBehaviour
{
    public float speed = 5f;
    public Material normalMat;
    public Material rewindMat;

    struct MoveRecord
    {
        public Vector3 pos;
        public float time;
        public MoveRecord(Vector3 p, float t)
        {
            pos = p;
            time = t;
        }
    }

    Queue<MoveRecord> moveQueue;
    Queue<Vector3> rewindQueue;

    bool doing = false;

    Renderer rend;

    void Start()
    {
        moveQueue = new Queue<MoveRecord>();
        rewindQueue = new Queue<Vector3>();
        rend = GetComponent<Renderer>();
        rend.material = normalMat;
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (!doing)
        {
            if (x != 0 || y != 0)
            {
                Vector3 moveDir = new Vector3(x, y, 0).normalized;
                Vector3 targetPos = transform.position + moveDir;
                moveQueue.Enqueue(new MoveRecord(targetPos, Time.time));
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if ((moveQueue.Count > 0 || rewindQueue.Count > 0) && !doing)
            {
                StartCoroutine(RunQueue());
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!doing && moveQueue.Count > 0)
            {
                PrepareRewind();
            }
        }
    }

    IEnumerator RunQueue()
    {
        doing = true;

        while (moveQueue.Count > 0)
        {
            Vector3 next = moveQueue.Dequeue().pos;
            while (Vector3.Distance(transform.position, next) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, next, speed * Time.deltaTime);
                yield return null;
            }
        }

        if (rewindQueue.Count > 0)
        {
            rend.material = rewindMat;

            while (rewindQueue.Count > 0)
            {
                Vector3 back = rewindQueue.Dequeue();
                while (Vector3.Distance(transform.position, back) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, back, speed * Time.deltaTime);
                    yield return null;
                }
            }

            rend.material = normalMat;
        }

        doing = false;
    }

    void PrepareRewind()
    {
        rewindQueue.Clear();
        float now = Time.time;

        List<MoveRecord> temp = new List<MoveRecord>(moveQueue);
        for (int i = temp.Count - 1; i >= 0; i--)
        {
            if (now - temp[i].time <= 2f)
            {
                rewindQueue.Enqueue(temp[i].pos);
            }
            else break;
        }
    }
}
