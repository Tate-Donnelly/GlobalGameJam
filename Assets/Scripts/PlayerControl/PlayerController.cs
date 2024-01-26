using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Params")]
    [SerializeField] float playerSpeed;
    [SerializeField] float sensitivity;

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
       rigidBody.velocity = (transform.right * localVelocity.x) + (transform.forward * localVelocity.y);
    }
}
