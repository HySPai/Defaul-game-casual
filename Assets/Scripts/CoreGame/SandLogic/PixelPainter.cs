using UnityEngine;

public class PixelPainter : MonoBehaviour
{
    public SandWorld world;

    void Update()
    {
        var canvas = world.canvas;
        var drawer = world.Drawer;

        if (Input.GetMouseButton(0))
        {
            Vector2Int p = canvas.ScreenToPixel(Input.mousePosition);
            drawer.DrawBrush(p.x, p.y, 3, world.CurrentColor32);
        }

        if (Input.GetMouseButton(1))
        {
            Vector2Int p = canvas.ScreenToPixel(Input.mousePosition);
            drawer.DrawBrush(p.x, p.y, 5, canvas.ClearColor);
        }
    }
}