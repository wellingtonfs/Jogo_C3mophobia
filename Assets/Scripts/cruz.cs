using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class cruz : MonoBehaviourPun,IPunObservable
{
    private int ativado = 0;

    private bool isPressed;
    public ParticleSystem holyLight;
    public Slider Stamina1;
    public Slider Stamina2;

    public float maxStamina;
    public float currentStamina;

    private WaitForSeconds regenTick = new WaitForSeconds(0.1f);
    private Coroutine isregen;

    GameObject phantom;

    [SerializeField] PhotonView MyView;
    // Start is called before the first frame update
    void Start()
    {
        if (MyView.IsMine)
        {
            maxStamina = GameObject.Find("ItemController").GetComponent<Controller>().staminaMax;
            currentStamina = GameObject.Find("ItemController").GetComponent<Controller>().current;
            transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(true);
            Stamina1.maxValue = maxStamina;
            Stamina2.maxValue = maxStamina;

            Stamina1.value = maxStamina;
            Stamina2.value = maxStamina;

            MyView.RPC("disableItem", RpcTarget.AllBuffered);
        }

    }

    // Update is called once per frame


    private void Update()
    {
        if (MyView.IsMine)
        {
            MyView.RPC("stamina", RpcTarget.AllBuffered, 16f * Time.deltaTime);
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (Stamina1.value > 3 & Stamina2.value > 3)
                {
                    MyView.RPC("activateItem", RpcTarget.All);

                }
                else
                {
                    MyView.RPC("disableItem", RpcTarget.All);
                }
            }
            else
            {
                MyView.RPC("disableItem", RpcTarget.All);
            }
        }
        
    }



    [PunRPC]
    void activateItem()
    {
        ativado = 1;
        holyLight.Play();
        transform.GetComponent<SphereCollider>().enabled = true;

        GameObject.FindGameObjectWithTag("Phantom").GetComponent<PhantomAction>().CruzAtivada(transform.position);
    }
    [PunRPC]
    void disableItem()
    {
        ativado = 0;
        holyLight.Stop();
        transform.GetComponent<SphereCollider>().enabled = false;
    }

    [PunRPC]
    public void stamina(float value)
    {
        if (currentStamina - value >= 0)
        {
            if (ativado == 1)
            {
                if(currentStamina > 3)
                {
                    maxStamina -= 5f * Time.deltaTime;
                    GameObject.Find("ItemController").GetComponent<Controller>().staminaMax = maxStamina;

                }
                currentStamina -= value;
                GameObject.Find("ItemController").GetComponent<Controller>().current = currentStamina;
                Stamina1.value = currentStamina;
                Stamina2.value = maxStamina;
            }
            else
            {
                if (currentStamina < maxStamina)
                {
                    currentStamina += 0.5f * Time.deltaTime;
                    GameObject.Find("ItemController").GetComponent<Controller>().current = currentStamina;
                    Stamina1.value = currentStamina;
                }
            }
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += 0.5f * Time.deltaTime;
            GameObject.Find("ItemController").GetComponent<Controller>().current = currentStamina;
            Stamina1.value = currentStamina;
        }
        else if (currentStamina < 3)
        {
            holyLight.Stop();
            transform.GetComponent<SphereCollider>().enabled = false;
        }

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

    public void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            phantom = other.gameObject;
            
            if(phantom.transform.tag == "Phantom")
            {
                photonView.RPC("changpos", RpcTarget.All);
            }
            
        }
    }
    [PunRPC]
    void changpos()
    {
        phantom.transform.position = PhantomUtils.GetPosAleatoria();
    }


}