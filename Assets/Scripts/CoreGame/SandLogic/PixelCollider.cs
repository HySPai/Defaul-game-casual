using System.Collections.Generic;
using UnityEngine;

public class PixelCollider
{
    private PixelCanvas canvas;
    private float pixelSize;

    public PixelCollider(PixelCanvas canvas, float pixelSize)
    {
        this.canvas = canvas;
        this.pixelSize = pixelSize;
    }

    bool IsSolid(int x, int y)
    {
        return !canvas.GetPixel(x, y).Equals(canvas.ClearColor);
    }

    bool IsEmpty(int x, int y)
    {
        return canvas.GetPixel(x, y).Equals(canvas.ClearColor);
    }

    public void Build(PolygonCollider2D collider)
    {
        collider.pathCount = 0;

        List<Vector2> points = new List<Vector2>();

        int w = canvas.Width;
        int h = canvas.Height;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (!IsSolid(x, y))
                    continue;

                // check if edge pixel
                if (IsEdge(x, y))
                {
                    AddPixelQuad(points, x, y);
                }
            }
        }

        if (points.Count > 0)
        {
            collider.pathCount = 1;
            collider.SetPath(0, points);
        }
    }

    bool IsEdge(int x, int y)
    {
        return IsEmpty(x - 1, y) ||
               IsEmpty(x + 1, y) ||
               IsEmpty(x, y - 1) ||
               IsEmpty(x, y + 1);
    }

    void AddPixelQuad(List<Vector2> points, int x, int y)
    {
        float px = x * pixelSize;
        float py = y * pixelSize;

        points.Add(new Vector2(px, py));
        points.Add(new Vector2(px + pixelSize, py));
        points.Add(new Vector2(px + pixelSize, py + pixelSize));
        points.Add(new Vector2(px, py + pixelSize));
    }
}