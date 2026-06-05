using UnityEngine;
using System.IO.Ports; // Necesario para la comunicación serial

public class ArduinoSerial : MonoBehaviour
{
    [Header("Configuración del Puerto")]
    public string portName = "COM4"; // Cambia esto al puerto que use tu Arduino (ej. COM5)
    public int baudRate = 9600;

    private SerialPort serialPort;

    [Header("Datos del Joystick (Solo lectura)")]
    public float joystickX;
    public float joystickY;
    public bool isButtonPressed;

    void Start()
    {
        // Inicializamos el puerto serial
        serialPort = new SerialPort(portName, baudRate);
        
        // Un tiempo de espera corto es vital para que Unity no se congele esperando datos
        serialPort.ReadTimeout = 50; 

        try
        {
            serialPort.Open();
            Debug.Log("Puerto Serial Abierto en: " + portName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al abrir el puerto serial: " + e.Message);
        }
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                // Leemos la línea entrante del Arduino
                string data = serialPort.ReadLine(); 
                ParseData(data);
            }
            catch (System.TimeoutException)
            {
                // Es normal que a veces ocurra un timeout si el loop de Unity va más rápido que el Arduino. Se ignora.
            }
        }
    }

    void ParseData(string data)
    {
        // Esperamos un formato "512,512,0"
        string[] values = data.Split(',');

        if (values.Length == 3)
        {
            float rawX = float.Parse(values[0]);
            float rawY = float.Parse(values[1]);

            // Normalizamos los valores analógicos (0 a 1023) a un rango de -1 a 1
            // 512 es el centro aproximado del joystick
            joystickX = (rawX - 512f) / 512f; 
            joystickY = (rawY - 512f) / 512f; 

            // Aplicamos una pequeña "zona muerta" (deadzone) para evitar que el personaje se mueva solo
            if (Mathf.Abs(joystickX) < 0.1f) joystickX = 0;
            if (Mathf.Abs(joystickY) < 0.1f) joystickY = 0;

            // Leemos el estado del botón
            isButtonPressed = int.Parse(values[2]) == 1;
        }
    }

    void OnDestroy()
    {
        // Regla de oro en mecatrónica: siempre cerrar el puerto al terminar para evitar bloqueos
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}