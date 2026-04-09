using UnityEngine;

public class CarVisual : MonoBehaviour
{
    [SerializeField] private Renderer ren;

    public void SetColor(Color color)
    {
        ren.material.color = color;
    }
}
