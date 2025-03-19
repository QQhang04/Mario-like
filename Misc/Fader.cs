using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fader : Singleton<Fader>
{
    public float speed = 1f;
    protected Image m_image;

    public void FadeOut() => FadeOut((() => { }));
    public void FadeIn() => FadeIn((() => { }));

    public void FadeOut(Action onFinished)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine(onFinished));
    }

    public void FadeIn(Action onFinished)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInRoutine(onFinished));
    }
    
    protected virtual IEnumerator FadeOutRoutine(Action onFinished)
    {
        while (m_image.color.a < 1)
        {
            var color = m_image.color;
            color.a += speed * Time.deltaTime;
            m_image.color = color;
            yield return null;
        }

        onFinished?.Invoke();
    }
    
    protected virtual IEnumerator FadeInRoutine(Action onFinished)
    {
        while (m_image.color.a > 0)
        {
            var color = m_image.color;
            color.a -= speed * Time.deltaTime;
            m_image.color = color;
            yield return null;
        }
        
        onFinished?.Invoke();
    }
    
    protected override void Awake()
    {
        base.Awake();
        m_image = GetComponent<Image>();
    }
}