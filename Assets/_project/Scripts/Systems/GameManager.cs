using UnityEngine;

/// <summary>
/// Estado global persistente entre escenas.
/// Singleton sencillo con DontDestroyOnLoad.
/// Aquí guardamos cosas como "¿el jugador ya recogió la batería?",
/// "¿ya pasó el terremoto?", etc.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Estado de la partida")]
    public bool hasBattery = false;
    public bool earthquakeTriggered = false;
    public bool shipPowered = false;

    private void Awake()
    {
        // Patrón Singleton: solo puede existir UN GameManager en todo el juego.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Que no se destruya al cambiar de escena.
        DontDestroyOnLoad(gameObject);
    }

    public void ResetState()
    {
        hasBattery = false;
        earthquakeTriggered = false;
        shipPowered = false;
    }
}