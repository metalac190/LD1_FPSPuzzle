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
