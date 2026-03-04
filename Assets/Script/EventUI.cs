using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;

public class EventUI : MonoBehaviour
{
    public List<GameObject> listaInstrucciones;
    public int currentIndex = 0;
    public List<string> mensajesInstrucciones;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        //Actualizar visibilidad de paneles
        updateVisibility();
    }

    void Update()
    {
        
    }

    //Metodo para activar visualizacion de paneles

    private void updateVisibility()
    {
        for (int i = 0;i < listaInstrucciones.Count;i++)
        {
            //Solo e panel en el indice actual esta activo
            listaInstrucciones[i].SetActive(i == currentIndex);
        }
    }

    //Metodo para cambiar escena

    public void ChangeSceneByIndex(int sceneIndex) 
    { 
        SceneManager.LoadScene(sceneIndex);
    }

    //Metodo para cambiar escena por nombre

    public void ChangeSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //Metodo para cambiar entre paneles

    public void CycleObjets()
    {
        //Incrementa el indice y vuelve al principio+
        currentIndex = (currentIndex + 1) % listaInstrucciones.Count;

        //Actualizar la visibilidad

        updateVisibility();
    }

    //Metodo para actualizar el texto mostrado

    private void UpdateTex() 
    { 
        if (mensajesInstrucciones.Count>0) 
        { 

        }
    }

    //Metodo ara salir d ela aplicacion

    public void ExitGame() 
    {
        Debug.Log("Va a salir");
        Application.Quit();
        Debug.Log("Ya salio");
    }
}
