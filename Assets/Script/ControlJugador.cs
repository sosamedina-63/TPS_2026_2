using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlJugador : MonoBehaviour
{
    [Header("Conexión y Movimiento")]
    public ConexionArduino arduino; // Arrastrar el objeto que tiene el script de Arduino aquí
    public float velocidad = 3.0f;

    [Header("Interacción con Objetos")]
    public Transform puntoDeAgarre; // Un objeto vacío frente a la cámara donde flotará el objeto
    public float distanciaAgarre = 2.5f;
    private GameObject objetoSujetado = null;

    // Variable para detectar el "clic" del botón y evitar que lo agarre/suelte mil veces por segundo
    private bool botonPresionadoAntes = false;

    void Update()
    {
        // Nos aseguramos de que el script de Arduino esté asignado
        if (arduino != null)
        {
            MoverJugador();
            ControlarInteraccion();
        }
    }

    void MoverJugador()
    {
        float movX = 0f;
        float movZ = 0f;

        // Umbrales para el eje X (Izquierda - Derecha)
        if (arduino.ejeX > 600) movX = 1f;  // Derecha
        if (arduino.ejeX < 400) movX = -1f; // Izquierda

        // Umbrales para el eje Y (Adelante - Atrás)
        if (arduino.ejeY > 600) movZ = 1f;  // Adelante
        if (arduino.ejeY < 400) movZ = -1f; // Atrás

        // Movemos al jugador en relación a hacia dónde está mirando
        Vector3 movimiento = new Vector3(movX, 0, movZ) * velocidad * Time.deltaTime;
        transform.Translate(movimiento);
    }

    void ControlarInteraccion()
    {
        // Recuerda que con INPUT_PULLUP en Arduino, 0 es presionado y 1 es soltado
        bool botonPresionadoAhora = (arduino.boton == 0);

        // Detectamos el flanco de bajada (justo el instante en que se presiona el botón)
        if (botonPresionadoAhora && !botonPresionadoAntes)
        {
            if (objetoSujetado == null)
            {
                IntentarAgarrar(); // Si no tengo nada, busco qué agarrar
            }
            else
            {
                SoltarObjeto(); // Si ya tengo algo en la mano, lo suelto
            }
        }

        botonPresionadoAntes = botonPresionadoAhora;
    }

    void IntentarAgarrar()
    {
        RaycastHit hit;
        // Trazamos un rayo invisible desde la cámara hacia el frente
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distanciaAgarre))
        {
            // Condición: El objeto DEBE tener el Tag "Interactuable"
            if (hit.collider.CompareTag("Interactuable"))
            {
                objetoSujetado = hit.collider.gameObject;

                // Desactivamos la gravedad/física para que no se caiga de nuestras manos
                Rigidbody rb = objetoSujetado.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                // Lo movemos a nuestro punto de agarre y lo hacemos hijo para que nos siga
                objetoSujetado.transform.position = puntoDeAgarre.position;
                objetoSujetado.transform.SetParent(puntoDeAgarre);
            }
        }
    }

    void SoltarObjeto()
    {
        // Restauramos la física del objeto
        Rigidbody rb = objetoSujetado.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        // Rompemos la relación de padre-hijo para que se quede en el mundo
        objetoSujetado.transform.SetParent(null);
        objetoSujetado = null;
    }
}