using System;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    
}

public abstract class Entity<T> : Entity where T : Entity<T>
{
    public EntityStateManager<T> states {get; private set;}
    protected virtual void HandleState() => states.Step();
    protected virtual void InitializeStateManager() => states = GetComponent<EntityStateManager<T>>();

    private void Awake()
    {
        InitializeStateManager();
    }

    protected void Update()
    {
        HandleState();
    }
}