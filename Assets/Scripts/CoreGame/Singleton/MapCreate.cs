using Sirenix.OdinInspector;
using UnityEngine;

public class MapCreate : SingletonMonoBehaviour<MapCreate>
{
    [SerializeField] private Transform mapPrent;
    [SerializeField] private EnumMatrixData matrixData;
    [SerializeField] private PrefabSO prefabSO;

    [Header("Grid Setting")]
    [SerializeField] private float cellHeight;
    [SerializeField] private float wallHeight;
    public static Cell[,] Grid;
    private Transform groundParent;
    private Transform wallParent;
    private Transform carParent;

    [Button]
    public void GenerateMap()
    {
        ClearMap();

        SetupParents();

        SandImage.Instance.ApplyImage(matrixData.Picture);

        var matrix = matrixData.Matrix;
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        Grid = new Cell[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector3 pos = GetWorldPos(r, c);
                var cell = matrix[r, c];

                if (prefabSO.ground != null)
                {
                    Vector3 groundPos = pos + Vector3.up * -0.2f;
                    Instantiate(prefabSO.ground, groundPos, Quaternion.identity, groundParent);
                }

                if (cell.type.HasFlag(CellType.Wall))
                {
                    var wall = Instantiate(prefabSO.wallPre, pos, Quaternion.identity, wallParent);
                    wall.CellType = CellType.Wall;
                    wall.Row = r;
                    wall.Col = c;
                    Grid[r, c] = wall;
                }
                if (cell.type.HasFlag(CellType.Car))
                {
                    Transform parent = GetCarColorParent(cell.carColor);

                    var carObj = Instantiate(prefabSO.carPre, pos, Quaternion.identity, parent);
                    var carController = carObj.GetComponent<CarController>();

                    if (carController != null)
                    {
                        carController.Init(cell.carColor);

                        var carCell = carController.Cell;

                        carCell.CellType = CellType.Car;

                        carCell.Row = r;
                        carCell.Col = c;

                        Grid[r, c] = carCell;

                        MapMoverManager.Instance.Cars.Add(carController);
                        ColorProgressManager.Instance.RegisterCar(cell.carColor);
                    }
                }
            }
        }
    }
    private void SetupParents()
    {
        groundParent = CreateChild("Ground");
        wallParent = CreateChild("Wall");
        carParent = CreateChild("Car");
    }

    private Transform CreateChild(string name)
    {
        var child = mapPrent.Find(name);

        if (child == null)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(mapPrent);
            go.transform.localPosition = Vector3.zero;
            return go.transform;
        }

        return child;
    }
    private Transform GetCarColorParent(ColorName color)
    {
        var colorParent = carParent.Find(color.ToString());

        if (colorParent == null)
        {
            GameObject go = new GameObject(color.ToString());
            go.transform.SetParent(carParent);
            go.transform.localPosition = Vector3.zero;
            return go.transform;
        }

        return colorParent;
    }
    public Vector3 GetCellWorldPosition(int r, int c)
    {
        return GetWorldPos(r, c);
    }
    private Vector3 GetWorldPos(int r, int c)
    {
        var matrix = matrixData.Matrix;
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        float offsetX = (rows - 1) * cellHeight * 0.5f;
        float offsetZ = (cols - 1) * cellHeight * 0.5f;

        return new Vector3(
            r * cellHeight - offsetX,
            0,
            -c * cellHeight + offsetZ
        ) + mapPrent.position;
    }
    private void ClearMap()
    {
        for (int i = mapPrent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(mapPrent.GetChild(i).gameObject);
        }
        MapMoverManager.Instance.Cars.Clear();
    }
}