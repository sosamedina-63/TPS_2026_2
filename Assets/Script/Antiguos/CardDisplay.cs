using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public TMP_Text displayText;
    public FirestoreInicialize cardAccess; 


    void Start()
    {
        cardAccess = GameObject.FindGameObjectWithTag("BD").GetComponent<FirestoreInicialize>();
        
    }

    public void OnTargetFound(Transform imageTargetTrasform) 
    {
        string cardName = imageTargetTrasform.name;
        Debug.Log($"Carta encontrada {cardName}");

        displayText = imageTargetTrasform.Find("Text").GetComponent<TextMeshPro>();
        if (displayText != null)
        {
            //Actividad 3: Recuperar datos desde firestore 
            cardAccess.FetchCardDataFromFirestore(cardName, displayText);
        }
        else 
        {
            Debug.LogError("Objeto no encontrado");
        }
    }
    public void OnTargetLost(Transform imageTargetTrasform)
    {

        displayText = imageTargetTrasform.Find("Text").GetComponent<TextMeshPro>();
        if (displayText != null)
        {
            displayText.text = "Buscando carta ... ";
        }
        else
        {
            Debug.LogError("Objeto no encontrado");
        }
    }

    void Update()
    {
        
    }
}
