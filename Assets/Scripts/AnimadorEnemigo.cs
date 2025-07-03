using UnityEngine;

public class AnimadorEnemigo : MonoBehaviour
{
    [Header("Sprites por Dirección")]
    public Sprite[] spritesArriba;
    public Sprite[] spritesDerecha;
    public Sprite[] spritesAbajo;
    public Sprite[] spritesIzquierda; // Opcional: si no los tienes, se usa flipX en derecha

    [Header("Configuración de Animación")]
    public float velocidadAnimacion = 6f;

    private SpriteRenderer sr;
    private int frameActual = 0;
    private float tiempoFrame = 0f;

    private Sprite[] animacionActual;
    private Vector2 ultimaDireccion = Vector2.down;
    private bool usarFlipXParaIzquierda => spritesIzquierda == null || spritesIzquierda.Length == 0;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // Inicializamos con animación hacia abajo si hay sprites
        if (spritesAbajo.Length > 0)
            CambiarAnimacion(spritesAbajo, false);
    }

    void Update()
    {
        if (animacionActual == null || animacionActual.Length == 0) return;

        tiempoFrame += Time.deltaTime;
        if (tiempoFrame >= 1f / velocidadAnimacion)
        {
            tiempoFrame = 0f;
            frameActual = (frameActual + 1) % animacionActual.Length;
            sr.sprite = animacionActual[frameActual];
        }
    }

    /// <summary>
    /// Llama a este método desde el script enemigo para actualizar la dirección y animación.
    /// </summary>
    public void ActualizarDireccion(Vector2 direccion)
    {
        if (direccion.sqrMagnitude < 0.01f) return; // No cambiar si no se mueve

        ultimaDireccion = direccion.normalized;

        if (Mathf.Abs(ultimaDireccion.x) > Mathf.Abs(ultimaDireccion.y))
        {
            // Movimiento horizontal
            if (ultimaDireccion.x > 0)
            {
                CambiarAnimacion(spritesDerecha, false);
            }
            else
            {
                if (usarFlipXParaIzquierda)
                    CambiarAnimacion(spritesDerecha, true);
                else
                    CambiarAnimacion(spritesIzquierda, false);
            }
        }
        else
        {
            // Movimiento vertical
            if (ultimaDireccion.y > 0)
                CambiarAnimacion(spritesArriba, false);
            else
                CambiarAnimacion(spritesAbajo, false);
        }
    }

    private void CambiarAnimacion(Sprite[] nuevaAnimacion, bool flipX)
    {
        if (nuevaAnimacion == null || nuevaAnimacion.Length == 0) return;

        if (animacionActual != nuevaAnimacion)
        {
            animacionActual = nuevaAnimacion;
            frameActual = 0;
            tiempoFrame = 0f;
            sr.sprite = animacionActual[0];
        }

        sr.flipX = flipX;
    }
}
