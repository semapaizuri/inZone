using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockChecker2 : MonoBehaviour
{

    public static bool blocked2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player1")
        {
            blocked2 = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "player1")
        {
            blocked2 = false;
        }
    }

}
