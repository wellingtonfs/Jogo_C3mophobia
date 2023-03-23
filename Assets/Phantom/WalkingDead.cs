using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WalkingDead : MonoBehaviourPun
{
    // Raiva do fantasma
    public float raiva = 0f;
    public float health = 3f;

    //variaveis

    bool started = false;

    //Scripts

    PhantomConsts consts;
    PhantomCounter counter;
    PhantomAmbiente ambiente;
    PhantomAction action;
    PhantomUtils utils;

    // Métodos

    private void Awake()
    {
        if (base.photonView.IsMine)
        {
            base.photonView.TransferOwnership(PhotonNetwork.MasterClient);
        }

        ambiente = GetComponent<PhantomAmbiente>();
        consts = GetComponent<PhantomConsts>();
        counter = GetComponent<PhantomCounter>();
        action = GetComponent<PhantomAction>();
        utils = GetComponent<PhantomUtils>();
    }

    void Start()
    {
        Debug.Log("phantom on");

        Iniciar();
    }

    void Update()
    {
        // não prosseguir até chamar a função Iniciar
        if (!photonView.IsMine) return;
        if (!started) return;

        // caso o fanstasma esteja realizando alguma ação

        if (action.InAction())
        {
            action.Do(); //update na ação

            return;
        }

        // caso nada esteja acontecendo

        if (!counter.NovoEstado()) return;

        //travar outros fantasmas

        EscolherProximaAcao();
    }

    // Métodos de Inicio

    public void Iniciar()
    {
        //desligar contagem
        started = true;

        //atualizar o fantasma com o ambiente (integração)
        ambiente.UpdateObjetos();
        ambiente.DesligarLuzMundo();

        Debug.Log("Iniciou a matança");
    }

    // Métodos gerais

    //faz algo no jogo a cada update
    void EscolherProximaAcao()
    {
        // probabilidade da próxima ação acontecer

        float pTempo = counter.GetProb();
        float pAmbiente = ambiente.GetProb();

        //Debug.Log("entradas pAmb: " + pAmbiente.ToString());
        //Debug.Log(pTempo);
        //Debug.Log(pAmbiente);

        float prob = utils.GetProbAction(pTempo, raiva, pAmbiente);

        //Debug.Log("saida: " + prob.ToString());

        if (!utils.RodarDado(prob)) return;

        //Debug.Log("AÇÃO DO FANTASMA");
        //Debug.Log(counter.timerGeral);

        int photonId = ambiente.GetIdVulneravel();
        var player = ambiente.GetPlayerVulneravel();

        string actionName = action.GetAction(pTempo, prob, player.GetComponent<PlayerInterface>().GetSanidade());
        //Debug.Log(actionName);

        action.DefineAction(actionName, player, photonId);

        //realizar alguma ação
        counter.PauseParaAcao();
    }

    [PunRPC]
    void UpdatePlayersList()
    {
        //obtendo lista de players

        ambiente.UpdatePlayers();
    }

    [PunRPC]
    public void UpdatePlayers()
    {
        base.photonView.RPC("UpdatePlayersList", RpcTarget.All);
    }

    public void AddRaiva()
    {
        raiva = Mathf.Min(raiva + 0.5f, 1f);
    }

    public void loseHealth()
    {
        health -= 1;

        if(health == 0)
        {
            PhotonNetwork.Instantiate("Vitoria", new Vector3(407, 229, 0), Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
    }


}