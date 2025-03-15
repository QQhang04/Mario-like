using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int initial = 3;
    // 血量变化后广播
    public UnityEvent onChange;
    
    public int max = 3;
    public int m_currentHealth;

    public int current
    {
        get { return m_currentHealth; }
        protected set
        {
            var last = m_currentHealth;
            if (value != last)
            {
                m_currentHealth = Mathf.Clamp(value, 0, max);
                onChange?.Invoke();
            }
        }
    }

    public void Reset()
    {
        current = initial; 
    }

    private void Awake()
    {
        current = initial;
    }
}