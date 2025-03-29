using System.Collections;
using UnityEngine;

public class PortalWithMomentum : MonoBehaviour
{
    public bool useFlash = true;
    public PortalWithMomentum exit;
    public float exitOffset = 1f;
    public AudioClip teleportClip;

    protected Collider m_collider;
    protected AudioSource m_audio;

    private Vector3 playerVelocity;
    protected float playerMomentum;
    protected Player m_player;

    protected PlayerCamera m_camera;

    public Vector3 position => transform.position;
    public Vector3 forward => transform.forward;

    protected virtual void Start()
    {
        m_collider = GetComponent<Collider>();
        m_audio = GetComponent<AudioSource>();
        m_camera = FindObjectOfType<PlayerCamera>();
        m_collider.isTrigger = true;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (exit && other.TryGetComponent(out Player player))
        {
            m_player = player;
            playerVelocity = player.velocity;
            playerVelocity.y = player.isGrounded ? 0f : player.velocity.y;
            
            // Debug.Log(playerVelocity + " " + transform.forward + " " + Vector3.Dot(playerVelocity,transform.forward));
            if (Vector3.Dot(playerVelocity,transform.forward) < -10f)
            {
                StartCoroutine(Teleport());
            }
        }
    }

    protected IEnumerator Teleport()
    {
        playerMomentum = playerVelocity.magnitude;
        m_player.controller.enabled = false;
        m_player.velocity = Vector3.zero;
        
        if (useFlash)
        {
            Flash.Instance?.Trigger();
        }

        playerMomentum = Mathf.Clamp(playerMomentum, 0, 18f);
        m_player.transform.position = exit.position;
        m_player.FaceDirection(exit.forward);
        
        yield return null;
        m_audio.PlayOneShot(teleportClip);
        m_player.controller.enabled = true;
        m_player.velocity = exit.transform.forward * playerMomentum;
    }
}