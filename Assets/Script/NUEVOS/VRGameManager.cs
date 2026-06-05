using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VRGameManager : MonoBehaviour
{
    // Patrón Singleton para que otros scripts lo encuentren al instante
    public static VRGameManager Instance; 

    [Header("Referencias a otros Sistemas")]
    public MonsterBehavior monster;
    public GameTimerThread timerThread;
    public FirebaseDataSender firebaseSender;

    [Header("Elementos del Juego")]
    public GameObject llave;
    public Transform puertaSalida;

    // Variables de control de estado (Máquina de estados finitos)
    private int panelesActivados = 0;
    private bool tieneLlave = false;
    private bool juegoTerminado = false;

    void Awake()
    {
        // Configuramos el Singleton
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
        Debug.Log("El gamer manager esta vivo");
    }

    void Start()
    {
        // Aseguramos que la llave esté invisible al iniciar
        if (llave != null) llave.SetActive(false);
    }

    // Método que llamarán los paneles al ponerse verdes
    public void RegistrarPanelActivado()
    {
        if (juegoTerminado) return;

        panelesActivados++;
        Debug.Log("Paneles activados: " + panelesActivados + "/3");

        // Condición 1: Despertar al monstruo con el primer panel
        if (panelesActivados == 1)
        {
            monster.ActivateMonster();
        }
        // Condición 2: Aparecer la llave con el tercer panel
        else if (panelesActivados == 3)
        {
            if (llave != null)
            {
                llave.SetActive(true);
                Debug.Log("¡Las llaves han aparecido! Búscalas.");
            }
        }
    }

    // Método que llamará la llave cuando la mires o presiones
    public void RecogerLlave()
    {
        tieneLlave = true;
        if (llave != null) llave.SetActive(false); // Desaparece porque ya la "agarraste"
        Debug.Log("¡Tienes la llave! Corre a la puerta de salida.");
    }
    // Método que llamará la llave cuando la mires o presiones
    /*public void RecogerLlave()
    {
        tieneLlave = true;
        Debug.Log("¡Tienes la llave! Corre a la puerta de salida.");
       
        // Iniciamos la rutina segura de apagado en lugar de hacerlo de golpe
        StartCoroutine(ApagarLlaveSeguro());
    }*/

    // Corrutina de seguridad para evitar colapsos de memoria (Memory Leaks/NullReference)
    private IEnumerator ApagarLlaveSeguro()
    {
        // Obligamos al procesador a esperar el final del frame (milisegundos)
        // Esto permite que el temporizador de la llave termine su ciclo de vida 
        yield return new WaitForEndOfFrame();
       
        if (llave != null)
        {
            llave.SetActive(false); // Ahora sí, desaparecemos la llave
        }
    }



    // Método que llamará la puerta cuando el jugador choque con ella
    public void IntentarEscapar()
    {
        if (tieneLlave && !juegoTerminado)
        {
            Victoria();
        }
        else if (!tieneLlave && !juegoTerminado)
        {
            Debug.Log("La puerta está cerrada. Necesitas encontrar las llaves primero.");
        }
    }

    private void Victoria()
    {
        juegoTerminado = true;
        Debug.Log("¡Escapaste con éxito!");
        
        // Detenemos los sistemas
        monster.StopMonster();
        timerThread.StopTimerThread();
        
        // Enviamos el hilo a Firebase
        firebaseSender.SendGameData(timerThread.timeElapsedInSeconds, "Victoria - Escapó");

        // Reiniciamos el nivel después de 3 segundos para el siguiente jugador
        StartCoroutine(ReiniciarNivel(3f));
    }

    // Este método es llamado por el monstruo si te alcanza
    public void GameOver()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        Debug.Log("¡El monstruo te atrapó!");
        
        // Detenemos cronómetro y enviamos la derrota a Firebase
        timerThread.StopTimerThread();
        firebaseSender.SendGameData(timerThread.timeElapsedInSeconds, "Derrota - Atrapado");

        // Reiniciamos rápido
        StartCoroutine(ReiniciarNivel(2f));
    }

    private IEnumerator ReiniciarNivel(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}