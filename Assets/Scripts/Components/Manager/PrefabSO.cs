using UnityEngine;
[CreateAssetMenu]

public class PrefabSO : ScriptableObject
{
    private static PrefabSO _instance;

    public static PrefabSO Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = Resources.Load<PrefabSO>("PrefabSO");
            }
            return _instance;
        }
    }

    public GameObject carPre;
    public GameObject wallPre;
    public GameObject ground;
}
