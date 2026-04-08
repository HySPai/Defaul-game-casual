using Unity.Collections;
using UnityEngine;

public class PixelCanvas : MonoBehaviour
{
    public Transform Stabilizer;

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public Material Material;
    public MeshRenderer RenderTarget;

    public Color32 ClearColor = new Color32(0, 0, 0, 0);

    private NativeArray<Color32> colorBuffer;
    private Texture2D canvasTexture;
    private Camera cam;

    public void Init(int width, int height)
    {
        Width = width;
        Height = height;

        if (colorBuffer.IsCreated)
            colorBuffer.Dispose();

        colorBuffer = new NativeArray<Color32>(Width * Height, Allocator.Persistent);

        canvasTexture = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
        canvasTexture.filterMode = FilterMode.Point;
        canvasTexture.wrapMode = TextureWrapMode.Clamp;

        Clear();

        canvasTexture.SetPixelData(colorBuffer, 0);
        canvasTexture.Apply();

        Material.mainTexture = canvasTexture;
        RenderTarget.material = Material;

        cam = Camera.main;
    }

    public void Clear()
    {
        for (int i = 0; i < colorBuffer.Length; i++)
        {
            colorBuffer[i] = ClearColor;
        }
    }

    public void SetPixel(int x, int y, Color32 color)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return;

        int index = y * Width + x;
        colorBuffer[index] = color;
    }

    public Color32 GetPixel(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return ClearColor;

        int index = y * Width + x;
        return colorBuffer[index];
    }

    public void Apply()
    {
        canvasTexture.SetPixelData(colorBuffer, 0);
        canvasTexture.Apply();
    }

    public Vector2Int ScreenToPixel(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.forward, RenderTarget.transform.position);

        if (!plane.Raycast(ray, out float enter))
            return new Vector2Int(-1, -1);

        Vector3 hit = ray.GetPoint(enter);

        Vector3 local = RenderTarget.transform.InverseTransformPoint(hit);

        float u = local.x + 0.5f;
        float v = local.y + 0.5f;

        int x = (int)(u * Width);
        int y = (int)(v * Height);

        return new Vector2Int(x, y);
    }
    public NativeArray<Color32> GetRawBuffer()
    {
        return colorBuffer;
    }
    private void OnDestroy()
    {
        if (colorBuffer.IsCreated)
            colorBuffer.Dispose();
    }
}