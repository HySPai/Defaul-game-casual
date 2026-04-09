using UnityEngine;
using Dreamteck.Splines;

public class CarMove : MonoBehaviour
{
    [SerializeField] private SplinePositioner positioner;
    [SerializeField] private CarCheck carCheck;

    public float Speed { get; private set; }
    public float Distance { get; set; }
    public CarCheck CarCheck => carCheck;

    public void Initialize(MoverManager moverManager, float speed)
    {
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