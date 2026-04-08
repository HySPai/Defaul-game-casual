using UnityEngine;

public class SandWorld : MonoBehaviour
{
    [Header("Canvas")]
    public PixelCanvas canvas;

    [Header("World Settings")]
    public int pixelWidth = 256;
    public int pixelHeight = 256;
    public float pixelSize = 0.02f;

    [Header("Simulation")]
    public float stepsPerSecond = 60f;

    [Header("Color")]
    public SO_Color colorDatabase;
    public ColorName currentColor = ColorName.Yellow;

    private float timer;

    private SandSimulation sand;
    private PixelDrawer drawer;

    public PixelDrawer Drawer => drawer;

    public Color32 CurrentColor32
    {
        get
        {
            Color c = colorDatabase.GetColor(currentColor);
            return (Color32)c;
        }
    }

    void Start()
    {
        canvas.Init(pixelWidth, pixelHeight);

        canvas.RenderTarget.transform.localScale = new Vector3(
            pixelWidth * pixelSize,
            pixelHeight * pixelSize,
            1f
        );

        sand = new SandSimulation(canvas);
        drawer = new PixelDrawer(canvas);
    }

    void Update()
    {
        RunSimulation();
        canvas.Apply();
    }

    void RunSimulation()
    {
        float stepTime = 1f / stepsPerSecond;
        timer += Time.deltaTime;

        while (timer >= stepTime)
        {
            sand.Step();
            timer -= stepTime;
        }
    }
}