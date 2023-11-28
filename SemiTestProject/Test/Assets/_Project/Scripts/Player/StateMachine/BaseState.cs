using UnityEngine;

public abstract class BaseState : IState
{
    protected readonly PlayerController_RB player;
    protected readonly Animator animator;

    protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    protected static readonly int JumpHash = Animator.StringToHash("Jump");

    protected const float crossFadeDuration = 0.1f;

    protected BaseState(PlayerController_RB player, Animator animator)
    {
        this.player = player;
        this.animator = animator;
    }

    public virtual void FixedUpdate()
    {//
    }

    public virtual void OnEnter()
    {//
    }

    public virtual void OnExit()
    {

        Debug.Log("exits");
    }

    public virtual void Updatae()
    {//
    }
}
