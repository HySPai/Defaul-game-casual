using System.Collections.Generic;
using UnityEngine;

public class ColorProgressManager : SingletonMonoBehaviour<ColorProgressManager>
{
    public Dictionary<ColorName, ColorProgressData> data = new();

    public void RegisterPixel(ColorName color)
    {
        if (!data.ContainsKey(color))
            data[color] = new ColorProgressData();

        data[color].totalPixels++;
    }

    public void RegisterCar(ColorName color)
    {
        if (!data.ContainsKey(color))
            data[color] = new ColorProgressData();

        data[color].carCount++;
    }

    public int GetTargetPerCar(ColorName color)
    {
        if (!data.ContainsKey(color)) return 0;

        var d = data[color];

        if (d.carCount == 0) return 0;

        return Mathf.CeilToInt((float)d.totalPixels / d.carCount);
    }
}

public class ColorProgressData
{
    public int totalPixels;
    public int carCount;
}