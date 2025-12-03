using System.Collections.Generic;
using UnityEngine;

public class Astar_WallAvoid : MonoBehaviour
{
    [SerializeField] int mapX = 21;
    [SerializeField] int mapY = 21;

    int[,] map;
    GameObject[,] tileObjs;
    List<GameObject> pathObjs = new List<GameObject>();

    Vector2Int start = new Vector2Int(1, 1);
    Vector2Int goal;

    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject pathPrefab;
    [SerializeField] GameObject mudPrefab;
    [SerializeField] GameObject forestPrefab;
    [SerializeField] Transform tileRoot;
    [SerializeField] GameObject EnemyPrefab;

    readonly Vector2Int[] dirs =
    {
        new Vector2Int( 1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int( 0, 1),
        new Vector2Int( 0,-1),
    };

    void Start()
    {
        MakeMaze();
        ShowShortestPath();
    }

    public void ShowMaze()
    {
        MakeMaze();
    }

    public void ShowShortestPath()
    {
        var path = Astar(map, start, goal);
        if (path == null) return;

        ClearPath();
        foreach (var p in path)
        {
            if (p == start || p == goal) continue;
            var o = Instantiate(pathPrefab, new Vector3(p.x, 0.5f, p.y), Quaternion.identity, tileRoot);
            pathObjs.Add(o);
        }
    }

    // --------------------- MAP 생성 ---------------------
    void MakeMaze()
    {
        start = new Vector2Int(1, 1);
        goal = new Vector2Int(mapX - 2, mapY - 2);

        bool ok = false;
        int tryCount = 0;

        while (!ok && tryCount < 50)
        {
            GenerateMap();
            ok = CanEscapeDFS();
            tryCount++;
        }

        BuildVisual();
    }

    void GenerateMap()
    {
        map = new int[mapX, mapY];

        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++)
            {
                if (x == 0 || y == 0 || x == mapX - 1 || y == mapY - 1)
                    map[x, y] = 0;
                else
                {
                    float r = Random.value;
                    if (r < 0.2f) map[x, y] = 0;
                    else if (r < 0.25f) map[x, y] = 1;
                    else if (r < 0.3f) map[x, y] = 2;
                    else if (r < 0.35f) map[x, y] = 3;
                    else map[x, y] = 4;
                }
            }
        }
        map[start.x, start.y] = 4;
        map[goal.x, goal.y] = 4;
    }

    bool CanEscapeDFS()
    {
        bool[,] visited = new bool[mapX, mapY];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            var cur = stack.Pop();
            if (cur == goal) return true;
            visited[cur.x, cur.y] = true;

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(nx, ny)) continue;
                if (map[nx, ny] == 0) continue;
                if (visited[nx, ny]) continue;

                stack.Push(new Vector2Int(nx, ny));
            }
        }
        return false;
    }

    void BuildVisual()
    {
        if (tileObjs != null)
        {
            for (int x = 0; x < mapX; x++)
                for (int y = 0; y < mapY; y++)
                    if (tileObjs[x, y] != null) Destroy(tileObjs[x, y]);
        }

        ClearPath();
        tileObjs = new GameObject[mapX, mapY];

        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++)
            {
                GameObject obj;
                int t = map[x, y];

                switch (t)
                {
                    case 0: obj = wallPrefab; break;
                    case 1: obj = forestPrefab; break;
                    case 2: obj = mudPrefab; break;
                    case 3: obj = EnemyPrefab; break;
                    default: obj = tilePrefab; break;
                }

                Vector3 pos = (t == 0 || t == 3) ? new Vector3(x, 1, y) : new Vector3(x, 0, y);
                tileObjs[x, y] = Instantiate(obj, pos, Quaternion.identity, tileRoot);
            }
        }
    }

    void ClearPath()
    {
        foreach (var o in pathObjs)
            Destroy(o);
        pathObjs.Clear();
    }

    // --------------------- A* (벽 회피) ---------------------
    List<Vector2Int> Astar(int[,] map, Vector2Int start, Vector2Int goal)
    {
        int[,] gCost = new int[mapX, mapY];
        bool[,] visited = new bool[mapX, mapY];
        Vector2Int?[,] parent = new Vector2Int?[mapX, mapY];

        const int INF = int.MaxValue;

        for (int x = 0; x < mapX; x++)
            for (int y = 0; y < mapY; y++)
                gCost[x, y] = INF;

        gCost[start.x, start.y] = 0;

        List<Vector2Int> open = new List<Vector2Int>() { start };

        while (open.Count > 0)
        {
            int bestIndex = 0;
            int bestF = gCost[open[0].x, open[0].y] + H(open[0], goal);

            for (int i = 1; i < open.Count; i++)
            {
                int f = gCost[open[i].x, open[i].y] + H(open[i], goal);
                if (f < bestF)
                {
                    bestF = f;
                    bestIndex = i;
                }
            }

            Vector2Int cur = open[bestIndex];
            open.RemoveAt(bestIndex);

            if (visited[cur.x, cur.y]) continue;
            visited[cur.x, cur.y] = true;

            if (cur == goal)
                return ReconstructPath(parent, start, goal);

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(nx, ny)) continue;
                if (map[nx, ny] == 0) continue;
                if (visited[nx, ny]) continue;

                int moveCost = MoveCost(map, nx, ny);
                int newG = gCost[cur.x, cur.y] + moveCost;

                if (newG < gCost[nx, ny])
                {
                    gCost[nx, ny] = newG;
                    parent[nx, ny] = cur;

                    Vector2Int next = new Vector2Int(nx, ny);
                    if (!open.Contains(next)) open.Add(next);
                }
            }
        }
        return null;
    }

    int MoveCost(int[,] map, int x, int y)
    {
        int t = map[x, y];
        int cost = (t == 4) ? 1 : 10;  // 4=땅 가장 싸고, 나머지 비용 높임

        if (IsNearWall(x, y)) cost += 2;

        return cost;
    }

    bool IsNearWall(int x, int y)
    {
        foreach (var d in dirs)
        {
            int nx = x + d.x;
            int ny = y + d.y;
            if (map[nx, ny] == 0) return true;
        }
        return false;
    }

    bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < mapX && y < mapY;
    }

    int H(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    List<Vector2Int> ReconstructPath(Vector2Int?[,] parent, Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int? cur = goal;

        while (cur.HasValue)
        {
            path.Add(cur.Value);
            if (cur.Value == start) break;
            cur = parent[cur.Value.x, cur.Value.y];
        }

        path.Reverse();
        return path;
    }
}
