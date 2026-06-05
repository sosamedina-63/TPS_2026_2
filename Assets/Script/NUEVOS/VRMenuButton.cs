using UnityEngine;
using UnityEngine.UI;

public class VRMenuButton : MonoBehaviour
{
    public enum TipoBoton { Inicio, Instrucciones, Salir }
    public TipoBoton tipoAccion;

    [Header("Interacción Visual")]
    public Image radialImage;
    public float fillTime = 3f;

    [Header("Conexión al Gestor (NUEVO)")]
    public VRMenuManager menuManager; // Ahora es PÚBLICO para conexión directa

    private bool gazedAt = false;
    private float temporizador = 0f;

    void Update()
    {
        if (gazedAt)
        {
            temporizador += Time.deltaTime;

            if (radialImage != null)
            {
                radialImage.fillAmount = Mathf.Clamp01(temporizador / fillTime);
            }

            if (temporizador >= fillTime)
            {
                EjecutarAccion();
            }
        }
    }

    private void EjecutarAccion()
    {
        gazedAt = false;
        temporizador = 0f;
        if (radialImage != null) radialImage.fillAmount = 0f;

        // Solo ejecutamos si el gestor está bien conectado
        if (menuManager != null)
        {
            switch (tipoAccion)
            {
                case TipoBoton.Inicio:
                    menuManager.IniciarJuego();
                    break;
                case TipoBoton.Instrucciones:
                    menuManager.MostrarInstrucciones();
                    break;
                case TipoBoton.Salir:
                    menuManager.MostrarMenuPrincipal();
                    break;
            }
        }
        else
        {
            Debug.LogError("Error: ¡El botón no tiene asignado el MenuManager en el Inspector!");
        }
    }

    public void OnPointerEnter()
    {
        gazedAt = true;
    }

    public void OnPointerExit()
    {
        if (!gameObject.activeInHierarchy) return;

        gazedAt = false;
        temporizador = 0f;
        if (radialImage != null) radialImage.fillAmount = 0f;
    }
}