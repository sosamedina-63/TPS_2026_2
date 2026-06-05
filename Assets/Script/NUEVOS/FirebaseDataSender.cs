using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// Estructura de datos compleja (JSON) para nuestra base de datos
[System.Serializable]
public class PlayerData
{
    public string nombre;
    public int edad;
    public int tiempoSegundos;
    public string estadoPartida;
}

public class FirebaseDataSender : MonoBehaviour
{
    [Header("Configuración de Firebase")]
    [Tooltip("Pega aquí la URL de tu base de datos, asegurándote de que termine en /")]
    public string databaseURL = "https://TU-PROYECTO-default-rtdb.firebaseio.com/"; 

    [Header("Perfil del Jugador")]
    public string playerName = "Gustavo"; 
    public int playerAge = 21;            

    // Este método público será llamado por el VRGameManager al terminar la partida
    public void SendGameData(int finalTime, string resultStatus)
    {
        // 1. Instanciamos y poblamos nuestro objeto con los datos
        PlayerData data = new PlayerData();
        data.nombre = playerName;
        data.edad = playerAge;
        data.tiempoSegundos = finalTime;
        data.estadoPartida = resultStatus;

        // 2. Convertimos el objeto a una cadena JSON (Serialización)
        string json = JsonUtility.ToJson(data);

        // 3. Iniciamos la corrutina asíncrona para no congelar el juego mientras se sube
        StartCoroutine(UploadToFirebase(json));
    }

    private IEnumerator UploadToFirebase(string jsonData)
    {
        // Usamos el nodo partidas y añadimos .json al final (requisito de la API REST de Firebase)
        string url = databaseURL + "partidas.json";

        // Usamos POST para que Firebase genere una clave única automática para cada registro
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Esperamos a que el servidor reciba la información
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error en la conexión con la base de datos: " + request.error);
            }
            else
            {
                Debug.Log("Registro guardado con éxito. Respuesta de Firebase: " + request.downloadHandler.text);
            }
        }
    }
}