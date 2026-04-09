using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public int Row;
    public int Col;

    public int G;
    public int H;
    public int F => G + H;

    public PathNode Parent;

    public PathNode(int r, int c)
    {
        Row = r;
        Col = c;
    }
}

public class MapMoverManager : SingletonMonoBehaviour<MapMoverManager>
{
    [SerializeField] private float moveSpeed;

    [SerializeField] private List<CarController> cars = new List<CarController>();

    public List<CarController> Cars => cars;

    int Heuristic(int col)
    {
        return Mathf.Abs(col - 0);
    }

    private static readonly Vector2Int[] directions =
    {
        new Vector2Int(0, 1),   // up
        new Vector2Int(0, -1),  // down
        new Vector2Int(-1, 0),  // left
        new Vector2Int(1, 0),   // right
    };

    public void CheckCar(CarController car)
    {
        if (car.IsMoving) return;

        var path = FindPath(car);

        if (path == null)
        {
            Debug.Log("No Path");
            return;
        }

        MoveCar(car, path);
    }
    List<Vector2Int> FindPath(CarController car)
    {
        var start = car.Cell;
        var open = new List<PathNode>();
        var closed = new HashSet<(int, int)>();

        var startNode = new PathNode(start.Row, start.Col);
        startNode.G = 0;
        startNode.H = Heuristic(start.Col);

        open.Add(startNode);

        while (open.Count > 0)
        {
            open.Sort((a, b) => a.F.CompareTo(b.F));
            var current = open[0];
            open.RemoveAt(0);

            // đạt mục tiêu (col = 0)
            if (current.Col == 0)
            {
                return BuildPath(current);
            }

            closed.Add((current.Row, current.Col));

            foreach (var dir in directions)
            {
                int nr = current.Row + dir.x;
                int nc = current.Col + dir.y;

                if (!IsInside(nr, nc)) continue;
                if (closed.Contains((nr, nc))) continue;

                var cell = MapCreate.Grid[nr, nc];

                if (cell != null &&
                    (cell.CellType == CellType.Wall ||
                     cell.CellType == CellType.Car))
                    continue;

                int newG = current.G + 1;

                var node = open.Find(n => n.Row == nr && n.Col == nc);
                if (node == null)
                {
                    node = new PathNode(nr, nc);
                    node.G = newG;
                    node.H = Heuristic(nc);
                    node.Parent = current;

                    open.Add(node);
                }
                else if (newG < node.G)
                {
                    node.G = newG;
                    node.Parent = current;
                }
            }
        }

        return null;
    }
    List<Vector2Int> BuildPath(PathNode node)
    {
        List<Vector2Int> path = new();

        while (node != null)
        {
            path.Add(new Vector2Int(node.Row, node.Col));
            node = node.Parent;
        }

        path.Reverse();
        return path;
    }
    void MoveCar(CarController car, List<Vector2Int> path)
    {
        if (path == null || path.Count == 0) return;

        car.IsMoving = true;

        Sequence seq = DOTween.Sequence();

        for (int i = 1; i < path.Count; i++)
        {
            var step = path[i];

            Vector3 worldPos = MapCreate.Instance.GetCellWorldPosition(step.x, step.y);

            seq.Append(
                car.transform.DOMove(worldPos, 0.2f)
                    .SetSpeedBased(true)
                    .SetEase(Ease.Linear)
                    .OnStart(() =>
                    {
                        // clear cell cũ
                        MapCreate.Grid[car.Cell.Row, car.Cell.Col] = null;
                    })
                    .OnComplete(() =>
                    {
                        // update cell mới
                        car.Cell.Row = step.x;
                        car.Cell.Col = step.y;

                        MapCreate.Grid[step.x, step.y] = car.Cell;
                    })
            );
        }

        seq.OnComplete(() =>
        {
            HandleCarExit(car);
        });
    }
    void HandleCarExit(CarController car)
    {
        // nếu đang ở col = 0 thì cho exit
        if (car.Cell.Col == 0)
        {
            int r = car.Cell.Row;
            int c = car.Cell.Col;

            // clear grid
            MapCreate.Grid[r, c] = null;

            // remove khỏi manager
            cars.Remove(car);

            // ẩn xe (hoặc destroy)
            car.gameObject.SetActive(false);

            Debug.Log("Car exited!");
        }

        car.IsMoving = false;
    }
    public Vector3 GetWorldPos(int r, int c)
    {
        return MapCreate.Instance.GetCellWorldPosition(r, c);
    }
    bool IsInside(int r, int c)
    {
        var grid = MapCreate.Grid;
        return r >= 0 && c >= 0 &&
               r < grid.GetLength(0) &&
               c < grid.GetLength(1);
    }
}