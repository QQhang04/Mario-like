using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // 血量变化后广播
    public UnityEvent onChange;
    // 受伤之后的广播
    public UnityEvent onDamage;
    
    public int initial = 3;
    public int max = 3;
    // 受伤之后无敌的冷却时间
    public float coolDown = 1f;
    protected float m_lastDamageTime;
    protected int m_currentHealth;

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
    
    public virtual bool recovering => Time.time < m_lastDamageTime + coolDown;
    public virtual bool isEmpty => m_currentHealth == 0;
    public virtual void Set(int amount) => current = amount;
    public virtual void Increase(int amount) => current += amount;
    public virtual void Damage(int amount)
    {
        if (!recovering)
        {
            current -= Mathf.Abs(amount);
            m_lastDamageTime = Time.time;
            onDamage?.Invoke();
        }
    }

    public void Reset() => current = initial;

    private void Awake() => current = initial;
}