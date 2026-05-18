using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Escotilla que carga otra escena cuando el jugador interactúa con ella.
/// Implementa IInteractable para ser detectable por PlayerInteraction.
/// </summary>
public class Hatch : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    [SerializeField] private string targetSceneName = "02_PlanetaExterior";
    [SerializeField] private string promptText = "Pulsa E para salir de la nave";
    [SerializeField] private bool requireBattery = false;
    [SerializeField] private string blockedMessage = "Necesitas conseguir una batería primero";

    public bool CanInteract()
    {
        return true;
    }

    public string GetPrompt()
    {
        return promptText;
    }

    public void Interact(GameObject interactor)
    {
        // Si esta escotilla requiere batería (caso de la nave al volver),
        // verificar el estado del GameManager.
        if (requireBattery)
        {
            bool hasBattery = GameManager.Instance != null && GameManager.Instance.hasBattery;
            if (!hasBattery)
            {
                if (DialogueUI.Instance != null)
                    DialogueUI.Instance.ShowMessage(blockedMessage);
                return;
            }
        }

        // Cargar la escena destino
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log($"[Hatch] Cargando escena: {targetSceneName}");
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("[Hatch] No se ha asignado una escena destino.");
        }
    }
}