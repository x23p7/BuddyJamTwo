using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputmanager : MonoBehaviour
{
    public static Inputmanager instance;
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;
    [SerializeField]
    private KeyCode actionKey1 = KeyCode.LeftShift;
    [SerializeField]
    private KeyCode actionKey2 = KeyCode.Mouse0;
    [SerializeField]
    private KeyCode actionKey3 = KeyCode.Mouse1;

    public bool initiated = false;
    public float horizontalInput = 0f;
    public float verticalInput = 0f;

    public bool jumpKeyDown = false;
    public bool jumpKeyHold = false;
    public bool jumpKeyUp = false;

    public bool action1KeyDown = false;
    public bool action1KeyHold = false;
    public bool action1KeyUp = false;

    public bool action2KeyDown = false;
    public bool action2KeyHold = false;
    public bool action2KeyUp = false;

    public bool action3KeyDown = false;
    public bool action3KeyHold = false;
    public bool action3KeyUp = false;
    private void Awake()
    {
        if (!initiated)
        {
            Initiate();
        }
    }
    public Inputmanager Initiate()
    {
        if (instance != null)
        {
            if (instance != this)
            {

                Destroy(this);
                return instance;
            }
        }
        else
        {
            instance = this;
            return instance;
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        jumpKeyDown = Input.GetKeyDown(jumpKey);
        jumpKeyHold = Input.GetKey(jumpKey);
        jumpKeyUp = Input.GetKeyUp(jumpKey);

        action1KeyDown = Input.GetKeyDown(actionKey1);
        action1KeyHold = Input.GetKey(actionKey1);
        action1KeyUp = Input.GetKeyUp(actionKey1);

        action2KeyDown = Input.GetKeyDown(actionKey2);
        action2KeyHold = Input.GetKey(actionKey2);
        action2KeyUp = Input.GetKeyUp(actionKey2);

        action3KeyDown = Input.GetKeyDown(actionKey3);
        action3KeyHold = Input.GetKey(actionKey3);
        action3KeyUp = Input.GetKeyUp(actionKey3);
    }
}
