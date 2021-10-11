using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerStatus),typeof(PlayerActions))]
public class PlayerMovement : MonoBehaviour
{
    private Inputmanager input_;
    private Rigidbody2D rig_;
    private Transform trans_;
    private PlayerStatus playerStatus_;
    private PlayerActions playerActions_;
    [SerializeField]
    private float jumpForce = 1000f;
    [SerializeField]
    private float runForce = 5000f;
    [SerializeField]
    private float maxVelocity = 5f;

    private bool jumpIntent = false;

    private RaycastHit2D hit_;
    // Start is called before the first frame update
    void OnEnable()
    {
        Initiate();
    }

    private void Update()
    {
        if (input_.jumpKeyDown && !jumpIntent)
        {
            Debug.Log($"Jumpbutton pressed, grounded is {playerStatus_.groundHit_.transform}");
            if (playerStatus_.grounded)
            {
                jumpIntent = true;
            }
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (jumpIntent && playerActions_.canJump)
        {
            Jump();
            jumpIntent = false;
        }
        if (playerStatus_.grounded)
        {
            if (rig_.velocity.magnitude < maxVelocity)
            {
                rig_.AddForce(Vector2.right * runForce * input_.horizontalInput, ForceMode2D.Force);
            }
        }
    }

    private void Initiate()
    {
        if (!input_)
        {
            input_ = Inputmanager.instance;
            if (!input_)
            {
                input_ = FindObjectOfType<Inputmanager>().Initiate();
                if (!input_)
                {
                    Debug.LogError($"Could not find an Inputmanager for {this.gameObject.name} please make sure one is loaded at any time");
                }
            }
        }

        if (!rig_)
        {
            rig_ = this.GetComponent<Rigidbody2D>();
        }

        if (!playerStatus_)
        {
            playerStatus_ = this.GetComponent<PlayerStatus>();
        }

        if (!trans_)
        {
            trans_ = this.transform;
        }
        if (!playerActions_)
        {
            playerActions_ = this.GetComponent<PlayerActions>();
        }
    }

    private void Jump()
    {
        rig_.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
