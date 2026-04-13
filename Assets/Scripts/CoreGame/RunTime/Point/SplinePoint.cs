using Dreamteck.Splines;
using UnityEngine;

public class SplinePoint : MonoBehaviour
{
    public SplinePositioner positioner;

    private float distance;

    public float Distance => distance;

    public void SetDistance(float value)
    {
        distance = value;
        positioner.position = value;
    }
}