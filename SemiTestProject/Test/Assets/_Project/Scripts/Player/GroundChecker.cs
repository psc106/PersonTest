using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] float groundDistance = 0.08f;
    [SerializeField] LayerMask groundLayers;

    public bool IsGrounded { get; private set; }

    private void Update()
    {
        IsGrounded = Physics.SphereCast(transform.position+Vector3.up* groundDistance, groundDistance, Vector3.down, out _, groundDistance, groundLayers);
    }
}
