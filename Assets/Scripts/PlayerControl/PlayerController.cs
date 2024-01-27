using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Params")]
    [SerializeField] 
    private float playerSpeed;
    
    [SerializeField] 
    private float sensitivity;
    
    [SerializeField] 
    private float baseHeadHeight;
    
    [SerializeField] 
    private float headBobAmplitude;
    
    [SerializeField] 
    float headBobFrequency;

    [SerializeField]
    private float gravityForce;

    [Header("Player Parts")]
    [SerializeField] 
    private CharacterController controller;
    
    [SerializeField] 
    private Transform cam;    

    private Vector2 localVelocity;
    private float gravity;

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
        gravity = controller.isGrounded ? 0 : gravity - gravityForce * Time.smoothDeltaTime;
        controller.Move((transform.right * localVelocity.x) + 
                        (transform.forward * localVelocity.y) + 
                        (transform.up * gravity));

        // Head bobbing
        var cam_local_pos = cam.localPosition;
        if (localVelocity != Vector2.zero || Mathf.Abs(cam_local_pos.y - baseHeadHeight) > 0.01 ) {
            cam_local_pos.y = baseHeadHeight + headBobAmplitude * Mathf.Sin(headBobFrequency * Time.unscaledTime); 
        }
        cam.localPosition = cam_local_pos;
    }
}
