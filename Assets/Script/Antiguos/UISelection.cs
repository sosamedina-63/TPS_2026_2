///Necesarias 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
//Opcionales
using System.Collections;
using System.Collections.Generic;

public class UISelection : MonoBehaviour
{
    //Crear una variable de clase solo en esta para indicar si se voltio a ver el boton
    public static bool gazedAt; //No iniciaste el programa viendo el boton
    [SerializeField] //Basicamente toda la linea inmediata despues de esto solo lo asigna al inspector pero será protegido las variables (Serializado)
    float fillTime = 5f; //Tiempo de relleno
    public Image radialImage; //La imagen que vas a estar modificando 
    public UnityEvent onFillComplet; //Evento generico que se asigna al terminar la carga

    //Proceso asincrono para utiliar las corrutinas que so unicamente de Unity
    private Coroutine fillCoroutine;

    void Start() //Se ejecuta una vez
    {
        gazedAt = false;
        radialImage.fillAmount = 0; //Propiedad para interactuar con la imagen en la cual 1(con relleno) y 0(sin relleno) fillAmout
    }

    public void OnPointerEnter() 
    {
        gazedAt = true;
        //Concurrencia: me permite hace cosas
        //(Asincrono: se da en la division de los recursos (tiempo) , Hilos: se ejecutan en orden secuancial y Paralelo: dependen de los nucleos de la computadora)
        if (fillCoroutine != null) 
        {
            StopCoroutine(fillCoroutine);
        }
        fillCoroutine = StartCoroutine(FillRadial());
    }

    public void OnPointerExit() 
    {
        gazedAt = false;

        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine); //Detiene el llenado
            fillCoroutine = null;
        }
        radialImage.fillAmount = 0f; //Reinicia el llenado a 0

    }
    private IEnumerator FillRadial() //Conatar cuanto tiempo estas ejecutando 
    {
        float elapasedTime = 0f;

        while (elapasedTime < fillTime) 
        {
            if (!gazedAt) //Dejamos de ver el boton 
            { 
                yield break; //Nos saca del metodo
            }
            elapasedTime += Time.deltaTime; //Time que me deja ingresar al reloj de la computadora y deltaTime hace la diferencia de tiempo desde que el tiempo que se ejecuto hasta el que se vuelva a llamar

            radialImage.fillAmount = Mathf.Clamp01(elapasedTime/fillTime);

            yield return null;
        }

        //El evento a ejecutar
        onFillComplet?.Invoke(); //Invoca el evento si esta completado

    }

    void Update()
    {
        
    }
}
