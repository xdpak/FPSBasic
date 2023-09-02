using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Camera for player view")]
    public Camera playerCamera;

    private float cameraRotationX = 0.0f;
    private float cameraRotationY = 0.0f;

    // The input system
    private PlayerInput playerInputActions;
    private PlayerInput.OnFootActions onFootActions;

    //[Header("Camera position offsets from player")]
    private float cameraOffsetX = 0.0f;
    private float cameraOffsetY = 0.25f;
    private float cameraOffsetZ = 0.0f;

    [Header("Player speed")]
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 12.0f;
    public bool isWalkingSpeed = true;
    private float currentPlayerSpeed;

    [Header("Mouse Settings")]
    public float mouseSensitivityX = 10.0f;
    public float mouseSensitivityY = 10.0f;
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    private void Awake()
    {
        playerInputActions = new PlayerInput();
        onFootActions = playerInputActions.OnFoot;

        onFootActions.Fire.performed += ctx => Fire();
        onFootActions.Sprint.performed += ctx => SprintToggle();

        // If no camera specified, use the first "main camera" found
        if (playerCamera == null)
        {
            Debug.LogWarning("No camera specified in 'PlayerController'. Attempting to use 'main camera'.");
            playerCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        onFootActions.Enable();
    }

    private void OnDisable()
    {
        onFootActions.Disable();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    void Start()
    {
        Debug.Log("PlayerController Start");

        SetCameraPosition();
        SetPlayerSpeed();

        // Initialise camera rotation so it matches the player rotation
        var playerRotation = transform.rotation;
        cameraRotationX = playerRotation.x;
        cameraRotationY = playerRotation.y;
        playerCamera.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, playerRotation.z);
    }

    void Update()
    {
        Move();
        Look();
    }

    private void Move()
    {
        var moveInput = onFootActions.Movement.ReadValue<Vector2>();

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = moveInput.x;
        moveDirection.z = moveInput.y;

        transform.Translate(moveDirection * Time.deltaTime * currentPlayerSpeed);

        // Move the camera to the player position
        SetCameraPosition();
    }

    private void Look()
    {
        var lookInput = onFootActions.Look.ReadValue<Vector2>();
        float inputX = lookInput.x;
        float inputY = lookInput.y;

        // Calculate camera rotation for looking up and down
        cameraRotationX -= inputY * Time.deltaTime * mouseSensitivityX;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);

        // Calculate camera rotation for looking left and right
        float yRotateAmount = inputX * Time.deltaTime * mouseSensitivityY;
        cameraRotationY += yRotateAmount;

        // Apply the rotations to the camera transform
        playerCamera.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0.0f);

        // Rotate player to look left or right (so the player and the camera are turned in the same direction)
        transform.Rotate(Vector3.up * yRotateAmount);
    }
   
    private void Fire()
    {
        Debug.Log("Fire");
    }
    
    private void SprintToggle()
    {
        Debug.Log("Sprint (toggle)");

        isWalkingSpeed = !isWalkingSpeed;
        SetPlayerSpeed();
    }

    /// <summary>
    /// Set the camera position to the player position (with an offset if applicable).
    /// </summary>
    private void SetCameraPosition()
    {
        var playerPosition = transform.position;
        var cameraPositionX = playerPosition.x + cameraOffsetX;
        var cameraPositionY = playerPosition.y + cameraOffsetY;
        var cameraPositionZ = playerPosition.z + cameraOffsetZ;
        playerCamera.transform.position = new Vector3(cameraPositionX, cameraPositionY, cameraPositionZ);
    }

    private void SetPlayerSpeed()
    {
        currentPlayerSpeed = isWalkingSpeed ? walkSpeed : sprintSpeed;
    }
}
