using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Librería de clase para Inteligencia Artificial

public class MonstruoVR : MonoBehaviour
{
    public Transform jugador; // Arrastra a tu XR Origin aquí
    public float distanciaAtrape = 2f;

    private float distanciaAtrapeSqr;
    private NavMeshAgent agente;

    // Esta variable la encenderemos desde el panel
    public bool estaDespierto = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        // Matemática exacta del AIManager del curso
        distanciaAtrapeSqr = distanciaAtrape * distanciaAtrape;
    }

    void Update()
    {
        if (estaDespierto && agente.enabled)
        {
            // 1. El monstruo persigue al jugador
            agente.SetDestination(jugador.position);

            // 2. Revisamos si lo alcanzó
            Vector3 posicionJugador = jugador.position;
            if ((transform.position - posicionJugador).sqrMagnitude < distanciaAtrapeSqr)
            {
                estaDespierto = false; // Se detiene
                agente.isStopped = true;
                Debug.Log("¡El monstruo te atrapó!");

                // 3. Le avisamos al GameManager que perdimos
                FindFirstObjectByType<GameManager>().ActivarDerrota();
            }
        }
    }
}