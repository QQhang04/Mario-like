using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    private struct InitialState
    {
        // Inspector 可改的
        public bool    collectOnContact;
        public bool    usePhysics;
        public Vector3 initialVelocity;
        public float   gravity;
        public bool    randomizeInitialDirection;
        public bool    hidden;
        public float   quickShowHeight, quickShowDuration, hideDuration;
        public bool    hasLifeTime;
        public float   lifeTimeDuration;
        public float   collisionRadius, bounceness, maxBounceYVelocity, minForceToStopPhysics;
        // Transform / Display / Collider
        public Vector3 position;
        public Quaternion rotation;
        public bool     displayActive;
        public bool     colliderEnabled;
    }
    private InitialState _init;
    
    private void RecordInitialState()
    {
        _init.collectOnContact          = collectOnContact;
        _init.usePhysics               = usePhysics;
        _init.initialVelocity          = initialVelocity;
        _init.gravity                  = gravity;
        _init.randomizeInitialDirection= randomizeInitialDirection;
        _init.hidden                   = hidden;
        _init.quickShowHeight          = quickShowHeight;
        _init.quickShowDuration        = quickShowDuration;
        _init.hideDuration             = hideDuration;
        _init.hasLifeTime              = hasLifeTime;
        _init.lifeTimeDuration         = lifeTimeDuration;
        _init.collisionRadius          = collisionRadius;
        _init.bounceness               = bounceness;
        _init.maxBounceYVelocity       = maxBounceYVelocity;
        _init.minForceToStopPhysics    = minForceToStopPhysics;

        _init.position      = transform.position;
        _init.rotation      = transform.rotation;
        _init.displayActive = display.activeSelf;
        _init.colliderEnabled = m_collider != null ? m_collider.enabled : true;
    }

    private void RestoreInitialState()
    {
        // 恢复 Inspector 字段
        collectOnContact           = _init.collectOnContact;
        usePhysics                 = _init.usePhysics;
        initialVelocity            = _init.initialVelocity;
        gravity                    = _init.gravity;
        randomizeInitialDirection  = _init.randomizeInitialDirection;
        hidden                     = _init.hidden;
        quickShowHeight            = _init.quickShowHeight;
        quickShowDuration          = _init.quickShowDuration;
        hideDuration               = _init.hideDuration;
        hasLifeTime                = _init.hasLifeTime;
        lifeTimeDuration           = _init.lifeTimeDuration;
        collisionRadius            = _init.collisionRadius;
        bounceness                 = _init.bounceness;
        maxBounceYVelocity         = _init.maxBounceYVelocity;
        minForceToStopPhysics      = _init.minForceToStopPhysics;

        // 恢复 Transform / Display / Collider
        transform.position         = _init.position;
        transform.rotation         = _init.rotation;
        display.SetActive(_init.displayActive);
        m_collider.enabled         = _init.colliderEnabled;

        // 恢复内部状态
        m_vanished         = false;
        m_ghosting         = true;
        m_elapsedGhostingTime = 0f;
        m_elapsedLifeTime     = 0f;
        m_velocity         = Vector3.zero;
    }
    
    [Header("General Settings")] 
    public GameObject display;

    public bool collectOnContact = true; 
    public AudioClip clip;
    public int times = 1;
    public ParticleSystem particles;

    public float ghostingDuration = 0.5f;
    
    [Header("Visibility Settings")] 
    public bool hidden;
    public float quickShowHeight = 2f;
    public float quickShowDuration = .25f;
    public float hideDuration = .5f;
    
    [Header("Life Time")] 
    public bool hasLifeTime;
    public float lifeTimeDuration = 5f;

    [Header("Physics Settings")] 
    public bool usePhysics;
    public Vector3 initialVelocity = new Vector3(0, 12, 0);
    public float gravity = 15f;
    public bool randomizeInitialDirection = true;
    public float collisionRadius = 0.5f;
    public float bounceness = .98f;
    public float maxBounceYVelocity = 10f;
    public AudioClip collectClip;
    public float minForceToStopPhysics = 3f;

    [Space(15)] 
    public PlayerEvent onCollect;
    protected AudioSource m_audio;
    protected Collider m_collider;
    protected Vector3 m_velocity;
    protected bool m_vanished;
    protected bool m_ghosting = true;
    protected float m_elapsedGhostingTime;
    protected float m_elapsedLifeTime;

    protected const int k_verticalMinRotation = 0;
    protected const int k_verticalMaxRotation = 30;
    protected const int k_horizontalMinRotation = 0;
    protected const int k_horizontalMaxRotation = 360;

    protected virtual void InitializeAudio()
    {
        if (!TryGetComponent(out m_audio))
        {
            m_audio = gameObject.AddComponent<AudioSource>();
        }
    }
    
    protected virtual void InitializeCollider()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }
    
    protected virtual void InitializeTransform()
    {
        transform.rotation = Quaternion.identity;
    }
    
    protected virtual void InitializeDisplay()
    {
        display.SetActive(!hidden);
    }
    
    protected virtual void InitializeVelocity()
    {
        var direction = initialVelocity.normalized;
        var force = initialVelocity.magnitude;

        if (randomizeInitialDirection)
        {
            var randomY = UnityEngine.Random.Range(k_horizontalMinRotation, k_horizontalMaxRotation);
            var randomZ = UnityEngine.Random.Range(k_verticalMinRotation, k_verticalMaxRotation);
            direction = Quaternion.Euler(0, 0, randomZ) * direction;
            direction = Quaternion.Euler(0, randomY, 0) * direction;
        }
        
        m_velocity = direction * force;
    }

    protected virtual void HandleGhosting()
    {
        if (m_ghosting)
        {
            m_elapsedGhostingTime += Time.deltaTime;

            if (m_elapsedGhostingTime >= ghostingDuration)
            {
                m_elapsedGhostingTime = 0;
                m_ghosting = false;
            }
        }
    }

    public virtual void Vanish()
    {
        if (!m_vanished)
        {
            m_vanished = true;
            m_elapsedLifeTime = 0;
            display.SetActive(false);
            m_collider.enabled = false;
            
            // 回收
            StartCoroutine(ReleaseToPool());
        }
    }
    
    protected virtual void HandleLeftTime()
    {
        if (hasLifeTime)
        {
            m_elapsedLifeTime += Time.deltaTime;
            if (m_elapsedLifeTime >= lifeTimeDuration)
            {
                Vanish();
            }
        }
    }
    
    private IEnumerator ReleaseToPool()
    {
        // 等待一帧或等粒子播放完
        yield return new WaitForSeconds(particles ? particles.main.duration : 0f);
        ObjectPool.Instance.PushObject(gameObject);
    }

    protected virtual void HandleMovement()
    {
        m_velocity.y -= gravity * Time.deltaTime;
    }

    protected virtual void HandleSweep()
    {
        var direction = m_velocity.normalized;
        var magnitude = m_velocity.magnitude;
        var distance = magnitude * Time.deltaTime;

        if (Physics.SphereCast(transform.position, collisionRadius, direction, out var hit, distance,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                var bounceDirection = Vector3.Reflect(direction, hit.normal);
                m_velocity = bounceDirection * (magnitude * bounceness);
                m_velocity.y = Mathf.Min(m_velocity.y, maxBounceYVelocity);
                m_audio.Stop();
                m_audio.PlayOneShot(collectClip);

                if (m_velocity.y <= minForceToStopPhysics)
                {
                    usePhysics = false;
                }
            }
        }
        
        transform.position += m_velocity * Time.deltaTime;
    }

    public virtual void Collect(Player player)
    {
        if ((!m_ghosting || hidden) && !m_vanished)
        {
            if (!hidden)
            {
                if (particles)
                    particles.Play();
                Vanish();
            }
            else
            {
                StartCoroutine(QuickShowRoutine());
            }
            
            StartCoroutine(CollectRoutine(player));
        }
    }

    protected virtual IEnumerator CollectRoutine(Player player)
    {
        for (int i = 0; i < times; i++)
        {
            m_audio.Stop();
            m_audio.PlayOneShot(clip);
            onCollect.Invoke(player);
            yield return new WaitForSeconds(0.1f);
        }
    }

    protected virtual IEnumerator QuickShowRoutine()
    {
        var elapsedTime = 0f;
        var initialPosition = transform.position;
        var targetPosition = initialPosition + Vector3.up * quickShowHeight;
        
        display.SetActive(true);
        m_collider.enabled = false;

        while (elapsedTime < quickShowDuration)
        {
            var t = elapsedTime / quickShowDuration;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = targetPosition;
        yield return new WaitForSeconds(hideDuration);
        transform.position = initialPosition;
        Vanish();
    }

    private void Awake()
    {
        InitializeAudio();
        InitializeCollider();
        InitializeTransform();
        InitializeDisplay();
        InitializeVelocity();
        
        RecordInitialState();
    }

    protected virtual void OnEnable()
    {
        RestoreInitialState();
        InitializeTransform();
        InitializeDisplay();
        InitializeVelocity();
    }

    protected virtual void Update()
    {
        if (!m_vanished)
        {
            HandleGhosting();
            HandleLeftTime();

            if (usePhysics)
            {
                HandleMovement();
                HandleSweep();
            }
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (collectOnContact && other.CompareTag(GameTag.Player))
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                Collect(player);
            }
        }
    }

    protected void OnDrawGizmos()
    {
        if (usePhysics)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, collisionRadius);
        }
    }
}