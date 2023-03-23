using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Controller : MonoBehaviourPun
{
    public float staminaMax;
    public float current;
    public float contagem;
    // Start is called before the first frame update
    private void Awake()
    {
        staminaMax = 100f;
        current = 100f;
        contagem = 0;
    }
    void Start()
    {
        if (base.photonView.IsMine)
        {
            base.photonView.TransferOwnership(PhotonNetwork.MasterClient);
        }
    }


}
