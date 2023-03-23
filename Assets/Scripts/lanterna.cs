using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using ClipperLib;

public class lanterna : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView MyView;
    private int ativado = 0;

    
    private void Update()
    {
        if (MyView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (ativado == 0)
                {
                    MyView.RPC("activateItem", RpcTarget.All, true);
                    ativado = 1;
                }
                else
                {
                    MyView.RPC("activateItem", RpcTarget.All, false);
                    ativado = 0;
                }
                
            }
        }
        
    }

    [PunRPC]
    void activateItem(bool active) 
    {
        if (active) 
        {
            transform.GetChild(4).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(4).gameObject.SetActive(true);
        }
        

    }


}
