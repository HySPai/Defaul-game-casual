using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class MoverSplineManager : SingletonMonoBehaviour<MoverSplineManager>
{
    [SerializeField] private int maxCars = 5;
    [SerializeField] private float speed;
    [SerializeField] private SplineComputer splineComputer;

    public List<CarController> cars = new List<CarController>();

    private int reservedSlots = 0;

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

    public void Register(CarController car)
    {
        ConsumeReservedSlot();

        if (cars.Contains(car)) return;

        if (cars.Count >= maxCars)
        {
            Debug.Log("Spline full!");
            return;
        }

        cars.Add(car);

        car.CarMove.Initialize(this, splineComputer, speed);

        PlaceCarAtEnd(car);
    }

    public void Unregister(CarController car)
    {
        if (!cars.Contains(car)) return;

        cars.Remove(car);

        car.gameObject.SetActive(false);

        Debug.Log("Car unregistered from spline." + car.name);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        for (int i = 0; i < cars.Count; i++)
        {
            MoveCar(cars[i], deltaTime);
        }
    }

    private void MoveCar(CarController car, float deltaTime)
    {
        float newDistance = car.CarMove.Distance + car.CarMove.Speed * deltaTime;

        float splineLength = splineComputer.CalculateLength();

        if (newDistance > splineLength)
        {
            newDistance %= splineLength;
        }

        car.CarMove.SetDistance(newDistance);
    }

    void PlaceCarAtEnd(CarController car)
    {
        float splineLength = splineComputer.CalculateLength();

        if (cars.Count == 1)
        {
            car.CarMove.SetDistance(0f);
            return;
        }

        var lastCar = cars[cars.Count - 2];

        float spacing = splineLength / maxCars;

        float targetDistance = lastCar.CarMove.Distance - spacing;

        if (targetDistance < 0)
            targetDistance += splineLength;

        car.CarMove.SetDistance(targetDistance);
    }
}