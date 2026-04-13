using UnityEngine;

public class CarMoveSpline : MonoBehaviour
{
    public SplinePoint Point { get; private set; }

    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float rotateSpeed = 10f;

    private float lastDistance;
    private bool initialized;
    private bool isSnapped;

    public void Initialize(MoverSplineManager moverManager)
    {
        initialized = false;
    }
    public void AssignPoint(SplinePoint point)
    {
        Point = point;
        isSnapped = false;
    }

    private void LateUpdate()
    {
        if (Point == null) return;

        float currentDistance = Point.Distance;

        if (initialized && currentDistance < lastDistance || isSnapped)
        {
            transform.position = Point.transform.position;
            transform.rotation = Point.transform.rotation;
            isSnapped = true;
        }
        else
        {
            transform.position = Vector3.Lerp(
                transform.position,
                Point.transform.position,
                Time.deltaTime * followSpeed
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Point.transform.rotation,
                Time.deltaTime * rotateSpeed
            );
        }

        lastDistance = currentDistance;
        initialized = true;
    }
}