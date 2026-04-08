using UnityEngine;

public class PixelDrawer
{
    private PixelCanvas canvas;

    public PixelDrawer(PixelCanvas canvas)
    {
        this.canvas = canvas;
    }

    public void DrawPixel(int x, int y, Color32 color)
    {
        canvas.SetPixel(x, y, color);
    }

    public void DrawBrush(int centerX, int centerY, int radius, Color32 color)
    {
        int r2 = radius * radius;

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y > r2)
                    continue;

                canvas.SetPixel(centerX + x, centerY + y, color);
            }
        }
    }

    public void DrawLine(int x0, int y0, int x1, int y1, Color32 color)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            canvas.SetPixel(x0, y0, color);

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = err * 2;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}