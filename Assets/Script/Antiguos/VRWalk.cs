using UnityEngine;

public class VRWalk : MonoBehaviour
{
    //Atributos / Variables(minusculas: propio de la clase) de clase 

    public Transform vrCamara; 
    public float angulo = 30.0f;
    public float speed = 3.0f; 
    public bool move;

    private CharacterController controller;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (vrCamara.eulerAngles.x >= angulo && vrCamara.eulerAngles.x < 60.0f)
        {
            move = true;
        }
        else
        {
            move = false;
        }
        if (move) 
        {
            Vector3 direccion = vrCamara.TransformDirection(Vector3.forward);//FORWARD nos indica la direccion(vector) hacia adelante.
            controller.SimpleMove(direccion*speed);//Muevete en la direccion del vector a una velocidad , incremento vectorial en la direccion que le puse.
        }
    }
}
