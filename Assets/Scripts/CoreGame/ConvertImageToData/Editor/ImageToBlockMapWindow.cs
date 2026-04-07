
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ImageToJsonBlockMapWindow : EditorWindow
{
    [Header("Source")]
    private Texture2D sourceImage;

    [Header("Grid")]
    private int gridWidth = 32;
    private int gridHeight = 32;

    [Header("Palette")]
    private List<Color> palette = new List<Color>()
    {
        new Color(1f, 0.5f, 0f),
        new Color(1f, 0.3f, 0.7f),
        new Color(0.7f, 0f, 1f),
    };

    // =============================
    // MENU
    // =============================
    [MenuItem("Tools/Image To JSON Block Map")]
    public static void Open()
    {
        GetWindow<ImageToJsonBlockMapWindow>("Image → JSON");
    }

    // =============================
    // GUI
    // =============================
    private void OnGUI()
    {
        GUILayout.Label("Source Image", EditorStyles.boldLabel);
        sourceImage = (Texture2D)EditorGUILayout.ObjectField(
            "Image",
            sourceImage,
            typeof(Texture2D),
            false
        );

        GUILayout.Space(10);

        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
        gridWidth = EditorGUILayout.IntField("Width", gridWidth);
        gridHeight = EditorGUILayout.IntField("Height", gridHeight);

        GUILayout.Space(10);

        GUILayout.Label("Palette", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Color"))
            palette.Add(Color.white);

        for (int i = 0; i < palette.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            palette[i] = EditorGUILayout.ColorField(palette[i]);
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                palette.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Generate JSON", GUILayout.Height(40)))
        {
            if (sourceImage == null)
            {
                Debug.LogError("Source image missing!");
                return;
            }

            GenerateJson();
        }
    }

    // =============================
    // CORE LOGIC
    // =============================
    private void GenerateJson()
    {
        Texture2D scaled = ScaleTexture(sourceImage, gridWidth, gridHeight);

        ImageBlockMapData data = new ImageBlockMapData
        {
            width = gridWidth,
            height = gridHeight,
            palette = ConvertPalette(),
            pixels = GeneratePixelData(scaled)
        };

        string json = JsonUtility.ToJson(data, true);

        string path = EditorUtility.SaveFilePanel(
            "Save Block Map JSON",
            Application.dataPath,
            sourceImage.name + "_BlockMap",
            "json"
        );

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
            Debug.Log("JSON saved at: " + path);
        }
    }

    // =============================
    // HELPERS
    // =============================
    private List<ColorData> ConvertPalette()
    {
        List<ColorData> list = new List<ColorData>();
        foreach (var c in palette)
        {
            list.Add(new ColorData
            {
                r = c.r,
                g = c.g,
                b = c.b
            });
        }
        return list;
    }

    private List<RowData> GeneratePixelData(Texture2D tex)
    {
        List<RowData> rows = new List<RowData>();

        for (int y = 0; y < gridHeight; y++)
        {
            RowData row = new RowData { row = new List<int>() };

            for (int x = 0; x < gridWidth; x++)
            {
                Color pixel = tex.GetPixel(x, y);
                int index = FindNearestColorIndex(pixel);
                row.row.Add(index);
            }

            rows.Add(row);
        }

        return rows;
    }

    private int FindNearestColorIndex(Color c)
    {
        float best = float.MaxValue;
        int bestIndex = 0;

        for (int i = 0; i < palette.Count; i++)
        {
            Color p = palette[i];
            float dist =
                (c.r - p.r) * (c.r - p.r) +
                (c.g - p.g) * (c.g - p.g) +
                (c.b - p.b) * (c.b - p.b);

            if (dist < best)
            {
                best = dist;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private Texture2D ScaleTexture(Texture2D source, int w, int h)
    {
        RenderTexture rt = RenderTexture.GetTemporary(w, h);
        Graphics.Blit(source, rt);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);

        return tex;
    }
}

[System.Serializable]
public class ImageBlockMapData
{
    public int width;
    public int height;
    public List<ColorData> palette;
    public List<RowData> pixels;
}

[System.Serializable]
public class ColorData
{
    public float r, g, b;
    public Color ToColor()
    {
        return new Color(r, g, b);
    }
}

[System.Serializable]
public class RowData
{
    public List<int> row;
}
