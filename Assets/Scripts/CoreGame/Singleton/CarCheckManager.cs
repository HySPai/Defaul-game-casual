using UnityEngine;

public class CarCheckManager : MonoBehaviour
{
    [SerializeField] private SandWorld world;
    [SerializeField] private MoverSplineManager moverManager;

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

    void HandleCar(CarController car)
    {
        var canvas = world.canvas;

        var carCheck = car.CarCheck;

        int removed = 0;

        // màu cần check
        Color color = world.colorDatabase.GetColor(carCheck.colorName);
        Color32 targetColor = (Color32)color;

        // vị trí xe → pixel
        Vector2Int pixelPos = world.WorldToPixel(car.transform.position);

        int centerX = pixelPos.x;
        int radius = carCheck.radiusCheck;

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
                    removed++;
                }
            }
        }
        if (removed > 0)
        {
            carCheck.AddRemoved(removed);

            int target = ColorProgressManager.Instance.GetTargetPerCar(carCheck.colorName);
            int current = carCheck.GetRemoved();

            int percent = target == 0 ? 100 : Mathf.Clamp((current * 100) / target, 0, 100);

            if (!HasAnyPixelOfColor(canvas, targetColor))
            {
                percent = 100;
            }

            carCheck.ProgressPercent = percent;

            if (percent >= 100)
            {
                MoverSplineManager.Instance.Unregister(car);
            }
        }
    }
    bool HasAnyPixelOfColor(PixelCanvas canvas, Color32 target)
    {
        for (int y = 0; y < canvas.Height; y++)
        {
            for (int x = 0; x < canvas.Width; x++)
            {
                if (canvas.GetPixel(x, y).Equals(target))
                    return true;
            }
        }
        return false;
    }
}