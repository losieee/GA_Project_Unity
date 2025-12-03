using System.Collections.Generic;
using UnityEngine;

public class AstarExample : MonoBehaviour
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
    }

    void Update()
    {

    }

    

    public void ShowWallAvoidPath()
    {
        ShowShortestPath_WallAvoid();
    }

    public void ShowEnemyAvoidPath()
    {
        ShowShortestPath_EnemyAvoid();
    }

    public void ShowMaze()
    {
        MakeMaze();
    }

   

    public void MakeMaze()
    {
        if (mapX < 3) mapX = 3;
        if (mapY < 3) mapY = 3;

        start = new Vector2Int(1, 1);
        goal = new Vector2Int(mapX - 2, mapY - 2);

        const int maxTry = 50;
        int tryCount = 0;
        bool ok = false;

        while (!ok && tryCount < maxTry)
        {
            GenerateRandomMapOnce();
            ok = CanEscapeDFS();
            tryCount++;
        }

        BuildVisual();
    }

    void GenerateRandomMapOnce()
    {
        map = new int[mapX, mapY];

        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++)
            {
                if (x == 0 || y == 0 || x == mapX - 1 || y == mapY - 1)
                {
                    map[x, y] = 0;   
                }
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

        map[start.x, start.y] = 1;
        map[goal.x, goal.y] = 1;
    }

    bool CanEscapeDFS()
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        bool[,] visited = new bool[w, h];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        stack.Push(start);
        visited[start.x, start.y] = true;

        while (stack.Count > 0)
        {
            var cur = stack.Pop();
            if (cur == goal) return true;

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(map, nx, ny)) continue;
                if (map[nx, ny] == 0) continue;
                if (visited[nx, ny]) continue;

                visited[nx, ny] = true;
                stack.Push(new Vector2Int(nx, ny));
            }
        }

        return false;
    }

    

    void BuildVisual()
    {
        if (tileObjs != null)
        {
            for (int x = 0; x < tileObjs.GetLength(0); x++)
                for (int y = 0; y < tileObjs.GetLength(1); y++)
                    if (tileObjs[x, y] != null)
                        Destroy(tileObjs[x, y]);
        }
        ClearPathVisual();

        tileObjs = new GameObject[mapX, mapY];

        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++)
            {
                Vector3 pos = new Vector3(x, 0, y);
                Vector3 wallPos = new Vector3(x, 1, y);
                GameObject prefabToUse = null;

                int t = map[x, y];

                if (t == 0)
                {
                    prefabToUse = wallPrefab;
                }
                else
                {
                    switch (t)
                    {
                        case 1: prefabToUse = forestPrefab; break;
                        case 2: prefabToUse = mudPrefab; break;
                        case 3: prefabToUse = EnemyPrefab; break;
                    }
                }

                if (prefabToUse == null)
                    prefabToUse = tilePrefab;

                if (t == 0 || t == 3)
                    tileObjs[x, y] = Instantiate(prefabToUse, wallPos, Quaternion.identity, tileRoot);
                else
                    tileObjs[x, y] = Instantiate(prefabToUse, pos, Quaternion.identity, tileRoot);
            }
        }
    }

    void ClearPathVisual()
    {
        foreach (var o in pathObjs)
            if (o != null) Destroy(o);
        pathObjs.Clear();
    }

 

    void ShowShortestPath_WallAvoid()
    {
        var path = Astar_WallAvoid(map, start, goal);
        if (path == null) return;

        ClearPathVisual();

        foreach (var p in path)
        {
            if (p == start || p == goal) continue;
            var obj = Instantiate(pathPrefab, new Vector3(p.x, 0.5f, p.y), Quaternion.identity, tileRoot);
            obj.GetComponent<Renderer>().material.color = Color.green;
            pathObjs.Add(obj);
        }
    }

    void ShowShortestPath_EnemyAvoid()
    {
        var path = Astar_EnemyAvoid(map, start, goal);
        if (path == null) return;

        ClearPathVisual();

        foreach (var p in path)
        {
            if (p == start || p == goal) continue;
            var obj = Instantiate(pathPrefab, new Vector3(p.x, 0.5f, p.y), Quaternion.identity, tileRoot);
            obj.GetComponent<Renderer>().material.color = Color.yellow;
            pathObjs.Add(obj);
        }
    }

  

    List<Vector2Int> Astar_WallAvoid(int[,] map, Vector2Int start, Vector2Int goal)
    {
        return AstarCommon(map, start, goal, MoveCost_WallAvoid);
    }

    List<Vector2Int> Astar_EnemyAvoid(int[,] map, Vector2Int start, Vector2Int goal)
    {
        return AstarCommon(map, start, goal, MoveCost_EnemyAvoid);
    }

    int F(Vector2Int pos, int[,] gCost, Vector2Int goal)
    {
        return gCost[pos.x, pos.y] + H(pos, goal);
    }

    int H(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    List<Vector2Int> AstarCommon(int[,] map, Vector2Int start, Vector2Int goal,
                                 System.Func<int[,], int, int, int> moveCostFunc)
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        int[,] gCost = new int[w, h];
        bool[,] visited = new bool[w, h];
        Vector2Int?[,] parent = new Vector2Int?[w, h];

        const int INF = int.MaxValue;

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                gCost[x, y] = INF;

        gCost[start.x, start.y] = 0;

        List<Vector2Int> open = new List<Vector2Int>();
        open.Add(start);

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

                if (!InBounds(map, nx, ny)) continue;
                if (map[nx, ny] == 0) continue;
                if (visited[nx, ny]) continue;

                int newG = gCost[cur.x, cur.y] + moveCostFunc(map, nx, ny);
                if (newG < gCost[nx, ny])
                {
                    gCost[nx, ny] = newG;
                    parent[nx, ny] = cur;

                    Vector2Int next = new Vector2Int(nx, ny);
                    if (!open.Contains(next))
                        open.Add(next);
                }
            }
        }

        return null;
    }

    int MoveCost_WallAvoid(int[,] map, int x, int y)
    {
        int cost = TileCost(map[x, y]);
        if (IsNearWall(map, x, y)) cost += 3;
        return cost;
    }

    int MoveCost_EnemyAvoid(int[,] map, int x, int y)
    {
        int cost = TileCost(map[x, y]);
        if (IsNearEnemy(map, x, y)) cost += 5;
        return cost;
    }

    int TileCost(int tile)
    {
        switch (tile)
        {
            case 4: return 1;
            case 1: return 10;
            case 2: return 10;
            case 3: return 10;
            default: return int.MaxValue;
        }
    }

    bool IsNearWall(int[,] map, int x, int y)
    {
        foreach (var d in dirs)
        {
            int nx = x + d.x;
            int ny = y + d.y;
            if (!InBounds(map, nx, ny)) continue;
            if (map[nx, ny] == 0) return true;
        }
        return false;
    }

    bool IsNearEnemy(int[,] map, int x, int y)
    {
        foreach (var d in dirs)
        {
            int nx = x + d.x;
            int ny = y + d.y;
            if (!InBounds(map, nx, ny)) continue;
            if (map[nx, ny] == 3) return true;
        }
        return false;
    }

    bool InBounds(int[,] map, int x, int y)
    {
        return x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1);
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
