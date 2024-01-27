using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float scaledPlayerSpeed;
    private float scaledGravityForce;

    private void Awake()
    {
        FlagSystem.OnFlagNotified += OnSpotlightSwitchedOn;
        QualitySettings.vSyncCount = 1;
    }

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        scaledPlayerSpeed = playerSpeed/100;
        scaledGravityForce = gravityForce / 1000;
    }

    void FixedUpdate() {
        gravity = controller.isGrounded ? 0 : gravity - scaledGravityForce;

        controller.Move((transform.right * localVelocity.x) +
            (transform.forward * localVelocity.y) +
            (transform.up * gravity));
    }

    void Update() {
        // Applying the relative velocity
        // Head bobbing
        var cam_local_pos = cam.localPosition;
        if (localVelocity != Vector2.zero || Mathf.Abs(cam_local_pos.y - baseHeadHeight) > 0.01 ) {
            cam_local_pos.y = baseHeadHeight + headBobAmplitude * Mathf.Sin(headBobFrequency * Time.unscaledTime); 
        }
        cam.localPosition = cam_local_pos;


    }

    private void OnSpotlightSwitchedOn(object sender, FlagArgs flagArgs)
    {
        if (flagArgs.flag != PuzzleFlag.SWITCH) return;
        scaledPlayerSpeed = playerSpeed / 100;
    }
}
