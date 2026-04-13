using UnityEngine;

public class CarCheck : MonoBehaviour
{
    public ColorName colorName;

    public int radiusCheck;

    [SerializeField] private int progressPercent;
    private int removedPixels;
    public int ProgressPercent
    {
        get => progressPercent;
        set => progressPercent = Mathf.Clamp(value, 0, 100);
    }
    public void AddRemoved(int amount)
    {
        removedPixels += amount;
    }

    public int GetRemoved() => removedPixels;
}