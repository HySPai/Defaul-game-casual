using UnityEngine;

public class CarMoveCell : MonoBehaviour
{
    private CarController carController;

    public void Init(CarController controller)
    {
        carController = controller;
    }

    private void OnMouseDown()
    {
        if (carController == null) return;

        MapMoverManager.Instance.CheckCar(carController);
    }
}