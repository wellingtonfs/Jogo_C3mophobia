using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using UnityEngine.UI;

public class Player : MonoBehaviourPun,IPunObservable
{

    public bool morto = false;

    [SerializeField]
    private LayerMask pickableLayerMask;

    [SerializeField]
    private Transform playerCameraTransform;

    [SerializeField]
    private GameObject GameUi;

    [SerializeField]
    [Min(1)]
    private float hitRange = 3;

    [SerializeField]
    private Transform pickUpParent;



    public GameObject item;

    private string inHandItem;
    private string bolsa;


    private RaycastHit hit;

    [SerializeField] PhotonView MyView;


    private void Update()
    {
        if (MyView.IsMine && !morto)
        {

            Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.red);

            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask))
            {
                GameUi.SetActive(true);
            }
            else
            {
                GameUi.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask))
                {
                    if (hit.collider.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                    {
                        base.photonView.RPC("Interact", RpcTarget.All);
                        base.photonView.RPC("setparent", RpcTarget.All);
                    }
                    else
                    {
                        hit.collider.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
                    }
                }
                
                
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                MyView.RPC("Drop", RpcTarget.All);
            }
            
        }
    }


    public void morrer()
    {
        if (base.photonView.IsMine)
        {
            base.photonView.RPC("Drop", RpcTarget.All);
            base.photonView.RPC("MeshR", RpcTarget.All);
        }
        

    }

    [PunRPC]
    public void MeshR()
    {
        transform.parent.gameObject.transform.GetChild(3).gameObject.SetActive(false);
        morto = true;

    }

    [PunRPC]
    private void Drop()
    {
        if (inHandItem != null)
        {
            transform.GetChild(0).GetComponent<ItemSwitch>().SetOn = 0;
            PhotonNetwork.Instantiate(bolsa, transform.position, transform.rotation);
            PhotonNetwork.Destroy(item);
            item = null;
            inHandItem = null;
            bolsa = null;
            
        }
    }
    [PunRPC]
    private void Interact()
    {
        
            if (hit.collider != null && inHandItem == null)
            {

                if (hit.collider.name.Remove(hit.collider.name.Length - 7) == "bolsa arma")
                {
                    item = (GameObject)PhotonNetwork.Instantiate("pistol", transform.GetChild(0).GetChild(1).position, transform.GetChild(0).GetChild(1).rotation);
                    MyView.transform.GetComponent<Player>().inHandItem = "Pistol(Clone)";

                }

                if (hit.collider.name.Remove(hit.collider.name.Length - 7) == "bolsa cruz")
                {
                    item = (GameObject)PhotonNetwork.Instantiate("croix T", transform.GetChild(0).GetChild(2).position, transform.GetChild(0).GetChild(2).rotation);
                    inHandItem = "croix T(Clone)";
                }

                if (hit.collider.name.Remove(hit.collider.name.Length - 7) == "bolsa armadilha")
                {
                    item = (GameObject)PhotonNetwork.Instantiate("scroll", transform.GetChild(0).GetChild(3).position, transform.GetChild(0).GetChild(3).rotation);
                    inHandItem = "scroll(Clone)";
                }

                

                bolsa = hit.collider.name.Remove(hit.collider.name.Length - 7);
                PhotonNetwork.Destroy(hit.collider.gameObject);
                

            }
            if (hit.collider != null)
            {
                if (hit.collider.name.Remove(hit.collider.name.Length - 7) == "pedra azul")
                {
                    PhotonNetwork.Instantiate("Item Azul", new Vector3(348, 196, 0), Quaternion.identity);
                    PhotonNetwork.Destroy(hit.collider.gameObject);
                }

                if (hit.collider.name.Remove(hit.collider.name.Length - 7) == "pedra vermelha")
                {
                    PhotonNetwork.Instantiate("Item Vermelho", new Vector3(348, 196, 0), Quaternion.identity);
                    PhotonNetwork.Destroy(hit.collider.gameObject);
                }
                if (hit.collider.name.Remove(hit.collider.name.Length - 7) == "pedra amarela")
                {
                    PhotonNetwork.Instantiate("Item Amarelo", new Vector3(348, 196, 0), Quaternion.identity);
                    PhotonNetwork.Destroy(hit.collider.gameObject);
                }
            }   
            


    }


    [PunRPC]
    void enableUI(GameObject cor)
    {
        cor.SetActive(true);
    }


    [PunRPC]
    void setparent()
    {
        if (GameObject.Find("Pistol(Clone)"))
        {
            item = GameObject.Find("Pistol(Clone)");
        }
        if (GameObject.Find("croix T(Clone)"))
        {
            item = GameObject.Find("croix T(Clone)");
            if (base.photonView.IsMine)
            {
                item.transform.GetChild(2).gameObject.SetActive(true);
            }
        }
        if (GameObject.Find("scroll(Clone)"))
        {
            item = GameObject.Find("scroll(Clone)");
        }

        Debug.Log(inHandItem);
        if(item != null)
        {
            item.transform.parent = MyView.transform.GetChild(0);
        }
        

    }



    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.GetChild(0).transform.GetChild(1).transform.gameObject.activeSelf);
            stream.SendNext(transform.GetChild(0).transform.GetChild(2).transform.gameObject.activeSelf);
            stream.SendNext(transform.GetChild(0).transform.GetChild(3).transform.gameObject.activeSelf);
        }
        else if (stream.IsReading)
        {
            transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive((bool)stream.ReceiveNext());
            transform.GetChild(0).transform.GetChild(2).transform.gameObject.SetActive((bool)stream.ReceiveNext());
            transform.GetChild(0).transform.GetChild(3).transform.gameObject.SetActive((bool)stream.ReceiveNext());

        }
    }

}
