using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GravityField : MonoBehaviour
{
    public float force = 75f;

    protected Collider m_collider;

    protected virtual void Start()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }
    
    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameTag.Player))
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                if (player.isGrounded)
                {
                    player.verticalVelocity = Vector3.zero;
                }
                player.velocity += transform.up * force * Time.deltaTime;
            }
        }
    }
}