using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class MoverManager : MonoBehaviour
{
    [SerializeField] private SplineComputer splineComputer;

    public List<CarMove> cars = new List<CarMove>();

    private void Start()
    {
        foreach (CarMove car in cars)
        {
            Register(car, 0.5f);
        }
    }

    public void Register(CarMove car, float speed)
    {
        car.Initialize(this, speed);
        cars.Add(car);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        for (int i = 0; i < cars.Count; i++)
        {
            MoveCar(cars[i], deltaTime);
        }
    }

    private void MoveCar(CarMove car, float deltaTime)
    {
        float newDistance = car.Distance + car.Speed * deltaTime;

        // Loop spline
        float splineLength = splineComputer.CalculateLength();
        if (newDistance > splineLength)
        {
            newDistance %= splineLength;
        }

        car.SetDistance(newDistance);
    }
}