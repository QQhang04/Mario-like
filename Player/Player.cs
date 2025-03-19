using UnityEngine;

public class Player : Entity<Player>
{
    public PlayerEvents playerEvents;
    public PlayerInputManager inputs {get; protected set;}
    public PlayerStatsManager stats {get; protected set;}
    
    protected Vector3 m_respawnPosition;
    protected Quaternion m_respawnRotation;
    
    public int jumpCounter { get; protected set; }
    public bool holding { get; protected set; }
    public bool onWater { get; protected set; }

    public Health health { get; protected set; }
    
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

    protected override void Awake()
    {
        base.Awake();
        InitializeInputs();
        InitializeStats();
        InitialTag();
        InitializeHealth();
        InitializeRespawn();
    }

    protected override void Update()
    {
        base.Update();
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

        if (isGrounded || canMultiJump || canCoyoteJump)
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

    public virtual void Throw()
    {
        // todo
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