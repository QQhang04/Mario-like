using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class Game : Singleton<Game>
{
    /// <summary>
    /// Called when the amount of retries change.
    /// </summary>
    public UnityEvent<int> OnRetriesSet;

    public int initialRetries = 3;
    protected int m_retries;
    
    public int retries
    {
        get { return m_retries; }

        set
        {
            m_retries = value;
            OnRetriesSet?.Invoke(m_retries);
        }
    }
    
    public List<GameLevel> levels;

    protected override void Awake()
    {
        base.Awake();
        retries = initialRetries;
        DontDestroyOnLoad(gameObject);
    }
}