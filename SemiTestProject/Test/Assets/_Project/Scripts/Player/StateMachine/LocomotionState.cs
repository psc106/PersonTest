using UnityEngine;

public class LocomotionState : State
{
    public LocomotionState(PlayerController_RB player, Animator animator ): base( player, animator )
    {

    }

    public override void OnEnter()
    {
        player.InitPlayer();
        animator.CrossFade(LocomotionHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        player.HandleMovement();
    }
}

