using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private RuleTile wallTile;

    [Header("Car")]
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private SO_Color colorData;

    [SerializeField] private EnumMatrixData dataMap;

    void Start()
    {
        InitMap();
    }

    void InitMap()
    {
        int rows = dataMap.Matrix.GetLength(0);
        int cols = dataMap.Matrix.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int flippedY = rows - 1 - row;
                Vector3Int pos = new Vector3Int(col, flippedY, 0);

                var cell = dataMap.GetCell(row, col);

                // WALL
                if (cell.IsWall)
                {
                    tilemap.SetTile(pos, wallTile);
                }

                // CAR
                if (cell.IsCar)
                {
                    SpawnCar(pos, cell.carColor);
                }
            }
        }
    }

    void SpawnCar(Vector3Int pos, ColorName colorName)
    {
        var car = Instantiate(carPrefab, grid.CellToWorld(pos), Quaternion.identity);

        var renderer = car.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = colorData.GetColor(colorName);
        }
    }
}