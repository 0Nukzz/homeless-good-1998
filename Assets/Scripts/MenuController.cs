using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Método para el botón Jugar
    public void Jugar()
    {
        // Cambia "NombreEscenaJuego" por el nombre real de tu escena de juego
        SceneManager.LoadScene("MainScene");
    }

    // Método para el botón Salir
    public void Salir()
    {
        // En el editor no hace nada, pero en build sí cierra la app
        Debug.Log("Salir del juego");
        Application.Quit();
    }
}
