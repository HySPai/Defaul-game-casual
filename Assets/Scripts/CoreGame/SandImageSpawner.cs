using UnityEngine;

public class SandImageSpawner : MonoBehaviour
{
    [SerializeField] private Texture2D picture;
    [SerializeField] private RenderSimulation renderSimulation;

    private void Start()
    {
        renderSimulation.SpawnFromTexture(picture);
    }
}