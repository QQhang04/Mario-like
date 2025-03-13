using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class UIAnimator : MonoBehaviour
{
    public UnityEvent OnShow;
    public UnityEvent OnHide;

    public bool hidenOnAwake;
    public string normalTrigger = "Normal";
    public string showTrigger = "Show";
    public string hideTrigger = "Hide";

    protected Animator m_animator;

    
    public virtual void Show()
    {
        m_animator.SetTrigger(showTrigger);
        OnShow?.Invoke();
    }
    
    public virtual void Hide()
    {
        m_animator.SetTrigger(hideTrigger);
        OnHide?.Invoke();
    }
    
    public virtual void SetActive(bool value) => gameObject.SetActive(value);

    protected virtual void Awake()
    {
        m_animator = GetComponent<Animator>();

        if (hidenOnAwake)
        {
            m_animator.Play(hideTrigger, 0, 1);
        }
    }
}