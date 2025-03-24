using System.Collections;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public Vector3 offset;
    public float duration;
    public float resetDuration;
    
    protected Vector3 m_initialPosition;

    public virtual void ApplyToOffset()
    {
        StopAllCoroutines();
        StartCoroutine(ApplyOffsetRoutine(m_initialPosition, m_initialPosition + offset, duration));
    }
    
    public virtual void Reset()
    {
        StopAllCoroutines();
        StartCoroutine(ApplyOffsetRoutine(transform.localPosition, m_initialPosition, resetDuration));
    }

    protected virtual IEnumerator ApplyOffsetRoutine(Vector3 from, Vector3 to, float duration)
    {
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            var t = elapsedTime / duration;
            t = EaseInOutQuad(t);
            
            transform.localPosition = Vector3.Lerp(from, to, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = to;
    }

    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }
    
    protected virtual void Start()
    {
        m_initialPosition = transform.localPosition;
    }
}