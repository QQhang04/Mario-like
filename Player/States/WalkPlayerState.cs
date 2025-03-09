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
        var inputDirection = player.inputs.GetMovementCameraDirection();

        if (inputDirection.sqrMagnitude > 0)
        {
            var dot = Vector3.Dot(player.lateralVelocity, inputDirection);
            if (dot >= player.stats.current.brakeThreshold)
            {
                player.Accelerate(inputDirection);
            }
        }
    }
}