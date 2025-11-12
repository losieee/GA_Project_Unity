using System.Collections.Generic;
using UnityEngine;

public class SimpleMaze : MonoBehaviour
{
    public int width = 21;
    public int height = 21;

    int[,] map;
    List<GameObject> spawned = new List<GameObject>();
    Vector2Int startPos;
    Vector2Int goalPos;
    List<Vector2Int> path = new List<Vector2Int>();

    void Start()
    {
        MakeMaze();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MakeMaze();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            FindPath();
            ShowPath();
        }
    }

    void MakeMaze()
    {
        foreach (var o in spawned) Destroy(o);
        spawned.Clear();
        path.Clear();

        map = new int[width, height];
        startPos = new Vector2Int(1, 1);
        goalPos = new Vector2Int(width - 2, height - 2);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;

        for (int x = 1; x < width - 1; x++)
            for (int y = 1; y < height - 1; y++)
                if (Random.value <= 0.7f) map[x, y] = 0;

        for (int i = 0; i < 40; i++)
        {
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, height - 1);
            map[x, y] = 0;
        }

        map[startPos.x, startPos.y] = 0;
        map[goalPos.x, goalPos.y] = 0;

        Draw();
    }

    void Draw()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    GameObject w = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    w.transform.position = new Vector3(x, 0.5f, y);
                    w.transform.localScale = new Vector3(1f, 1f, 1f);
                    w.GetComponent<Renderer>().material.color = Color.red;
                    spawned.Add(w);
                }
                else
                {
                    GameObject f = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    f.transform.position = new Vector3(x, -0.5f, y);
                    f.transform.localScale = new Vector3(1f, 0.1f, 1f);
                    f.GetComponent<Renderer>().material.color = Color.white;
                    spawned.Add(f);
                }
            }
        }

        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
        s.transform.position = new Vector3(startPos.x, 0.0f, startPos.y);
        s.transform.localScale = new Vector3(1f, 0.6f, 1f);
        s.GetComponent<Renderer>().material.color = Color.cyan;
        spawned.Add(s);

        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.transform.position = new Vector3(goalPos.x, 0.0f, goalPos.y);
        g.transform.localScale = new Vector3(1f, 0.6f, 1f);
        g.GetComponent<Renderer>().material.color = Color.yellow;
        spawned.Add(g);
    }

    void FindPath()
    {
        path.Clear();
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        bool[,] vis = new bool[width, height];
        q.Enqueue(startPos);
        vis[startPos.x, startPos.y] = true;
        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };
        bool found = false;

        while (q.Count > 0)
        {
            Vector2Int cur = q.Dequeue();
            if (cur == goalPos)
            {
                found = true;
                break;
            }
            for (int i = 0; i < 4; i++)
            {
                int nx = cur.x + dx[i];
                int ny = cur.y + dy[i];
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    if (!vis[nx, ny] && map[nx, ny] == 0)
                    {
                        vis[nx, ny] = true;
                        Vector2Int next = new Vector2Int(nx, ny);
                        parent[next] = cur;
                        q.Enqueue(next);
                    }
                }
            }
        }

        if (!found) return;

        Vector2Int p = goalPos;
        while (p != startPos)
        {
            path.Add(p);
            if (!parent.ContainsKey(p)) break;
            p = parent[p];
        }
        path.Add(startPos);
        path.Reverse();
    }

    void ShowPath()
    {
        foreach (var o in spawned)
        {
            if (o != null && o.name.Contains("PathMarker")) Destroy(o);
        }
        spawned.RemoveAll(o => o == null || o.name.Contains("PathMarker"));

        for (int i = 0; i < path.Count; i++)
        {
            Vector2Int p = path[i];
            GameObject m = GameObject.CreatePrimitive(PrimitiveType.Cube);
            m.name = "PathMarker";
            m.transform.position = new Vector3(p.x, 0.2f, p.y);
            m.transform.localScale = new Vector3(0.6f, 0.2f, 0.6f);
            m.GetComponent<Renderer>().material.color = Color.blue;
            spawned.Add(m);
        }
    }
}
