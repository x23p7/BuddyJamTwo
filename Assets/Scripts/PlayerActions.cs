using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerStatus), typeof(PlayerMovement), typeof(Rigidbody2D))]
public class PlayerActions : MonoBehaviour
{
    Inputmanager input_;
    public bool canJump;
    public bool canDoubleJump;
    public bool canDash;
    [SerializeField]
    private float dashForce = 500f;
    private bool dashIntent = false;
    public bool canShrink;
    public bool canGrappleHook;
    public bool canAttack;

    private PlayerMovement playerMovementScript;
    private PlayerStatus playerStatusScript;
    private Rigidbody2D playerRig;

    private bool hookShotIntent = false;
    private bool hookOut = false;
    [SerializeField]
    private float hookshotRange = 8f;
    [SerializeField]
    private GameObject hook;

    private Transform trans_;
    private RaycastHit2D hit_;
    // Start is called before the first frame update
    void Start()
    {
        if (Inputmanager.instance)
        {
            input_ = Inputmanager.instance;
        }
        else
        {
            input_ = GameObject.FindObjectOfType<Inputmanager>();
            if (!input_)
            {
                Debug.LogError("No inputmanager found for PlayerActions. PlayerActions will NOT work! Please make sure there is an inputmanager loaded");
            }
            input_.Initiate();
        }
        playerMovementScript = this.GetComponent<PlayerMovement>();
        playerStatusScript = this.GetComponent<PlayerStatus>();
        playerRig = this.GetComponent<Rigidbody2D>();
        trans_ = this.transform;
        hook.SetActive(false);
    }
    private void Update()
    {
        if (input_.action2KeyDown)
        {
            Debug.Log("Hookshot intent expressed");
            hookShotIntent = true;
        }
        if (input_.action1KeyDown)
        {
            dashIntent = true;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (hookShotIntent && !hookOut && canGrappleHook)
        {
            hookOut = true;
            StartCoroutine(HookShot((Camera.main.ScreenToWorldPoint(Input.mousePosition) - trans_.position).normalized));
            hookShotIntent = false;
        }
        if (dashIntent && canDash)
        {
            Dash();
            dashIntent = false;
        }
    }

    private IEnumerator HookShot(Vector3 shotDirection)
    {
        hit_ = Physics2D.Raycast(this.transform.position, shotDirection, hookshotRange, playerStatusScript.groundLayermask);
        if (hit_.transform)
        {
            Debug.Log("Hook hit");
            playerRig.velocity = Vector3.zero;
            playerRig.bodyType = RigidbodyType2D.Kinematic;
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
            targetPos -= shotDirection;
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
            playerRig.bodyType = RigidbodyType2D.Dynamic;
            hook.SetActive(false);
        }
        hookOut = false;
    }

    private void Dash()
    {
        playerRig.velocity = Vector3.up*5f;
        playerRig.AddForce(Vector3.right * Mathf.Sign(input_.horizontalInput) * dashForce, ForceMode2D.Impulse);
    }
}
