using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerStatus : MonoBehaviour
{
    public float health = 100;
    public RaycastHit2D groundHit_;
    [SerializeField]
    private float groundCheckDistance = .7f;
    public LayerMask groundLayermask;
    [HideInInspector]
    public bool grounded;
    private void Update()
    {
        groundHit_ = Physics2D.Raycast(this.transform.position, Vector2.down, groundCheckDistance, groundLayermask);
        grounded = groundHit_.transform;
    }
}
