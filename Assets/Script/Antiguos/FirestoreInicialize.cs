using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Threading.Tasks;
using UnityEngine.Networking;//Networking permite crear objetos para mandarlos a una URL HTML y recibir su respuesta 
using System;
using UnityEngine.SceneManagement;//Permite los cambios de escena


public class FirestoreInicialize : MonoBehaviour
{
    private static FirebaseFirestore firestore;
    [SerializeField]
    private TMP_InputField cardNameInput;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        firestore = FirebaseFirestore.DefaultInstance; //Buscanr dentro del archivo como inicializar la base de datos
    }

    public void FetchAndStoreCardData()//Meto que tienes que tienes que mandar a llamar para hacer la solicutud en la pagina WEB
    {
        string cardName = cardNameInput.text;
        StartCoroutine(GetCardData(cardName));
    }

    //Actividad 1 
    private IEnumerator GetCardData(string cardName) //El metodo va a ser una corrutina y IEnumerator me pone caulquier accion numerable(cuenta y da los resultados)
    {
        string url = $"https://db.ygoprodeck.com/api/v7/cardinfo.php?name={UnityWebRequest.EscapeURL(cardName)}";//La cadena la tranforma en un codigo de escape que puede leer la URL en el sitio WEB
        //Crea el objeto que tiene la URL para la solicitud 
        UnityWebRequest request = UnityWebRequest.Get(url);
        //Se queda aquí hasta que reciba una solcitud
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            ProcessCardData(json);
        }
        else 
        {
            Debug.LogError("Error obteniendo datos: "+request.error);
        }
    }

    private void ProcessCardData(string json) 
    {
        //Parsero JSON data
        var cardData = JsonUtility.FromJson<CardDataResponse>(json);//Desde un formato tipo JSON a el arreglo que obtuvios 

        if (cardData.data.Length > 0) //Ahora la vamos dividir en cada elemento 
        {
            var card = cardData.data[0];
            string cardType = card.type;

            //Estructura en firestore 
            string collection = DetermineCollection(cardType);//Determinar la coleccion de la data 

            string cardName = card.name;

            //Crear documento de referencia basado en el tipo de carta y su nombre 
            DocumentReference documentReference = firestore
                .Collection("Cartas")
                .Document(collection)
                .Collection(cardName)
                .Document("Datos");

            //Preparar datos para guardar en Firestore 
            var cardInfo = new Dictionary<string, object>
            {
                {"ATK", card.atk},
                {"DEF", card.def},
                {"Desc", card.desc},
                {"Level", card.level}
            };

            //Guadar datos en Firestore 
            documentReference.SetAsync(cardInfo).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"Datos de {cardName} almacenados correctamente");
                }
                else
                {
                    Debug.LogError("Error guardando datos" + task.ToString());
                }
            });
        }
        else 
        {
            Debug.LogError("Carta no encontrada carta no encontrada ");
        }
    }
    public void TxtAR() 
    {
        SceneManager.LoadScene("AR");
    }

    public void FetchCardDataFromFirestore(string cardName, TMP_Text textMesh) 
    {
        FetchCardData(cardName,textMesh);
    }

    private async void FetchCardData(string cardName, TMP_Text textMesh) 
    {
        textMesh.text = "Buscando datos...";

        List<string> collections = new List<string> { "Mounstro", "Magia", "Trampa", "Otros"};
        List<Task<DocumentSnapshot>> tasks = new List<Task<DocumentSnapshot>>(); //L

        //Iniciar todas las consultas 
        foreach (string collection in collections) 
        {
            DocumentReference docRef = firestore.Collection("Cartas")
                .Document(collection).Collection(cardName).Document("Datos");

            //Le pido a mi tarea que reciba eso datos
            tasks.Add(docRef.GetSnapshotAsync());
        }

        try 
        {
            //Esperar a que todas las consultas terminen
            DocumentSnapshot[] snapshots = await Task.WhenAll(tasks);

            //Buscar la primera que tenga datos
            foreach(var snapshot in snapshots)
            {
                if (snapshot.Exists) 
                {
                    Dictionary<string, object> cardData = snapshot.ToDictionary();
                    textMesh.text = $"Nombre {cardName} \n" +
                                    $"ATK {cardData["ATK"]} \n" +
                                    $"DEF {cardData["DEF"]} \n" +
                                    $"Description {cardData["Desc"]} \n" +
                                    $"Nivel {cardData["Level"]}";
                    return;
                }
            }

            //Si ninguna tiene datos
            Debug.Log("Carta no encontrada");
            textMesh.text = "Carta no encontrada";

        }
        catch (Exception ex) 
        {
            Debug.LogError("Error al buscar la carta: "+ ex.Message);
            textMesh.text = "Error al buscar la carta: " + ex.Message;
        }
    }

    public string DetermineCollection(string cardType)
    {
        if (cardType.Contains("Monster")) 
        {
            return "Mounstro";
        }
        if (cardType.Contains("Spell"))
        {
            return "Magia";
        }
        if (cardType.Contains("Trap"))
        {
            return "Trampa";
        }
        else 
        {
            return "Otros";
        }
    }

    void Start()
    {
       
    }
    void Update()
    {
        
    }
}


//Clase para mapear las repuestas tipo JSON 
[System.Serializable]
public class CardDataResponse 
{
    public CardData[] data;//El arreglo que le llegara a la base de datos
}

[System.Serializable]
//Datos de la carta 
public class CardData 
{
    //Los nombres tienen que ser iguales a los que aparecen en la respuesta del servicio WEB
    public string name;
    public string type;
    public string desc;
    public int atk;
    public int def;
    public int level;

}
