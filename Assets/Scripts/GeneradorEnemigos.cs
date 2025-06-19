using UnityEngine;
using System.Collections.Generic;

public class GeneradorEnemigos : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    public GameObject enemigoPrefab;
    public Transform jugador;
    public int maxEnemigos = 20;
    public float radioSpawn = 15f; // Radio donde aparecen los enemigos
    public float radioMinimo = 10f; // Distancia mínima del jugador para spawn
    public float tiempoEntreSpawns = 2f;
    
    private List<GameObject> enemigos = new List<GameObject>();
    private float tiempoUltimoSpawn;

    void Update()
    {
        // Limpiar lista de enemigos destruidos
        enemigos.RemoveAll(enemigo => enemigo == null);
        
        // Generar enemigo si es necesario
        if (Time.time - tiempoUltimoSpawn > tiempoEntreSpawns && enemigos.Count < maxEnemigos)
        {
            GenerarEnemigo();
            tiempoUltimoSpawn = Time.time;
        }
    }

    void GenerarEnemigo()
    {
        Vector2 posicionSpawn = ObtenerPosicionSpawnAleatoria();
        GameObject nuevoEnemigo = Instantiate(enemigoPrefab, posicionSpawn, Quaternion.identity);
        enemigos.Add(nuevoEnemigo);
    }

    Vector2 ObtenerPosicionSpawnAleatoria()
    {
        Vector2 posicionJugador = jugador.position;
        Vector2 direccionAleatoria;
        Vector2 posicionSpawn;
        
        int intentos = 0;
        do
        {
            // Generar dirección aleatoria
            float angulo = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            direccionAleatoria = new Vector2(Mathf.Cos(angulo), Mathf.Sin(angulo));
            
            // Generar distancia entre radioMinimo y radioSpawn
            float distancia = Random.Range(radioMinimo, radioSpawn);
            posicionSpawn = posicionJugador + direccionAleatoria * distancia;
            
            intentos++;
        } while (Vector2.Distance(posicionSpawn, posicionJugador) < radioMinimo && intentos < 10);
        
        return posicionSpawn;
    }

    void OnDrawGizmosSelected()
    {
        if (jugador != null)
        {
            // Dibujar radio de spawn (amarillo)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(jugador.position, radioSpawn);
            
            // Dibujar radio mínimo (rojo)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(jugador.position, radioMinimo);
        }
    }
}