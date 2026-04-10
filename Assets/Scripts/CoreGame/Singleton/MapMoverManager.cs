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
    [SerializeField] private float rotateSpeed;

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
    float GetRotationY(Vector2Int dir)
    {
        if (dir == new Vector2Int(1, 0)) return 90f;
        if (dir == new Vector2Int(-1, 0)) return -90f;
        if (dir == new Vector2Int(0, 1)) return 180f;
        if (dir == new Vector2Int(0, -1)) return 0;

        return 0f;
    }
    public void CheckCar(CarController car)
    {
        if (car.IsMoving) return;

        // spline full → chặn luôn
        if (!MoverSplineManager.Instance.HasAvailableSlot())
        {
            car.CarMoveCell.SetInteract(false);
            Debug.Log("Spline Full!");
            return;
        }

        // reserve slot NGAY khi click
        MoverSplineManager.Instance.ReserveSlot();

        car.IsMoving = true;

        var path = FindPath(car);

        if (path == null)
        {
            car.IsMoving = false;
            MoverSplineManager.Instance.ConsumeReservedSlot();
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
        car.CarMoveCell.SetInteract(false);

        Sequence seq = DOTween.Sequence();

        for (int i = 1; i < path.Count; i++)
        {
            var step = path[i];
            var prev = path[i - 1];

            Vector3 worldPos = MapCreate.Instance.GetCellWorldPosition(step.x, step.y);

            // tính direction
            Vector2Int dir = new Vector2Int(step.x - prev.x, step.y - prev.y);

            float targetY = GetRotationY(dir);

            Tween moveTween = car.transform
                .DOMove(worldPos, moveSpeed)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnStart(() =>
                {
                    MapCreate.Grid[car.Cell.Row, car.Cell.Col] = null;
                })
                .OnComplete(() =>
                {
                    car.Cell.Row = step.x;
                    car.Cell.Col = step.y;

                    MapCreate.Grid[step.x, step.y] = car.Cell;
                });

            Tween rotateTween = car.transform
                .DORotate(new Vector3(0, targetY, 0), rotateSpeed);

            // chạy cùng lúc
            seq.Append(
                DOTween.Sequence()
                    .Join(moveTween)
                    .Join(rotateTween)
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

            cars.Remove(car);

            MoverSplineManager.Instance.Register(car, MoverSplineManager.Instance.SplineComputer, 0.8f);

            Debug.Log("Ngon luôn!");
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