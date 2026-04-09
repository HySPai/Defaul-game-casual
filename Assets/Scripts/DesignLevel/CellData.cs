using System;
using UnityEngine;

[Serializable]
public struct CellData
{
    public CellType type;
    public ColorName carColor;

    public bool IsWall => type.HasFlag(CellType.Wall);
    public bool IsCar => type.HasFlag(CellType.Car);

    public void Clear()
    {
        type = CellType.None;
        carColor = default;
    }
}