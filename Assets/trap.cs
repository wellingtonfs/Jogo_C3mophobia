using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Diagnostics;

public class trap : MonoBehaviourPun
{
    /*
    float trapcooldown = 5f;
    PhantomUtils utils;
    GameObject phantom;

    
    void OnTriggerEnter(Collider other)
    {

        phantom = other.gameObject;
        Debug.Log(phantom.transform.name);
        if (phantom.transform.tag == "Phantom")
        {
                
            phantom.GetComponent<NavMeshAgent>().speed = 0;

            utils.SetVisibilidade(true);

            Invoke("moveAgain", trapcooldown);
            PhotonNetwork.Destroy(other.gameObject);
        }
    }
    void moveAgain()
    {
        phantom.GetComponent<NavMeshAgent>().speed = 4;
        utils.SetVisibilidade(false);
    }

    */
}
