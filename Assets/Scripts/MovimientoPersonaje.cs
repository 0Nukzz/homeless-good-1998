using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MovimientoPersonaje : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Sistema de Vida")]
    public int vidaMaxima = 3;
    public int vidaActual;

    [Header("Knockback")]
    public float fuerzaKnockback = 5f;
    public float duracionKnockback = 0.3f;       // Duración del knockback
    public float duracionRalentizado = 1f;
    public float factorRalentizado = 0.3f;

    private Rigidbody2D rb;
    private Vector2 movimiento;

    private bool estaRalentizado = false;
    private float tiempoRalentizado = 0f;
    private float velocidadOriginal;

    // Variables knockback
    private bool enKnockback = false;
    private float tiempoKnockback = 0f;
    private Vector2 velocidadKnockback;

    // SpriteRenderer para tintineo
    private SpriteRenderer sr;

    [Header("Referencia Animador")]
    public AnimadorPersonaje animadorPersonaje; // Asignar en inspector o buscar en Start()

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Obtener el SpriteRenderer del hijo Visual
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
            Debug.LogError("No se encontró SpriteRenderer en Player > Visual");

        // Buscar animador si no está asignado
        if (animadorPersonaje == null)
            animadorPersonaje = GetComponentInChildren<AnimadorPersonaje>();

        vidaActual = vidaMaxima;
        velocidadOriginal = velocidad;
        rb.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        // Manejar ralentización
        if (estaRalentizado)
        {
            tiempoRalentizado -= Time.deltaTime;
            if (tiempoRalentizado <= 0f)
            {
                estaRalentizado = false;
                velocidad = velocidadOriginal;
            }
        }

        // Manejar knockback (tiempo)
        if (enKnockback)
        {
            tiempoKnockback -= Time.deltaTime;
            if (tiempoKnockback <= 0f)
            {
                enKnockback = false;
                velocidadKnockback = Vector2.zero;
            }
        }

        // Capturar input sólo si no está en knockback
        if (!enKnockback)
        {
            movimiento = Vector2.zero;
            if (Keyboard.current.wKey.isPressed) movimiento.y = 1;
            if (Keyboard.current.sKey.isPressed) movimiento.y = -1;
            if (Keyboard.current.aKey.isPressed) movimiento.x = -1;
            if (Keyboard.current.dKey.isPressed) movimiento.x = 1;

            movimiento = movimiento.normalized;
        }
        else
        {
            // Durante knockback no se mueve con input
            movimiento = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (enKnockback)
        {
            rb.linearVelocity = velocidadKnockback;
        }
        else
        {
            rb.linearVelocity = movimiento * velocidad;
        }

        // Actualizar animación con la velocidad actual
        if (animadorPersonaje != null)
        {
            animadorPersonaje.ActualizarMovimientoYDireccion(rb.linearVelocity);
        }
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
        Debug.Log("Recibiendo daño, debería tintinear rojo");

        vidaActual--;

        StartCoroutine(FlashRojo());

        Vector2 direccionKnockback = (transform.position - enemigoTransform.position).normalized;
        velocidadKnockback = direccionKnockback * fuerzaKnockback;
        enKnockback = true;
        tiempoKnockback = duracionKnockback;

        estaRalentizado = true;
        tiempoRalentizado = duracionRalentizado;
        velocidad = velocidadOriginal * factorRalentizado;

        Debug.Log($"Jugador recibió daño. Vida restante: {vidaActual}");

        if (vidaActual <= 0)
        {
            GameOver();
        }
    }

    private IEnumerator FlashRojo()
    {
        if (sr == null) yield break;

        Color colorOriginal = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        sr.color = colorOriginal;
    }

    void GameOver()
    {
        Debug.Log("GAME OVER - Llamando a GameManager...");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogWarning("No se encontró GameManager, recargando escena...");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }

    public int GetVidaActual() => vidaActual;
    public int GetVidaMaxima() => vidaMaxima;

    public Vector2 GetVelocidadActual()
    {
        return rb.linearVelocity;
    }
}
