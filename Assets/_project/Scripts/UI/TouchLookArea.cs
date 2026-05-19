using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Área transparente que captura arrastres de dedo y los convierte
/// en input de mirar (rotar cámara). Se ubica cubriendo toda la pantalla,
/// PERO debe estar DETRÁS de los botones y el joystick en orden de Hierarchy.
/// </summary>
public class TouchLookArea : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Sensibilidad")]
    [SerializeField] private float sensitivity = 1.5f;

    private Vector2 lastPosition;
    private bool dragging = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        lastPosition = eventData.position;
        dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging) return;

        Vector2 currentPosition = eventData.position;
        Vector2 delta = (currentPosition - lastPosition) * sensitivity;
        lastPosition = currentPosition;

        if (MobileControls.Instance != null)
            MobileControls.Instance.SetLookInput(delta);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        if (MobileControls.Instance != null)
            MobileControls.Instance.SetLookInput(Vector2.zero);
    }

    private void Update()
    {
        // Si dejamos de tocar pero no llamamos a OnPointerUp,
        // forzamos el reset del LookInput.
        if (!dragging && MobileControls.Instance != null)
            MobileControls.Instance.SetLookInput(Vector2.zero);
    }
}