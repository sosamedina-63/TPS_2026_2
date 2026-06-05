using UnityEngine;
using UnityEngine.SceneManagement; // Librería obligatoria para cambiar de escenas

public class VRMenuManager : MonoBehaviour
{
    [Header("Contenedores de Interfaz")]
    public GameObject panelPrincipal;
    public GameObject panelInstrucciones;

    [Header("Configuración de Nivel")]
    public string nombreEscenaJuego = "PROYECTO"; // IMPORTANTE: Pon aquí el nombre exacto de tu escena de la fábrica

    void Start()
    {
        // Estado inicial por defecto: Mostrar menú, ocultar instrucciones
        MostrarMenuPrincipal();
    }

    public void MostrarInstrucciones()
    {
        panelPrincipal.SetActive(false);
        panelInstrucciones.SetActive(true);
    }

    public void MostrarMenuPrincipal()
    {
        panelInstrucciones.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    public void IniciarJuego()
    {
        // Carga la escena de la actividad principal
        SceneManager.LoadScene(nombreEscenaJuego);
    }
}