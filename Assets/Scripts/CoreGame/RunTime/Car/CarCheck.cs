using UnityEngine;

public class CarCheck : MonoBehaviour
{
    public ColorName colorName;

    public int radiusCheck;

    [SerializeField] private int progressPercent;

    public int ProgressPercent
    {
        get => progressPercent;
        set => progressPercent = Mathf.Clamp(value, 0, 100);
    }
}