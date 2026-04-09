using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private CellType cellType;

    public CellType CellType
    {
        get => cellType;
        set => cellType = value;
    }
}
