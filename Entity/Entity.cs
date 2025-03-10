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

    public virtual bool SphereCast(Vector3 direction, float distance, out RaycastHit hit,
        int layer = Physics.DefaultRaycastLayers,
        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        var castDistance = Mathf.Abs(distance - radius);
        return Physics.SphereCast(position, radius, direction, out hit, castDistance, layer, queryTriggerInteraction);
    }

    public Vector3 stepPosition => position - transform.up * (height * .5f - controller.stepOffset);
    public virtual bool IsPointUnderStep(Vector3 point) => stepPosition.y > point.y;
}

public abstract class Entity<T> : Entity where T : Entity<T>
{
    protected virtual void OnDrawGizmos()
    {
        // 设置 Gizmos 的颜色
        Gizmos.color = Color.yellow;

        // 绘制 stepPosition 点（在世界坐标系中的位置）
        Gizmos.DrawSphere(stepPosition, .1f); // 可以调整半径大小
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(groundHit.point, 0.1f); // 例如绘制 position 点

    }
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
    }

    protected virtual void Awake()
    {
        InitializeController();
        InitializeStateManager();
    }

    protected virtual void Update()
    {
        if (controller.enabled)
        {
            HandleState();
            HandleController();
            HandleGround();
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

    protected virtual void HandleHighLedge(RaycastHit hit)
    {
        // TODO
    }

    protected virtual bool EvaluateLanding(RaycastHit hit)
    {
        return IsPointUnderStep(hit.point) && Vector3.Angle(hit.normal, Vector3.up) < controller.slopeLimit;
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

            velocity = direction * speed;
            turningVelocity = Vector3.MoveTowards(turningVelocity, Vector3.zero, turningDelta);
            lateralVelocity = velocity + turningVelocity;
        }
    }

    public virtual void Decelerate(float deceleration)
    {
        var delta = deceleration * decelerationMultiplier * Time.deltaTime;
        lateralVelocity = Vector3.MoveTowards(lateralVelocity, Vector3.zero, delta);
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
}