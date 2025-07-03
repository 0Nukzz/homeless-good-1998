using UnityEngine;

public class AnimadorPersonaje : MonoBehaviour
{
    [Header("Sprites por Dirección")]
    public Sprite[] spritesArriba;
    public Sprite[] spritesDerecha;
    public Sprite[] spritesAbajo;
    public Sprite[] spritesIzquierda;

    [Header("Sprites Idle (Opcional)")]
    public Sprite spriteIdleArriba;
    public Sprite spriteIdleDerecha;
    public Sprite spriteIdleAbajo;
    public Sprite spriteIdleIzquierda;

    [Header("Configuración de Animación")]
    public float velocidadAnimacion = 8f;
    public float umbralMovimiento = 0.1f;
    public bool usarSpritesIzquierda = false;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private MovimientoPersonaje movimientoScript;

    private Sprite[] animacionActual;
    private int frameActual = 0;
    private float tiempoFrame = 0f;
    private bool estaCaminando = false;
    private Vector2 ultimaDireccion = Vector2.down;

    private enum Direccion { Arriba, Derecha, Abajo, Izquierda }
    private Direccion direccionActual = Direccion.Abajo;

    // Para que el sistema sepa si debe usar dirección del movimiento o del mouse
    private Vector2 direccionMouse = Vector2.zero;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("No se encontró SpriteRenderer en este objeto.");

        rb = GetComponentInParent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("No se encontró Rigidbody2D en el padre.");

        movimientoScript = GetComponentInParent<MovimientoPersonaje>();
        if (movimientoScript == null)
            Debug.LogError("No se encontró MovimientoPersonaje en el padre.");

        // Asignación inicial para confirmar visualmente
        if (spriteIdleAbajo != null)
            spriteRenderer.sprite = spriteIdleAbajo;
        else if (spritesAbajo.Length > 0)
            spriteRenderer.sprite = spritesAbajo[0];
    }

    void Update()
    {
        if (estaCaminando)
        {
            ActualizarFrame();
        }
        else
        {
            // Si está idle, asegurarse que el frame se reinicie
            frameActual = 0;
            tiempoFrame = 0f;
        }
    }

    // Este método se llama desde SistemaDisparo para actualizar la dirección de disparo (mouse)
    public void SetDireccionDesdeDisparo(Vector2 direccion)
    {
        if (direccion == Vector2.zero) return;

        direccionMouse = direccion.normalized;

        // Si no se está moviendo, actualiza el sprite para que apunte a la dirección del mouse
        if (!estaCaminando)
        {
            Direccion nuevaDir = ObtenerDireccionPrincipal(direccionMouse);
            if (nuevaDir != direccionActual)
            {
                direccionActual = nuevaDir;
                CambiarAAnimacionIdle();
                ActualizarFlip();
            }
        }
    }

    // Este método se llama desde MovimientoPersonaje para actualizar animación por movimiento y dirección mouse
    public void ActualizarMovimientoYDireccion(Vector2 velocidadMovimiento)
    {
        float magnitudVelocidad = velocidadMovimiento.magnitude;
        bool estaCaminandoNuevo = magnitudVelocidad > umbralMovimiento;

        if (estaCaminandoNuevo)
        {
            estaCaminando = true;
            ultimaDireccion = velocidadMovimiento.normalized;
            direccionActual = ObtenerDireccionPrincipal(ultimaDireccion);
            CambiarAAnimacionCaminar();
        }
        else
        {
            estaCaminando = false;
            if (direccionMouse != Vector2.zero)
            {
                direccionActual = ObtenerDireccionPrincipal(direccionMouse);
            }
            CambiarAAnimacionIdle();
        }

        ActualizarFlip();
    }

    Direccion ObtenerDireccionPrincipal(Vector2 direccion)
    {
        if (direccion == Vector2.zero)
            return direccionActual;

        if (Mathf.Abs(direccion.x) > Mathf.Abs(direccion.y))
            return direccion.x > 0 ? Direccion.Derecha : Direccion.Izquierda;
        else
            return direccion.y > 0 ? Direccion.Arriba : Direccion.Abajo;
    }

    void CambiarAAnimacionCaminar()
    {
        Sprite[] nuevaAnimacion = null;

        switch (direccionActual)
        {
            case Direccion.Arriba:
                nuevaAnimacion = spritesArriba;
                break;
            case Direccion.Derecha:
                nuevaAnimacion = spritesDerecha;
                break;
            case Direccion.Abajo:
                nuevaAnimacion = spritesAbajo;
                break;
            case Direccion.Izquierda:
                nuevaAnimacion = (usarSpritesIzquierda && spritesIzquierda.Length > 0) ? spritesIzquierda : spritesDerecha;
                break;
        }

        CambiarAnimacion(nuevaAnimacion);
    }

    void CambiarAAnimacionIdle()
    {
        Sprite spriteIdle = null;

        switch (direccionActual)
        {
            case Direccion.Arriba:
                spriteIdle = spriteIdleArriba;
                break;
            case Direccion.Derecha:
                spriteIdle = spriteIdleDerecha;
                break;
            case Direccion.Abajo:
                spriteIdle = spriteIdleAbajo;
                break;
            case Direccion.Izquierda:
                spriteIdle = usarSpritesIzquierda ? spriteIdleIzquierda : spriteIdleDerecha;
                break;
        }

        if (spriteIdle != null)
        {
            spriteRenderer.sprite = spriteIdle;
            animacionActual = null;
        }
        else if (animacionActual != null && animacionActual.Length > 0)
        {
            spriteRenderer.sprite = animacionActual[0];
        }
    }

    void ActualizarFlip()
    {
        if (!usarSpritesIzquierda)
        {
            spriteRenderer.flipX = direccionActual == Direccion.Izquierda;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    void CambiarAnimacion(Sprite[] nuevaAnimacion)
    {
        if (nuevaAnimacion == null || nuevaAnimacion.Length == 0) return;

        if (animacionActual != nuevaAnimacion)
        {
            animacionActual = nuevaAnimacion;
            frameActual = 0;
            tiempoFrame = 0f;

            if (animacionActual.Length > 0)
            {
                spriteRenderer.sprite = animacionActual[0];
            }
        }
    }

    void ActualizarFrame()
    {
        if (animacionActual == null || animacionActual.Length == 0) return;

        tiempoFrame += Time.deltaTime;
        float tiempoPorFrame = 1f / velocidadAnimacion;

        if (tiempoFrame >= tiempoPorFrame)
        {
            tiempoFrame = 0f;
            frameActual = (frameActual + 1) % animacionActual.Length;
            spriteRenderer.sprite = animacionActual[frameActual];
        }
    }
}
