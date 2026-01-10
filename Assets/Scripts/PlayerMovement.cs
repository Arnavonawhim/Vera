using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stamina UI")]
    public Slider staminaSlider;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float gravity = -19.62f;
    
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 20f;
    [SerializeField] private float staminaRegenRate = 15f;
    [SerializeField] private float staminaRegenDelay = 1f;
    
    
    [Header("Crouch Settings")]
    [SerializeField] private float standingHeight = 1f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float standingCameraHeight = 0.6f;
    [SerializeField] private float crouchCameraHeight = 0.1f;
    
    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundMask;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isSprinting;
    private float currentStamina;
    private float staminaRegenTimer;
    private float horizontalInput;
    private float verticalInput;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (controller == null)
        {
            Debug.LogError("CharacterController component not found on " + gameObject.name);
            enabled = false;
            return;
        }
        
        if (cameraHolder == null)
        {
            cameraHolder = transform.Find("CameraHolder");
            if (cameraHolder == null)
            {
                Debug.LogError("CameraHolder not found! Please assign it in the inspector or create a child object named 'CameraHolder'");
            }
        }
        
        currentStamina = maxStamina;
        controller.height = standingHeight;
        controller.center = new Vector3(0, standingHeight * 0.5f, 0);
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
        
        if (cameraHolder != null)
        {
            cameraHolder.localPosition = new Vector3(0, standingCameraHeight, 0);
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        HandleInput();
        HandleGroundCheck();
        HandleCrouch();
        HandleMovement();
        HandleStamina();
        staminaSlider.value = currentStamina;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
        }
        
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && currentStamina > 0)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }
    
    void HandleGroundCheck()
    {
        Vector3 rayStart = transform.position + new Vector3(0, controller.radius, 0);
        isGrounded = Physics.Raycast(rayStart, Vector3.down, controller.radius + groundCheckDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
    
    void HandleCrouch()
    {
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        float targetCameraHeight = isCrouching ? crouchCameraHeight : standingCameraHeight;
        
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        float targetCenter = isCrouching ? crouchHeight * 0.5f : standingHeight * 0.5f;
        controller.center = new Vector3(0, targetCenter, 0);

        
        if (cameraHolder != null)
        {
            Vector3 currentCameraPos = cameraHolder.localPosition;
            currentCameraPos.y = Mathf.Lerp(currentCameraPos.y, targetCameraHeight, Time.deltaTime * crouchTransitionSpeed);
            cameraHolder.localPosition = currentCameraPos;
        }
    }
    
    void HandleMovement()
    {
        float currentSpeed = walkSpeed;
        
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        
        Vector3 moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;
        
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }
        
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    void HandleStamina()
    {
        bool isMoving = horizontalInput != 0 || verticalInput != 0;
        
        if (isSprinting && isMoving)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(0, currentStamina);
            staminaRegenTimer = staminaRegenDelay;
            
            if (currentStamina <= 0)
            {
                isSprinting = false;
            }
        }
        else
        {
            if (staminaRegenTimer > 0)
            {
                staminaRegenTimer -= Time.deltaTime;
            }
            else
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(maxStamina, currentStamina);
            }
        }
    }
    
    public float GetCurrentStamina() => currentStamina;
    public float GetMaxStamina() => maxStamina;
    public float GetStaminaPercentage() => currentStamina / maxStamina;
    public bool IsCrouching() => isCrouching;
    public bool IsSprinting() => isSprinting;
    
    private void OnDrawGizmosSelected()
    {
        if (controller == null) return;
        
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayStart = transform.position + new Vector3(0, controller.radius, 0);
        Gizmos.DrawRay(rayStart, Vector3.down * (controller.radius + groundCheckDistance));
        
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + controller.center;
        Gizmos.DrawWireSphere(center, controller.radius);
        
        if (cameraHolder != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(cameraHolder.position, 0.1f);
        }
    }
}