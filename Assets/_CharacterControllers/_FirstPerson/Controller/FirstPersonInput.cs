using UnityEngine;

[RequireComponent(typeof(FirstPersonInput))]
[RequireComponent(typeof(Player))]
public class FirstPersonInput : MonoBehaviour {

    // states
    public bool JumpInput { get; private set; }   // boolean for whether or not we've received jump input

    FirstPersonMotor firstPersonMotor;
    //Player player;

    private void Awake()
    {
        // caching
        firstPersonMotor = GetComponent<FirstPersonMotor>();
        //player = GetComponent<Player>();
    }

    private void Update()
    {
        ProcessMoveInput();
        ProcessRotateInput();
        ProcessRunningInput();
        ProcessJumpInput();
    }

    //Calculate and send out Movement Input to the FPS Controller
    void ProcessMoveInput()
    {
        // store x/y move input as a 0 to 1 value
        float horizontalAmount = Input.GetAxis("Horizontal");
        float verticalAmount = Input.GetAxis("Vertical");

        // store x/y into a single vector
        Vector2 input = new Vector2(horizontalAmount, verticalAmount);

        // normalize it
        if (input.sqrMagnitude > 1)
        {
            input.Normalize();
        }

        // send the input to the FPS Controller to process
        firstPersonMotor.SetMoveInput(input);
    }

    //Calculate and send out Rotation Input to the FPS Controller
    void ProcessRotateInput()
    {
        // calculate mouse input
        float xInput = Input.GetAxis("Mouse X");
        float yInput = Input.GetAxis("Mouse Y");
        // store x/y into a single vector
        Vector2 rotateInput = new Vector2(xInput, yInput);

        // normalize it
        if (rotateInput.sqrMagnitude > 1)
        {
            rotateInput.Normalize();
        }

        // send the input to the FPS Controller to process
        firstPersonMotor.SetRotateInput(rotateInput);
    }

    //Send out Running Input to the FPS Controller
    void ProcessRunningInput()
    {
        // if we're pressing the run button, we're receiving input. Otherwise we're not
        if (Input.GetKey(KeyCode.LeftShift))
        {
            firstPersonMotor.SetRunInput(true);
        }
        else
        {
            firstPersonMotor.SetRunInput(false);
        }
    }

    //Send out Jumping Input to the FPS Controller
    void ProcessJumpInput()
    {
        // if we're pressing the jump button, we're receiving input. Otherwise we're not
        if (Input.GetButtonDown("Jump"))
        {
            firstPersonMotor.SetJumpInput(true);
        } else
        {
            firstPersonMotor.SetJumpInput(false);
        }
    }
}
