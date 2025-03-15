using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class LevelFinisher : Singleton<LevelFinisher>
{
    public UnityEvent OnExit;
    public UnityEvent OnFinish;
    
    public float loadingDelay = 1f;
    public bool unlockNextLevel;
    public string nextScene;
    
    protected Game m_game => Game.Instance;
    protected LevelPauser m_pauser => LevelPauser.Instance;
    protected Level m_level => Level.Instance;
    protected GameLoader m_loader => GameLoader.Instance;
    protected LevelScore m_score => LevelScore.Instance;
    
    public string exitScene;
    
    protected virtual IEnumerator ExitRoutine()
    {
        m_pauser.Pause(false);
        m_pauser.canPause = false;
        m_level.player.inputs.enabled = false;
        yield return new WaitForSeconds(loadingDelay);
        Game.LockCursor(false);
        m_loader.Load(exitScene);
        OnExit?.Invoke();
    }
    
    protected virtual IEnumerator FinishRoutine()
    {
        m_pauser.Pause(false);
        m_pauser.canPause = false;
        m_score.stopTime = true;
        m_level.player.inputs.enabled = false;

        yield return new WaitForSeconds(loadingDelay);

        if (unlockNextLevel)
        {
            m_game.UnlockNextLevel();
        }

        Game.LockCursor(false);
        m_score.Consolidate();
        m_loader.Load(nextScene);
        OnFinish?.Invoke();
    }
    
    public virtual void Exit()
    {
        StopAllCoroutines();
        StartCoroutine(ExitRoutine());
    }

    public virtual void Finish()
    {
        StopAllCoroutines();
        StartCoroutine(FinishRoutine());
    }
}