using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class ItemBox : MonoBehaviour, IEntityContact
{
    protected BoxCollider m_collider;
    protected Vector3 m_initialScale;
    public Collectable[] collectables;
    protected bool m_enabled = true;
    public MeshRenderer itemBoxRenderer;
    public Material emptyItemBoxMaterial;
    
    [Space(15)]
    public UnityEvent onCollect;
    public UnityEvent onDisable;

    protected int m_index;
    
    protected virtual void InitializeCollectables()
    {
        foreach (var collectable in collectables)
        {
            
        }
    }

    public virtual void Collect(Player player)
    {
        if (m_enabled)
        {
            if (m_index < collectables.Length)
            {
                // if (collectables[m_index].)
            }
        }
    }

    public void Disable()
    {
        if (m_enabled)
        {
            m_enabled = false;
            itemBoxRenderer.sharedMaterial = emptyItemBoxMaterial;
            onDisable?.Invoke();
        }
    }
    
    protected void Start()
    {
        m_collider = GetComponent<BoxCollider>();
        m_initialScale = transform.localScale;
        InitializeCollectables();
    }

    public void OnEntityContact(Entity entity)
    {
        if (entity is Player player)
        {
            if (entity.velocity.y > 0 && entity.position.y < m_collider.bounds.min.y)
            {
                
            }
        }
    }
}
