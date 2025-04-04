using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelStarter : Singleton<LevelStarter>
{
    public UnityEvent OnStart;
    protected Level m_level => Level.Instance;
    protected LevelScore m_score => LevelScore.Instance;
    protected LevelPauser m_pauser => LevelPauser.Instance;
    
    public float enablePlayerDelay = 1f;

    protected IEnumerator Routine()
    {
        Game.LockCursor();
        m_level.player.controller.enabled = false;
        m_level.player.inputs.enabled = false;
        yield return new WaitForSeconds(enablePlayerDelay);
        m_score.stopTime = false;
        m_level.player.controller.enabled = true;
        m_level.player.inputs.enabled = true;
        m_pauser.canPause = true;
        OnStart?.Invoke();
    }
    protected void Start()
    {
        StartCoroutine(Routine());
    }
}