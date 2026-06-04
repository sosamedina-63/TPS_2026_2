using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelEstacion : MonoBehaviour
{
    public Material materialVerde; // Arrastra tu material color verde aquí
    public MonstruoVR monstruo;    // Arrastra a tu monstruo aquí

    // Solo marcaremos esta casilla en el Inspector del Panel 1
    public bool esElPrimerPanel = false;

    private bool yaActivado = false;

    void Update()
    {
        // Si usamos el botón del joystick, el ControlJugador intenta "agarrarlo" haciéndolo su hijo
        if (transform.parent != null && !yaActivado)
        {
            ActivarPanel();
        }
    }

    void ActivarPanel()
    {
        yaActivado = true;

        // Cambiamos el color de rojo a verde
        GetComponent<Renderer>().material = materialVerde;

        // Si es el panel 1, le damos la orden al monstruo de atacar
        if (esElPrimerPanel && monstruo != null)
        {
            monstruo.estaDespierto = true;
            Debug.Log("¡Panel 1 activado! El monstruo ha despertado.");
        }

        // Lo "soltamos" inmediatamente para que el panel no se quede pegado a tu cámara
        transform.SetParent(null);
    }
}