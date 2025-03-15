using System;
using UnityEngine.Events;

public class LevelScore : Singleton<LevelScore>
{
    // 当ui数据数量改变时发出广播
    public UnityEvent<int> OnCoinsSet;
    public UnityEvent<bool[]> OnStarsSet;
    public UnityEvent<int> OnRetriesSet;
    // 确认场景数据加载完之后再做后处理
    public UnityEvent OnScoreLoaded;
    
    public bool stopTime { get; set; } = true;
    protected int m_coins;
    protected bool[] m_stars = new bool[GameLevel.starsLevel];
    protected Game m_game;
    protected GameLevel m_level;

    public int coins
    {
        get { return m_coins; }
        set
        {
            m_coins = value;
            OnCoinsSet?.Invoke(m_coins);
        }
    }
    
    public bool[] stars => (bool[])m_stars.Clone(); 
    
    public float time { get; protected set; }

    public virtual void Consolidate()
    {
        if (m_level != null)
        {
            if (m_level.time == 0 || time < m_level.time)
            {
                m_level.time = time;
            }

            if (coins > m_level.coins)
            {
                m_level.coins = coins;
            }

            m_level.stars = (bool[])stars.Clone();
            m_game.RequestSaving();
        }
    }

    protected void Start()
    {
        m_game = Game.Instance;
        m_level = m_game?.GetCurrentLevel();

        if (m_level != null)
        {
            m_stars = (bool[])m_level.stars.Clone();
        }
        
        OnScoreLoaded?.Invoke();
    }
}