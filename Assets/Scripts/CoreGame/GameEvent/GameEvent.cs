using System;

public static class GameEvent
{
    public static Action OnPlay { get; set; }
    public static Action OnWin { get; set; }
    public static Action OnLose { get; set; }
    public static Action OnReplay { get; set; }
}
