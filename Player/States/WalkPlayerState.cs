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
        player.SnapToGround();
        player.Spin();
        player.PickAndThrow();
        
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
            player.states.Change<BrakePlayerState>();
        }
        
        if (player.inputs.GetCrouchAndCraw())
        {
            player.states.Change<CrouchPlayerState>();
        }
    }
    
    public override void OnContact(Player player, Collider other)
    {
        player.PushRigidbody(other);
    }
}