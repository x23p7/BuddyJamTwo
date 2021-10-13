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

    [SerializeField]
    private string pickupTag = "Pickup";
    private void Update()
    {
        groundHit_ = Physics2D.Raycast(this.transform.position, Vector2.down, groundCheckDistance, groundLayermask);
        grounded = groundHit_.transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(pickupTag))
        {
            Pickup pickUp = other.GetComponent<Pickup>();
            pickUp.pickingEntity = this.gameObject;
            pickUp.onPickupEvent.Invoke();
        }
    }
}
