using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhantomUtils : MonoBehaviourPun
{
    GameObject modelPhantom;
    PhantomConsts consts;

    bool lastStatus = false;

    void Start()
    {
        modelPhantom = transform.GetChild(1).gameObject;
        consts = GetComponent<PhantomConsts>();
    }

    [PunRPC]
    void setVisibilidade(bool status)
    {
        modelPhantom.SetActive(status);
    }

    public void SetVisibilidade(bool status)
    {
        this.photonView.RPC("setVisibilidade", RpcTarget.All, status);

        //status = true;

        //if (status == lastStatus) return;

        //lastStatus = status;

        //this.photonView.RPC("setVisibilidade", RpcTarget.AllViaServer, status);
    }

    // obter distancia
    public float GetDistancia(Vector3 pos1, Vector3 pos2)
    {
        Vector3 diff_dist = pos1 - pos2;
        float sqrLen = diff_dist.sqrMagnitude;

        return sqrLen;
    }

    // verificar se alguma ação deve ser tomada
    // retorna True quando deu certo 
    public bool RodarDado(float prob)
    {
        return prob > Random.Range(0.0001f, 0.9999f); //0
    }

    public bool JogarMoeda()
    {
        float prob = Random.Range(0f, 1f);
        return prob > 0.5f;
    }

    public T ChoiceInVec<T>(T[] values)
    {
        int idx = IntAleatorio(0, values.Length - 1);

        return values[idx];
    }

    public T ChoiceInList<T>(List<T> values)
    {
        int idx = IntAleatorio(0, values.Count - 1);

        return values[idx];
    }

    public float GetProbAction(float pTempo, float pRaiva, float pAmbiente)
    {
        float tempoRaiva = Mathf.Max(pTempo, pRaiva);

        float prob = Mathf.Max(consts.prob, 0.001f);

        float probAcao = (Mathf.Pow(tempoRaiva, consts.grau) + (pAmbiente * tempoRaiva * consts.pesoAmbiente)) / (1.0f / prob);

        return Mathf.Clamp(probAcao, 0f, consts.probMax);
    }

    public int RandInt(int start, int max)
    {
        return IntAleatorio(start, max);
    }

    static public Vector3 GetPosAleatoria()
    {
        int size = PhantomConsts.spawns.Length;

        return PhantomConsts.spawns[IntAleatorio(0, size - 1)];
    }

    static public int IntAleatorio(int start, int length)
    {
        float prob = Random.Range(0f, 1f);

        return start + Mathf.RoundToInt(prob * (length - start));
    }
}
