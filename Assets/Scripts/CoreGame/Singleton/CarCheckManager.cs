using UnityEngine;

public class CarCheckManager : MonoBehaviour
{
    [SerializeField] private SandWorld world;
    [SerializeField] private MoverManager moverManager;

    private void Update()
    {
        HandleAllCars();
    }

    void HandleAllCars()
    {
        var cars = moverManager.cars;

        for (int i = 0; i < cars.Count; i++)
        {
            HandleCar(cars[i]);
        }
    }

    void HandleCar(CarMove car)
    {
        var canvas = world.canvas;

        // màu cần check
        Color color = world.colorDatabase.GetColor(car.CarCheck.colorName);
        Color32 targetColor = (Color32)color;

        // vị trí xe → pixel
        Vector2Int pixelPos = world.WorldToPixel(car.transform.position);

        int centerX = pixelPos.x;
        int radius = car.CarCheck.radiusCheck;

        int r2 = radius * radius;

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = 0; dy <= radius; dy++)
            {
                if (dx * dx + dy * dy > r2)
                    continue;

                int x = centerX + dx;
                int y = dy;

                if (x < 0 || x >= canvas.Width || y >= canvas.Height)
                    continue;

                Color32 current = canvas.GetPixel(x, y);

                if (current.Equals(targetColor))
                {
                    canvas.SetPixel(x, y, canvas.ClearColor);
                }
            }
        }
    }
}