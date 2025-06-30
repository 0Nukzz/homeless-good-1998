using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoPersonaje : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    
    [Header("Sistema de Vida")]
    public int vidaMaxima = 3;
    public int vidaActual;
    
    [Header("Knockback")]
    public float fuerzaKnockback = 5f;
    public float duracionRalentizado = 1f;
    public float factorRalentizado = 0.3f;
    
    private Rigidbody2D rb;
    private Vector2 movimiento;
    private bool estaRalentizado = false;
    private float tiempoRalentizado = 0f;
    private float velocidadOriginal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vidaActual = vidaMaxima;
        velocidadOriginal = velocidad;
    }

    void Update()
    {
        // Manejar ralentización
        if (estaRalentizado)
        {
            tiempoRalentizado -= Time.deltaTime;
            if (tiempoRalentizado <= 0)
            {
                estaRalentizado = false;
                velocidad = velocidadOriginal;
            }
        }

        // Capturar input de movimiento
        movimiento = Vector2.zero;
        
        if (Keyboard.current.wKey.isPressed) movimiento.y = 1;
        if (Keyboard.current.sKey.isPressed) movimiento.y = -1;
        if (Keyboard.current.aKey.isPressed) movimiento.x = -1;
        if (Keyboard.current.dKey.isPressed) movimiento.x = 1;
        
        // Normalizar movimiento diagonal
        movimiento = movimiento.normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movimiento * velocidad * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            RecibirDano(other.transform);
        }
    }

    public void RecibirDano(Transform enemigoTransform)
    {
        vidaActual--;
        
        // Aplicar knockback y ralentización
        Vector2 direccionKnockback = (transform.position - enemigoTransform.position).normalized;
        rb.AddForce(direccionKnockback * fuerzaKnockback, ForceMode2D.Impulse);
        
        // Aplicar ralentización
        estaRalentizado = true;
        tiempoRalentizado = duracionRalentizado;
        velocidad = velocidadOriginal * factorRalentizado;
        
        Debug.Log($"Jugador recibió daño. Vida restante: {vidaActual}");
        
        // Game Over si no hay vida
        if (vidaActual <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER - Llamando a GameManager...");
        
        // Llamar al GameManager en lugar de recargar directamente
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            // Fallback si no hay GameManager
            Debug.LogWarning("No se encontró GameManager, recargando escena...");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }

    // Método público para acceder a la vida actual (para UI)
    public int GetVidaActual()
    {
        return vidaActual;
    }

    public int GetVidaMaxima()
    {
        return vidaMaxima;
    }
}