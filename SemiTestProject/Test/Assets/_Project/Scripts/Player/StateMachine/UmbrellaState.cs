using UnityEngine;

public class UmbrellaState : State
{
    public UmbrellaState(PlayerController_RB player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        animator.CrossFade(UmbrellaHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        player.HandleMovement();
        player.HandleJump();
    }

}
