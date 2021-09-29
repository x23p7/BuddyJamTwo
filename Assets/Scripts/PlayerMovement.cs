using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerStatus))]
public class PlayerMovement : MonoBehaviour
{
    private Inputmanager input_;
    private Rigidbody2D rig_;
    private Transform trans_;
    private PlayerStatus playerStatus_;
    [SerializeField]
    private float jumpForce = 1000f;
    [SerializeField]
    private float runForce = 5000f;
    [SerializeField]
    private float maxVelocity = 5f;
    [SerializeField]
    private float hookshotRange = 8f;
    [SerializeField]
    private GameObject hook;

    private bool jumpIntent = false;
    private bool hookShotIntent = false;
    private bool hookOut = false;

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
        if (input_.action2KeyDown)
        {
            Debug.Log("Hookshot intent expressed");
            hookShotIntent = true;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (jumpIntent)
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
        if (hookShotIntent && !hookOut)
        {
            hookOut = true;
            StartCoroutine(HookShot((Camera.main.ScreenToWorldPoint(Input.mousePosition) - trans_.position).normalized));
            hookShotIntent = false;
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
        hook.SetActive(false);
    }

    private void Jump()
    {
        rig_.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private IEnumerator HookShot(Vector3 shotDirection)
    {
        Debug.Log("shooting hook");
        Debug.DrawRay(trans_.position, shotDirection * hookshotRange, Color.red, 5f);
        hit_ = Physics2D.Raycast(this.transform.position, shotDirection, hookshotRange, playerStatus_.groundLayermask);
        if (hit_.transform)
        {
            Debug.Log("Hook hit");
            rig_.velocity = Vector3.zero;
            rig_.bodyType = RigidbodyType2D.Kinematic;
            float passedTime = 0;
            Vector3 startPos = trans_.position;
            Vector3 targetPos = hit_.point.x * Vector3.right + hit_.point.y * Vector3.up + trans_.position.z * Vector3.forward;
            Transform hookTrans = hook.transform;
            hookTrans.position = trans_.position;
            hookTrans.rotation = Quaternion.LookRotation(shotDirection);
            hook.SetActive(true);
            while (passedTime < .1f)
            {
                hookTrans.position = Vector3.Lerp(startPos, targetPos, passedTime / .1f);
                passedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            passedTime = 0;
            targetPos = targetPos - shotDirection;
            while (passedTime < .5f)
            {
                trans_.position = Vector3.Lerp(startPos, targetPos, passedTime / .5f);
                passedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            while (input_.action1KeyHold)
            {
                yield return new WaitForEndOfFrame();
            }
            rig_.bodyType = RigidbodyType2D.Dynamic;
            hook.SetActive(false);
        }
        hookOut = false;
    }
}
