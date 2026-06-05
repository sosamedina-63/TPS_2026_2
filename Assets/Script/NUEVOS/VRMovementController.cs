using UnityEngine;

public class VRMovementController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform vrCamera;            // Asigna aquí la Main Camera del visor VR
    public ArduinoSerial arduinoSource;   // Asigna el GameObject que tiene el script de Arduino

    [Header("Configuración de Movimiento")]
    public float speed = 4.0f;            // Velocidad de desplazamiento del jugador

    private CharacterController controller;
    private float gravity = 9.81f;        // Gravedad simple para mantener al jugador en el suelo

    void Start()
    {
        // Obtenemos de forma automática el componente CharacterController acoplado
        controller = GetComponent<CharacterController>();

        if (arduinoSource == null)
        {
            Debug.LogError("Falta asignar la referencia al script ArduinoSerial en el inspector.");
        }

        if (vrCamera == null)
        {
            // Intenta buscar la cámara principal si no se asignó manualmente
            vrCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        if (arduinoSource == null || !controller.enabled) return;

        // 1. Obtener los vectores de dirección de la cámara VR
        Vector3 forward = vrCamera.forward;
        Vector3 right = vrCamera.right;

        // 2. Proyectar el movimiento únicamente en el plano horizontal (XZ)
        // Esto evita que el jugador vuele si mira hacia arriba o se entierre si mira hacia abajo
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // 3. Calcular el vector de dirección combinando la orientación con los ejes del Joystick
        // joystickY controla avance/retroceso (forward) y joystickX controla laterales (right)
        Vector3 moveDirection = (forward * arduinoSource.joystickY) + (right * arduinoSource.joystickX);

        // 4. Aplicar una gravedad básica para que el personaje no flote al bajar rampas o escalones
        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // 5. Ejecutar el movimiento a través del CharacterController
        controller.Move(moveDirection * speed * Time.deltaTime);
    }
}