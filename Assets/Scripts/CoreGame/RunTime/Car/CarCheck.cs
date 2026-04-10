using UnityEngine;

public class CarCheck : MonoBehaviour
{
    public ColorName colorName;

    public int radiusCheck = 2;

    public int progressPercent; // 0 → 100

    [HideInInspector] public int totalPixel;
}