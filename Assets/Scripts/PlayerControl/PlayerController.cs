using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Params")]
    [SerializeField] float playerSpeed;
    [SerializeField] float sensitivity;
    [SerializeField] float baseHeadHeight;
    [SerializeField] float headBobAmplitude;
    [SerializeField] float headBobFrequency;

    [Header("Player Parts")]
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] Transform cam;    

    Vector2 localVelocity;

    public void OnMove(InputAction.CallbackContext context) {
        localVelocity = context.ReadValue<Vector2>() * playerSpeed;
    }

    public void OnLook(InputAction.CallbackContext context) {
        var delta = context.ReadValue<Vector2>();
        transform.rotation = Quaternion.Euler(transform.eulerAngles + (Vector3.up * delta.x * sensitivity));
    
        var cam_euler = cam.eulerAngles + (Vector3.right * -delta.y * sensitivity);
        if (cam_euler.x > 180) cam_euler.x -= 360;
        cam_euler.x = Mathf.Clamp(cam_euler.x, -90, 90);
        cam.rotation = Quaternion.Euler(cam_euler);
    }

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        // Applying the relative velocity
        rigidBody.velocity = (transform.right * localVelocity.x) + (transform.forward * localVelocity.y); 

        // Head bobbing
        var cam_local_pos = cam.localPosition;
        if (localVelocity != Vector2.zero || Mathf.Abs(cam_local_pos.y - baseHeadHeight) > 0.01 ) {
            cam_local_pos.y = baseHeadHeight + headBobAmplitude * Mathf.Sin(headBobFrequency * Time.unscaledTime); 
        }
        cam.localPosition = cam_local_pos;
    }
}
