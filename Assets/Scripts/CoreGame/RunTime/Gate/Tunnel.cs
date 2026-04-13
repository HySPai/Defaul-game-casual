using UnityEngine;

public enum ETunnelType
{
    Up,
    Down,
    Right,
    Left,
}

public class Tunnel : MonoBehaviour
{
    [SerializeField] private SO_Color colorConfig;
    [SerializeField] private ETunnelType gateType;
    [SerializeField] private TunnelVisual tunnelVisual;

    public void Init(SO_Color colorConfig, ColorName colorName, ETunnelType type)
    {
        this.colorConfig = colorConfig;
        this.gateType = type;

        if (colorConfig != null && tunnelVisual != null)
        {
            Color color = colorConfig.GetColor(colorName);
            tunnelVisual.SetColor(color);
        }
    }
}
