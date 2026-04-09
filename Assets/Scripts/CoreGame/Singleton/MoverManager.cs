using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class MoverManager : SingletonMonoBehaviour<MoverManager>
{
    [SerializeField] private SplineComputer splineComputer;

    public List<CarController> cars = new List<CarController>();

    private void Start()
    {
        for (int i = 0; i < cars.Count; i++)
        {
            cars[i].CarMove.Initialize(this, splineComputer, 0.8f);
        }
    }

    public void Register(CarController car, SplineComputer splineComputer, float speed)
    {
        car.CarMove.Initialize(this, splineComputer, speed);

        if (!cars.Contains(car))
        {
            cars.Add(car);
        }
    }

    public void Unregister(CarController car)
    {
        if (cars.Contains(car))
        {
            cars.Remove(car);
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