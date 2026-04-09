using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private CellType cellType;

    public int Row;
    public int Col;

    public CellType CellType
    {
        get => cellType;
        set => cellType = value;
    }
}