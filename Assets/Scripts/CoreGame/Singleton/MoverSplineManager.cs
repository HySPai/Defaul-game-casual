using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class MoverSplineManager : SingletonMonoBehaviour<MoverSplineManager>
{
    [SerializeField] private int maxCars = 5;
    [SerializeField] private float speed;
    [SerializeField] private SplineComputer splineComputer;
    [SerializeField] private GameObject pointPrefab;

    public List<CarController> cars = new List<CarController>();
    public float SplineLength => splineLength;
    private List<SplinePoint> points = new List<SplinePoint>();
    private HashSet<SplinePoint> occupiedPoints = new HashSet<SplinePoint>();

    private int reservedSlots = 0;
    private float splineLength;

    private void Start()
    {
        splineLength = splineComputer.CalculateLength();
        CreatePoints();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        for (int i = 0; i < points.Count; i++)
        {
            MovePoint(points[i], deltaTime);
        }
    }

    void CreatePoints()
    {
        float spacing = splineLength / maxCars;

        for (int i = 0; i < maxCars; i++)
        {
            GameObject go = Instantiate(pointPrefab, transform);

            var point = go.GetComponent<SplinePoint>();

            point.positioner.spline = splineComputer;
            point.positioner.mode = SplinePositioner.Mode.Distance;

            float distance = spacing * i;

            point.SetDistance(distance);

            points.Add(point);
        }
    }
    void MovePoint(SplinePoint point, float deltaTime)
    {
        float newDistance = point.Distance + speed * deltaTime;

        if (newDistance > splineLength)
            newDistance %= splineLength;

        point.SetDistance(newDistance);
    }

    public bool HasAvailableSlot()
    {
        return (cars.Count + reservedSlots) < maxCars;
    }

    public void ReserveSlot()
    {
        reservedSlots++;
    }

    public void ConsumeReservedSlot()
    {
        reservedSlots = Mathf.Max(0, reservedSlots - 1);
    }

    public void Register(CarController car, SplinePoint point)
    {
        ConsumeReservedSlot();

        if (cars.Contains(car)) return;
        if (cars.Count >= maxCars)
        {
            Debug.Log("Spline full!");
            return;
        }

        cars.Add(car);
        occupiedPoints.Add(point);

        car.CarMove.Initialize(this);
        car.CarMove.AssignPoint(point);
    }
    public void Unregister(CarController car)
    {
        if (!cars.Contains(car)) return;

        var point = car.CarMove.Point;

        if (point != null)
            occupiedPoints.Remove(point);

        cars.Remove(car);

        car.gameObject.SetActive(false);
    }

    public SplinePoint PeekAvailablePoint()
    {
        SplinePoint bestPoint = null;
        float minDistance = float.MaxValue;

        foreach (var point in points)
        {
            if (occupiedPoints.Contains(point)) continue;

            if (point.Distance < minDistance)
            {
                minDistance = point.Distance;
                bestPoint = point;
            }
        }

        return bestPoint;
    }
}