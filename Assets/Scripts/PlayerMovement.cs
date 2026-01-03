using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float gravity = -19.62f;
    
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 20f; 
    [SerializeField] private float staminaRegenRate = 15f; 
    [SerializeField] private float staminaRegenDelay = 1f; 
    
    [Header("Crouch")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    
    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isSprinting;
    
    // Stamina variables
    private float currentStamina;
    private float staminaRegenTimer;
    
    private float horizontalInput;
    private float verticalInput;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina;
        
        controller.height = standingHeight;
        
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
        
        // Debug info 
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
        Vector3 spherePosition = transform.position - new Vector3(0, controller.height / 2, 0);
        isGrounded = Physics.CheckSphere(spherePosition, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }
    }
    
    void HandleCrouch()
    {
        float targetHeight = isCrouching ? crouchHeight : standingHeight;

        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        controller.center = new Vector3(0, controller.height / 2, 0);
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
        moveDirection.Normalize(); 
        
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    void HandleStamina()
    {
        if (isSprinting && (horizontalInput != 0 || verticalInput != 0))
        {
            // Drain stamina while sprinting and moving
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(0, currentStamina);
            
            // Reset regen timer
            staminaRegenTimer = staminaRegenDelay;
            
            // Stop sprinting if stamina depleted
            if (currentStamina <= 0)
            {
                isSprinting = false;
            }
        }
        else
        {
            // Regenerate stamina after delay
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
    public bool IsCrouching() => isCrouching;
    public bool IsSprinting() => isSprinting;
    
    private void OnDrawGizmosSelected()
    {
        if (controller != null)
        {
            Gizmos.color = Color.red;
            Vector3 spherePosition = transform.position - new Vector3(0, controller.height / 2, 0);
            Gizmos.DrawWireSphere(spherePosition, groundCheckDistance);
        }
    }
}