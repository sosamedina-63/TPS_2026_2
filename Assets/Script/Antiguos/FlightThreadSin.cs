using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Threading;
using Unity.VisualScripting;
using System.IO;


public class FlightThreadSin : MonoBehaviour
{
    public float speed = 50f;
    public float rotationSpeed = 100f;
    public Transform cameraTransform;
    public Vector2 movementInput;

    //Me va a servir para la actividad 1 y 2  
    //Control de iteraciones 
    public int turbulenceIterations = 1000000;

    //Lista de vectores de posicion calculados de la turbulencia
    private List<Vector3> turbulenceForces = new List<Vector3>();

    //Variables para manipular el hilo secundario 
    private Thread turbulenceThread; //La instancia del hilo secundario 
    private bool isTurbulenceRunning = false; // Bandera para saber si sigue sigue el calculo 
    private bool stopTurbulenceThread = false; //Bandera para saber si el hilo terminó
    private float capturesTime; //Variable para almacenar el tiempo transcurrido 

    //Bander de control sobre lectura 
    public bool read = false;
    public bool write = false;
    private object filelock = new object();

    //Ruta de almacenamiento de archivo 
    string filepath;

    //Metodo para mover la nave
    public void OnMovement(InputValue value) //( On: Devolver la llamada)
    {
        movementInput = value.Get<Vector2>(); //(Get: Componente obtener el valor que se asocio a esta variable)
    }

    void Start()
    {
        filepath = Application.dataPath + "/TurbulenceData.txt";
        Debug.Log("Ruta al archivo:" + filepath);
    }

    void Update()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("No hay camara asignada");
            return;
        }

        //Tiempo transcurrido 
        capturesTime = Time.time;

        //Proceso pesado en hilo secundario 
        if (!isTurbulenceRunning)
        {
            isTurbulenceRunning = true;
            stopTurbulenceThread = false;

            turbulenceThread = new Thread(() =>
            SimulateTurbulence(capturesTime));
            turbulenceThread.Start();
        }

        //Mover la nave linealmente
        Vector3 moveDirection = cameraTransform.forward * movementInput.y * speed * Time.deltaTime;
        this.transform.position += moveDirection;

        //Mover la nave en rotacion 
        float yaw = movementInput.x * rotationSpeed * Time.deltaTime;
        this.transform.Rotate(0, yaw, 0);

        //ACTIVIDAD 3 Parte 2: Sincronizar hilos

        if (write && !read) 
        {
            TryReadFile();
            read = true;
        }

    }

    public void SimulateTurbulence(float time)
    {
        turbulenceForces.Clear();

        //Repeticiones 

        for (int i = 0; i < turbulenceIterations; i++)
        {
            //Verificar si se debe detener el hilo 
            if (stopTurbulenceThread)
            {
                break;
            }

            Vector3 force = new Vector3(
                    Mathf.PerlinNoise(i * 0.001f, time) * 2 - 1,
                    Mathf.PerlinNoise(i * 0.002f, time) * 2 - 1,
                    Mathf.PerlinNoise(i * 0.003f, time) * 2 - 1

                );

            turbulenceForces.Add(force);
        }

        //Señal en consola de inicio del hilo
        Debug.Log("Iniciando simulación de turbulencia");

        Debug.Log("Escribiendo archivo ...");

        //ACTIVIDAD 3: Metodo para escritura del archivo 
        lock (filelock) 
        {
            //Escritura del archivo
            using (StreamWriter writer = new StreamWriter(filepath, false))//Cada vez que abara un archivo lo va a cerrar, escribe sin conocer lo anterurr 
             {
                foreach (var force in turbulenceForces)
                {
                     writer.WriteLine(force.ToString());
                }
                 writer.Flush(); //Limpia el bufer para no almacenar nada 
             }
        }
        

        Debug.Log("Archivo escrito");

        //Simula completa
        isTurbulenceRunning = false;
        write = true;
    }

    void TryReadFile()
    {
        
        try
        {
            lock (filelock) 
            {
                if (File.Exists(filepath))
                {
                    string content = File.ReadAllText(filepath);
                    Debug.Log("El archivo fue leido" + content);
                }
                else 
                {
                    Debug.Log("Ocurrio un problema");
                }
            }
        }
        catch (IOException ex)
        {
            Debug.LogError("Error de acceso al archivo" + ex.Message);
        }
    }

    private void OnDestroy()
    {
        //Indicar el cierre del hilo secundario
        stopTurbulenceThread = true;

        //Verificar si el hilo existe y se esta ejecutando 
        if (turbulenceThread != null && turbulenceThread.IsAlive)
        {
            //Lo unimos al hilo principal y cerramos ejecucion 
            turbulenceThread.Join();
        }
    }
}
