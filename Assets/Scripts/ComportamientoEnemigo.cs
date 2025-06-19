using UnityEngine;

public class ComportamientoEnemigo : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 3f;
    public float rangoDeteccion = 8f; // Distancia para detectar al jugador
    public float rangoAtaque = 1.5f; // Distancia para atacar
    public int vida = 3;
    
    private Transform jugador;
    private Rigidbody2D rb;
    private bool jugadorDetectado = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (jugador == null) return;
        
        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);
        
        // Detectar si el jugador está en rango
        if (distanciaAlJugador <= rangoDeteccion)
        {
            jugadorDetectado = true;
        }
        
        // Si el jugador fue detectado, perseguirlo
        if (jugadorDetectado)
        {
            PerseguirJugador();
        }
    }

    void PerseguirJugador()
    {
        Vector2 direccion = (jugador.position - transform.position).normalized;
        rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
        
        // Rotar hacia el jugador
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Proyectil"))
        {
            RecibirDano(1);
        }
    }

    void RecibirDano(int dano)
    {
        vida -= dano;
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
    }
}