using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Pickable : MonoBehaviour, IEntityContact
{
    public UnityEvent onPicked;
    public UnityEvent onRespawn;
    public UnityEvent onReleased;
    
    [Header("General Settings")] 
    public Vector3 offset;
    public float releaseOffset = .5f;
    
    [Header("Attack Settings")] 
    public bool canAttackEnemies = true;
    public int damage = 1;
    public float minDamageSpeed = 5f;
    
    [Header("Respawn Settings")]
    public bool canAutoRespawn = true;
    public bool respawnOnHitHazards; // 打击伤害物后重生
    public float respawnHeightLimit = -100f;
    
    protected Collider m_collider;
    protected Rigidbody m_rb;
    protected Vector3 m_initialPosition;
    protected Quaternion m_initialRotation;
    protected Transform m_initialParent;
    
    protected RigidbodyInterpolation m_rigidbodyInterpolation;

    public bool beingHold { get; protected set; }
    
    protected void Start()
    {
        m_collider = GetComponent<Collider>();
        m_rb = GetComponent<Rigidbody>();
        
        // 注意要local！
        m_initialPosition = transform.localPosition;
        m_initialRotation = transform.localRotation;
        m_initialParent = transform.parent;
    }

    public virtual void Pickup(Transform slot)
    {
        if (!beingHold)
        {
            beingHold = true;
            transform.SetParent(slot);
            transform.localPosition = Vector3.zero + offset;
            transform.localRotation = Quaternion.identity;
            m_rb.isKinematic = true;
            m_collider.isTrigger = true;
            m_rigidbodyInterpolation = m_rb.interpolation;
            m_rb.interpolation = RigidbodyInterpolation.None;
            
            onPicked?.Invoke();
        }
    }
    
    public virtual void Release(Vector3 direction, float force)
    {
        if (beingHold)
        {
            transform.parent = null;
            transform.position += direction * releaseOffset;
            m_collider.isTrigger = m_rb.isKinematic = beingHold = false;
            m_rb.interpolation = m_rigidbodyInterpolation;
            m_rb.velocity = direction * force;
            onReleased?.Invoke();
        }
    }

    public void OnEntityContact(Entity entity)
    {
        if (canAttackEnemies && entity is Enemy && m_rb.velocity.magnitude >= minDamageSpeed)
        {
            entity.ApplyDamage(damage, transform.position);
        }
    }

    protected virtual void EvaluateHazardRespawn(Collider collider)
    {
        if (canAutoRespawn && respawnOnHitHazards && collider.CompareTag(GameTag.Hazard))
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        m_rb.velocity = Vector3.zero;
        transform.localPosition = m_initialPosition;
        transform.localRotation = m_initialRotation;
        m_rb.isKinematic = m_collider.isTrigger = beingHold = false;
        onRespawn?.Invoke();
    }

    protected void OnTriggerEnter(Collider other)
    {
        EvaluateHazardRespawn(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        EvaluateHazardRespawn(other.collider);
    }
}