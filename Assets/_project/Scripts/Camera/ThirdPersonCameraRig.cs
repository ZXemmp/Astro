using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Rota el rig de cámara con el movimiento del mouse.
/// Este script va sobre el GameObject 'CameraRig' (hijo del Player).
/// El rig se mueve con el Player automáticamente al ser hijo de él,
/// pero la rotación con el mouse la hace este script.
/// La Cinemachine Camera apunta a 'CameraFollowTarget', que es hijo del rig,
/// así que cuando el rig rota, la cámara orbita.
/// </summary>
public class ThirdPersonCameraRig : MonoBehaviour
{
    [Header("Sensibilidad")]
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 70f;

    [Header("Cursor")]
    [SerializeField] private bool lockCursor = true;

    private PlayerInputActions inputActions;
    private Vector2 lookInput;
    private float yaw;
    private float pitch;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled  += OnLook;

        // En móvil NO bloqueamos el cursor (no aplica)
        bool isMobile = MobileControls.Instance != null && MobileControls.Instance.IsMobileMode;
        if (lockCursor && !isMobile)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnDisable()
    {
        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Look.canceled  -= OnLook;
        inputActions.Player.Disable();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        // Si estamos en móvil y hay input de arrastre, usarlo (sobrescribe mouse)
        Vector2 effectiveLook = lookInput;
        if (MobileControls.Instance != null && MobileControls.Instance.IsMobileMode)
        {
            Vector2 mobileLook = MobileControls.Instance.LookInput;
            if (mobileLook.sqrMagnitude > 0.01f)
                effectiveLook = mobileLook;
        }

        yaw   += effectiveLook.x * mouseSensitivity;
        pitch -= effectiveLook.y * mouseSensitivity;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}