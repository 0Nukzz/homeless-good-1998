using UnityEngine;

public class Proyectil : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Destruir proyectil al colisionar con enemigos o paredes
        if (other.CompareTag("Enemy"))
        {
            // El daño ya se maneja en ComportamientoEnemigo
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}