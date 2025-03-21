using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityStateManagerListener : MonoBehaviour
{
    public UnityEvent onEnter;
    public UnityEvent onExit;

    public List<String> states;
    
    protected EntityStateManager m_manager;
    protected void Start()
    {
        if (m_manager == null)
        {
            m_manager = GetComponentInParent<EntityStateManager>();
        }
        
        m_manager.events.onEnter.AddListener(OnEnter);
        m_manager.events.onExit.AddListener(OnExit);
    }

    protected virtual void OnEnter(Type state)
    {
        if (states.Contains(state.Name))
        {
            onEnter.Invoke();
        }
    }

    protected virtual void OnExit(Type state)
    {
        if (states.Contains(state.Name))
        {
            onExit.Invoke();
        }
    }
    
}