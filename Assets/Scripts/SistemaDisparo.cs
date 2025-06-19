using UnityEngine;
using UnityEngine.InputSystem;

public class SistemaDisparo : MonoBehaviour
{
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;
    public float velocidadProyectil = 10f;
    
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Disparar con click izquierdo usando Input System
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Disparar();
        }
    }

    void Disparar()
    {
        // Obtener posición del mouse en el mundo
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 posicionMouse = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cam.nearClipPlane));
        posicionMouse.z = 0;

        // Calcular dirección del disparo
        Vector2 direccion = (posicionMouse - puntoDisparo.position).normalized;

        // Crear proyectil
        GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);
        
        // Aplicar velocidad al proyectil
        Rigidbody2D rbProyectil = proyectil.GetComponent<Rigidbody2D>();
        rbProyectil.linearVelocity = direccion * velocidadProyectil;
        
        // Destruir proyectil después de 3 segundos
        Destroy(proyectil, 3f);
    }
}