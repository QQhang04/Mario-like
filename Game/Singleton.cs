using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T m_instance;

    public static T Instance
    {
        // 懒汉单例
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<T>();
            }
            
            return m_instance;
        }
    }

    protected virtual void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}