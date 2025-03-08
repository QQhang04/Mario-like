using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityStateManager : MonoBehaviour {

}

public abstract class EntityStateManager<T> : EntityStateManager where T : Entity<T>
{
    protected List<EntityState<T>> m_list = new List<EntityState<T>>();
    protected Dictionary<Type, EntityState<T>> m_states = new Dictionary<Type, EntityState<T>>();
    protected abstract List<EntityState<T>> GetStateList();
    public EntityState<T> current {get; private set;}
    public T entity { get; private set; }

    protected virtual void InitializeStates()
    {
        m_list = GetStateList();

        foreach (var state in m_list)
        {
            var type = state.GetType();

            if (!m_states.ContainsKey(type))
            {
                m_states.Add(type, state);
            }
        }

        if (m_list.Count > 0)
        {
            current = m_list[0];
        }
    }

    protected void Start()
    {
        InitializeStates();
    }

    public virtual void Step()
    {
        if (current != null && Time.timeScale > 0)
        {
            current.Step(entity);
        }
    }
}
