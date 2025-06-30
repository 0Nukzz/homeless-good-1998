using UnityEngine;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        scriptJugador = jugador?.GetComponent<MovimientoPersonaje>();
        velocidadOriginal = velocidad;
    }

    void Update()
    {
        if (jugador == null) return;
        
        // Manejar knockback
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
        
        // Detectar si el jugador está en rango
        if (distanciaAlJugador <= rangoDeteccion)
        {
            jugadorDetectado = true;
        }
        
        // Si el jugador fue detectado, perseguirlo o atacarlo
        if (jugadorDetectado)
        {
            if (distanciaAlJugador <= rangoAtaque && Time.time - tiempoUltimoAtaque >= tiempoEntreAtaques)
            {
                AtacarJugador();
            }
            else if (distanciaAlJugador > distanciaMinima)
            {
                // Solo perseguir si está fuera de la distancia mínima
                PerseguirJugador();
            }
            else
            {
                // Si está muy cerca, mantener distancia (opcional: moverse ligeramente hacia atrás)
                MantenDistanciaMinima();
            }
        }
    }

    void PerseguirJugador()
    {
        Vector2 direccion = (jugador.position - transform.position).normalized;
        rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
        
        // Rotar hacia el jugador
        RotarHaciaJugador(direccion);
    }
    
    void MantenDistanciaMinima()
    {
        // Opcional: empujar ligeramente al enemigo hacia atrás si está demasiado cerca
        float distanciaActual = Vector2.Distance(transform.position, jugador.position);
        
        if (distanciaActual < distanciaMinima * 0.8f) // Si está muy muy cerca
        {
            Vector2 direccionAlejarse = (transform.position - jugador.position).normalized;
            rb.MovePosition(rb.position + direccionAlejarse * velocidad * 0.3f * Time.fixedDeltaTime);
        }
        
        // Seguir rotando hacia el jugador aunque no se mueva
        Vector2 direccionAlJugador = (jugador.position - transform.position).normalized;
        RotarHaciaJugador(direccionAlJugador);
    }
    
    void RotarHaciaJugador(Vector2 direccion)
    {
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
    }
    
    void AtacarJugador()
    {
        tiempoUltimoAtaque = Time.time;
        
        // Aplicar daño al jugador
        if (scriptJugador != null)
        {
            scriptJugador.RecibirDano(transform);
        }
        
        // Empujar al jugador ligeramente para crear separación
        Rigidbody2D rbJugador = jugador.GetComponent<Rigidbody2D>();
        if (rbJugador != null)
        {
            Vector2 direccionEmpuje = (jugador.position - transform.position).normalized;
            rbJugador.AddForce(direccionEmpuje * fuerzaEmpujeAtaque, ForceMode2D.Impulse);
        }
        
        // También empujar al enemigo ligeramente hacia atrás
        Vector2 direccionRetroceso = (transform.position - jugador.position).normalized;
        rb.AddForce(direccionRetroceso * fuerzaEmpujeAtaque * 0.5f, ForceMode2D.Impulse);
        
        Debug.Log($"Enemigo atacó al jugador");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Proyectil"))
        {
            // Aplicar knockback desde la posición del proyectil
            Vector2 direccionKnockback = (transform.position - other.transform.position).normalized;
            AplicarKnockback(direccionKnockback);
            
            RecibirDano(1);
        }
    }

    void AplicarKnockback(Vector2 direccion)
    {
        // Aplicar fuerza de knockback
        rb.AddForce(direccion * fuerzaKnockback, ForceMode2D.Impulse);
        
        // Pausar movimiento temporalmente
        enKnockback = true;
        tiempoKnockback = duracionKnockback;
        velocidad = 0f; // Detener movimiento durante knockback
    }

    void RecibirDano(int dano)
    {
        vida -= dano;
        Debug.Log($"Enemigo recibió daño. Vida restante: {vida}");
        
        if (vida <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Dibujar rango de detección
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
        
        // Dibujar rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
        
        // Dibujar distancia mínima
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaMinima);
    }
}