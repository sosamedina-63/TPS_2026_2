using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GazeAndJoystickInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public Transform vrCamera;             // La cámara del visor VR
    public ArduinoSerial arduinoSource;    // El script que lee el joystick
    public Image radialImage;              // La imagen de la interfaz que se va llenando (Retícula)

    [Header("Configuración")]
    public float interactionDistance = 10f; // Distancia máxima para interactuar
    public float fillTime = 3f;             // Segundos necesarios mirando el objeto

    private GameObject currentTarget;
    private bool isGazing;
    private Coroutine fillCoroutine;

    void Update()
    {
        RaycastHit hit;
        // Trazamos un rayo (Raycast) desde el centro de la cámara hacia adelante
        if (Physics.Raycast(vrCamera.position, vrCamera.forward, out hit, interactionDistance))
        {
            // Verificamos si el objeto golpeado tiene la etiqueta "Interactable"
            if (hit.collider.CompareTag("Interactable"))
            {
                GameObject target = hit.collider.gameObject;

                // Si es un objeto nuevo al que acabamos de mirar
                if (target != currentTarget)
                {
                    ResetInteraction();
                    currentTarget = target;
                    isGazing = true;
                    // Iniciamos el hilo/corrutina del temporizador
                    fillCoroutine = StartCoroutine(FillRadial()); 
                }

                // Interacción alternativa: si se presiona el botón físico del joystick
                if (arduinoSource != null && arduinoSource.isButtonPressed)
                {
                    CompleteInteraction();
                }
            }
            else
            {
                // Miramos a un objeto que no es interactuable
                ResetInteraction();
            }
        }
        else
        {
            // No estamos mirando a ningún objeto
            ResetInteraction();
        }
    }

    // Corrutina basada en los scripts del curso para llenar la imagen
    private IEnumerator FillRadial()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fillTime)
        {
            if (!isGazing) 
            {
                yield break; // Nos saca del método si dejamos de mirar
            }

            elapsedTime += Time.deltaTime; // Tiempo transcurrido
            radialImage.fillAmount = Mathf.Clamp01(elapsedTime / fillTime);
            yield return null;
        }

        // Si el ciclo termina, se completó el tiempo de mirada
        CompleteInteraction();
    }

    private void CompleteInteraction()
    {
        if (currentTarget != null)
        {
            // Buscamos el script del panel/llave para ejecutar su acción
            InteractableObject interactable = currentTarget.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                interactable.Activate();
            }

            // Cambiamos la etiqueta para evitar volver a interactuar con el mismo objeto
            currentTarget.tag = "Untagged"; 
        }
        ResetInteraction();
    }

    private void ResetInteraction()
    {
        isGazing = false;
        currentTarget = null;
        
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
        }
        
        if (radialImage != null)
        {
            radialImage.fillAmount = 0f; // Reiniciamos el círculo visual
        }
    }
}