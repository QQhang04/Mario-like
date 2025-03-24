using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Toggle : MonoBehaviour
{
    public bool state = true;
    public float delay;
    public Toggle[] multiTrigger;
    
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;

    public virtual void Set(bool value)
    {
        StopAllCoroutines();
        StartCoroutine(SetRoutine(value));
    }

    protected IEnumerator SetRoutine(bool value)
    {
        if (value == state) yield break;
        
        yield return new WaitForSeconds(delay);

        if (value)
        {
            state = true;

            foreach (var toggle in multiTrigger)
            {
                Debug.Log("Set" + value);
                toggle.Set(value);
            }
            
            onActivate?.Invoke();
        }
        else
        {
            state = false;
            
            foreach (var toggle in multiTrigger)
            {
                Debug.Log("Set" + value);
                toggle.Set(value);
            }
            
            onDeactivate?.Invoke();
        }
    }
}