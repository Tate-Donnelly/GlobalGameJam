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
    private float scaledPlayerSpeed;
    private float scaledGravityForce;

    private Tool[] inventory = { new Tool("Hand"), new Tool("Hand"), new Tool("Hand"), new Tool("Hand")};
    private int activeTool = 0;

    public void OnMove(InputAction.CallbackContext context) {
        localVelocity = context.ReadValue<Vector2>() * scaledPlayerSpeed;
    }

    public void OnLook(InputAction.CallbackContext context) {
        var delta = context.ReadValue<Vector2>();
        transform.rotation = Quaternion.Euler(transform.eulerAngles + (Vector3.up * delta.x * sensitivity));
    
        var cam_euler = cam.eulerAngles + (Vector3.right * -delta.y * sensitivity);
        if (cam_euler.x > 180) cam_euler.x -= 360;
        cam_euler.x = Mathf.Clamp(cam_euler.x, -90, 90);
        cam.rotation = Quaternion.Euler(cam_euler);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Ray ray = new Ray(cam.transform.position, Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)).direction * 20);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData))
        {
            if(hitData.collider.tag == "Interactable")
            {
                hitData.collider.GetComponent<IInteractable>().InteractAction();
            }
        }
    }

    public void OnInventory0(InputAction.CallbackContext context)
    {
        activeTool = 0;
        UIManager.Instance.HighlightInventory(activeTool);
    }

    public void OnInventory1(InputAction.CallbackContext context)
    {
        activeTool = 1;
        UIManager.Instance.HighlightInventory(activeTool);
    }

    public void OnInventory2(InputAction.CallbackContext context)
    {
        activeTool = 2;
        UIManager.Instance.HighlightInventory(activeTool);
    }

    public void OnInventory3(InputAction.CallbackContext context)
    {
        activeTool = 3;
        UIManager.Instance.HighlightInventory(activeTool);
    }

    private void Awake()
    {
        FlagSystem.OnFlagNotified += OnSpotlightSwitchedOn;
        QualitySettings.vSyncCount = 1;
    }

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        scaledPlayerSpeed = playerSpeed/100;
        scaledGravityForce = gravityForce / 1000;

        UIManager.Instance.HighlightInventory(activeTool);
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
