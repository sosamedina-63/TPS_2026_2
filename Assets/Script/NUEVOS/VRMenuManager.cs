using UnityEngine;
using UnityEngine.SceneManagement;

public class VRMenuManager : MonoBehaviour
{
    [Header("Contenedores de Interfaz")]
    public GameObject panelPrincipal;
    public GameObject panelInstrucciones;

    [Header("Configuración de Nivel")]
    public string nombreEscenaJuego = "PROYECTO"; 

    void Start()
    {
       
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
       
        SceneManager.LoadScene(nombreEscenaJuego);
    }
}