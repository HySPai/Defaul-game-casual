using UnityEngine;

public class SandSimulation
{
    private PixelCanvas canvas;
    private Color32 emptyColor;

    public SandSimulation(PixelCanvas canvas)
    {
        this.canvas = canvas;
        emptyColor = canvas.ClearColor;
    }

    bool IsEmpty(int x, int y)
    {
        if (x < 0 || x >= canvas.Width || y < 0 || y >= canvas.Height)
            return false;

        return canvas.GetPixel(x, y).Equals(emptyColor);
    }

    bool IsSand(Color32 c)
    {
        // tất cả pixel khác empty đều là "cát"
        return !c.Equals(emptyColor);
    }

    public void Step()
    {
        int width = canvas.Width;
        int height = canvas.Height;

        for (int y = 1; y < height; y++)
        {
            bool leftToRight = Random.value < 0.5f;

            if (leftToRight)
            {
                for (int x = 0; x < width; x++)
                {
                    UpdatePixel(x, y);
                }
            }
            else
            {
                for (int x = width - 1; x >= 0; x--)
                {
                    UpdatePixel(x, y);
                }
            }
        }
    }

    void UpdatePixel(int x, int y)
    {
        Color32 current = canvas.GetPixel(x, y);

        if (!IsSand(current))
            return;

        int belowY = y - 1;

        // 1. ↓ rơi thẳng
        if (IsEmpty(x, belowY))
        {
            Move(x, y, x, belowY, current);
            return;
        }

        // 2. chéo có "nền"
        bool canLeft = IsEmpty(x - 1, belowY) && !IsEmpty(x - 1, belowY - 1);
        bool canRight = IsEmpty(x + 1, belowY) && !IsEmpty(x + 1, belowY - 1);

        // random nếu cả 2
        if (canLeft && canRight)
        {
            if (Random.value < 0.5f)
                Move(x, y, x - 1, belowY, current);
            else
                Move(x, y, x + 1, belowY, current);

            return;
        }

        if (canLeft)
        {
            Move(x, y, x - 1, belowY, current);
            return;
        }

        if (canRight)
        {
            Move(x, y, x + 1, belowY, current);
            return;
        }
    }

    void Move(int x1, int y1, int x2, int y2, Color32 color)
    {
        canvas.SetPixel(x1, y1, emptyColor);
        canvas.SetPixel(x2, y2, color);
    }
}