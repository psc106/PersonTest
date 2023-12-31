﻿using UnityEngine;

public class JumpState : State 
{
    public JumpState(PlayerController_RB player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        animator.CrossFade(JumpHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        player.HandleMovement();
        player.HandleJump();
    }
}