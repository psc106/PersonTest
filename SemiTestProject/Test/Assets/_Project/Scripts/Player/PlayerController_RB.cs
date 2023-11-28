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
    [SerializeField, Self] GroundChecker groundChecker;
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

    float currentSpeed;
    float velocity;
    float jumpVelocity;

    Vector3 movement;
    StateMachine stateMachine;

    List<Timer> timers;
    CountDownTimer jumpTimer;
    CountDownTimer jumpCooldownTimer;

    static readonly int Speed = Animator.StringToHash("Speed");

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
        var JumpState = new JumpState(this, animator);

        At(locomotionState, JumpState, new FuncPredicate(() => jumpTimer.IsRunning));
        At(JumpState, locomotionState, new FuncPredicate(() => groundChecker.IsGrounded && !jumpTimer.IsRunning));

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
        if(performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.IsGrounded)
        {
            jumpTimer.Start();
        }
        else if(!performed && jumpTimer.IsRunning)
        {
            jumpTimer.Stop();
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
        stateMachine.FixedUpdate();

    }

    void UpdateAnimator()
    {
        animator.SetFloat(Speed, currentSpeed);
    }

    public void HandleMovement()
    {

        Vector3 adjustDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
        if (adjustDirection.magnitude > 0f)
        {
            HandleRotation(adjustDirection);
            HandleHorizontalMovement(adjustDirection);
            SmoothSpeed(adjustDirection.magnitude);
        }
        else
        {
            SmoothSpeed(0);

            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    private void HandleHorizontalMovement(Vector3 adjustDirection)
    {
        Vector3 velocity = adjustDirection * moveSpeed * Time.fixedDeltaTime;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }

    private void HandleRotation(Vector3 adjustDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(adjustDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.LookAt(transform.position + adjustDirection);
    }

    private void HandleTimers()
    {
        foreach(var timer in timers)
        {
            timer.Tick(Time.deltaTime);
        }
    }
    public void HandleJump()
    {
        if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
        {
            jumpVelocity = 0;
            return;
        }

        if (!jumpTimer.IsRunning)
        {
            jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
           /* float launchPoint = 0.9f;
            if(jumpTimer.Progress > launchPoint)
            {
                //v = sqrt(2gh) v 속도 g 그래비티 h 높이
                jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
            }
            else
            {
                jumpVelocity += (1 - jumpTimer.Progress) * jumpSpeed * Time.fixedDeltaTime;
            }
        }
        else
        {
            jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;*/
        }

        rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
    }

    void SmoothSpeed(float value)
    {
        currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
    }
}
