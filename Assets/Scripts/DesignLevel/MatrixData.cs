using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
[System.Flags]
public enum CellType
{
    None = 0,
    Wall = 1 << 0,
    Car = 1 << 1,
    Ground = 1 << 2,
}
[CreateAssetMenu(fileName = "SO_EnumMatrixData", menuName = "ScriptableObjects/Enum Matrix Data")]
public class EnumMatrixData : ScriptableObject
{
    [SerializeField] private Texture2D picture;

    [SerializeField] private int rows = 5;
    [SerializeField] private int cols = 5;

    [SerializeField] private CellData[] data;

    [Header("Color Config")]
    [SerializeField] private SO_Color colorConfig; // 🔥 gán ở đây
    public SO_Color ColorConfig => colorConfig;
    public Texture2D Picture => picture;
    // =========================
    // MATRIX API
    // =========================
    private void OnEnable()
    {
#if UNITY_EDITOR
        if (colorConfig == null)
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:SO_Color");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                colorConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<SO_Color>(path);

                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
    public CellData[,] Matrix
    {
        get
        {
            var result = new CellData[rows, cols];
            EnsureData();

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    result[r, c] = data[r * cols + c];

            return result;
        }
        set
        {
            rows = value.GetLength(0);
            cols = value.GetLength(1);

            data = new CellData[rows * cols];

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    data[r * cols + c] = value[r, c];
        }
    }

    public CellData GetCell(int r, int c)
    {
        EnsureData();
        return data[r * cols + c];
    }

    private void EnsureData()
    {
        if (data == null || data.Length != rows * cols)
            data = new CellData[rows * cols];
    }

    // =========================
    // ODIN TABLE
    // =========================

    [ShowInInspector, DoNotDrawAsReference]
    [TableMatrix(
        HorizontalTitle = "Map",
        DrawElementMethod = nameof(DrawCell),
        ResizableColumns = false,
        RowHeight = 40)]
    private CellData[,] Table
    {
        get => Matrix;
        set => Matrix = value;
    }

    // =========================
    // DRAWER (INSTANCE METHOD)
    // =========================

    private CellData DrawCell(Rect rect, CellData value)
    {
#if UNITY_EDITOR
        Event e = Event.current;

        float half = rect.height * 0.5f;

        Rect typeRect = new Rect(rect.x, rect.y, rect.width, half);
        Rect colorRect = new Rect(rect.x, rect.y + half, rect.width, half);

        // ===== CLICK TO TOGGLE (ONLY OUTSIDE FIELDS) =====
        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            // nếu click vào vùng dropdown thì KHÔNG xử lý
            if (!typeRect.Contains(e.mousePosition) && !colorRect.Contains(e.mousePosition))
            {
                e.Use();

                if (value.type == CellType.None)
                    value.type = CellType.Wall;
                else if (value.type == CellType.Wall)
                    value.type = CellType.Car;
                else
                    value.type = CellType.None;
            }
        }

        // ===== TYPE =====
        value.type = (CellType)UnityEditor.EditorGUI.EnumFlagsField(typeRect, value.type);

        // ===== RESET =====
        if (!value.type.HasFlag(CellType.Car))
        {
            value.carColor = default;
        }

        // ===== COLOR PICK =====
        if (value.type.HasFlag(CellType.Car))
        {
            value.carColor = (ColorName)UnityEditor.EditorGUI.EnumPopup(colorRect, value.carColor);
        }

        // ===== BACKGROUND =====
        Color bg = Color.black;

        if (value.IsWall)
        {
            bg = Color.gray;
        }

        if (value.IsCar)
        {
            // 🔥 dùng SO_Color thật
            if (colorConfig != null)
                bg = colorConfig.GetColor(value.carColor);
            else
                bg = Color.green;
        }

        UnityEditor.EditorGUI.DrawRect(rect.Padding(1), bg);

        // ===== LABEL =====
        GUI.Label(rect, value.type.ToString(), new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            normal = new GUIStyleState { textColor = Color.white }
        });

#endif
        return value;
    }
}