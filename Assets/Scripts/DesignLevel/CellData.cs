using System;
using UnityEngine;

[Serializable]
public struct CellData
{
    public CellType type;
    public ColorName carColor;

    public ColorName tunnelColor;
    public ETunnelType tunnelType;

    public bool IsWall => type.HasFlag(CellType.Wall);
    public bool IsCar => type.HasFlag(CellType.Car);
    public bool IsTunnel => type.HasFlag(CellType.Tunnel);

    public void Clear()
    {
        type = CellType.None;
        carColor = default;
    }
}