using UnityEngine;

public class WalkPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {

    }

    protected override void OnExit(Player player)
    {

    }

    protected override void OnStep(Player player)
    {
        player.Jump();
        player.Gravity();
        player.Fall();
        
        var inputDirection = player.inputs.GetMovementCameraDirection();

        if (inputDirection.sqrMagnitude > 0)
        {
            var dot = Vector3.Dot(player.lateralVelocity, inputDirection);
            if (dot >= player.stats.current.brakeThreshold)
            {
                player.Accelerate(inputDirection);
                player.FaceDirectionSmooth(player.lateralVelocity);
            }
            else
            {
                player.states.Change<BrakePlayerState>();
            }
        }
        else
        {
            player.Friction();
            if (player.lateralVelocity.sqrMagnitude <= 0)
            {
                player.states.Change<IdlePlayerState>();
            }
        }
    }
    
    public override void OnContact(Player player, Collider other)
    {
        player.PushRigidbody(other);
    }
}