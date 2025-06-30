using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Game Over")]
    public GameObject panelGameOver;
    public Text textoTiempo;
    public Button botonReintentar;
    
    // Variables de control
    private float tiempoInicio;
    private bool juegoTerminado = false;
    
    // Singleton para acceso global
    public static GameManager Instance;
    
    void Awake()
    {
        // Patrón Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Inicializar
        tiempoInicio = Time.time;
        panelGameOver.SetActive(false);
        
        // Configurar botón - SOLO EL BOTÓN REINTENTAR
        botonReintentar.onClick.AddListener(ReiniciarJuego);
        
        // Pausar el tiempo inicialmente está en 1 (normal)
        Time.timeScale = 1f;
    }
    
    public void GameOver()
    {
        if (juegoTerminado) return; // Evitar múltiples llamadas
        
        juegoTerminado = true;
        
        // Calcular tiempo sobrevivido
        float tiempoSobrevivido = Time.time - tiempoInicio;
        
        // Actualizar UI
        textoTiempo.text = $"Sobreviviste {tiempoSobrevivido:F1} segundos";
        
        // Mostrar panel y pausar juego
        panelGameOver.SetActive(true);
        Time.timeScale = 0f; // Pausar el juego
        
        Debug.Log($"Game Over - Tiempo: {tiempoSobrevivido:F1}s");
    }
    
    public void ReiniciarJuego()
    {
        Debug.Log("Reiniciando juego...");
        Time.timeScale = 1f; // Restaurar tiempo normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // Método público para verificar si el juego terminó
    public bool EstaJuegoTerminado()
    {
        return juegoTerminado;
    }
}