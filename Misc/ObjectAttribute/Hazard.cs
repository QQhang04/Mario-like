using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hazard : MonoBehaviour, IEntityContact
{
    protected Collider m_collider;
    //标识该危险物体是否为实心 若为 true，表示玩家无法穿过它
    public bool isSolid;
    public bool damageOnlyFromAbove;
    public int damage = 1;

    protected void Awake()
    {
        tag = GameTag.Hazard;
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = !isSolid;
    }

    protected virtual void TryToApplyDamageTo(Player player)
    {
        if (!damageOnlyFromAbove || player.verticalVelocity.y <= 0 && player.IsPointUnderStep(m_collider.bounds.max))
        {
            player.ApplyDamage(damage, transform.position);
        }
    }

    public virtual void OnEntityContact(Entity entity)
    {
        if (entity is Player player)
        {
            TryToApplyDamageTo(player);
        }
    }
    
    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameTag.Player))
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                TryToApplyDamageTo(player);
            }
        }
    }
}