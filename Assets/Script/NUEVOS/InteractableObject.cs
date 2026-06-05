using UnityEngine;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour
{
    public enum TipoObjeto { Panel, Llave }
    public TipoObjeto tipo = TipoObjeto.Panel;

    [Header("Materiales")]
    public Material colorRojo;
    public Material colorVerde;
    private Renderer objRenderer;

    [Header("Interacción por Mirada")]
    public Image radialImage;
    public float fillTime = 3f;

    private bool gazedAt = false;
    private bool yaActivado = false;
    private float temporizador = 0f; // Reemplaza a la corrutina por un cálculo numérico directo

    private ArduinoSerial arduinoSource;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
       
        if (colorRojo != null && tipo == TipoObjeto.Panel)
        {
            objRenderer.material = colorRojo;
        }

        // Conexión con tu placa
        arduinoSource = FindObjectOfType<ArduinoSerial>();
    }

    void Update()
    {
        // Si el objeto ya se activó/agarró, ignoramos todo lo demás
        if (yaActivado) return;

        // 1. Interacción física inmediata con el botón del Arduino
        if (gazedAt && arduinoSource != null && arduinoSource.isButtonPressed)
        {
            CompletarInteraccion();
            return;
        }

        // 2. Interacción visual estable (Cardboard)
        if (gazedAt)
        {
            temporizador += Time.deltaTime; // El reloj avanza frame por frame

            if (radialImage != null)
            {
                // Dibuja el progreso en el círculo
                radialImage.fillAmount = Mathf.Clamp01(temporizador / fillTime);
            }

            // Si se alcanzan los 3 segundos
            if (temporizador >= fillTime)
            {
                CompletarInteraccion();
            }
        }
    }

    public void Activate()
    {
        if (yaActivado) return;
        yaActivado = true;

        if (tipo == TipoObjeto.Panel)
        {
            if (colorVerde != null) objRenderer.material = colorVerde;
           
            if (VRGameManager.Instance != null)
                VRGameManager.Instance.RegistrarPanelActivado();
        }
        else if (tipo == TipoObjeto.Llave)
        {
            if (VRGameManager.Instance != null)
                VRGameManager.Instance.RecogerLlave();
        }
    }

    private void CompletarInteraccion()
    {
        if (yaActivado) return;
       
        // Limpieza de variables
        gazedAt = false;
        temporizador = 0f;
        if (radialImage != null) radialImage.fillAmount = 0f;
       
        // Ejecuta la orden
        Activate();
    }

    // ---------------------------------------------------------
    // EVENTOS NATIVOS CARDBOARD
    // ---------------------------------------------------------

    public void OnPointerEnter()
    {
        if (yaActivado) return;
        gazedAt = true; // Inicia la condición para que el Update empiece a contar
    }

    public void OnPointerExit()
    {
        if (yaActivado) return;
       
        // El jugador quitó la vista antes de los 3 segundos
        gazedAt = false;
        temporizador = 0f; // Reiniciamos el reloj

        if (radialImage != null) radialImage.fillAmount = 0f; // Vaciamos el círculo
    }
}

/*using UnityEngine;
using UnityEngine.UI; // Necesario para manipular la imagen de la retícula
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    public enum TipoObjeto { Panel, Llave }
    public TipoObjeto tipo = TipoObjeto.Panel;

    [Header("Materiales")]
    public Material colorRojo;
    public Material colorVerde;
    private Renderer objRenderer;

    [Header("Interacción por Mirada")]
    public Image radialImage; // Arrastra aquí tu círculo de carga del Canvas
    public float fillTime = 3f; // Segundos necesarios para activarlo
   
    private bool gazedAt = false;
    private bool yaActivado = false;
    private Coroutine timerCoroutine;

    private ArduinoSerial arduinoSource; // Referencia para el botón físico

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
       
        if (colorRojo != null && tipo == TipoObjeto.Panel)
        {
            objRenderer.material = colorRojo;
        }

        // Buscamos el gestor de Arduino automáticamente en la escena
        arduinoSource = FindFirstObjectByType<ArduinoSerial>();
    }

    void Update()
    {
        // Alternativa de Hardware: Si lo estamos mirando, no se ha activado,
        // y se presiona el botón físico del joystick de Arduino
        if (gazedAt && !yaActivado && arduinoSource != null && arduinoSource.isButtonPressed)
        {
            CompletarInteraccion();
        }
    }

    public void Activate()
    {
        if (yaActivado) return;
        yaActivado = true;

        if (tipo == TipoObjeto.Panel)
        {
            if (colorVerde != null) objRenderer.material = colorVerde;
           
            // Le avisamos al gestor central
            if (VRGameManager.Instance != null)
                VRGameManager.Instance.RegistrarPanelActivado();
        }
        else if (tipo == TipoObjeto.Llave)
        {
            if (VRGameManager.Instance != null)
                VRGameManager.Instance.RecogerLlave();
        }
    }

    private void CompletarInteraccion()
    {
        if (yaActivado) return;
       
        gazedAt = false;
        if (radialImage != null) radialImage.fillAmount = 0f;
       
        Activate();
    }

    // ---------------------------------------------------------
    // SISTEMA DE EVENTOS DE CARDBOARD Y TEMPORIZADOR
    // ---------------------------------------------------------

    public void OnPointerEnter()
    {
        if (yaActivado) return;
       
        // El visor detectó el panel
        gazedAt = true;
        timerCoroutine = StartCoroutine(FillRadialTimer());
    }

   public void OnPointerExit()
    {
        // NUEVA LÍNEA DE SEGURIDAD:
        // Si el objeto ya se apagó (porque lo agarramos), aborta la función para evitar errores.
        if (!gameObject.activeInHierarchy) return;

        // El jugador desvió la mirada antes de tiempo
        gazedAt = false;
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        if (radialImage != null) radialImage.fillAmount = 0f;
    }

    // Corrutina basada en la lógica de clase para llenar el anillo visual
    private IEnumerator FillRadialTimer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fillTime)
        {
            if (!gazedAt) { yield break; } // Rompe el ciclo si dejamos de mirar

            elapsedTime += Time.deltaTime;
            if (radialImage != null)
            {
                radialImage.fillAmount = Mathf.Clamp01(elapsedTime / fillTime);
            }
            yield return null; // Espera al siguiente frame
        }

        // Si el ciclo termina con éxito (pasaron los 3 segundos)
        CompletarInteraccion();
    }
}*/