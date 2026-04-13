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

    // =========================
    // CREATE POINTS
    // =========================
    void CreatePoints()
    {
        float spacing = splineLength / maxCars;

        for (int i = 0; i < maxCars; i++)
        {
            GameObject go = Instantiate(pointPrefab, transform);

            var point = go.GetComponent<SplinePoint>();

            // 🔥 QUAN TRỌNG: set trước
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

    // =========================
    // SLOT SYSTEM
    // =========================
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

    // =========================
    // REGISTER / UNREGISTER
    // =========================
    public void Register(CarController car)
    {
        ConsumeReservedSlot();

        if (cars.Contains(car)) return;
        if (cars.Count >= maxCars)
        {
            Debug.Log("Spline full!");
            return;
        }

        var point = GetAvailablePoint();
        if (point == null)
        {
            Debug.Log("No available point!");
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

        Debug.Log("Car unregistered: " + car.name);
    }

    // =========================
    // POINT MANAGEMENT
    // =========================
    SplinePoint GetAvailablePoint()
    {
        for (int i = 0; i < points.Count; i++)
        {
            if (!occupiedPoints.Contains(points[i]))
                return points[i];
        }

        return null;
    }
}