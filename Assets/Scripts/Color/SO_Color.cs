using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Color", menuName = "Color/SO_color", order = 1)]
[Serializable]
public class SO_Color : ScriptableObject
{
    [SerializeField] private string[] colorNames;

    [Button("Generate Enum")]
    public void GenerateEnum()
    {
        EnumGenerator.GenerateEnum("ColorName", "Assets/Scripts/Color/", colorNames);

        if (colorIDs == null)
            colorIDs = new ColorID[colorNames.Length];

        if (colorIDs.Length != colorNames.Length)
        {
            Array.Resize(ref colorIDs, colorNames.Length);
        }

        for (int i = 0; i < colorNames.Length; i++)
        {
            if (colorIDs[i] == null)
                colorIDs[i] = new ColorID();

            colorIDs[i].colorName = (ColorName)i;
        }
    }

    [SerializeField, TableList, ShowIf("@colorNames != null && colorNames.Length > 0")]
    private ColorID[] colorIDs;

    public Color GetColor(ColorName colorName)
    {
        foreach (var item in colorIDs)
        {
            if (item.colorName == colorName)
            {
                return item.color;
            }
        }

        Debug.LogError("Không tìm thấy Color: " + colorName);
        return Color.white;
    }
}
[Serializable]
public class ColorID
{
    public ColorName colorName;
    public Color color;
}