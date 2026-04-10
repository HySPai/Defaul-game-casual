using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    private UIWin UIWin => GameViewsManager.Instance.GetView<UIWin>();
    private UILose UILose => GameViewsManager.Instance.GetView<UILose>();
    private UIMainGameplay UIMainGameplay => GameViewsManager.Instance.GetView<UIMainGameplay>();

    private void Start()
    {
        Register();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Win();
        }
        //GameViewsManager.Instance.GetView<UITutorial>().ShowHandClick(handTutPosTransform.position);
#endif
    }

    private void Register()
    {
        GameEvent.OnPlay += Play;
        GameEvent.OnWin += Win;
        GameEvent.OnLose += Lose;
        GameEvent.OnReplay += Replay;
    }

    public void Play()
    {
        UIMainGameplay.Show();
        MapCreate.Instance.GenerateMap();
    }

    [Button]
    public void Win()
    {
        PlayerprefSave.Level++;
        GameViewsManager.Instance.GetView<UIWin>().ShowByAlpha();
        //DOVirtual.DelayedCall(1.5f, () => UIWin.Show());
    }

    private void Lose()
    {
        DOVirtual.DelayedCall(1.5f, () => UILose.Show());
    }

    public void Replay()
    {
        UIMainGameplay.Show();
    }
    public void NextLevel()
    {
        PlayerprefSave.Level++;
        UIMainGameplay.Show();
    }

    public void BackLevel()
    {
        PlayerprefSave.Level = Mathf.Max(1, PlayerprefSave.Level - 1);
        UIMainGameplay.Show();
    }
}
