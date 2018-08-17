using System;
using UnityEngine;

[Serializable]
public class MouseLook
{
    [SerializeField] float xSensitivity = 2f;       // x sensitivity on camera movement
    [SerializeField] float ySensitivity = 2f;       // y sensitivity on camera movement
    [SerializeField] bool clampVerticalRotation = true;     // should we prevent camera from rotating above a certain amount?
    [SerializeField] float minimumX = -90F;         // minimum bounds of vertical camera rotation
    [SerializeField] float maximumX = 90F;          // maximum bounds of vertical camera rotation
    [SerializeField] bool smoothMovement = false;       // should we smooth camera rotation. Gives a 'lag' feel, but realistic
    [SerializeField] float smoothTime = 5f;         // if smooth is enabled, how much should we smooth?
    [SerializeField] bool lockCursor = true;        // should 
  
    // states
    private bool isCursorLocked = true;         // current state of cursor lock
    private Quaternion characterTargetRot;      // target rotation of character
    private Quaternion cameraTargetRot;         // target rotation of camera

    //Sets which character and camera to use
    public void Init(Transform _character, Transform _camera)
    {
        characterTargetRot = _character.localRotation;
        cameraTargetRot = _camera.localRotation;
    }

    //Control rotation for the given character and camera
    public void LookRotation(Transform _character, Transform _camera, Vector2 _rotateInput)
    {
        // multiply input with sensitivity to get the desired rotate amount
        float _yRot = _rotateInput.x * xSensitivity;
        float _xRot = _rotateInput.y * ySensitivity;

        // rotate by degrees on the euler
        characterTargetRot *= Quaternion.Euler (0f, _yRot, 0f);
        cameraTargetRot *= Quaternion.Euler (-_xRot, 0f, 0f);

        // if we're clamping vertical rotation, pass in the target rot and clamp it
        if(clampVerticalRotation)
            cameraTargetRot = ClampRotationAroundXAxis (cameraTargetRot);

        // if we're using smoothing, lerp between current rotation, desired rotation, using smooth amount per second
        if(smoothMovement)
        {
            _character.localRotation = Quaternion.Slerp (_character.localRotation, characterTargetRot,
                smoothTime * Time.deltaTime);
            _camera.localRotation = Quaternion.Slerp (_camera.localRotation, cameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        // otherwise just immediately rotate it
        else
        {
            _character.localRotation = characterTargetRot;
            _camera.localRotation = cameraTargetRot;
        }
        // handle our cursor lock
        UpdateCursorLock();
    }

    //Do all checking for managing whether or not the Cursor should be locked
    public void UpdateCursorLock()
    {
        //if the user set "lockCursor" we check & properly lock the cursos
        if (lockCursor)
            InternalLockUpdate();
    }

    //Enables or Disables the cursor. Useful cause we don't want our mouse to show in FPS games
    public void SetCursorLock(bool _value)
    {
        // assign the lockCursor to the received value
        lockCursor = _value;
        if (!lockCursor)
        {//we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    //Handle all potential cursor lock scenarios
    private void InternalLockUpdate()
    {
        // if we press Escape, disable
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            isCursorLocked = false;
        }
        // if we've clicked in the game, we're playing. Lock the cursor
        else if(Input.GetMouseButtonUp(0))
        {
            isCursorLocked = true;
        }

        // otherwise keep the cursor state the same!
        // if the cursor is locked, keep it locked and hidden
        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        // if the cursor is not locked, keep it unlocked and visible
        else if (!isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    //Limit rotation from going above a certain degrees. This prevents camera from twisting itself upside down
    Quaternion ClampRotationAroundXAxis(Quaternion _quaternion)
    {
        _quaternion.x /= _quaternion.w;
        _quaternion.y /= _quaternion.w;
        _quaternion.z /= _quaternion.w;
        _quaternion.w = 1.0f;

        // calculate our x angle using quaternions
        float _angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (_quaternion.x);
        // clamp our x angle between the min/max
        _angleX = Mathf.Clamp (_angleX, minimumX, maximumX);
        // reassign the x angle back into the quaternion
        _quaternion.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * _angleX);

        return _quaternion;
    }

}
