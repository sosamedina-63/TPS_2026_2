using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LlaveFinal : MonoBehaviour
{
    private bool yaAvisamos = false;
    private GameManager gameManager;

    void Start()
    {
        // Cacheamos la referencia una sola vez, más eficiente que buscarlo en Update
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        if (transform.parent != null && !yaAvisamos)
        {
            yaAvisamos = true;
            gameManager.ActivarVictoria(); // Solo una vez
            Debug.Log("¡El jugador agarró la llave final!");
        }
    }
}