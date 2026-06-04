using UnityEngine;
//Librerias para los trabajos de concurrencias 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Dynamic;



public class Concurrencia : MonoBehaviour
{
    [Header("Activa los metodos")]
    public bool useSincrono;
    public bool useThread;
    public bool useThask;
    public bool useCoroutine;

    [Header("Esfera a mover")]
    public Transform sincronoSphere;
    public Transform threadSphere;
    public Transform taskSphere;
    public Transform coroutineSphere;
    public Transform mainCube;

    //Acciones a ejecutar en el hilo secundario
    private Queue<Action> mainThreadActions = new Queue<Action>(); //Para mandar a ejecutar cosas debe estar dentro de esta cola de aciones 


    void Start()
    {
        if (useSincrono) MoveSincrono();
        if (useThread) MoveWithThread();
        if (useThask) MoveWithTask(); //Tarea asincrona 
        if (useCoroutine) StartCoroutine(MoveWithCoroutine()); //Para las corrutinas se tiene que inicializar primero la rutina
    }

    // Mantiene el hilo principal 
    void Update()
    {
        //Siempre gira el cubo de referencia 
        mainCube.Rotate(Vector3.up,50*Time.deltaTime);//Ver girar respecto a y

        //Ejecuta las acciones en el hilo principal
        lock (mainThreadActions) //Vas bloquear el hilo secundario mientras tengas acciones en el hilo principal (lock)
        {
            while (mainThreadActions.Count > 0) 
            { 
                mainThreadActions.Dequeue().Invoke(); //Saca el de arriba (Dequeue) Y luego lo ejecutas (Invoke)

            }
        }
    }

    //Metodos para herramientas de concurrencia 
    void MoveSincrono() 
    {
        for (int i = 0; i <= 100; i++) 
        {
            sincronoSphere.position += Vector3.right * 0.05f;
        }
        Thread.Sleep(50); //Distinguir las pequeñas diferencias en el movimineto asincrono
    }

    //Movimiento con hilo secundario
    void MoveWithThread() 
    {
        //Aqui se ponen las acciones que va a realizar el hilo secundario
        new Thread(() => 
        {
            for (int i = 0; i <= 100; i++)
            {
                Thread.Sleep(50);  //Quiero ver el moviemto antes de que empieze la tarea 
                lock (mainThreadActions) //No sale de la accion hasta terminarla si no lo bloqueas el hilo se queda abierto 
                {
                    mainThreadActions.Enqueue(() => 
                    {
                        threadSphere.position += Vector3.right * 0.05f;
                    });
                }
            }
        }).Start(); // => Tal que 
    }

    //Metodo con task asincrono (async), tarea asincona dentro de mi hilo principal dedica un tiempo para realizar la tarea
    async void MoveWithTask() 
    {
        //Vas a pasausar el momento en la tarea, te esperas un momento en lo que ejecutas la tarea 
        await Task.Run(() => 
        {
            for (int i = 0; i <= 100; i++)
            {
                Thread.Sleep(50);

                //TE QUEDAS AQUI HASTA TERMINES LA SIGUINETE ACCION 
                lock (mainThreadActions) 
                {
                    mainThreadActions.Enqueue(()=> 
                    {
                        taskSphere.position += Vector3.right * 0.05f;
                    });
                }
            }

            
        });
    }

    //Coroutine 
    IEnumerator MoveWithCoroutine()
    {
        for (int i = 0; i <= 100; i++)
        {
            coroutineSphere.position += Vector3.right * 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}