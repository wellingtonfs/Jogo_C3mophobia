using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuzMundo : MonoBehaviourPun
{
    // Start is called before the first frame update
    bool started = false;
    public float speed = 0.5f;
    Quaternion targetRotation;

    void Start()
    {
        targetRotation = Quaternion.Euler(-90f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!started) return;

        base.photonView.RPC("cooldown", RpcTarget.AllBuffered);
        
    }

    [PunRPC]
    void cooldown()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);

        if (Mathf.Abs(transform.rotation.x - targetRotation.x) < 0.1f)
        {
            started = false;
            this.enabled = false;
            //GetComponent<Light>().enabled = false;
        }
    }

    public void Iniciar()
    {
        started = true;
    }
}
