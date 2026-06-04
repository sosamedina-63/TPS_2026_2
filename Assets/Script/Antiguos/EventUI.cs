//using NUnit.Framework;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.Universal;

public class EventUI : MonoBehaviour
{
    public List<GameObject> listaInstrucciones; //GameObject las intancias que ya estan en el juego 
    public int currentIndex = 0; //Es el indice actual
    public List<string> mensajesInstrucciones; //Lista para ir cambiando un cadena de textos(lista de objetos que tiene) 
    public TextMeshProUGUI textMeshProUGUI;   //Almacena unicamente el componente del texto 
    
    //Se ejecita antes de start, permite guardar todas las cofiguraciones que quiero guardar despues (mantener entre scene)
    private void Awake() 
    {
        DontDestroyOnLoad(this.gameObject); //Cuanda la clase esta haciendo una intancia dentro de la clase
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Actualizar visibilidad de paneles 
        UpdateVisibility();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Metodo para actualizar visibilidad de paneles 
    private void UpdateVisibility() 
    {
        for (int i=0; i < listaInstrucciones.Count;i++) //Contamos a partir del primer elemento que es el indice 0 por lo que es solo <
        {
            //Solo el panel en el indice actual esta activo
            listaInstrucciones[i].SetActive(i == currentIndex); //Para poder modificar la visibilidad del panel SetActive
        }
    }

    //Metodo para cambiar entre paneles 
    public void CycleObject(int direction) 
    {
       //Modulo me permite establecer un rango (si la divicion es perfecta), incrementa el indice y vuelve al principio
       currentIndex = (currentIndex + direction + listaInstrucciones.Count) % listaInstrucciones.Count;

        //Actualizar la visibilidad
        UpdateVisibility();
    }

    //Metodo para actualizar el texto mostrado
    private void UpdateText() 
    {
        //Si tienes mensajes y un objeto donde ponerlo vas actualizar el texto
        if (mensajesInstrucciones.Count > 0 && textMeshProUGUI != null) 
        {
            textMeshProUGUI.text = mensajesInstrucciones[currentIndex];
        }
    }
    public void CycleText(int direction)
    {
        //Modulo me permite establecer un rango (si la divicion es perfecta), incrementa el indice y vuelve al principio
        currentIndex = (currentIndex + direction + mensajesInstrucciones.Count) % mensajesInstrucciones.Count;

        //Actualizar la visibilidad
        UpdateText();
    }



    //Metodo para cambiear de escena (indice y nombre)
    //INDICE
    public void ChangeSceneByIndex(int sceneIndex) 
    {
        SceneManager.LoadScene(sceneIndex);
    }
                    //NOMBRE
    public void ChangeSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //Metodo para cambiar la escena actual 
    public void ReloadCurrentScene() 
    {
        //Guarda en un variable la escena actual en la que nos encontramos en una variable del tipo escena
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    //Metodo para salir de la aplicacion
    public void ExitGame() 
    {
        Debug.Log("Va a salir");
        Application.Quit();
        Debug.Log("Ya salio");
    }
}
