using UnityEngine;

public class BrakePlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        
    }

    protected override void OnExit(Player player)
    {
        
    }

    protected override void OnStep(Player player)
    {
        // var inputDirection = player.inputs.GetMovementCameraDirection();
        //
        // if (player.stats.current.canBackflip 
        //     && Vector3.Dot(inputDirection, player.transform.forward) < 0)
        // {
        //     player.Backflip(player.stats.current.backflipBackwardTurnForce);
        // }
        // else
        // {
        //     if (player.lateralVelocity.sqrMagnitude == 0)
        //     {
        //         player.states.Change<IdlePlayerState>();
        //     }
        // }
        player.Jump();
        player.Fall();
        player.Decelerate();
        
        if (player.lateralVelocity.sqrMagnitude == 0)
        {
            player.states.Change<IdlePlayerState>();
        }
    }

    public override void OnContact(Player entity, Collider other) { }
}