using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] float playerSpeed;


    public void OnMove(InputAction.CallbackContext context) {
       var vel = context.ReadValue<Vector2>();
       rigidBody.velocity = new Vector3(vel.x * playerSpeed, vel.y * playerSpeed, 0);
    }

    public void OnLook(InputAction.CallbackContext context) { 
    }
}
