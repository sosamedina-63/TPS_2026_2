using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // Fundamental para leer el puerto COM

public class ConexionArduino : MonoBehaviour
{
    // Cambia "COM3" por el puerto que estés usando en Arduino IDE
    SerialPort puerto = new SerialPort("COM4", 9600);

    // Variables públicas para que ControlJugador las pueda usar para moverse o agarrar cosas
    public float ejeX = 0f;
    public float ejeY = 0f;
    public int boton = 1;

    void Start()
    {
        try
        {
            puerto.Open();
            puerto.ReadTimeout = 20; // Previene que Unity se trabe esperando datos
            Debug.Log("Conexión con Arduino establecida.");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("No se pudo abrir el puerto: " + e.Message);
        }
    }

    void Update()
    {
        if (puerto.IsOpen)
        {
            try
            {
                // Leemos la línea que llega del Arduino
                string datosArduino = puerto.ReadLine();

                // Partimos la información usando la coma como separador
                string[] valores = datosArduino.Split(',');

                // Nos aseguramos de que llegaron los 3 datos correctamente
                if (valores.Length == 3)
                {
                    // Convertimos el texto a números
                    ejeX = float.Parse(valores[0]);
                    ejeY = float.Parse(valores[1]);
                    boton = int.Parse(valores[2]);
                }
            }
            catch (System.TimeoutException)
            {
                // TimeoutException es normal en comunicación serial, simplemente lo ignoramos
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error al leer datos: " + e.Message);
            }
        }
    }

    // Regla de oro: Siempre cerrar el puerto al salir, o Unity crasheará
    void OnApplicationQuit()
    {
        if (puerto.IsOpen)
        {
            puerto.Close();
            Debug.Log("Puerto Serial cerrado de forma segura.");
        }
    }
}