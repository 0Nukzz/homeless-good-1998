using UnityEngine;

public class Proyectil : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Destruir proyectil al colisionar con cualquier objeto
        if (other.CompareTag("Enemy") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}