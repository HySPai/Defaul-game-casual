using UnityEngine;

public class CarMoveSpline : MonoBehaviour
{
    public SplinePoint Point { get; private set; }

    public void Initialize(MoverSplineManager moverManager)
    {
        // Hiện tại không cần gì thêm, giữ để mở rộng sau
    }

    public void AssignPoint(SplinePoint point)
    {
        Point = point;
    }

    private void Update()
    {
        if (Point == null) return;

        transform.position = Point.transform.position;
        transform.rotation = Point.transform.rotation;
    }
}