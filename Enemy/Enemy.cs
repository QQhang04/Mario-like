using UnityEngine;

public class Enemy : Entity<Enemy>
{
    public EnemyEvents enemyEvents;
    
    protected Collider[] m_sightOverlaps = new Collider[1024];
    protected Collider[] m_contactAttackOverlaps = new Collider[1024];
    
    public Player player { get; protected set; }
    public Health health { get; protected set; }
    public EnemyStatsManager stats { get; protected set; }
    public WaypointManager waypoints { get; protected set; }
    protected virtual void InitializeHealth() => health = GetComponent<Health>();
    protected virtual void InitializeStats() => stats = GetComponent<EnemyStatsManager>();
    protected virtual void InitailizeTag() => tag = GameTag.Enemy;
    protected virtual void InitializeWayPointManager() => waypoints = GetComponent<WaypointManager>();
    
    public virtual void Accelerate(Vector3 direction, float acceleration, float topSpeed) =>
        Accelerate(direction, stats.current.turningDrag, acceleration, topSpeed);
    public virtual void Decelerate() => Decelerate(stats.current.deceleration);
    public virtual void Friction() => Decelerate(stats.current.friction);
    public virtual void Gravity() => Gravity(stats.current.gravity);
    public virtual void SnapToGround() => SnapToGround(stats.current.snapForce);
    public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, stats.current.rotationSpeed);


    protected override void Awake()
    {
        base.Awake();
        InitializeHealth();
        InitailizeTag();
        InitializeStats();
        InitializeWayPointManager();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        HandleSight();
        ContactAttack();
    }
    
    private void HandleSight()
    {
        if (!player)
        {
            var overlaps = Physics.OverlapSphereNonAlloc(position, stats.current.spotRange, m_sightOverlaps);

            for (int i = 0; i < overlaps; i++)
            {
                if (m_sightOverlaps[i].CompareTag(GameTag.Player))
                {
                    if (m_sightOverlaps[i].TryGetComponent<Player>(out var player))
                    {
                        this.player = player;
                        if (states.ContainsStateOfType(typeof(FollowEnemyState)))
                            states.Change<FollowEnemyState>();
                        
                        enemyEvents.OnPlayerSpotted?.Invoke();
                        return;
                    }
                }
            }
        }
        else
        {
            var distance = Vector3.Distance(player.transform.position, transform.position);
            if (player.health.current == 0 || distance > stats.current.viewRange)
            {
                player = null;
                if (states.ContainsStateOfType(typeof(WaypointEnemyState)))
                    states.Change<WaypointEnemyState>();
                enemyEvents.OnPlayerScaped?.Invoke();
            }
        }
    }

    private void ContactAttack()
    {
        if (stats.current.canAttackOnContact)
        {
            var overlaps = OverlapEntity(m_contactAttackOverlaps, stats.current.contactOffset);

            for (int i = 0; i < overlaps; i++)
            {
                if (m_contactAttackOverlaps[i].CompareTag(GameTag.Player) &&
                    m_contactAttackOverlaps[i].TryGetComponent<Player>(out var player))
                {
                    var stepping = controller.bounds.max + Vector3.down * stats.current.contactSteppingTolerance;

                    if (!player.IsPointUnderStep(stepping))
                    {
                        if (stats.current.contactPushback)
                        {
                            lateralVelocity = -transform.forward * stats.current.contactPushBackForce;
                        }

                        player.ApplyDamage(stats.current.contactDamage, transform.position);
                        enemyEvents.OnPlayerContact?.Invoke();
                    }
                }
            }
        }
    }

    public override void ApplyDamage(int amount, Vector3 origin)
    {
        if (!health.isEmpty && !health.recovering)
        {
            Debug.Log("Apply!");
            health.Damage(amount);
            enemyEvents.OnDamage?.Invoke();

            if (health.isEmpty)
            {
                controller.enabled = false;
                enemyEvents.OnDie?.Invoke();
            }
        }
    }
}