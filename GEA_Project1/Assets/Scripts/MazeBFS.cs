using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBFS : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject player;

    public int w = 21;
    public int h = 21;

    int[,] map;
    GameObject[,] tile;
    Vector2Int start = new Vector2Int(1, 1);
    Vector2Int goal;
    List<Vector2Int> path;

    void Start()
    {
        goal = new Vector2Int(w - 2, h - 2);

        if (player == null)
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.transform.localScale = new Vector3(0.7f, 1f, 0.7f);
        }

        MakeNewMaze();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            MakeNewMaze();
    }

    void MakeNewMaze()
    {
        StopAllCoroutines();

        if (tile != null)
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    if (tile[x, y] != null)
                        Destroy(tile[x, y]);

        map = null;

        while (true)
        {
            map = MakeMaze();
            if (CheckDFS(start.x, start.y, new bool[w, h])) break;
        }

        DrawMaze();
        player.transform.position = new Vector3(start.x, 0.5f, start.y);
        path = null;
    }

    int[,] MakeMaze()
    {
        int[,] m = new int[w, h];

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                m[x, y] = 1;

        for (int x = 1; x < w; x += 2)
            for (int y = 1; y < h; y += 2)
                m[x, y] = 0;

        System.Random r = new System.Random();

        for (int x = 1; x < w - 1; x += 2)
            for (int y = 1; y < h - 1; y += 2)
            {
                int dir = r.Next(4);
                int nx = x + (dir == 0 ? 1 : dir == 1 ? -1 : 0);
                int ny = y + (dir == 2 ? 1 : dir == 3 ? -1 : 0);

                if (nx > 0 && ny > 0 && nx < w && ny < h)
                    m[nx, ny] = 0;
            }

        m[start.x, start.y] = 0;
        m[goal.x, goal.y] = 0;

        return m;
    }

    bool CheckDFS(int x, int y, bool[,] v)
    {
        if (map[x, y] == 1) return false;
        if (x == goal.x && y == goal.y) return true;

        v[x, y] = true;

        Vector2Int[] d = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

        foreach (var i in d)
        {
            int nx = x + i.x;
            int ny = y + i.y;

            if (nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
            if (map[nx, ny] == 1) continue;
            if (v[nx, ny]) continue;

            if (CheckDFS(nx, ny, v)) return true;
        }

        return false;
    }

    void DrawMaze()
    {
        tile = new GameObject[w, h];

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
            {
                GameObject p = map[x, y] == 1 ? wallPrefab : floorPrefab;
                GameObject o = Instantiate(p,
                    new Vector3(x, map[x, y] == 1 ? 0.5f : 0f, y),
                    Quaternion.identity);

                o.transform.localScale =
                    map[x, y] == 1 ? new Vector3(1, 1, 1) : new Vector3(1, 0.1f, 1);

                tile[x, y] = o;
            }
    }

    List<Vector2Int> FindPath()
    {
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        bool[,] v = new bool[w, h];
        Vector2Int?[,] parent = new Vector2Int?[w, h];

        q.Enqueue(start);
        v[start.x, start.y] = true;

        Vector2Int[] d = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

        while (q.Count > 0)
        {
            Vector2Int cur = q.Dequeue();
            if (cur == goal) return Build(parent);

            foreach (var i in d)
            {
                int nx = cur.x + i.x;
                int ny = cur.y + i.y;

                if (nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
                if (map[nx, ny] == 1) continue;
                if (v[nx, ny]) continue;

                v[nx, ny] = true;
                parent[nx, ny] = cur;
                q.Enqueue(new Vector2Int(nx, ny));
            }
        }
        return null;
    }

    List<Vector2Int> Build(Vector2Int?[,] p)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        Vector2Int? cur = goal;

        while (cur.HasValue)
        {
            list.Add(cur.Value);
            cur = p[cur.Value.x, cur.Value.y];
        }

        list.Reverse();
        return list;
    }

    public void ShowPath()
    {
        path = FindPath();
        if (path == null) return;

        foreach (var v in path)
            tile[v.x, v.y].GetComponent<Renderer>().material.color = Color.green;
    }

    public void AutoMove()
    {
        if (path == null) path = FindPath();
        if (path == null) return;

        StopAllCoroutines();
        StartCoroutine(Move(path));
    }

    IEnumerator Move(List<Vector2Int> p)
    {
        foreach (var v in p)
        {
            Vector3 t = new Vector3(v.x, 0.5f, v.y);

            while (Vector3.Distance(player.transform.position, t) > 0.05f)
            {
                player.transform.position =
                    Vector3.MoveTowards(player.transform.position, t, Time.deltaTime * 3f);
                yield return null;
            }
        }
    }
}
