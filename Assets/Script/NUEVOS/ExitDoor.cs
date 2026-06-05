using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Si el que entra en el área de la puerta es el Jugador
        if (other.CompareTag("Player"))
        {
            VRGameManager.Instance.IntentarEscapar();
        }
    }
}