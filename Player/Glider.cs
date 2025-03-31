using System;
using System.Collections;
using UnityEngine;

public class Glider : MonoBehaviour
{
    protected Player player;
    protected AudioSource audioSource;
    
    public TrailRenderer[] trails;
    public AudioClip openClip;
    public AudioClip closeClip;
    public float scaleDuration = .7f;
    

    private void Start()
    {
       InitializeGlider();
       InitializePlayer();
       InitializeAudio();
       InitializeCallBacks();
    }

    protected virtual void InitializeGlider()
    {
       transform.localScale = Vector3.zero;
       SetTrailsEmitting(false);
    }
    
    protected virtual void InitializePlayer()
    {
       if (player == null)
       {
          player = GetComponentInParent<Player>();
       }
    }
    
    protected virtual void InitializeAudio()
    {
       if (!TryGetComponent(out audioSource))
       {
          audioSource = gameObject.AddComponent<AudioSource>();
       }
    }
    
    protected virtual void InitializeCallBacks()
    {
       player.playerEvents.OnGlidingStart.AddListener(OnGlidingStart);
       player.playerEvents.OnGlidingStop.AddListener(OnGlidingStop);
    }

    private void OnGlidingStart()
    {
       StopAllCoroutines();
       StartCoroutine(GliderRoutine(Vector3.zero, Vector3.one));
       SetTrailsEmitting(true);
       audioSource.PlayOneShot(openClip);
    }
    
    private void OnGlidingStop()
    {
       StopAllCoroutines();
       StartCoroutine(GliderRoutine(Vector3.one, Vector3.zero));
       SetTrailsEmitting(false);
       audioSource.PlayOneShot(closeClip);
    }

    private IEnumerator GliderRoutine(Vector3 from, Vector3 to)
    {
       float t = 0f;

       transform.localScale = from;

       while (t < scaleDuration)
       {
          transform.localScale = Vector3.Lerp(from, to, t / scaleDuration);
          t += Time.deltaTime;
          yield return null;
       }
       
       transform.localScale = to;
    }
    
    

    private void SetTrailsEmitting(bool isEmitting)
    {
       if (trails != null)
       {
          foreach (var trail in trails)
          {
             trail.enabled = isEmitting;
          }
       }
    }
}