using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading; // Librería obligatoria para usar Hilos
using TMPro; // <-- 1. Agregamos esta librería para usar TextMeshPro
using UnityEngine.UI;   // Necesario para modificar el Texto de la interfaz

public class HiloTiempo : MonoBehaviour
{
    public int tiempoTotalSegundos = 300; // 300 segundos = 5 minutos
    public bool juegoActivo = true;
    public TextMeshProUGUI textoReloj; // <-- 2. Cambiamos 'Text' por 'TextMeshProUGUI'

    private Thread hiloTemporizador;

    void Start()
    {
        // 1. Inicializamos y arrancamos el hilo secundario
        hiloTemporizador = new Thread(MetodoTemporizador);
        hiloTemporizador.Start();
        Debug.Log("Hilo del temporizador iniciado.");
    }

    // Este método corre completamente separado del rendimiento de Unity
    void MetodoTemporizador()
    {
        while (juegoActivo && tiempoTotalSegundos > 0)
        {
            Thread.Sleep(1000); // El hilo "duerme" por 1 segundo exacto
            tiempoTotalSegundos--; // Restamos un segundo
        }

        if (tiempoTotalSegundos <= 0)
        {
            juegoActivo = false;
            Debug.Log("¡Se acabó el tiempo!");
            // Aquí más adelante podemos lanzar la pantalla de "Game Over"
        }
    }

    void Update()
    {
        // 2. El hilo principal de Unity actualiza la UI leyendo la variable
        // Convertimos los segundos totales a formato Minutos:Segundos
        int minutos = tiempoTotalSegundos / 60;
        int segundos = tiempoTotalSegundos % 60;

        if (textoReloj != null)
        {
            textoReloj.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    void OnApplicationQuit()
    {
        // 3. Regla de seguridad: Detener el hilo al cerrar el juego
        juegoActivo = false;

        if (hiloTemporizador != null && hiloTemporizador.IsAlive)
        {
            hiloTemporizador.Abort();
            Debug.Log("Hilo del temporizador cerrado de forma segura.");
        }
    }
}
