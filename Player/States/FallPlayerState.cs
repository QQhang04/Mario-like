using UnityEngine;

public class FallPlayerState : PlayerState {
    protected override void OnEnter(Player player)
    {
        
    }

    protected override void OnExit(Player player)
    {
        
    }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.SnapToGround();
        player.FaceDirectionSmooth(player.lateralVelocity);
        player.AccelerateToInputDirection();
        player.Spin();
        player.PickAndThrow();
        player.StompAttack();

        if (player.isGrounded)
        {
            player.states.Change<IdlePlayerState>();
        }
    }

    public override void OnContact(Player player, Collider other)
    {
        player.PushRigidbody(other);
    }
}