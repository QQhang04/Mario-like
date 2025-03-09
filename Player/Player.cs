using UnityEngine;

public class Player : Entity<Player>
{
    public PlayerEvents playerEvents;
    public PlayerInputManager inputs {get; protected set;}
    public PlayerStatsManager stats {get; protected set;}

    protected virtual void InitializeInputs() => inputs = GetComponent<PlayerInputManager>();
    protected virtual void InitializeStats() => stats = GetComponent<PlayerStatsManager>();

    protected override void Awake()
    {
        base.Awake();
        InitializeInputs();
        InitializeStats();
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
    }
}