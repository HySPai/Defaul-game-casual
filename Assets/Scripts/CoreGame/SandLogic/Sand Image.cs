using UnityEngine;

public class SandImage : SingletonMonoBehaviour<SandImage>
{
    public void ApplyImage(Texture2D picture)
    {
        if (picture == null || SandWorld.Instance == null) return;

        SandWorld world = SandWorld.Instance;

        var canvas = world.canvas;

        int canvasW = canvas.Width;
        int canvasH = canvas.Height;

        if (canvasW == 0 || canvasH == 0)
        {
            Debug.LogError("Canvas chưa được Init!");
            return;
        }

        int imgW = picture.width;
        int imgH = picture.height;

        Color32[] palette = ConvertPalette(world.colorDatabase.GetAllColors());

        for (int y = 0; y < canvasH; y++)
        {
            for (int x = 0; x < canvasW; x++)
            {
                float u = (float)x / canvasW;
                float v = (float)y / canvasH;

                int imgX = Mathf.Clamp(Mathf.FloorToInt(u * imgW), 0, imgW - 1);
                int imgY = Mathf.Clamp(Mathf.FloorToInt(v * imgH), 0, imgH - 1);

                Color32 imgColor = picture.GetPixel(imgX, imgY);

                imgColor.a = 255;

                Color32 closest = GetClosestColor(imgColor, palette);

                canvas.SetPixel(x, y, closest);
            }
        }

        canvas.Apply();
    }

    Color32[] ConvertPalette(Color[] colors)
    {
        Color32[] result = new Color32[colors.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            result[i] = (Color32)colors[i];
        }

        return result;
    }

    Color32 GetClosestColor(Color32 target, Color32[] palette)
    {
        int minDist = int.MaxValue;
        Color32 best = palette[0];

        foreach (var c in palette)
        {
            int dist = ColorDistance(target, c);

            if (dist < minDist)
            {
                minDist = dist;
                best = c;
            }
        }

        return best;
    }

    int ColorDistance(Color32 a, Color32 b)
    {
        int dr = a.r - b.r;
        int dg = a.g - b.g;
        int db = a.b - b.b;

        return dr * dr + dg * dg + db * db;
    }
}