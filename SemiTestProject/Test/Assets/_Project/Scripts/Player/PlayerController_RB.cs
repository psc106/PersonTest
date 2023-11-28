using Cinemachine;
using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class PlayerController_RB: ValidatedMonoBehaviour
{

    [Header("References")]
    [SerializeField, Self] Rigidbody rb;
    [SerializeField, Self] Animator animator;
    [SerializeField, Self] TerrainChecker terrainChecker;
    [SerializeField, Anywhere] CinemachineFreeLook freeLookCam;
    [SerializeField, Anywhere] InputReader input;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] float smoothTime = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] float jumpSpeed = 10;
    [SerializeField] float jumpDuration = 0.5f;
    [SerializeField] float jumpCooldown = 0f;
    [SerializeField] float jumpMaxHeight = 2f;
    [SerializeField] float gravityMultiplier = 3f;


    Transform mainCam;

    Vector3 currentSpeed;
    Vector3 velocity;
    float jumpVelocity;
    bool umbState = false;

    Vector3 movement;
    StateMachine stateMachine;

    List<Timer> timers;
    CountDownTimer jumpTimer;
    CountDownTimer jumpCooldownTimer;

    static readonly int Forward = Animator.StringToHash("Forward");
    static readonly int Side = Animator.StringToHash("Side");

    private void Awake()
    {
        mainCam = Camera.main.transform;
        freeLookCam.Follow = transform;
        freeLookCam.LookAt = transform;

        freeLookCam.OnTargetObjectWarped(transform, transform.position - freeLookCam.transform.position - Vector3.forward);

        rb.freezeRotation = true;

        jumpTimer = new CountDownTimer(jumpDuration);
        jumpCooldownTimer = new CountDownTimer(jumpCooldown);
        timers = new List<Timer>(2) { jumpTimer, jumpCooldownTimer };

        jumpTimer.OnTimerStart += () => jumpVelocity = jumpSpeed;
        jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();

        stateMachine = new StateMachine();

        var locomotionState = new LocomotionState(this, animator);
        var jumpState = new JumpState(this, animator);
        var airState = new AirState(this, animator);
        var interactState = new InteractState(this, animator);
        var umbrellaState = new UmbrellaState(this, animator);

        At(locomotionState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));
        At(locomotionState, airState, new FuncPredicate(() => terrainChecker.IsAir));
        //At(locomotionState, interactState, new FuncPredicate(() => ));

        At(interactState, airState, new FuncPredicate(() => terrainChecker.IsAir));
        //At(interactState, locomotionState, new FuncPredicate(() => ));

        At(jumpState, locomotionState, new FuncPredicate(() => terrainChecker.IsGrounded && !jumpTimer.IsRunning));
        At(jumpState, airState, new FuncPredicate(() => terrainChecker.IsAir && !jumpTimer.IsRunning));

        At(airState, locomotionState, new FuncPredicate(() => terrainChecker.IsGrounded));
        At(airState, umbrellaState, new FuncPredicate(() => umbState ));

        At(umbrellaState, locomotionState, new FuncPredicate(() => terrainChecker.IsGrounded));
        At(umbrellaState, airState, new FuncPredicate(() => !umbState ));

        stateMachine.SetState(locomotionState);
    }

    void At(IState from, IState to, IPredicate condition) =>  stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    private void OnEnable()
    {
        input.Jump += OnJump;
    }

    private void OnDisable()
    {
        input.Jump -= OnJump;
    }

    void OnJump(bool performed)
    {
        if (!terrainChecker.IsAir && !umbState)
        {
            if(performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && terrainChecker.IsGrounded)
            {
                jumpTimer.Start();
            }
            else if(!performed && jumpTimer.IsRunning)
            {
                jumpTimer.Stop();
            }
        }
        else if (!performed && terrainChecker.IsAir)
        {
            if (umbState)
            {
                umbState = false;
            }
            else
            {
                umbState = true;
            }
        }
    }

    private void Start() => input.EnablePlayerActions();


    private void Update()
    {
        movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
        stateMachine.Update();

        HandleTimers();
        UpdateAnimator();
    }
    private void FixedUpdate()
    {
        Debug.Log(transform.position.y);
        stateMachine.FixedUpdate();
    }

    private void LateUpdate()
    {

    }

    void UpdateAnimator()
    {
        animator.SetFloat(Forward, currentSpeed.z);
        animator.SetFloat(Side, currentSpeed.x);
    }


    public void InitPlayer()
    {
        umbState = false;
        jumpTimer.Stop();
    }

    public void HandleJump()
    {

        if (umbState)
        {
            if (terrainChecker.IsWindZone)
            {
                jumpVelocity += 2 * Time.fixedDeltaTime;
                jumpVelocity = Mathf.Clamp(jumpVelocity, 0, 5);
            }
            else
            {
                jumpVelocity -= 2 * Time.fixedDeltaTime;
                jumpVelocity = Mathf.Clamp(jumpVelocity, -2, 5);
            }
        }
        else
        {
            if (!jumpTimer.IsRunning && terrainChecker.IsGrounded)
            {
                jumpVelocity = 0;
                return;
            }

            if (!jumpTimer.IsRunning)
            {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }

            if (terrainChecker.IsCeiling && jumpVelocity > 0)
            {
                jumpVelocity = 0;
            }
        }

        rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
    }
    public void HandleMovement()
    {
        Vector3 adjustDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
        if (adjustDirection.magnitude > 0f)
        {
            HandleHorizontalMovement(adjustDirection);
            SmoothSpeed(adjustDirection);
        }
        else
        {
            SmoothSpeed(Vector3.zero);
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    private void HandleHorizontalMovement(Vector3 adjustDirection)
    {
        Vector3 velocity = adjustDirection * moveSpeed * Time.fixedDeltaTime;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }
/*  private void HandleRotation(Vector3 adjustDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(adjustDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.LookAt(transform.position + adjustDirection);
    }*/

    private void HandleTimers()
    {
        foreach(var timer in timers)
        {
            timer.Tick(Time.deltaTime);
        }
    }
   

    void SmoothSpeed(Vector3 value)
    {
        currentSpeed = Vector3.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
    }
}
