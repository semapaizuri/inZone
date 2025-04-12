using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballSc : MonoBehaviour
{
    public AudioSource groundhitS;
    public AudioSource rimhitS;
    public AudioSource dribbleS;

    public bool ballInHands;

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("ground"))
        {
            groundhitS.Play();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("rim"))
        {
            rimhitS.Play();
        }
        if (other.gameObject.CompareTag("ground"))
        {
            dribbleS.Play();
        }
    }

}
