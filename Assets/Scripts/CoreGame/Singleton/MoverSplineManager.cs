using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class MoverSplineManager : SingletonMonoBehaviour<MoverSplineManager>
{
    [SerializeField] private int maxCars = 5;

    [SerializeField] private SplineComputer splineComputer;

    public List<CarController> cars = new List<CarController>();
    public SplineComputer SplineComputer => splineComputer;
    private float spacing;
    private void Start()
    {
        float splineLength = splineComputer.CalculateLength();
        spacing = splineLength / maxCars;

        for (int i = 0; i < cars.Count; i++)
        {
            cars[i].CarMove.Initialize(this, splineComputer, 0.8f);
        }

        UpdateCarPositions();
    }
    public void Register(CarController car, SplineComputer splineComputer, float speed)
    {
        if (cars.Count >= maxCars)
        {
            Debug.Log("Max cars reached!");
            return;
        }

        car.CarMove.Initialize(this, splineComputer, speed);

        if (!cars.Contains(car))
        {
            cars.Add(car);
        }

        UpdateCarPositions();
    }
    public void Unregister(CarController car)
    {
        if (cars.Contains(car))
        {
            cars.Remove(car);
            UpdateCarPositions();
        }
    }
    void UpdateCarPositions()
    {
        float splineLength = splineComputer.CalculateLength();
        spacing = splineLength / maxCars;

        for (int i = 0; i < cars.Count; i++)
        {
            float targetDistance = i * spacing;

            cars[i].CarMove.SetDistance(targetDistance);
        }
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
}