using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PianoAR : MonoBehaviour
{
    public AudioClip[] clips;
    public AudioSource audioClip;
    string btnName;

    void Start()
    {
        audioClip = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))//Indice 0 corresponde al boton derecho.GetMouse detecta si presionamos el boton del mause
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//Crea un rayo a partir de donde se dio el click 
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) //Simula la fisica del rayo virtual que creamos a partir del punto donde se dio click en la cámara y si hay colición se le agraga a hit 
            {
                btnName = hit.transform.name;
                
                switch (btnName) 
                {
                    case "Do":
                        audioClip.clip = clips[0];
                        audioClip.Play();
                        break;
                    case "Re":
                        audioClip.clip = clips[1];
                        audioClip.Play();
                        break;
                    case "Mi":
                        audioClip.clip = clips[2];
                        audioClip.Play();
                        break;
                    case "Fa":
                        audioClip.clip = clips[3];
                        audioClip.Play();
                        break;
                    case "Sol":
                        audioClip.clip = clips[4];
                        audioClip.Play();
                        break;
                    case "La":
                        audioClip.clip = clips[5];
                        audioClip.Play();
                        break;
                    case "Si":
                        audioClip.clip = clips[6];
                        audioClip.Play();
                        break;
                    default: 
                        break;
                }
            }
        }


        else if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)//REGISTRAMOS EL PIRMER TOQUE 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))  
            {
                btnName = hit.transform.name;

                switch (btnName)
                {
                    case "Do":
                        audioClip.clip = clips[0];
                        audioClip.Play();
                        break;
                    case "Re":
                        audioClip.clip = clips[1];
                        audioClip.Play();
                        break;
                    case "Mi":
                        audioClip.clip = clips[2];
                        audioClip.Play();
                        break;
                    case "Fa":
                        audioClip.clip = clips[3];
                        audioClip.Play();
                        break;
                    case "Sol":
                        audioClip.clip = clips[4];
                        audioClip.Play();
                        break;
                    case "La":
                        audioClip.clip = clips[5];
                        audioClip.Play();
                        break;
                    case "Si":
                        audioClip.clip = clips[6];
                        audioClip.Play();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
