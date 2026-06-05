using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement; 

public class MonsterBehavior : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player; // El objetivo a perseguir

    [Header("Configuración")]
    public float catchDistance = 2f; // A qué distancia te atrapa
    public float speed = 2.5f;       // Velocidad (debe ser menor a la del jugador)

    private NavMeshAgent agent;
    private bool isMonsterActive = false;
    private float catchDistanceSqr;

    void Start()
    {
        // Obtenemos el componente de navegación automáticamente
        agent = GetComponent<NavMeshAgent>();
        
        if (agent != null)
        {
            agent.speed = speed;
            // El monstruo empieza completamente detenido
            agent.isStopped = true; 
        }

        // Optimizamos el cálculo de distancia como se vio en la práctica (sin usar Vector3.Distance)
        catchDistanceSqr = catchDistance * catchDistance; 
    }

    void Update()
    {
        // Si no está activo o falta el jugador, no hace nada
        if (!isMonsterActive || player == null || agent == null) return;

        // Le indicamos al agente que su destino es la posición actual del jugador
        agent.SetDestination(player.position);

        // Comprobamos la distancia al cuadrado entre el monstruo y el jugador
        if ((transform.position - player.position).sqrMagnitude < catchDistanceSqr)
        {
            CatchPlayer();
        }
    }

    // Este método será llamado por el VRGameManager cuando se active el primer panel
    public void ActivateMonster()
    {
        isMonsterActive = true;
        if (agent != null) agent.isStopped = false;
        Debug.Log("¡El monstruo ha despertado!");
    }

    // Este método detiene al monstruo (útil para cuando el jugador gana y sale por la puerta)
    public void StopMonster()
    {
        isMonsterActive = false;
        if (agent != null) agent.isStopped = true;
    }

     private void CatchPlayer()
    {
        isMonsterActive = false;
        if (agent != null) agent.isStopped = true;
        
        // Delegamos el fin del juego al Manager
        VRGameManager.Instance.GameOver(); 
    }
}