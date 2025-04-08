using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballSc : MonoBehaviour
{
    public AudioSource groundhitS;
    public AudioSource rimhitS;
    public AudioSource dribbleS;
    

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "ground")
        {
            groundhitS.Play();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "rim")
        {
            rimhitS.Play();
        }
        if (other.gameObject.tag == "ground")
        {
            dribbleS.Play();
        }
    }

}
