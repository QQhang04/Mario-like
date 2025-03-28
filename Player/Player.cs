using UnityEngine;

public class Player : Entity<Player>
{
    public PlayerEvents playerEvents;
    public PlayerInputManager inputs {get; protected set;}
    public PlayerStatsManager stats {get; protected set;}
    
    protected Vector3 m_respawnPosition;
    protected Quaternion m_respawnRotation;
    
    protected Vector3 m_skinInitialPosition;
    protected Quaternion m_skinInitialRotation;
    
    public Transform pickableSlot;
    public Transform skin;
    
    public int jumpCounter { get; protected set; }
    public bool holding { get; protected set; }
    public bool onWater { get; protected set; }
    public int airSpinCounter { get; protected set; }
    
    public int airDashCounter { get; protected set; }
    public float lastDashTime { get; protected set; }

    public Health health { get; protected set; }
    public Pickable pickable { get; protected set; }

    
    public virtual void SnapToGround() => SnapToGround(stats.current.snapForce);

    public virtual void FaceDirectionSmooth(Vector3 direction) =>
        FaceDirection(direction, stats.current.rotationSpeed);

    public virtual void Decelerate() => Decelerate(stats.current.deceleration);
    protected virtual void InitializeInputs() => inputs = GetComponent<PlayerInputManager>();
    protected virtual void InitializeStats() => stats = GetComponent<PlayerStatsManager>();
    protected virtual void InitialTag() => tag = GameTag.Player;
    protected virtual void InitializeHealth() => health = GetComponent<Health>();
    protected virtual void InitializeRespawn()
    {
        m_respawnPosition = transform.position;
        m_respawnRotation = transform.rotation;
    }
    
    protected virtual void InitializeSkin()
    {
        if (skin)
        {
            m_skinInitialPosition = skin.localPosition;
            m_skinInitialRotation = skin.localRotation;
        }
    }
    
    public virtual void SetSkinParent(Transform parent)
    {
        if (skin)
        {
            skin.parent = parent;
        }
    }
    public virtual void ResetJumps() => jumpCounter = 0;
    
    public virtual void ResetSkinParent()
    {
        if (skin)
        {
            skin.parent = transform;
            skin.localPosition = m_skinInitialPosition;
            skin.localRotation = m_skinInitialRotation;
        }
    }

    public virtual void SetRespawn(Vector3 pos, Quaternion rot)
    {
        m_respawnPosition = pos;
        m_respawnRotation = rot;
    }
    
    public virtual bool FitsIntoPosition(Vector3 position)
    {
        var bounds = controller.bounds;
        var radius = controller.radius - controller.skinWidth;
        var offset = height * 0.5f - radius;
        var top = position + Vector3.up * offset;
        var bottom = position - Vector3.up * offset;

        return !Physics.CheckCapsule(top, bottom, radius,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
    }

    protected override void Awake()
    {
        base.Awake();
        InitializeInputs();
        InitializeStats();
        InitializeSkin();
        InitialTag();
        InitializeHealth();
        InitializeRespawn();
        
        entityEvents.OnRailsEnter.AddListener(() =>
        {
            ResetJumps();
            ResetAirSpins();
            ResetAirDash();
            StartGrind();
        });
    }

    protected override void EnterGround(RaycastHit hit)
    {
        if (!isGrounded)
        {
            groundHit = hit;
            isGrounded = true;
            entityEvents.OnGroundEnter.Invoke();
            jumpCounter = 0;
        }
    }
    public virtual void Accelerate(Vector3 direction)
    {
        var turningDrag = isGrounded && inputs.GetRun() 
            ? stats.current.runningTurningDrag 
            : stats.current.turningDrag;
        var acceleration = isGrounded && inputs.GetRun()
            ? stats.current.runningAcceleration
            : stats.current.acceleration;
        var topSpeed = inputs.GetRun()
            ? stats.current.runningTopSpeed
            : stats.current.topSpeed;
        var finalAcceleration = isGrounded ? acceleration : stats.current.airAcceleration;
        
        Accelerate(direction, turningDrag, finalAcceleration, topSpeed);
        
        if (inputs.GetRunUp())
        {
            lateralVelocity = Vector3.ClampMagnitude(lateralVelocity, topSpeed);
        }
    }
    
    public virtual void AccelerateToInputDirection()
    {
        var inputDirection = inputs.GetMovementCameraDirection();
        Accelerate(inputDirection);
    }

    public virtual void Backflip(float force)
    {
        if (stats.current.canBackflip)
        {
            verticalVelocity = Vector3.up * stats.current.backflipJumpHeight;
            lateralVelocity = -transform.forward * force;
            states.Change<BackflipPlayerState>();
            playerEvents.OnBackflip.Invoke();
        }
    }

    public virtual void BackflipAcceleration()
    {
        var direction = inputs.GetMovementCameraDirection();
        Accelerate(direction, stats.current.backflipTurningDrag, stats.current.backflipAirAcceleration, stats.current.backflipTopSpeed);
    }

    public virtual void Friction()
    {
        Decelerate(stats.current.friction);
    }
    
    public virtual void Gravity()
    {
        if (!isGrounded && verticalVelocity.y > -stats.current.gravityTopSpeed)
        {
            var speed = verticalVelocity.y;
            var force = verticalVelocity.y > 0 ? stats.current.gravity : stats.current.fallGravity;
            speed -= force * gravityMultiplier * Time.deltaTime;
            speed = Mathf.Max(speed, -stats.current.gravityTopSpeed);
            verticalVelocity = new Vector3(0, speed, 0);
        }
    }

    public virtual void Fall()
    {
        if (!isGrounded)
        {
            states.Change<FallPlayerState>();
        }
    }

    public virtual void Jump()
    {
        var canMultiJump = (jumpCounter > 0) && (jumpCounter < stats.current.multiJumps);
        var canCoyoteJump = (jumpCounter == 0) && (Time.time < lastGroundTime + stats.current.coyoteJumpThreshold);
        var holdJump = !holding;

        if ((isGrounded || canMultiJump || canCoyoteJump || onRails) && holdJump)
        {
            if (inputs.GetJumpDown())
            {
                Jump(stats.current.maxJumpHeight);
            }
        }

        if (inputs.GetJumpUp() && (jumpCounter > 0) && verticalVelocity.y > stats.current.minJumpHeight)
        {
            verticalVelocity = stats.current.minJumpHeight * Vector3.up;
        }
    }

    public virtual void Jump(float jumpHeight)
    { 
        jumpCounter++;
        verticalVelocity = Vector3.up * jumpHeight;
        states.Change<FallPlayerState>();
        playerEvents.OnJump.Invoke();
    }
    
    protected override bool EvaluateLanding(RaycastHit hit)
    {
        return base.EvaluateLanding(hit) && !hit.collider.CompareTag(GameTag.Spring);
    }

    public virtual void LedgeGrab()
    {
        if (stats.current.canLedgeHang && velocity.y < 0 && !holding &&
            states.ContainsStateOfType(typeof(LedgeHangingPlayerState)) &&
            DetectingLedge(stats.current.ledgeMaxForwardDistance, stats.current.ledgeMaxDownwardDistance, out var hit))
        {
            Debug.Log("there is a ledge");
            if (!(hit.collider is CapsuleCollider) && !(hit.collider is SphereCollider))
            {
                var ledgeDistance = radius + stats.current.ledgeMaxForwardDistance;
                var lateralOffset = transform.forward * ledgeDistance;
                var verticalOffset = Vector3.down * (height * 0.5f) - center;
                velocity = Vector3.zero;
                transform.parent = hit.collider.CompareTag(GameTag.Platform) ? hit.transform : null;
                transform.position = hit.point - lateralOffset + verticalOffset;
                states.Change<LedgeHangingPlayerState>();
                playerEvents.OnLedgeGrabbed?.Invoke();
            }
        }
    }

    protected virtual bool DetectingLedge(float forwardDistance, float downwardDistance, out RaycastHit ledgeHit)
    {
        var contactOffset = Physics.defaultContactOffset + positionDelta;
        var ledgeMaxDistance = radius + forwardDistance;
        var ledgeHeightOffset = height * 0.5f + contactOffset;
        var upwardOffset = transform.up * ledgeHeightOffset;
        var forwardOffset = transform.forward * ledgeMaxDistance;

        if (Physics.Raycast(position + upwardOffset, transform.forward, ledgeMaxDistance,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(position + forwardOffset * .01f, transform.up, ledgeHeightOffset,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            ledgeHit = new RaycastHit();
            return false;
        }

        var origin = position + upwardOffset + forwardOffset;
        var distance = downwardDistance + contactOffset;
			
        return Physics.Raycast(origin, Vector3.down, out ledgeHit, distance,
            stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore);
    }
    
    
    
    private void OnDrawGizmos()
    {
        // DrawLedgeDetectionGizmos();
        DrawLedgeHangingGizmos();
    }

    private void DrawLedgeDetectionGizmos()
    {
        if (stats == null) return;

        var contactOffset = Physics.defaultContactOffset + positionDelta;
        var ledgeMaxDistance = radius + stats.current.ledgeMaxForwardDistance;
        var ledgeHeightOffset = height * 0.5f + contactOffset;
        var upwardOffset = transform.up * ledgeHeightOffset;
        var forwardOffset = transform.forward * ledgeMaxDistance;

        Vector3 origin1 = transform.position + upwardOffset;
        Vector3 origin2 = transform.position + forwardOffset * .01f;
        Vector3 rayOrigin = transform.position + upwardOffset + forwardOffset;
        Vector3 rayDirection = Vector3.down;
        float rayDistance = stats.current.ledgeMaxDownwardDistance + contactOffset;

        // 设置 Gizmos 颜色
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin1, origin1 + transform.forward * ledgeMaxDistance); // 前向射线
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin2, origin2 + transform.up * ledgeHeightOffset); // 向上检测
        Gizmos.color = Color.green;
        Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * rayDistance); // 向下检测
    }
   
    private void DrawLedgeHangingGizmos()
    {
        if (stats == null) return;

        var ledgeTopMaxDistance = radius + stats.current.ledgeMaxForwardDistance;
        var ledgeTopHeightOffset = height * 0.5f + stats.current.ledgeMaxDownwardDistance;
        var topOrigin = transform.position + Vector3.up * ledgeTopHeightOffset + transform.forward * ledgeTopMaxDistance;
        var sideOrigin = transform.position + Vector3.up * height * 0.5f + Vector3.down * stats.current.ledgeSideHeightOffset;
        var rayDistance = radius + stats.current.ledgeSideMaxDistance;
        var rayRadius = stats.current.ledgeSideCollisionRadius;

        // Draw SphereCast for side detection
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(sideOrigin, rayRadius);
        Gizmos.DrawLine(sideOrigin, sideOrigin + transform.forward * rayDistance);

        // Draw Raycast for top detection
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(topOrigin, topOrigin + Vector3.down * height);

        // Draw Raycast for ledge side detection
        Gizmos.color = Color.green;
        Vector3 ledgeSideOrigin = sideOrigin + transform.right * radius;
        Gizmos.DrawLine(ledgeSideOrigin, ledgeSideOrigin + (transform.forward * rayDistance));
    }
    

    public virtual void ResetAirSpins() => airSpinCounter = 0;
    public virtual void ResetAirDash() => airDashCounter = 0;

    public virtual void StartGrind() => states.Change<RailGrindPlayerState>();
    
    public virtual void Spin()
    {
        bool canAirSpin = isGrounded || (stats.current.canAirSpin && airSpinCounter < stats.current.allowedAirSpins);
        if (stats.current.canSpin && canAirSpin && !holding && inputs.GetSpinDown())
        {
            if (!isGrounded)
            {
                airSpinCounter++;
            }
            
            states.Change<SpinPlayerState>();
            playerEvents.OnSpin?.Invoke();
        }
    }

    public virtual void PushRigidbody(Collider other)
    {
        if (!IsPointUnderStep(other.bounds.max) &&
            other.TryGetComponent(out Rigidbody rigidbody))
        {
            var force = lateralVelocity * stats.current.pushForce;
            rigidbody.velocity += force / rigidbody.mass * Time.deltaTime;
        }
    }

    public override void ApplyDamage(int amount, Vector3 origin)
    {
        if (!health.isEmpty && !health.recovering)
        {
            health.Damage(amount);
            
            var damageDir = origin - transform.position;
            damageDir.y = 0;
            damageDir = damageDir.normalized;
            FaceDirection(damageDir);
            lateralVelocity = -transform.forward * stats.current.hurtBackwardsForce;

            if (!onWater)
            {
                verticalVelocity = Vector3.up * stats.current.hurtUpwardForce;
                states.Change<HurtPlayerState>();
            }

            playerEvents.OnHurt?.Invoke();

            if (health.isEmpty)
            {
                Throw();
                playerEvents.OnDie?.Invoke();
            }
        }
    }

    public virtual void PickAndThrow()
    {
        if (stats.current.canPickUp && inputs.GetPickAndDropDown())
        {
            if (!holding)
            {
                if (CapsuleCast(transform.forward,
                        stats.current.pickDistance, out var hit))
                {
                    if (hit.transform.TryGetComponent(out Pickable pickable))
                    {
                        PickUp(pickable);
                    }
                }
            }
            else
            {
                Throw();
            }
        }
    }

    public virtual void PickUp(Pickable pickable)
    {
        if (!holding && (isGrounded || stats.current.canPickUpOnAir))
        {
            holding = true;
            this.pickable = pickable;
            pickable.Pickup(pickableSlot);
            pickable.onRespawn.AddListener(RemovePickable);
            playerEvents.OnPickUp?.Invoke();
        }
    }

    public virtual void RemovePickable()
    {
        if (holding)
        {
            pickable = null;
            holding = false;
        }
    }

    public virtual void Throw()
    {
        if (holding)
        {
            var force = lateralVelocity.magnitude * stats.current.throwVelocityMultiplier;
            pickable.Release(transform.forward, force);
            pickable = null;
            holding = false;
            playerEvents.OnThrow?.Invoke();
        }
    }
    
    public virtual void StompAttack()
    {
        if (!isGrounded && !holding && stats.current.canStompAttack && inputs.GetStompDown())
        {
            states.Change<StompPlayerState>();
        }
    }

    public virtual void Respawn()
    {
        health.Reset();
        controller.enabled = false;
        transform.SetPositionAndRotation(m_respawnPosition, m_respawnRotation);
        controller.enabled = true;
        controller.Move(Vector3.zero);
        states.Change<IdlePlayerState>();
    }
    
}