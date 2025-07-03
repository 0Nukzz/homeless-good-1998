using UnityEngine;
using UnityEngine.InputSystem;

public class SistemaDisparo : MonoBehaviour
{
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;

    [Header("Referencias")]
    public Transform objetivoRotacion;   // El objeto Arma (que rota y cambia posición)
    public AnimadorPersonaje animadorPersonaje;  // Referencia al script AnimadorPersonaje

    [Header("Posiciones Arma Local")]
    public Vector3 posicionArmaDerecha = new Vector3(0.5f, 0f, 0f);
    public Vector3 posicionArmaIzquierda = new Vector3(-0.5f, 0f, 0f);
    public Vector3 posicionArmaArriba = new Vector3(0f, 0.5f, 0f);
    public Vector3 posicionArmaAbajo = new Vector3(0f, -0.5f, 0f);

    public float velocidadProyectil = 10f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (animadorPersonaje == null)
            Debug.LogWarning("No asignaste AnimadorPersonaje en SistemaDisparo.");
    }

    void Update()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        mouseScreenPos.z = 10f;  // Ajusta según tu cámara

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        Vector2 direccion = (mouseWorldPos - objetivoRotacion.position).normalized;

        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        objetivoRotacion.rotation = Quaternion.Euler(0, 0, angulo);

        float umbralDireccion = 0.3f;

        Vector2 dirPrincipal = Vector2.zero;

        if (Mathf.Abs(direccion.x) > Mathf.Abs(direccion.y))
        {
            if (Mathf.Abs(direccion.x) > umbralDireccion)
                dirPrincipal = new Vector2(Mathf.Sign(direccion.x), 0);
        }
        else
        {
            if (Mathf.Abs(direccion.y) > umbralDireccion)
                dirPrincipal = new Vector2(0, Mathf.Sign(direccion.y));
        }

        if (dirPrincipal == Vector2.left)
        {
            objetivoRotacion.localPosition = posicionArmaIzquierda;
            var sr = objetivoRotacion.GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipY = true;
        }
        else if (dirPrincipal == Vector2.right)
        {
            objetivoRotacion.localPosition = posicionArmaDerecha;
            var sr = objetivoRotacion.GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipY = false;
        }
        else if (dirPrincipal == Vector2.up)
        {
            objetivoRotacion.localPosition = posicionArmaArriba;
            var sr = objetivoRotacion.GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipY = false;
        }
        else if (dirPrincipal == Vector2.down)
        {
            objetivoRotacion.localPosition = posicionArmaAbajo;
            var sr = objetivoRotacion.GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipY = false;
        }

        // ¡Aquí la clave! Actualizamos el sprite del personaje según dirección de disparo:
        if (animadorPersonaje != null && dirPrincipal != Vector2.zero)
        {
            animadorPersonaje.SetDireccionDesdeDisparo(dirPrincipal);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Disparar(direccion);
        }
    }

    void Disparar(Vector2 direccion)
    {
        GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);
        Rigidbody2D rb = proyectil.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direccion * velocidadProyectil;
        Destroy(proyectil, 3f);
    }
}
