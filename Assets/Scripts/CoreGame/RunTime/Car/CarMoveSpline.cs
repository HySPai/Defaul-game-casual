using UnityEngine;
using Dreamteck.Splines;

public class CarMoveSpline : MonoBehaviour
{
    [SerializeField] private SplinePositioner positioner;

    public float Speed { get; private set; }
    public float Distance { get; set; }

    public void Initialize(MoverManager moverManager, SplineComputer splineComputer, float speed)
    {
        positioner.spline = splineComputer;
        positioner.mode = SplinePositioner.Mode.Distance;
        Speed = speed;
        Distance = 0f;
    }

    public void SetDistance(float distance)
    {
        Distance = distance;
        positioner.SetDistance(distance);
    }
}