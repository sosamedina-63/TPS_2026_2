using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            VRGameManager.Instance.IntentarEscapar();
        }
    }
}