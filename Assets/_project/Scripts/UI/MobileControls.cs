using UnityEngine;

/// <summary>
/// Singleton que gestiona los controles táctiles para móvil/WebGL.
/// Detecta automáticamente si la plataforma es táctil y activa/desactiva la UI.
/// Los scripts del Player y la cámara leen sus valores en lugar de usar
/// Input.GetAxis directamente.
/// </summary>
public class MobileControls : MonoBehaviour
{
    public static MobileControls Instance { get; private set; }

    [Header("UI de controles táctiles")]
    [SerializeField] private GameObject mobileUI;

    [Header("Forzar modo móvil (solo para pruebas en Editor)")]
    [SerializeField] private bool forceMobileMode = false;

    // Valores que leerán los scripts del Player y la cámara
    public Vector2 MoveInput { get; set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpPressedThisFrame { get; private set; }
    public bool InteractPressedThisFrame { get; private set; }

    // Variables internas para gestionar el "PressedThisFrame"
    private bool jumpQueued = false;
    private bool interactQueued = false;

    public bool IsMobileMode { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        DetectPlatform();
    }

    private void DetectPlatform()
    {
        // Detectar si estamos en plataforma táctil
        IsMobileMode = forceMobileMode
                    || Application.isMobilePlatform
                    || Input.touchSupported;

        if (mobileUI != null)
            mobileUI.SetActive(IsMobileMode);

        Debug.Log($"[MobileControls] Mobile mode: {IsMobileMode}");
    }

    private void Update()
    {
        // Convertir los "queued" en "PressedThisFrame" que se resetean tras un frame
        JumpPressedThisFrame = jumpQueued;
        InteractPressedThisFrame = interactQueued;
        jumpQueued = false;
        interactQueued = false;
    }

    // === Métodos públicos llamados desde los botones de UI ===

    public void SetLookInput(Vector2 delta)
    {
        LookInput = delta;
    }

    public void OnJumpButton()
    {
        jumpQueued = true;
    }

    public void OnInteractButton()
    {
        interactQueued = true;
    }
}