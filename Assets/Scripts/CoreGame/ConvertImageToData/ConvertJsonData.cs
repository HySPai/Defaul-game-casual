using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region deserialize JSON

[System.Serializable]
public class PixelRow
{
    public int[] row;
}

[System.Serializable]
public class PaletteColor
{
    public float r;
    public float g;
    public float b;

    public Color ToColor()
    {
        return new Color(r, g, b, 1f);
    }
}

[System.Serializable]
public class PixelData
{
    public int width;
    public int height;
    public List<PaletteColor> palette;
    public List<PixelRow> pixels;
}

#endregion

public class ConvertJsonData : MonoBehaviour
{
    [SerializeField]
    private TextAsset jsonFile;
    [ShowInInspector]
    private PixelData pixelData;
    private void Start()
    {
        LoadData();
    }
    [Button]
    public void LoadData()
    {
        if (jsonFile == null)
        {
            Debug.LogError("Json file is null");
            return;
        }
        pixelData = JsonUtility.FromJson<PixelData>(jsonFile.text);
        if (pixelData == null)
        {
            Debug.LogError("Failed to parse PixelData");
            return;
        }
        GeneratePictureManager.Instance.GeneratePicture(pixelData);
    }
}
