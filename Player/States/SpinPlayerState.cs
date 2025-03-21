using UnityEditor.VersionControl;
using UnityEngine;

public class SpinPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        if (!player.isGrounded)
        {
            
        }
    }

    protected override void OnExit(Player player)
    {
        
    }

    protected override void OnStep(Player player)
    {
       player.Gravity();
       player.SnapToGround();
       player.AccelerateToInputDirection();

       if (timeSinceEntered >= player.stats.current.spinDuration)
       {
           if (player.isGrounded)
           {
               player.states.Change<IdlePlayerState>();
           }
           else
           {
               player.states.Change<FallPlayerState>();
           }
       }
    }
    
    public override void OnContact(Player player, Collider other)
    {

    }
}