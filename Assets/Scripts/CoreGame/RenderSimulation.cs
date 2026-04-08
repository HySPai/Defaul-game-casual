using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum SandColorType
{
    RandomSpread,
    Gradient
}

public class RenderSimulation : MonoBehaviour
{
    [Header("Boilerplate")]
    [SerializeField] RawImage screenUI;
    [SerializeField] ComputeShader computeShader;

    RenderTexture screenTexture;
    RenderTexture boundTextureInput;
    RenderTexture boundTextureOutput;
    ComputeBuffer claimsBuffer;
    ComputeBuffer sandColorsBuffer;

    uint[] emptyClaims;

    [Header("Canvas")]
    [SerializeField] Color backgroundColor;
    [SerializeField] int pixelSize = 4;
    [SerializeField] float simulationSpeed = 60f; // step / second


    [Header("Simulation")]
    [SerializeField] SandColorType sandColorType;
    [SerializeField] Gradient sandColorsGradient;
    [SerializeField] float brushRadius = 10;

    bool doSimulation = true;
    int simulationStep = 0;
    float simulationAccumulator = 0f;
    int width;
    int height;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) doSimulation = !doSimulation;

        brushRadius -= Input.mouseScrollDelta.y * 2;
        brushRadius = Mathf.Clamp(brushRadius, 1, 300);

        simulationAccumulator += Time.deltaTime * simulationSpeed;

        while (simulationAccumulator >= 1f)
        {
            StepSimulation();
            simulationAccumulator -= 1f;
        }
    }
    void Init()
    {
        Rect rect = screenUI.rectTransform.rect;

        width = Mathf.RoundToInt(rect.width / pixelSize);
        height = Mathf.RoundToInt(rect.height / pixelSize);

        computeShader.SetInt("Width", width);
        computeShader.SetInt("Height", height);
        computeShader.SetVector("BGColor", backgroundColor);

        claimsBuffer = new ComputeBuffer(width * height, sizeof(uint));
        computeShader.SetBuffer(0, "Claims", claimsBuffer);
        emptyClaims = new uint[width * height];

        sandColorsBuffer = new ComputeBuffer(sandColorsGradient.colorKeys.Length, sizeof(float) * 4);
        computeShader.SetBuffer(0, "SandColors", sandColorsBuffer);
        computeShader.SetInt("NumSandColors", sandColorsGradient.colorKeys.Length);

        screenTexture = CreateRT();
        boundTextureInput = CreateRT();
        boundTextureOutput = CreateRT();

        computeShader.SetTexture(0, "Input", boundTextureInput);
        computeShader.SetTexture(0, "Result", boundTextureOutput);
        computeShader.SetTexture(1, "Result", boundTextureOutput);

        computeShader.Dispatch(1,
            Mathf.CeilToInt(width / 8f),
            Mathf.CeilToInt(height / 8f),
            1);

        screenUI.texture = screenTexture;
    }

    RenderTexture CreateRT()
    {
        var rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBHalf);
        rt.enableRandomWrite = true;
        rt.filterMode = FilterMode.Point;
        rt.Create();
        return rt;
    }

    void StepSimulation()
    {
        if (boundTextureInput == null)
        {
            Init();
        }

        Rect rect = screenUI.rectTransform.rect;

        Vector2 localMouse;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            screenUI.rectTransform,
            Input.mousePosition,
            null,
            out localMouse
        );

        float mouseX = (localMouse.x + rect.width * 0.5f) / pixelSize;
        float mouseY = (localMouse.y + rect.height * 0.5f) / pixelSize;

        Graphics.Blit(boundTextureOutput, screenTexture);
        Graphics.Blit(boundTextureOutput, boundTextureInput);

        claimsBuffer.SetData(emptyClaims);

        Color[] sandColorsArr = new Color[sandColorsGradient.colorKeys.Length];
        for (int i = 0; i < sandColorsArr.Length; i++)
        {
            Color c = sandColorsGradient.colorKeys[i].color;

            c = c.linear;

            sandColorsArr[i] = c;
        }
        sandColorsBuffer.SetData(sandColorsArr);

        computeShader.SetInt("SandColorType", (int)sandColorType);
        computeShader.SetFloat("BrushRad", brushRadius);
        computeShader.SetVector("MousePos", new Vector2(mouseX, mouseY));
        computeShader.SetBool("MouseDownLeft", Input.GetMouseButton(0));
        computeShader.SetBool("MouseDownRight", Input.GetMouseButton(1));
        computeShader.SetBool("DoSimulation", doSimulation);

        simulationStep++;
        computeShader.SetInt("SimStep", simulationStep);

        computeShader.Dispatch(0,
            Mathf.CeilToInt(width / 8f),
            Mathf.CeilToInt(height / 8f),
            1);
    }
    public void SpawnFromTexture(Texture2D texture)
    {
        if (boundTextureOutput == null)
        {
            Init();
        }

        // Đảm bảo texture readable
        Texture2D readableTex = texture;

        // Tạo texture đúng size grid
        Texture2D scaled = new Texture2D(width, height, TextureFormat.RGBA32, false);
        scaled.filterMode = FilterMode.Point;

        // Resize thủ công (TRÁNH BLUR)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float u = (x + 0.5f) / width;
                float v = (y + 0.5f) / height;

                Color c = readableTex.GetPixelBilinear(u, v);
                c = c.linear;
                scaled.SetPixel(x, y, c);
            }
        }

        scaled.Apply();

        Color[] pixels = scaled.GetPixels();
        Vector4[] data = new Vector4[pixels.Length];

        for (int i = 0; i < pixels.Length; i++)
        {
            Color c = pixels[i];

            if (c.a > 0.01f)
            {
                data[i] = new Vector4(c.r, c.g, c.b, c.a);
            }
            else
            {
                data[i] = backgroundColor;
            }
        }

        ComputeBuffer initBuffer = new ComputeBuffer(data.Length, sizeof(float) * 4);
        initBuffer.SetData(data);

        int kernel = computeShader.FindKernel("InitializeFromTexture");

        computeShader.SetBuffer(kernel, "InitData", initBuffer);
        computeShader.SetTexture(kernel, "Result", boundTextureOutput);

        computeShader.Dispatch(kernel,
            Mathf.CeilToInt(width / 8f),
            Mathf.CeilToInt(height / 8f),
            1);

        initBuffer.Release();
    }
    void OnDisable()
    {
        claimsBuffer?.Dispose();
        sandColorsBuffer?.Dispose();

        screenTexture?.Release();
        boundTextureInput?.Release();
        boundTextureOutput?.Release();
    }
}