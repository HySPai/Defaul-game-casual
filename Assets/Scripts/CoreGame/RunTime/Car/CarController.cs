using Dreamteck.Splines;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private CarMoveSpline carMove;
    [SerializeField] private CarCheck carCheck;
    [SerializeField] private CarVisual carVisual;
    [SerializeField] private SO_Color colorConfig;
    [SerializeField] private Cell cell;

    public CarMoveSpline CarMove => carMove;
    public CarCheck CarCheck => carCheck;
    public CarVisual CarVisual => carVisual;
    public bool IsMoving { get; set; }
    public Cell Cell => cell;

    public void Init(ColorName colorName)
    {
        if (carCheck != null)
        {
            carCheck.colorName = colorName;
        }

        if (colorConfig != null && carVisual != null)
        {
            Color color = colorConfig.GetColor(colorName);
            carVisual.SetColor(color);
        }
    }
}