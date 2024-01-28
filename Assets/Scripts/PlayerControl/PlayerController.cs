using System;
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
    [SerializeField]
    private Transform flashlight;
    [SerializeField]
    private Transform dickyTransform;

    private Vector2 localVelocity;
    private float gravity;
    private float scaledPlayerSpeed;
    private float scaledGravityForce;

    [Header("Pick Ups")]
    [SerializeField] private List<Tool> inventory = new List<Tool>() {};
    private int activeTool = 0;
    private bool flashlightOn = false;
    private bool hasFlashlight = false;
    private bool hasKnife = false;
    private bool hasKey = false;
    private Dictionary<string, int> dictionaryTools = new ()
    {
        ["hand"] = 0,
        ["knife"] = 1,
        ["flashlight"] = 2,
        ["key"] = 3
    };

    [Header("Interactables Detection")]
    private GameObject lastDetected;

    [Header("EXPOSED FOR DEBUG")]
    [SerializeField]
    private bool dead = false;

    [SerializeField]

    public void OnMove(InputAction.CallbackContext context) {
        if (dead) return;
        localVelocity = context.ReadValue<Vector2>() * scaledPlayerSpeed;
    }

    public void OnLook(InputAction.CallbackContext context) {
        if (dead) return;
        var delta = context.ReadValue<Vector2>();
        transform.rotation = Quaternion.Euler(transform.eulerAngles + (Vector3.up * delta.x * sensitivity));
    
        var cam_euler = cam.eulerAngles + (Vector3.right * -delta.y * sensitivity);
        if (cam_euler.x > 180) cam_euler.x -= 360;
        cam_euler.x = Mathf.Clamp(cam_euler.x, -90, 90);
        cam.rotation = Quaternion.Euler(cam_euler);
        
        flashlight.forward = cam.forward;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (dead) return;
        if(context.performed)
            inventory[activeTool].InteractAction(inventory[activeTool].type);
    }

    public void OnPickup(InputAction.CallbackContext context)
    {
        if (dead) return;
        if (!context.performed)
            return;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)).direction * 5);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData))
        {
            if (hitData.collider.tag == "Pickup")
            {
                hitData.collider.GetComponent<IPickupable>().Pickup();
                UIManager.Instance.UpdateSlots(hitData.collider.GetComponent<IPickupable>().GetName(), dictionaryTools[hitData.collider.GetComponent<IPickupable>().GetName()]);
                activeTool = dictionaryTools[hitData.collider.GetComponent<IPickupable>().GetName()];
            }
        }
    }

    public void OnInventory0(InputAction.CallbackContext context)
    {
        if (dead) return;
        if (!context.performed)
            return;
        activeTool = 0;
        UIManager.Instance.HighlightInventory(activeTool);
    }

    public void OnInventory1(InputAction.CallbackContext context)
    {
        if (dead) return;
        if (!context.performed)
            return;
        if (!hasKnife)
        {
            return;
        }
        activeTool = 1;
        UIManager.Instance.HighlightInventory(activeTool);
    }

    public void OnInventory2(InputAction.CallbackContext context)
    {
        if (dead) return;
        if (!context.performed)
            return;
        if (!hasFlashlight)
        {
            return;
        }
        activeTool = 2;
        UIManager.Instance.HighlightInventory(activeTool);
    }

    public void OnInventory3(InputAction.CallbackContext context)
    {
        if (dead) return;
        if (!context.performed)
            return;
        if (!hasKey)
        {
            return;
        }
        activeTool = 3;
        UIManager.Instance.HighlightInventory(activeTool);
    }


    private void Awake()
    {
        FlagSystem.OnFlagNotified += OnSpotlightSwitchedOn;
        FlagSystem.OnFlagNotified += OnUntieHands;
        FlagSystem.OnFlagNotified += OnPlayerDied;
        QualitySettings.vSyncCount = 1;
    }

    private void OnDestroy() {
        FlagSystem.OnFlagNotified -= OnSpotlightSwitchedOn;
        FlagSystem.OnFlagNotified -= OnUntieHands;
        FlagSystem.OnFlagNotified -= OnPlayerDied;
    }

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        scaledPlayerSpeed = 0;
        scaledGravityForce = gravityForce / 1000;

        UIManager.Instance.HighlightInventory(activeTool);
        flashlight.gameObject.SetActive(false);
    }

    void FixedUpdate() {
        if (dead) return;
        gravity = controller.isGrounded ? 0 : gravity - scaledGravityForce;

        controller.Move((transform.right * localVelocity.x) +
            (transform.forward * localVelocity.y) +
            (transform.up * gravity));
    }

    void Update() {
        DisplayInteractable();
        if (dead) return;
        // Applying the relative velocity
        // Head bobbing
        var cam_local_pos = cam.localPosition;
        if (localVelocity != Vector2.zero || Mathf.Abs(cam_local_pos.y - baseHeadHeight) > 0.01 ) {
            cam_local_pos.y = baseHeadHeight + headBobAmplitude * Mathf.Sin(headBobFrequency * Time.unscaledTime); 
        }
        cam.localPosition = cam_local_pos;

        // Ray trace for interactables
    }

    private void OnSpotlightSwitchedOn(object sender, FlagArgs flagArgs)
    {
        if (flagArgs.flag != PuzzleFlag.SWITCH) return;
    }

    private void OnUntieHands(object sender, FlagArgs flagArgs)
    {
        if (flagArgs.flag != PuzzleFlag.UNTIE) return;
        scaledPlayerSpeed = playerSpeed / 100;
    }

    private void OnPlayerDied(object sender, FlagArgs flagArgs) {
        if (flagArgs.flag != PuzzleFlag.PLAYER_DEATH) return;
        dead = true;
        StartCoroutine(PlayerDiedCoroutine());
    }

    private IEnumerator PlayerDiedCoroutine() {
        Debug.Log("Coroutine Started");
        Vector3 target = Vector3.Normalize(dickyTransform.position - cam.position);
        while (cam.forward != target) {
            Vector3 interpolatedRotation = Vector3.Lerp(cam.forward, target, 0.05f);
            cam.forward = interpolatedRotation; 
            yield return null; 
        }
    }

    public void EnableFlashlight()
    {
        hasFlashlight = true;
    }

    public void EnableKey()
    {
        hasKey = true;
    }

    public void EnableKnife()
    {
        hasKnife = true;
    }

    private void DisplayInteractable()
    {
        UIManager.Instance.DisplayInteractable("");
        if (lastDetected != null)
            lastDetected.GetComponent<Outline>().enabled = false;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)).direction);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData, 2f))
        {
            if (hitData.collider.tag == "Interactable" || hitData.collider.tag == "Pickup")
            {
                //UI Manager does stuff
                UIManager.Instance.DisplayInteractable(hitData.collider.name);
                hitData.collider.GetComponent<Outline>().enabled = true;
                lastDetected = hitData.collider.gameObject;
            }
        }
    }
}
