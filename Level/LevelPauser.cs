using UnityEngine;
using UnityEngine.Events;

public class LevelPauser : Singleton<LevelPauser>
{
    // 记录当前暂停状态
    public bool paused { get; protected set; }
    public bool canPause { get; set; }
    
    public UIAnimator pauseScreen;
    
    public UnityEvent onPause;
    public UnityEvent onUnpause;

    public virtual void Pause(bool value)
    {
        if (value == paused) return;

        if (!paused)
        {
            Game.LockCursor(false);
            paused = true;
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
            pauseScreen?.Show();
            onPause?.Invoke();
        }
        else
        {
            Game.LockCursor(true);
            paused = false;
            Time.timeScale = 1;
            pauseScreen?.Hide();
            onUnpause?.Invoke();
        }
    }
}