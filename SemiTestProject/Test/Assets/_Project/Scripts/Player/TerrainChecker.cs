using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class TerrainChecker : MonoBehaviour
{
    [SerializeField] float groundDistance = 0.08f;
    [SerializeField] float ceilingDistance = 0.08f;
    [SerializeField] LayerMask groundLayers;
    [SerializeField] LayerMask windLayers;
    float height;
    RaycastHit hit;

    public bool IsGrounded { get; private set; }
    public bool IsCeiling { get; private set; }
    public bool IsSlope { get; private set; }
    public bool IsJump { get; private set; }
    public bool IsAir { get; private set; }

    public bool IsWindZone { get; private set; }


    private void Awake()
    {
        height = GetComponent<CapsuleCollider>().height;
    }

    private void Update()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, 0.2f, groundLayers) && hit.normal.y < 30;
        IsCeiling  = Physics.Raycast(transform.position + (Vector3.up * height * transform.localScale.y) - Vector3.up * 0.05f, Vector3.up, 0.1f, groundLayers);
        IsSlope    = IsGrounded && hit.normal.y > 0;
        IsJump     = !IsAir && !IsGrounded && Physics.Raycast(transform.position, Vector3.down, 7, groundLayers);
        IsAir      = !IsGrounded && !IsJump;


        IsWindZone = Physics.Raycast(transform.position, Vector3.down, out hit, 30, windLayers);

    }

}