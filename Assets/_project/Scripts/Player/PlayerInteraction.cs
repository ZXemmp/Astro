using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Detecta objetos interactuables al frente del jugador
/// usando un SphereCast (un rayo "gordito").
/// Cuando hay uno detectado y el jugador pulsa E, se ejecuta su acción.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Detección")]
    [SerializeField] private float interactRange = 2.5f;
    [SerializeField] private float interactRadius = 0.5f;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private Transform rayOrigin;

    [Header("UI")]
    [SerializeField] private DialogueUI dialogueUI;

    private PlayerInputActions inputActions;
    private IInteractable currentTarget;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        if (rayOrigin == null) rayOrigin = transform;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        inputActions.Player.Interact.performed -= OnInteract;
        inputActions.Player.Disable();
    }

    private void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        Vector3 origin = rayOrigin.position + Vector3.up * 1f;
        Vector3 direction = transform.forward; // usamos hacia donde mira el Player

        if (Physics.SphereCast(origin, interactRadius, direction,
                               out RaycastHit hit, interactRange, interactableMask))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract())
            {
                if (currentTarget != interactable)
                {
                    currentTarget = interactable;
                    if (dialogueUI != null)
                        dialogueUI.ShowPrompt(interactable.GetPrompt());
                }
                return;
            }
        }

        // No hay nada interactuable al frente
        if (currentTarget != null)
        {
            currentTarget = null;
            if (dialogueUI != null) dialogueUI.HidePrompt();
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (currentTarget != null && currentTarget.CanInteract())
            currentTarget.Interact(this.gameObject);
    }

    // Visualización en el editor para ver el rango de interacción
    private void OnDrawGizmosSelected()
    {
        if (rayOrigin == null) return;
        Vector3 origin = rayOrigin.position + Vector3.up * 1f;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin + transform.forward * interactRange, interactRadius);
        Gizmos.DrawLine(origin, origin + transform.forward * interactRange);
    }
}

/// <summary>
/// Contrato común para todo objeto interactuable.
/// Cualquier script que implemente esta interfaz puede ser activado con E.
/// </summary>
public interface IInteractable
{
    bool CanInteract();
    void Interact(GameObject interactor);
    string GetPrompt();
}