using UnityEngine;

public class JumpState : BaseState 
{
    public JumpState(PlayerController_RB player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        Debug.Log("enter");
        animator.CrossFade(JumpHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        player.HandleMovement();
        player.HandleJump();
    }
}