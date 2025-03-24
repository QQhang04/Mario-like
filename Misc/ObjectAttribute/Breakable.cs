using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider), typeof(AudioSource))]
public class Breakable : MonoBehaviour
{
    public UnityEvent OnBreak;
    
    public GameObject display;
    public AudioClip breakSound;
    
    protected Collider m_collider;
    protected AudioSource m_audio;
    protected Rigidbody m_rb;
    
    public bool broken { get; protected set; }

    public virtual void Break()
    {
        if (!broken)
        {
            if (m_rb)
            {
                m_rb.isKinematic = true;
            }
            
            broken = true;
            display.SetActive(false);
            m_collider.enabled = false;
            m_audio.PlayOneShot(breakSound);
            
            OnBreak?.Invoke();
        }
    }
    
    private void Start()
    {
        m_collider = GetComponent<Collider>();
        m_audio = GetComponent<AudioSource>();
        TryGetComponent(out m_rb); // 兼容性更好
    }

}