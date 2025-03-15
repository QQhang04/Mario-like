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
    
    /// <summary>
    ///  SetActive(true) 是 Unity 提供的 GameObject 原生方法，
    /// 它本身不依赖 Awake()，即便 GameObject 一开始是未激活的，
    /// 其他对象仍可以访问该 GameObject 并调用 SetActive(true)。
    /// </summary>
    /// <param name="value">是否激活</param>
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