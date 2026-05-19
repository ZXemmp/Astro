using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controla el movimiento del astronauta en tercera persona.
/// Usa CharacterController para colisiones limpias.
/// Soporta caminar, correr, saltar y gravedad.
/// Lee input tanto de teclado/gamepad como de MobileControls (joystick virtual).
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Salto y gravedad")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float groundedGravity = -2f;

    [Header("Detección de suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.25f;
    [SerializeField] private LayerMask groundMask;

    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool sprintHeld;
    private bool jumpPressed;
    private Vector3 velocity;
    private bool isGrounded;

    public bool IsGrounded => isGrounded;
    public Vector2 MoveInput => moveInput;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerInputActions();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled  += OnMove;
        inputActions.Player.Sprint.performed += ctx => sprintHeld = true;
        inputActions.Player.Sprint.canceled  += ctx => sprintHeld = false;
        inputActions.Player.Jump.performed   += ctx => jumpPressed = true;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled  -= OnMove;
        inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        // === Sumar input móvil si existe ===
        if (MobileControls.Instance != null && MobileControls.Instance.IsMobileMode)
        {
            // Si el joystick virtual está dando input, usarlo
            Vector2 mobileMove = MobileControls.Instance.MoveInput;
            if (mobileMove.sqrMagnitude > 0.01f)
                moveInput = mobileMove;

            // Si se pulsó el botón de salto en móvil
            if (MobileControls.Instance.JumpPressedThisFrame)
                jumpPressed = true;
        }

        CheckGround();
        HandleMovement();
        HandleJumpAndGravity();
    }

    private void CheckGround()
    {
        if (groundCheck == null) return;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void HandleMovement()
    {
        // Dirección relativa a la cámara
        Vector3 camForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
        Vector3 camRight   = cameraTransform != null ? cameraTransform.right   : Vector3.right;
        camForward.y = 0f; camRight.y = 0f;
        camForward.Normalize(); camRight.Normalize();

        Vector3 desiredDir = camForward * moveInput.y + camRight * moveInput.x;
        float speed = sprintHeld ? sprintSpeed : walkSpeed;

        controller.Move(desiredDir * speed * Time.deltaTime);

        // Rotar el modelo hacia la dirección de movimiento
        if (desiredDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot,
                                                  rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleJumpAndGravity()
    {
        if (isGrounded && velocity.y < 0f)
            velocity.y = groundedGravity;

        if (jumpPressed && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        jumpPressed = false;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}