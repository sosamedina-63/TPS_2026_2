using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Anim : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        //Ejemplo de buscar si fuera general (controlador)
        //animator = GameObject.FindGameObjectWithTag("Dragón").GetComponent<Animator>();

    }

    public void PlayAnim()
    {
        animator.enabled = true;
        animator.Play("AnimFight");
    }
    public void StopAnim() 
    {
        animator.enabled = false;
    }

    void Update()
    {
        
    }
}
