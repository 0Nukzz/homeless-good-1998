using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIVidaSistema : MonoBehaviour
{
    [Header("UI Corazones")]
    public GameObject corazonPrefab;
    public Transform contenedorCorazones;
    
    [Header("Configuración de Posicionamiento")]
    public Vector2 offsetJugador = new Vector2(0, 80); // Offset en unidades de mundo
    public float escalaFija = 1f;
    
    private MovimientoPersonaje jugador;
    private List<GameObject> corazones = new List<GameObject>();
    private RectTransform rectTransform;
    private Canvas canvas;
    private Camera camara;

    void Start()
    {
        // Encontrar referencia al jugador
        jugador = FindFirstObjectByType<MovimientoPersonaje>();
        camara = Camera.main;
        
        if (jugador != null)
        {
            rectTransform = contenedorCorazones.GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            
            // Configurar el contenedor para que no escale automáticamente
            ConfigurarContenedor();
            CrearCorazones();
        }
    }

    void ConfigurarContenedor()
    {
        // Asegurar que el RectTransform del contenedor tenga configuración fija
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localScale = Vector3.one * escalaFija;
        
        // Si el contenedor tiene un LayoutGroup, configurarlo
        HorizontalLayoutGroup layoutGroup = contenedorCorazones.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childScaleWidth = false;
            layoutGroup.childScaleHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
        }
    }

    void Update()
    {
        if (jugador != null)
        {
            ActualizarCorazones();
            SeguirJugador();
        }
    }

    void CrearCorazones()
    {
        Debug.Log("Creando corazones. Vida máxima: " + jugador.GetVidaMaxima());

        // Limpiar corazones existentes
        foreach (GameObject corazon in corazones)
        {
            if (corazon != null)
                Destroy(corazon);
        }
        corazones.Clear();

        // Crear corazones según la vida máxima
        for (int i = 0; i < jugador.GetVidaMaxima(); i++)
        {
            GameObject nuevoCorazon = Instantiate(corazonPrefab, contenedorCorazones);
            corazones.Add(nuevoCorazon);
            
            // Configurar cada corazón individualmente
            RectTransform rtCorazon = nuevoCorazon.GetComponent<RectTransform>();
            
            // Fijar tamaño y escala
            rtCorazon.localScale = Vector3.one;
            rtCorazon.anchorMin = new Vector2(0, 0.5f);
            rtCorazon.anchorMax = new Vector2(0, 0.5f);
            rtCorazon.pivot = new Vector2(0, 0.5f);
            
            // Establecer tamaño fijo (ajusta estos valores según tu sprite)
            rtCorazon.sizeDelta = new Vector2(32, 32); // Tamaño fijo de 32x32 píxeles
            
            // Posición horizontal relativa
            rtCorazon.anchoredPosition = new Vector2(i * 36, 0); // 36 píxeles entre corazones
            
            // Desactivar cualquier componente que pueda alterar el tamaño
            ContentSizeFitter sizeFitter = nuevoCorazon.GetComponent<ContentSizeFitter>();
            if (sizeFitter != null)
            {
                sizeFitter.enabled = false;
            }
            
            Debug.Log($"[CrearCorazones] Corazón {i}: escala={rtCorazon.localScale}, tamaño={rtCorazon.sizeDelta}");
        }
    }

    void ActualizarCorazones()
    {
        int vidaActual = jugador.GetVidaActual();
        
        // Actualizar visibilidad de corazones
        for (int i = 0; i < corazones.Count; i++)
        {
            if (corazones[i] != null)
            {
                corazones[i].SetActive(i < vidaActual);
            }
        }
    }

    void SeguirJugador()
    {
        if (jugador == null || rectTransform == null || camara == null) return;

        // Método más estable: convertir la posición del mundo a posición de viewport
        Vector3 posicionJugadorMundo = jugador.transform.position;
        
        // Aplicar offset en unidades de mundo antes de convertir
        Vector3 posicionConOffset = posicionJugadorMundo + new Vector3(offsetJugador.x, offsetJugador.y, 0);
        
        // Convertir a viewport (valores entre 0 y 1)
        Vector3 posicionViewport = camara.WorldToViewportPoint(posicionConOffset);
        
        // Convertir viewport a posición de pantalla
        Vector2 posicionPantalla = new Vector2(
            posicionViewport.x * Screen.width,
            posicionViewport.y * Screen.height
        );

        // Para Canvas Screen Space - Overlay, usar posición directa
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rectTransform.position = posicionPantalla;
        }
        else
        {
            // Para otros modos, convertir a coordenadas locales
            Vector2 posicionLocal;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                posicionPantalla,
                canvas.worldCamera,
                out posicionLocal
            );
            rectTransform.anchoredPosition = posicionLocal;
        }

        // Mantener escala fija
        rectTransform.localScale = Vector3.one * escalaFija;
    }

    // Método para ajustar el offset desde el inspector o código
    public void SetOffset(Vector2 nuevoOffset)
    {
        offsetJugador = nuevoOffset;
    }
    
    // Método para ajustar la escala
    public void SetEscala(float nuevaEscala)
    {
        escalaFija = nuevaEscala;
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one * escalaFija;
        }
    }
}