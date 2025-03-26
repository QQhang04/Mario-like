using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CheckPoint : MonoBehaviour
{
    public Transform respawn;
    public AudioClip clip;


    public UnityEvent OnActivate;

    protected Collider m_collider;
    protected AudioSource m_audio;
    
    public bool activated { get; protected set; }
    
    public virtual void Activate(Player player)
    {
        if (!activated)
        {
            activated = true;
            m_audio.PlayOneShot(clip);
            player.SetRespawn(respawn.position, respawn.rotation);
            OnActivate?.Invoke();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!activated && other.CompareTag(GameTag.Player))
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                Activate(player);
            }
        }
    }

    protected virtual void Awake()
    {
        if (!TryGetComponent(out m_audio))
        {
            m_audio = gameObject.AddComponent<AudioSource>();
        }

        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }
}