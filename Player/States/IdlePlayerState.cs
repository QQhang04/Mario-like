using UnityEngine;

public class IdlePlayerState : PlayerState
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
        player.Friction();
        player.Spin();
        
        var inputDirection = player.inputs.GetMovementDirection();

        if (inputDirection.sqrMagnitude > 0 || player.lateralVelocity.sqrMagnitude > 0)
        {
            player.states.Change<WalkPlayerState>();
        }
    }
    
    public override void OnContact(Player player, Collider other)
    {
        player.PushRigidbody(other);
    }
}