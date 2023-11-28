using UnityEngine;

public class AirState : State
{
    public AirState(PlayerController_RB player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        animator.CrossFade(AirHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        player.HandleMovement();
        player.HandleJump();
    }
}
