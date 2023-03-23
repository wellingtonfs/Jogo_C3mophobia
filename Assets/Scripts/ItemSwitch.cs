using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSwitch : MonoBehaviourPun
{
    public int SetOn = 0;
    public GameObject ui1;
    public GameObject ui2;


    [SerializeField] PhotonView MyView;

    // Start is called before the first frame update

    void Update()
    {
        if (MyView.IsMine)
        {
            
            MyView.RPC("SelectItem", RpcTarget.AllBuffered);


        }
    }

    [PunRPC]
    void SelectItem()
    {
        if(SetOn == 1)
        {

        }
        else if(SetOn == 2)
        {
         
        }
        else if(SetOn == 3)
        {
            
        }

        else
        {
            
        }

        
        
    }
        
          
}
