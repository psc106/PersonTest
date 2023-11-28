using UnityEngine;

public class InteractState : State
{
    public InteractState(PlayerController_RB player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        animator.CrossFade(InteractHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
    }
}
