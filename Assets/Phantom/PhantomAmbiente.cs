using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomAmbiente : MonoBehaviourPun
{
    Luz[] luzes;
    PortaTrigger[] portas;
    public GameObject[] players;   //lista de players

    LuzMundo luzMundo;

    PhantomConsts consts;
    PhantomUtils utils;

    int playerTarget = -1;

    // Start is called before the first frame update
    void Start()
    {
        consts = GetComponent<PhantomConsts>();
        utils = GetComponent<PhantomUtils>();

        //Debug.Log("Menor Distancia"); //60
        //Debug.Log("Maior Distancia"); //700
    }

    public GameObject[] GetPlayers()
    {
        return players;
    }

    public PortaTrigger[] GetPortas()
    {
        return portas;
    }

    public Luz[] GetLuzes()
    {
        return luzes;
    }

    public void UpdatePlayers()
    {
        //obtendo lista de players
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public void UpdateObjetos()
    {
        //obter todas as luzes do jogo
        var luzesObj = GameObject.FindGameObjectsWithTag("Luz");

        if (luzesObj.Length > 0)
        {
            luzes = new Luz[luzesObj.Length];

            for (int i = 0; i < luzesObj.Length; i++)
            {
                luzes[i] = luzesObj[i].GetComponent<Luz>();
            }
        }

        //obter todas as portas do jogo
        var portasObj = GameObject.FindGameObjectsWithTag("Porta");

        if (portasObj.Length > 0)
        {
            portas = new PortaTrigger[portasObj.Length];

            for (int i = 0; i < portasObj.Length; i++)
            {
                portas[i] = portasObj[i].GetComponent<PortaTrigger>();
            }
        }

        //luz do mundo
        var luz = GameObject.Find("LuzMundo");

        if (luz != null)
        {
            luzMundo = luz.GetComponent<LuzMundo>();
        }
    }

    public void DesligarLuzMundo()
    {
        luzMundo.Iniciar();
    }

    // probabilidade de ações
    public float GetProb()
    {
        if (players.Length == 1)
        {
            playerTarget = 0;
            return Mathf.Clamp(1f - players[0].GetComponent<PlayerInterface>().GetSanidade(), consts.minProbAmbiente, 1f);
        }

        List<float> distancias = GetDistancias();
        List<float> insanidades = GetInsanidades();

        float maior = -1;
        List<int> targetList = new();

        for (int i = 0; i < players.Length; i++)
        {
            float media = (consts.pesoDistancia * distancias[i] + consts.pesoSanidade * insanidades[i]) / (consts.pesoDistancia + consts.pesoSanidade);

            if (Mathf.Abs(media - maior) < 0.001f)
            {
                targetList.Add(i);
            }
            else if (media > maior)
            {
                maior = media;
                targetList.Clear();
                targetList.Add(i);
            }
        }

        playerTarget = PhantomUtils.IntAleatorio(0, targetList.Count - 1);
        playerTarget = targetList[playerTarget];

        return Mathf.Clamp(maior, consts.minProbAmbiente, 1f);
    }

    public int GetIdVulneravel()
    {
        if (playerTarget < 0) throw new Exception("Nenhum player selecionado");

        return players[playerTarget].GetPhotonView().ViewID;
    }

    public GameObject GetPlayerVulneravel()
    {
        if (playerTarget < 0) throw new Exception("Nenhum player selecionado");

        return players[playerTarget];
    }

    // funções auxs

    List<float> GetDistancias()
    {
        List<float> list = new List<float>();

        for (int i = 0; i < players.Length; i++)
        {
            float menorLocal = float.PositiveInfinity;

            for (int j = 0; j < players.Length; j++)
            {
                if (i == j) continue;

                float dist = utils.GetDistancia(players[i].transform.position, players[j].transform.position);

                if (dist < menorLocal)
                {
                    menorLocal = dist;
                }
            }

            if (menorLocal < consts.minDist)
            {
                list.Add(0);
                continue;
            }

            menorLocal /= consts.maxDist;

            list.Add(Mathf.Clamp(menorLocal, 0f, 1f));
        }

        return list;
    }
    

    List<float> GetInsanidades()
    {
        List<float> list = new List<float>();

        for (int i = 0; i < players.Length; i++)
        {
            list.Add(
                1f - players[i].GetComponent<PlayerInterface>().GetSanidade()
            );
        }

        return list;
    }


}
