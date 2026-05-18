using TMPro;
using UnityEngine;

/// <summary>
/// UI básica para mostrar prompts de interacción (ej. "Pulsa E para entrar")
/// y mensajes narrativos temporales (ej. "Necesitas la batería").
/// Es un Singleton: solo existe una instancia accesible desde cualquier script.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("Prompt (siempre visible mientras detecta interactuable)")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TMP_Text promptText;

    [Header("Mensaje (aparece y desaparece)")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float defaultMessageDuration = 3f;

    private float messageTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (promptPanel  != null) promptPanel.SetActive(false);
        if (messagePanel != null) messagePanel.SetActive(false);
    }

    private void Update()
    {
        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f && messagePanel != null)
                messagePanel.SetActive(false);
        }
    }

    public void ShowPrompt(string text)
    {
        if (promptPanel == null || promptText == null) return;
        promptText.text = text;
        promptPanel.SetActive(true);
    }

    public void HidePrompt()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
    }

    public void ShowMessage(string text, float duration = -1f)
    {
        if (messagePanel == null || messageText == null) return;
        messageText.text = text;
        messagePanel.SetActive(true);
        messageTimer = duration > 0f ? duration : defaultMessageDuration;
    }
}