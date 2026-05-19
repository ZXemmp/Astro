using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Joystick virtual circular para controles táctiles.
/// El handle (mando) se arrastra dentro del background (base).
/// El output es un Vector2 normalizado (-1..1, -1..1) accesible vía
/// MobileControls.Instance.MoveInput.
/// </summary>
public class VirtualJoystick : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Referencias")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    [Header("Configuración")]
    [SerializeField] private float handleRange = 70f;
    [SerializeField] private float deadZone = 0.1f;

    private Vector2 input = Vector2.zero;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        if (handle != null)
            handle.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (background == null || handle == null) return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out localPoint);

        // localPoint es relativo al pivote del background.
        // Lo limitamos al radio handleRange.
        Vector2 clamped = Vector2.ClampMagnitude(localPoint, handleRange);
        handle.anchoredPosition = clamped;

        // Normalizar el input a -1..1
        input = clamped / handleRange;

        // Aplicar dead zone
        if (input.magnitude < deadZone) input = Vector2.zero;

        // Enviar al MobileControls
        if (MobileControls.Instance != null)
            MobileControls.Instance.MoveInput = input;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
        if (MobileControls.Instance != null)
            MobileControls.Instance.MoveInput = Vector2.zero;
    }
}