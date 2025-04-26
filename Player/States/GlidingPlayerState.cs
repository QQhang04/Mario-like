using UnityEngine;


public class GlidingPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        player.velocity = Vector3.zero;
        player.playerEvents.OnGlidingStart?.Invoke();
    }

    protected override void OnExit(Player player)
    {
        player.playerEvents.OnGlidingStop?.Invoke();
    }

    protected override void OnStep(Player player)
    {
        Vector3 inputDirection = player.inputs.GetMovementCameraDirection();
        HandleGlidingGravity(player);
        
        player.FaceDirection(player.lateralVelocity);
        player.Accelerate(inputDirection, player.stats.current.glidingTurningDrag,
            player.stats.current.airAcceleration, player.stats.current.topSpeed);
        player.LedgeGrab();
        
        if (player.isGrounded) 
            player.states.Change<IdlePlayerState>();
        if (!player.inputs.GetGliderDown())
            player.states.Change<FallPlayerState>();
    }
    
    public override void OnContact(Player player, Collider other)
    {
        if (!player.isGrounded)
        {
            player.WallDrag(other);
        }
    }

    private void HandleGlidingGravity(Player player)
    {
        var yVelocity = player.verticalVelocity.y;
        yVelocity -= player.stats.current.glidingGravity * Time.deltaTime;
        yVelocity = Mathf.Max(yVelocity, -player.stats.current.glidingMaxFallSpeed);
        player.verticalVelocity = new Vector3(0, yVelocity, 0);
    }
    
}