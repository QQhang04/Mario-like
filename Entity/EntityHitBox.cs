using System;
using UnityEngine;

public class EntityHitBox : MonoBehaviour
{
    protected Entity m_entity;
    protected Collider m_collider;

    [Header("Attack Settings")] 
    public int damage = 1;

    [Header("Breakable Settings")]
    public bool breakObjects;
    
    [Header("Rebound Settings")] 
    public bool rebound = true;
    public float reboundMinForce = 10f;
    public float reboundMaxForce = 25f;
    
    [Header("Push Back Settings")]
    public bool pushBack;
    public float pushBackMinMagnitude = 5f;
    public float pushBackMaxMagnitude = 10f;
    
    protected virtual void InitializeEntity()
    {
        if (!m_entity)
        {
            m_entity = GetComponentInParent<Entity>();
        }
    }
    
    protected virtual void InitializeCollider()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }
    
    private void Start()
    {
        InitializeEntity();
        InitializeCollider();
    }
    
    protected void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }

    protected virtual void HandleCustomCollision(Collider other) 
    {
        // todo 
    }
    
    protected virtual void HandleCollision(Collider other) 
    {
        // Character Controller实际继承Collider类
        if (other != m_entity.controller)
        {
            if (other.TryGetComponent(out Entity target))
            {
                HandleEntityAttack(target);
                HandleRebound();
                HandlePushBack();
            }
            else if (other.TryGetComponent(out Breakable breakable))
            {
                HandleBreakableObject(breakable);
            }
        }
    }

    protected virtual void HandleEntityAttack(Entity other)
    {
        other.ApplyDamage(damage, transform.position);
    }
    
    // 设置人物回弹
    protected virtual void HandleRebound()
    {
        if (rebound)
        {
            var force = -m_entity.velocity.y;
            force = Mathf.Clamp(force, reboundMinForce, reboundMaxForce);
            m_entity.verticalVelocity = Vector3.up * force;
        }
    }
    
    // 设置反向阻尼
    protected virtual void HandlePushBack()
    {
        if (pushBack)
        {
            var force = -m_entity.lateralVelocity.magnitude;
            force = Mathf.Clamp(force, pushBackMinMagnitude, pushBackMaxMagnitude);
            m_entity.verticalVelocity = -Vector3.forward * force;
        }
    }
    
    protected virtual void HandleBreakableObject(Breakable breakable)
    {
        if (breakObjects)
        {
            breakable.Break();
        }
    }
}