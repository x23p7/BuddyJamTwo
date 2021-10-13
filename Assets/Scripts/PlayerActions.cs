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

    [SerializeField]
    private string shrinkzoneTag = "Shrinkzone";
    [SerializeField]
    private float shrinkScale = 0.1f;
    [SerializeField]
    private float shrinkIntentTime = 1f;
    [SerializeField]
    private float shrinkTime = .5f;
    private Vector3 originalScale = Vector3.one;
    [SerializeField]
    private float normalAttackStartup = .25f;
    [SerializeField]
    private float normalAttackActivetime = .05f;
    [SerializeField]
    private Hitbox normalAttackHitbox;
    [SerializeField]
    private Transform turnSwitchObjectParent;
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
        normalAttackHitbox.gameObject.SetActive(false);
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
        if (input_.action3KeyDown)
        {
            StartCoroutine(NormalAttack());
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
        if (Mathf.Abs(input_.horizontalInput) > 0)
        {
            turnSwitchObjectParent.localScale = Vector3.right * Mathf.Sign(input_.horizontalInput) + Vector3.up + Vector3.forward;
        }
        turnSwitchObjectParent.rotation = Quaternion.Euler(Vector3.zero);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log($"Tagcompare is {other.CompareTag(shrinkzoneTag)}, input is {input_.horizontalInput != 0} velocity is {playerRig.velocity.magnitude <= 0.1f}");
        if (other.CompareTag(shrinkzoneTag) && input_.horizontalInput != 0 && playerRig.velocity.magnitude <= 0.1f)
        {
            StartCoroutine(Shrink(shrinkIntentTime));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(shrinkzoneTag) && trans_.localScale != originalScale)
        {
            StartCoroutine(UnShrink());
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
        playerRig.velocity = Vector3.up * 5f;
        playerRig.AddForce(Vector3.right * Mathf.Sign(input_.horizontalInput) * dashForce, ForceMode2D.Impulse);
    }

    private IEnumerator Shrink(float shrinkIntentTime)
    {
        Debug.Log("Starting shrink check");
        float timer = 0;
        trans_.rotation = Quaternion.Euler(Vector3.zero);
        while (playerRig.velocity.magnitude <= 0.1f && input_.horizontalInput != 0 && timer <= shrinkIntentTime)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log($"One condition broke, timer is {timer}");
        if (timer >= shrinkIntentTime)
        {
            timer = 0;
            Vector3 startingScale = trans_.localScale.x * Vector3.right + trans_.localScale.y * Vector3.up + Vector3.forward;
            while (timer < shrinkTime)
            {
                trans_.localScale = Vector3.Lerp(startingScale, Vector3.right + Vector3.up * shrinkScale + Vector3.forward, timer / shrinkTime);
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            trans_.localScale = Vector3.right + Vector3.up * shrinkScale + Vector3.forward;
        }
    }

    private IEnumerator UnShrink()
    {
        float timer = 0;
        Vector3 startingScale = trans_.localScale.x * Vector3.right + trans_.localScale.y * Vector3.up + Vector3.forward;
        while (timer < shrinkTime)
        {
            trans_.localScale = Vector3.Lerp(startingScale, originalScale, timer / shrinkTime);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        trans_.localScale = originalScale;
    }

    private IEnumerator NormalAttack()
    {
        float timer = 0;
        while (timer < normalAttackStartup)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        normalAttackHitbox.gameObject.SetActive(true);
        timer = 0;
        while (timer < normalAttackActivetime)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        normalAttackHitbox.gameObject.SetActive(false);
    }
}
