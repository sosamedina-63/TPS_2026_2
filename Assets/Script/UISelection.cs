//Necesarias
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
//Opcionales
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

public class UISelection : MonoBehaviour
{
    public static bool gazedAt;
    [SerializeField]
    float fillTime = 5f;
    public Image radialImage;
    public UnityEvent onFillComplete; //Evento generico que se asigna al termnar la carga

    //Proyecto asincrono
    private Coroutine fillCoroutine;

    void Start()
    {
        gazedAt = false;
        radialImage.fillAmount = 0; 
    }

    public void OnPointerEnter() 
    {
        gazedAt = true;

        if(fillCoroutine != null) 
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
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }
        radialImage.fillAmount = 0f;
    }

   private IEnumerator FillRadial() 
    {
        float elapasedTime = 0f;

        while (elapasedTime < fillTime) 
        {
            if (gazedAt) //Dejamos de ver el boton
            {
                yield break;
            }

            //elapasedTime = elapasedTime + Time.deltaTime;
            elapasedTime += Time.deltaTime;
            radialImage.fillAmount = Mathf.Clamp01(elapasedTime/fillTime);

            yield return null;
        }

        //El evento a ejecutar
        onFillComplete?.Invoke();
    }



    void Update()
    {
        
    }
}
