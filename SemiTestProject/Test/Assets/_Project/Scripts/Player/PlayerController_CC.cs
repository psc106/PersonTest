using Cinemachine;
using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerController_CC : ValidatedMonoBehaviour
{

    [Header("References")]
    [SerializeField, Self] CharacterController controller;
    [SerializeField, Self] Animator animator;
    [SerializeField, Self] TerrainChecker groundChecker;
    [SerializeField, Anywhere] CinemachineFreeLook freeLookCam;
    [SerializeField, Anywhere] InputReader input;

    [Header("Settings")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] float smoothTime = 0.2f;

    Transform mainCam;

    float currentSpeed;
    float velocity;

    Vector3 movement;

    static readonly int Speed = Animator.StringToHash("Speed");

    private void Awake()
    {
        mainCam = Camera.main.transform;
        freeLookCam.Follow = transform;
        freeLookCam.LookAt = transform;

        freeLookCam.OnTargetObjectWarped(transform, transform.position - freeLookCam.transform.position - Vector3.forward);

       
    }
    private void Start() => input.EnablePlayerActions();


    private void Update()
    {
        movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
        HandleMovement();
        UpdateAnimator();
    }
    private void FixedUpdate()
    {
        // HandleJump();

    }

    void UpdateAnimator()
    {
        animator.SetFloat(Speed, currentSpeed);
    }

    void HandleMovement()
    {
        Vector3 movementDirection = new Vector3(input.Direction.x, 0f, input.Direction.y).normalized;

        Vector3 adjustDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movementDirection;
        if (adjustDirection.magnitude > 0f)
        {
            HandleRotation(adjustDirection);
            HandleCharacterController(adjustDirection);
            SmoothSpeed(adjustDirection.magnitude);
        }
        else
        {
            SmoothSpeed(0);

        }
    }

    private void HandleCharacterController(Vector3 adjustDirection)
    {
        var adjustMovement = adjustDirection * (moveSpeed * Time.deltaTime);
        controller.Move(adjustMovement);
    }

    private void HandleRotation(Vector3 adjustDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(adjustDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.LookAt(transform.position + adjustDirection);
    }

    void SmoothSpeed(float value)
    {
        currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
    }
}
