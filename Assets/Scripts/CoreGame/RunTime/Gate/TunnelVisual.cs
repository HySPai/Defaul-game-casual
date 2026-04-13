using UnityEngine;

public class TunnelVisual : MonoBehaviour
{
    [SerializeField] private Renderer ren;

    public void SetColor(Color color)
    {
        ren.material.color = color;
    }
}
