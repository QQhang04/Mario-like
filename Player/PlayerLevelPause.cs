using System;
using UnityEngine;

public class PlayerLevelPause : MonoBehaviour
{
    protected Player m_player => GetComponent<Player>();
    protected LevelPauser m_pauser => LevelPauser.Instance;

    protected void Start()
    {
        
    }
    
    protected virtual void Update()
    {
        if (m_player.inputs.GetPauseDown())
        {
            var value = m_pauser.paused;
            m_pauser.Pause(!value);
        }
    }
}