using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Conexiones")]
    public BaseDatos baseDatos; // Arrastra aquí el objeto GestorBaseDatos
    public HiloTiempo reloj;    // Arrastra aquí el objeto que tiene el HiloTiempo

    private bool juegoTerminado = false;

    void Update()
    {
        // CASO 1: DERROTA
        // El hilo principal vigila el tiempo. Si llega a 0 y el juego no ha terminado...
        if (reloj.tiempoTotalSegundos <= 0 && !juegoTerminado)
        {
            TerminarJuego("Derrota");
        }
    }

    // CASO 2: VICTORIA
    // Este método lo vas a mandar a llamar cuando abran la última puerta o resuelvan el puzzle final
    public void ActivarVictoria()
    {
        if (!juegoTerminado)
        {
            TerminarJuego("Victoria");
        }
    }

    public void ActivarDerrota()
    {
        if (!juegoTerminado)
        {
            TerminarJuego("Derrota - Atrapado");
        }
    }

    // EL DISPARADOR
    void TerminarJuego(string resultadoFinal)
    {
        juegoTerminado = true;
        reloj.juegoActivo = false; // Le avisamos al Hilo que se detenga

        // Ejemplo de cómo mandar los datos a Firebase
        GetComponent<BaseDatos>().GuardarPartida("Jugador1", 120, "Victoria");

        // Mandamos a Firebase el nombre del equipo, los segundos que sobraron y si ganaron o perdieron.
        baseDatos.GuardarPartida("Equipo Gustavo", reloj.tiempoTotalSegundos, resultadoFinal);

        Debug.Log("Partida terminada. Resultado enviado a Firebase: " + resultadoFinal);
    }
}