using UnityEngine;

public class CarMoveCell : MonoBehaviour
{
    private CarController carController;

    private void Awake()
    {
        carController = GetComponent<CarController>();
    }

    private void OnMouseDown()
    {
        if (carController == null) return;

        MapMoverManager.Instance.CheckCar(carController);
    }
}