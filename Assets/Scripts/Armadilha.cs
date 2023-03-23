using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Armadilha : MonoBehaviourPun,IPunObservable
{
    public Camera fpcCam;
    public float contagem;
    private Vector3 escala;
    [SerializeField]
    private GameObject armadilha;

    [SerializeField]
    public GameObject obj;

    public GameObject objetoAtual;

    [SerializeField] PhotonView MyView;
    // Start is called before the first frame update
    void Start()
    {
        escala = new Vector3(0.2f, 0.2f, 0.2f);
        armadilha.transform.localScale = escala;
        contagem = GameObject.Find("ItemController").GetComponent<Controller>().contagem;

    }

    // Update is called once per frame
    void Update()
    {
        if (MyView.IsMine)
        {
            if (objetoAtual != null)
            {
                MyView.RPC("moveObj", RpcTarget.All);
            }
            if (obj.activeSelf != true)
            {
                Destroy(objetoAtual);
                objetoAtual = null;
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                MyView.RPC("placeObj", RpcTarget.All);
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                MyView.RPC("soltaritem", RpcTarget.All);
            }
        }
                
    }

    [PunRPC]
    void placeObj()
    {
        if (objetoAtual != null)
        {
            Destroy(objetoAtual);
        }
        else
        {
            objetoAtual = Instantiate(armadilha);
            //objetoAtual.AddComponent<BoxCollider>();
            //objetoAtual.transform.GetComponent<BoxCollider>().size = new Vector3(5, 2, 5);
            //objetoAtual.transform.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
            //objetoAtual.transform.GetComponent<BoxCollider>().isTrigger = false;
            //objetoAtual.transform.GetComponent<BoxCollider>().enabled = false;
        }
    }

    [PunRPC]
    void moveObj()
    {
        RaycastHit hitInfo;


        if (Physics.Raycast(fpcCam.transform.position, fpcCam.transform.forward, out hitInfo))
        { 
            
            objetoAtual.transform.position = hitInfo.point;
            objetoAtual.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);

        }

    }

    [PunRPC]
    void soltaritem()
    {
        if (contagem < 5)
        {
            //objetoAtual.transform.GetComponent<BoxCollider>().enabled = true;
            contagem += 1;
            GameObject.Find("ItemController").GetComponent<Controller>().contagem += 1;
            GameObject.FindGameObjectWithTag("Phantom").GetComponent<PhantomAction>().Armadilha(objetoAtual.GetPhotonView().ViewID, objetoAtual.transform.position);
            objetoAtual = null;
        }
        else
        {
            Destroy(objetoAtual);
        }
        
    }

    [PunRPC]
    public void die()
    {
        Destroy(objetoAtual);
        objetoAtual = null;
    }


    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (stream.IsReading)
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();

        }
    }
}
