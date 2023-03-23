using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Luz : MonoBehaviourPun
{
    bool piscar = false;
    bool ligado = false;
    Light luz;

    float timer = 0;
    float interval = 0;
    float? timeLimit = null;

    // Start is called before the first frame update
    void Start()
    {
        luz = GetComponent<Light>();
        luz.enabled = false;
        interval = Random.Range(0.01f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!piscar) return;

        timer += Time.deltaTime;

        if (timeLimit != null)
        {
            timeLimit -= Time.deltaTime;

            if (timeLimit < 0)
            {
                timeLimit = null;
                phantomAtividade(false);
                return;
            }
        }

        if (timer > interval)
        {
            ligado = !ligado;

            if (ligado) luz.enabled = true;
            else luz.enabled = false;

            timer = 0;
            interval = Random.Range(0.05f, 0.5f);
        }

    }

    [PunRPC]
    void phantomAtividade(bool status)
    {
        if (status == piscar) return;

        if (timeLimit != null)
        {
            if (!status) timeLimit = null;
        }

        ligado = false;
        piscar = status;

        if (status)
        {
            timer = 0;
        } else
        {
            luz.enabled = false;
        }
    }

    [PunRPC]
    void piscarLamp(float tLimit)
    {
        phantomAtividade(true);
        timeLimit = tLimit;
    }

    // metodos públicos

    public void PhantomAtividade(bool status)
    {
        this.photonView.RPC("phantomAtividade", RpcTarget.AllViaServer, status);
    }

    public void Piscar(float tLimit)
    {
        this.photonView.RPC("piscarLamp", RpcTarget.All, tLimit);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 Pos()
    {
        return GetPosition();
    }
}
