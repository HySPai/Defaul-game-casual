using UnityEngine;

public class CarMoveCell : MonoBehaviour
{
    private CarController carController;
    [SerializeField] private bool canInteract;
    public void Init(CarController controller)
    {
        carController = controller;
    }

    private void OnMouseDown()
    {
        if (carController == null || !canInteract) return;

        MapMoverManager.Instance.CheckCar(carController);
    }
    public void SetInteract(bool value)
    {
        canInteract = value;
    }
}