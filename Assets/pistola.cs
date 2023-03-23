using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class pistola : MonoBehaviourPun,IPunObservable
{
    public float cooldownTime = 2.0f;
    private float startTime;

    public int damage = 0;
    public Camera fpcCam;
    public ParticleSystem muzzleFlash;
    // Start is called before the first frame update
    [SerializeField] PhotonView MyView;


    private void Start()
    {
        if (MyView.IsMine)
        {
            fpcCam.transform.rotation = transform.parent.parent.parent.GetChild(0).GetComponent<Camera>().transform.rotation;
        }
    }
    private void Update()
    {
        if (MyView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if(Time.time >= startTime + cooldownTime)
                {
                    startTime = Time.time;
                    MyView.RPC("Shoot", RpcTarget.All);
                }
                
            }
        }
        
    }

    


    [PunRPC]
    void Shoot()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        if (Physics.Raycast(fpcCam.transform.position, fpcCam.transform.forward, out hit))
        {
            Debug.Log(hit.transform.name);
            if (GameObject.Find("Item Azul(Clone)") && GameObject.Find("Item Amarelo(Clone)") && GameObject.Find("Item Vermelho(Clone)") && hit.transform.name == "Phantom(Clone)")
            {   
                hit.transform.GetComponent<WalkingDead>().loseHealth();
            }
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(fpcCam.transform.position);
            stream.SendNext(fpcCam.transform.rotation);
        }
        else if (stream.IsReading)
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            fpcCam.transform.position = (Vector3)stream.ReceiveNext();
            fpcCam.transform.rotation = (Quaternion)stream.ReceiveNext();

        }
    }
}
