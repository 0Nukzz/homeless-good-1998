using UnityEngine;
using System.Collections;

public class ComportamientoEnemigo : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 3f;
    public float rangoDeteccion = 8f;
    public float rangoAtaque = 1.5f;
    public float distanciaMinima = 0.8f; // Nueva: distancia mínima al jugador
    public int vida = 2;

    [Header("Knockback")]
    public float fuerzaKnockback = 3f;
    public float duracionKnockback = 0.2f;

    [Header("Comportamiento de Ataque")]
    public float tiempoEntreAtaques = 1f; // Tiempo entre ataques
    public float fuerzaEmpujeAtaque = 2f; // Fuerza para empujar al jugador al atacar

    private Transform jugador;
    private Rigidbody2D rb;
    private bool jugadorDetectado = false;
    private bool enKnockback = false;
    private float tiempoKnockback = 0f;
    private float velocidadOriginal;
    private float tiempoUltimoAtaque = 0f;
    private MovimientoPersonaje scriptJugador;

    // Nueva referencia para controlar la animación sin rotar el objeto
    private AnimadorEnemigo animadorEnemigo;

    // SpriteRenderer para tintinear
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        scriptJugador = jugador?.GetComponent<MovimientoPersonaje>();
        velocidadOriginal = velocidad;

        animadorEnemigo = GetComponent<AnimadorEnemigo>();

        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
            Debug.LogWarning("No se encontró SpriteRenderer en enemigo para tintinear.");
    }

    void Update()
    {
        if (jugador == null) return;

        if (enKnockback)
        {
            tiempoKnockback -= Time.deltaTime;
            if (tiempoKnockback <= 0)
            {
                enKnockback = false;
                velocidad = velocidadOriginal;
            }
            return; // No hacer nada más mientras está en knockback
        }

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        if (distanciaAlJugador <= rangoDeteccion)
            jugadorDetectado = true;

        if (jugadorDetectado)
        {
            if (distanciaAlJugador <= rangoAtaque && Time.time - tiempoUltimoAtaque >= tiempoEntreAtaques)
            {
                AtacarJugador();
            }
            else if (distanciaAlJugador > distanciaMinima)
            {
                PerseguirJugador();
            }
            else
            {
                MantenDistanciaMinima();
            }
        }
    }

    void PerseguirJugador()
    {
        Vector2 direccion = (jugador.position - transform.position).normalized;
        rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);

        if (animadorEnemigo != null)
            animadorEnemigo.ActualizarDireccion(direccion);
    }

    void MantenDistanciaMinima()
    {
        float distanciaActual = Vector2.Distance(transform.position, jugador.position);

        if (distanciaActual < distanciaMinima * 0.8f)
        {
            Vector2 direccionAlejarse = (transform.position - jugador.position).normalized;
            rb.MovePosition(rb.position + direccionAlejarse * velocidad * 0.3f * Time.fixedDeltaTime);

            if (animadorEnemigo != null)
                animadorEnemigo.ActualizarDireccion(direccionAlejarse);
        }
        else
        {
            Vector2 direccionAlJugador = (jugador.position - transform.position).normalized;

            if (animadorEnemigo != null)
                animadorEnemigo.ActualizarDireccion(direccionAlJugador);
        }
    }

    void AtacarJugador()
    {
        tiempoUltimoAtaque = Time.time;

        if (scriptJugador != null)
        {
            scriptJugador.RecibirDano(transform);
        }

        Rigidbody2D rbJugador = jugador.GetComponent<Rigidbody2D>();
        if (rbJugador != null)
        {
            Vector2 direccionEmpuje = (jugador.position - transform.position).normalized;
            rbJugador.AddForce(direccionEmpuje * fuerzaEmpujeAtaque, ForceMode2D.Impulse);
        }

        Vector2 direccionRetroceso = (transform.position - jugador.position).normalized;
        rb.AddForce(direccionRetroceso * fuerzaEmpujeAtaque * 0.5f, ForceMode2D.Impulse);

        Debug.Log($"Enemigo atacó al jugador");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Proyectil"))
        {
            Vector2 direccionKnockback = (transform.position - other.transform.position).normalized;
            AplicarKnockback(direccionKnockback);

            RecibirDano(1);
        }
    }

    void AplicarKnockback(Vector2 direccion)
    {
        rb.AddForce(direccion * fuerzaKnockback, ForceMode2D.Impulse);

        enKnockback = true;
        tiempoKnockback = duracionKnockback;
        velocidad = 0f;
    }

    void RecibirDano(int dano)
    {
        vida -= dano;
        Debug.Log($"Enemigo recibió daño. Vida restante: {vida}");

        // Iniciar tintineo rojo
        StartCoroutine(FlashRojo());

        if (vida <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashRojo()
    {
        if (sr == null) yield break;

        Color colorOriginal = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = colorOriginal;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaMinima);
    }
}
