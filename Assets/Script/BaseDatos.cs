using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database; // Librería obligatoria del curso para Realtime Database

public class BaseDatos : MonoBehaviour
{
    // Variable para guardar la referencia a la base de datos
    private DatabaseReference referenciaDB;

    void Start()
    {
        // Inicializamos la referencia apuntando a la raíz de tu base de datos en Firebase
        referenciaDB = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("Conexión a Firebase lista.");
    }

    // Creamos una clase pequeña para estructurar los datos que vamos a enviar
    // Serializable es necesario para que Unity lo pueda convertir a JSON
    [System.Serializable]
    public class DatosPartida
    {
        public string nombre_jugador;
        public int tiempo_sobrante;
        public string resultado;

        // Constructor para asignar los valores fácilmente
        public DatosPartida(string nombre, int tiempo, string res)
        {
            nombre_jugador = nombre;
            tiempo_sobrante = tiempo;
            resultado = res;
        }
    }

    // Este método lo llamaremos cuando el jugador gane o pierda
    public void GuardarPartida(string jugador, int tiempo, string estadoJuego)
    {
        // 1. Creamos un nuevo objeto con los datos de esta partida
        DatosPartida nuevaPartida = new DatosPartida(jugador, tiempo, estadoJuego);

        // 2. Convertimos ese objeto a texto en formato JSON
        string json = JsonUtility.ToJson(nuevaPartida);

        // 3. Generamos un ID único automático para no sobreescribir partidas anteriores
        string idUnico = referenciaDB.Child("HistorialVR").Push().Key;

        // 4. Subimos el JSON a la base de datos en la ruta: HistorialVR -> idUnico
        referenciaDB.Child("HistorialVR").Child(idUnico).SetRawJsonValueAsync(json);

        Debug.Log("Datos enviados a Firebase: Jugador " + jugador + " - Resultado: " + estadoJuego);
    }
}