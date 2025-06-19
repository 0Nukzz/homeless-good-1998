using UnityEngine;

public class SeguimientoCamara : MonoBehaviour
{
    public Transform objetivo; // El personaje a seguir
    public float velocidadSeguimiento = 2f;
    public Vector3 offset = new Vector3(0, 0, -10); // Mantener la cámara atrás del personaje

    void LateUpdate()
    {
        if (objetivo != null)
        {
            Vector3 posicionDeseada = objetivo.position + offset;
            Vector3 posicionSuave = Vector3.Lerp(transform.position, posicionDeseada, velocidadSeguimiento * Time.deltaTime);
            transform.position = posicionSuave;
        }
    }
}