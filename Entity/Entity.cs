using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public EntityEvents entityEvents;
    public bool isGrounded { get; protected set; } = true;
    public Vector3 velocity { get; set; }
    public float turningDragMultipler { get; set; } = 1f;
    public float topSpeedMultiplier { get; set; } = 1f;
    public float accelerationMultiplier { get; set; } = 1f;
    public float decelerationMultiplier { get; set; } = 1f;
    public float gravityMultiplier { get; set; } = 1f;
    
    public CharacterController controller {get; protected set;}
    public float originalHeight { get; protected set; }
    public Vector3 unsizePosition => position - transform.up * height * 0.5f + transform.up * originalHeight * 0.5f;
    public Vector3 lateralVelocity
    {
        get {return new Vector3(velocity.x, 0, velocity.z);}
        set {velocity = new Vector3(value.x, velocity.y, value.z);}
    }

    public Vector3 verticalVelocity
    {
        get {return new Vector3(0, velocity.y, 0);}
        set {velocity = new Vector3(velocity.x, value.y, velocity.z);}
    }

    protected readonly float m_groundOffset = .1f;
    public RaycastHit groundHit;
    public float lastGroundTime { get; protected set; }
    public float height => controller.height;
    public float radius => controller.radius;
    public Vector3 center => controller.center;
    public Vector3 position => transform.position + center;
    
    protected Collider[] m_contactBuffer = new Collider[10];
    protected CapsuleCollider m_collider;

    public virtual bool SphereCast(Vector3 direction, float distance, out RaycastHit hit,
        int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        var castDistance = Mathf.Abs(distance - radius);
        return Physics.SphereCast(position, radius, direction, out hit, castDistance, layer, queryTriggerInteraction);
    }

    public Vector3 stepPosition => position - transform.up * (height * .5f - controller.stepOffset);
    public virtual bool IsPointUnderStep(Vector3 point) => stepPosition.y > point.y;

    public virtual int OverlapEntity(Collider[] result, float skinOffset = 0)
    {
        var contactOffset = skinOffset + controller.skinWidth + Physics.defaultContactOffset;
        var overlapsRadius = radius + contactOffset;
        var offset = (height + contactOffset) * 0.5f - overlapsRadius;
        var top = position + Vector3.up * offset;
        var bottom = position + Vector3.down * offset;
        return Physics.OverlapCapsuleNonAlloc(top, bottom, overlapsRadius, result);
    }
}

public abstract class Entity<T> : Entity where T : Entity<T>
{
    public EntityStateManager<T> states {get; private set;}
    protected virtual void HandleState() => states.Step();
    protected virtual void InitializeStateManager() => states = GetComponent<EntityStateManager<T>>();
    protected virtual void InitializeController()
    {
        controller = GetComponent<CharacterController>();
        
        if (!controller)
            controller = gameObject.AddComponent<CharacterController>();

        controller.skinWidth = .005f;
        controller.minMoveDistance = 0;
        
        originalHeight = controller.height;
    }

    protected virtual void InitializeCollider()
    {
        m_collider = gameObject.AddComponent<CapsuleCollider>();
        m_collider.height = controller.height;
        m_collider.radius = controller.radius;
        m_collider.center = controller.center;
        m_collider.isTrigger = true;
        m_collider.enabled = false;
    }
    
    protected virtual void Awake()
    {
        InitializeController();
        InitializeStateManager();
        InitializeCollider();
    }

    protected virtual void Update()
    {
        if (controller.enabled)
        {
            HandleState();
            HandleController();
            HandleGround();
            HandleContacts();
        }
    }

    protected virtual void HandleGround()
    {
        var distance = (height * .5f) + m_groundOffset;
        if (SphereCast(Vector3.down, distance, out var hit) && verticalVelocity.y <= 0)
        {
            if (!isGrounded)
            {
                if (EvaluateLanding(hit))
                {
                    EnterGround(hit);
                }
                else
                {
                    HandleHighLedge(hit);
                }
            }
        }
        else
        {
            ExitGround();
        }
    }

    protected virtual void HandleContacts()
    {
        var overlaps = OverlapEntity(m_contactBuffer);

        for (int i = 0; i < overlaps; i++)
        {
            if (!m_contactBuffer[i].isTrigger && m_contactBuffer[i].transform != transform)
            {
                OnContact(m_contactBuffer[i]);

                var listeners = m_contactBuffer[i].GetComponents<IEntityContact>();

                // 广播 把自己当前的状态传到被碰撞并且实现接口的物体上
                foreach (var contact in listeners)
                {
                    contact.OnEntityContact((T)this);
                }

                if (m_contactBuffer[i].bounds.min.y > position.y)
                {
                    verticalVelocity = Vector3.Min(verticalVelocity, Vector3.zero);
                }
            }
        }
    }

    protected virtual void HandleHighLedge(RaycastHit hit)
    {
        // TODO
    }

    protected virtual bool EvaluateLanding(RaycastHit hit)
    {
        return IsPointUnderStep(hit.point) && Vector3.Angle(hit.normal, Vector3.up) < controller.slopeLimit;
    }
    
    protected virtual void OnContact(Collider other)
    {
        if (other)
        {
            states.OnContact(other);
        }
    }

    protected virtual void EnterGround(RaycastHit hit)
    {
        if (!isGrounded)
        {
            groundHit = hit;
            isGrounded = true;
            entityEvents.OnGroundEnter.Invoke();
        }
    }
    
    protected virtual void ExitGround()
    {
        if (isGrounded)
        {
            isGrounded = false;
            transform.parent = null;
            lastGroundTime = Time.time;
            verticalVelocity = Vector3.Max(verticalVelocity, Vector3.zero);
            entityEvents.OnGroundExit?.Invoke();
        }
    }

    protected virtual void HandleController()
    {
        if (controller.enabled)
        {
            controller.Move(velocity * Time.deltaTime);
            return;
        }
        
        transform.position += velocity * Time.deltaTime;
    }

    public virtual void Accelerate(Vector3 direction, float turningDrag, float acceleration, float topSpeed)
    {
        if (direction.sqrMagnitude > 0)
        {
            var speed = Vector3.Dot(direction, lateralVelocity);
            var velocity = direction * speed;
            var turningVelocity = lateralVelocity - velocity;
            var turningDelta = turningDrag * turningDragMultipler * Time.deltaTime;
            var targetTopSpeed = topSpeed * topSpeedMultiplier;

            if (lateralVelocity.magnitude < targetTopSpeed || speed < 0)
            {
                speed += acceleration * accelerationMultiplier * Time.deltaTime;
                speed = Mathf.Clamp(speed, -targetTopSpeed, targetTopSpeed);
            }

            velocity = direction * speed;;
            
            turningVelocity = Vector3.MoveTowards(turningVelocity, Vector3.zero, turningDelta);
            lateralVelocity = velocity + turningVelocity;
        }
    }

    public virtual void Decelerate(float deceleration)
    {
        var delta = deceleration * decelerationMultiplier * Time.deltaTime;
        lateralVelocity = Vector3.MoveTowards(lateralVelocity, Vector3.zero, delta);
    }
    
    public virtual void SnapToGround(float force)
    {
        if (isGrounded && (verticalVelocity.y <= 0))
        {
            verticalVelocity = Vector3.down * force;
        }
    }

    public virtual void FaceDirection(Vector3 direction, float degreesPerSpeed)
    {
        if (direction != Vector3.zero)
        {
            var rotation = transform.rotation;
            var rotationDelta = degreesPerSpeed * Time.deltaTime;
            var target = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(rotation, target, rotationDelta);
        }
    }
    
    public virtual void FaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0)
        {
            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = rotation;
        }
    }
    
    public virtual void ApplyDamage(int amount, Vector3 origin)
    {
    }
}