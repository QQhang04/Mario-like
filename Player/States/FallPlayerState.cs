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
        player.FaceDirectionSmooth(player.lateralVelocity);

        if (player.isGrounded)
        {
            player.states.Change<IdlePlayerState>();
        }
    }
}