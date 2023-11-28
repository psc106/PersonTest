using UnityEngine;

public abstract class State : IState
{
    protected readonly PlayerController_RB player;
    protected readonly Animator animator;

    protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    protected static readonly int JumpHash = Animator.StringToHash("Jump");
    protected static readonly int AirHash = Animator.StringToHash("Air");
    protected static readonly int InteractHash = Animator.StringToHash("Interact");
    protected static readonly int UmbrellaHash = Animator.StringToHash("Umbrella");

    protected const float crossFadeDuration = 0.1f;

    protected State(PlayerController_RB player, Animator animator)
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

    }

    public virtual void Updatae()
    {//
    }
}
