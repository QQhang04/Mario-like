using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game : Singleton<Game>
{
    public UnityEvent<int> OnRetriesSet;

    public int initialRetries = 3;
    
    protected int m_retries;
    protected int m_dataIndex;
    protected DateTime m_createdAt;
    protected DateTime m_updatedAt;
    
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

    public static void LockCursor(bool value = true)
    {
#if UNITY_STANDALONE
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
#endif
    }

    public virtual void LoadState(int index, GameData data)
    {
        m_dataIndex = index;
        m_retries = data.retries;
        m_createdAt = DateTime.Parse(data.createdAt);
        m_updatedAt = DateTime.Parse(data.updatedAt);

        for (int i = 0; i < data.levels.Length; i++)
        {
            levels[i].LoadState(data.levels[i]);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        retries = initialRetries;
        DontDestroyOnLoad(gameObject);
    }
}