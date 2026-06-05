using UnityEngine;
using System.Threading; // Librería obligatoria para concurrencia

public class GameTimerThread : MonoBehaviour
{
    [Header("Datos del Temporizador")]
    public int timeElapsedInSeconds = 0; // Este dato lo mandaremos a Firebase después

    // Variables para manipular el hilo secundario (Idénticas a tu script FlightThread)
    private Thread timerThread; 
    private bool isTimerRunning = false; 
    private bool stopTimerThread = false; 

    void Start()
    {
        // En cuanto empiece la escena, arranca el cronómetro en segundo plano
        StartTimerThread();
    }

    public void StartTimerThread()
    {
        if (!isTimerRunning)
        {
            stopTimerThread = false;
            isTimerRunning = true;
            timeElapsedInSeconds = 0;

            // Instanciamos y arrancamos el hilo secundario referenciando el método CountTime
            timerThread = new Thread(CountTime);
            timerThread.IsBackground = true; // Asegura que el hilo muera si la aplicación se cierra forzosamente
            timerThread.Start();
            
            Debug.Log("Hilo secundario del temporizador: INICIADO.");
        }
    }

    // ---------------------------------------------------------
    // ESTE MÉTODO SE EJECUTA FUERA DEL HILO PRINCIPAL DE UNITY
    // ---------------------------------------------------------
    private void CountTime()
    {
        // Mientras no se active la bandera de paro (ganar o perder)
        while (!stopTimerThread)
        {
            // Detenemos este hilo específicamente durante 1000 milisegundos (1 segundo)
            Thread.Sleep(1000);

            // Incrementamos la variable de tiempo
            timeElapsedInSeconds++;
        }
        
        isTimerRunning = false;
    }

    // Método público que será llamado por el Game Manager cuando abras la puerta o te atrapen
    public void StopTimerThread()
    {
        stopTimerThread = true;
        Debug.Log("Hilo secundario detenido. Tiempo final: " + timeElapsedInSeconds + " segundos.");
    }

    // Seguridad en la ejecución: Si cambiamos de escena o cerramos el juego, 
    // DEBEMOS matar el hilo secundario para evitar fugas de memoria (Memory Leaks).
    void OnDestroy()
    {
        if (isTimerRunning)
        {
            stopTimerThread = true;
            // Esperamos un momento a que el hilo termine su ciclo de Sleep y se cierre limpiamente
            if (timerThread != null && timerThread.IsAlive)
            {
                timerThread.Join(1500); 
            }
        }
    }
}