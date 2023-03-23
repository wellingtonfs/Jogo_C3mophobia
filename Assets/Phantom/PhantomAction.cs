using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Device;
using Photon.Pun;
using UnityEngine.AI;

public class PhantomAction : MonoBehaviourPun
{
    // Start is called before the first frame update

    GameObject targetPlayer = null;
    GameObject audioObj; //voz do fantasma
    GameObject[] players = null;

    string beforeAction = null;
    string currentAction = null;

    int targetId = 0;
    bool auxBool = false;
    bool teste = false;

    UnityEngine.AI.NavMeshAgent agent;
    Dictionary<string, Action> Actions;

    AudioSource audioTelefone; //telefone tocando
    PhantomConsts consts;
    PhantomUtils utils;
    PhantomAmbiente ambiente;
    PhantomCounter counter;
    PortaTrigger auxPorta = null;
    Luz auxLuz = null;

    Vector3? auxVector = null;
    List<Tuple<int, Vector3, float>> armadilhas;

    void Start()
    {
        consts = GetComponent<PhantomConsts>();
        utils = GetComponent<PhantomUtils>();
        ambiente = GetComponent<PhantomAmbiente>();
        counter = GetComponent<PhantomCounter>();

        if (photonView.IsMine)
        {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            agent.enabled = true;
        }

        Actions = new Dictionary<string, Action>
        {
            { "MoverPorta", MoverPorta },
            { "AlterarLuz", AlterarLuz },
            { "TrocarSala", TrocarSala },
            { "MatarPlayer", MatarPlayer },
            { "TocarAudio", TocarAudio },
        };

        audioObj = GameObject.Find("AudioPhantom");
        audioTelefone = GameObject.Find("phone").GetComponent<AudioSource>();

        armadilhas = new List<Tuple<int, Vector3, float>>();
    }

    // update
    public void Do()
    {
        if (!InAction()) return;
        if (players == null)
        {
            players = ambiente.GetPlayers();
        }

        // verificar armadilhas

        if (armadilhas.Count > 0)
        {
            List<int> removes = new List<int>();

            for (int i = 0; i < armadilhas.Count; i++)
            {
                if (armadilhas[i].Item3 < counter.timerGeral)
                {
                    removes.Add(i);
                    continue;
                }

                if (utils.GetDistancia(transform.position, armadilhas[i].Item2) < 10f)
                {
                    cairArmadilha();

                    removes.Add(i);

                    break;
                }
            }

            if (removes.Count > 0)
            {
                GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");
                removes.Reverse();

                foreach (int i in removes)
                {
                    int id = armadilhas[i].Item1;

                    foreach (GameObject trap in traps)
                    {
                        if (trap.GetPhotonView().ViewID == id)
                        {
                            Destroy(trap);
                            break;
                        }
                    }
                }
            }    
        }

        if (currentAction == "armadilha") return;

        // executar ação

        Actions[currentAction]();
    }

    void mudarPos(Vector3 pos)
    {
        agent.SetDestination(pos);
    }

    void EncerrarAcao(string action)
    {
        auxPorta = null;
        auxVector = null;
        auxLuz = null;
        auxBool = false;
        currentAction = null;
        beforeAction = action;

        utils.SetVisibilidade(false);

        //PiscarLampadasProximas(false);

        Debug.Log("Encerrando " + action);
    }

    void PiscarLampadasProximas()
    {
        foreach (Luz luz in ambiente.GetLuzes())
        {
            luz.PhantomAtividade( utils.GetDistancia(transform.position, luz.GetPosition()) < consts.distLampToAction );
        }
    }

    void PiscarLampadasProximas(bool status)
    {
        foreach (Luz luz in ambiente.GetLuzes())
        {
            luz.PhantomAtividade(status);
        }
    }

    [PunRPC]
    void TocarParaTodos(Vector3 position, int acao)
    {
        if (acao == 0) //telefone
        {
            if (!audioTelefone.isPlaying) audioTelefone.Play();

            return;
        }

        audioObj.transform.position = position;

        AudioSource audio = audioObj.GetComponents<AudioSource>()[acao-1];

        if (audio.isPlaying) return;

        audio.Play();
    }

    [PunRPC]
    void BaterCoracao(int photonId)
    {
        var players = ambiente.GetPlayers();

        foreach (var player in players)
        {
            if (player.GetPhotonView().ViewID != photonId) continue;

            player.GetComponent<PlayerInterface>().Caçando(true);
        }
    }

    [PunRPC]
    void PararSomCoracao()
    {
        var players = ambiente.GetPlayers();

        foreach (var player in players)
        {
            player.GetComponent<PlayerInterface>().Caçando(false);
        }
    }

    [PunRPC]
    void cruzAtivada(Vector3 pos)
    {
        if (!photonView.IsMine) return;

        if (currentAction != "MatarPlayer") return;
        if (utils.GetDistancia(transform.position, pos) >= 30f) return;

        this.photonView.RPC("PararSomCoracao", RpcTarget.All);
        EncerrarAcao(currentAction);

        agent.enabled = false;

        transform.position = utils.ChoiceInVec<Vector3>(PhantomConsts.spawns);

        agent.enabled = true;
    }

    public void CruzAtivada(Vector3 pos)
    {
        this.photonView.RPC("cruzAtivada", RpcTarget.All, pos);
    }

    void voltarAndar()
    {
        Debug.Log("voltou andar");
        transform.position = utils.ChoiceInVec<Vector3>(PhantomConsts.spawns);
        agent.enabled = true;

        GetComponent<WalkingDead>().AddRaiva();
    }

    void cairArmadilha()
    {
        Debug.Log("caiu armadilha");
        if (currentAction == "MatarPlayer")
        {
            this.photonView.RPC("PararSomCoracao", RpcTarget.All);
        }

        EncerrarAcao(currentAction);
        currentAction = "armadilha";

        agent.enabled = false;

        Invoke("voltarAndar", 5f);
    }

    [PunRPC]
    public void armadilha(int id, Vector3 pos) //caso alguma armadilha seja colocada
    {
        if (!photonView.IsMine) return;

        armadilhas.Add
        (
            new Tuple<int, Vector3, float> (id, pos, counter.timerGeral + consts.tempoMaxArmadilha)
        );
    }

    public void Armadilha(int id, Vector3 pos)
    {
        this.photonView.RPC("armadilha", RpcTarget.All, id, pos);
    }

    // Parte responsável por cada ação

    void TrocarSala()
    {
        if (teste)
        {
            Debug.Log("Trocando de sala");
            teste = false;
        }

        //verificar se está próximo de algum player ou se já chegou ao destino

        if (auxVector != null)
        {
            //verificar se já chegou ao destino

            if (utils.GetDistancia(transform.position, auxVector ?? transform.position) < 10f)
            {
                EncerrarAcao("TrocarSala");
                return;
            }

            //caso esteja só mudando de posição no estado invisivel, neste caso, não matará ninguém

            if (auxBool) return;

            //PiscarLampadasProximas();

            //verificar se está próximo de algum player, caso esteja, matar player

            //for (int i = 0; i < players.Length; i++)
            //{
            //    float dist = utils.GetDistancia(transform.position, players[i].transform.position);

            //    utils.SetVisibilidade(dist < consts.proximidadeVisivel);
            //
            //    if (dist < consts.proximidadeMatar)
            //    {
            //        teste = true;
            //        auxVector = null;
            //        currentAction = "MatarPlayer";
            //        targetPlayer = players[i];
            //        return;
            //    }
            //}

            return;
        }

        //caso esteja só se realocando depos de matar, busca alguma sala aleatória

        if (auxBool && beforeAction == "MatarPlayer")
        {
            //auxVector = utils.ChoiceInVec<Vector3>(PhantomConsts.spawns);
            auxVector = PhantomConsts.spawns[0];

            mudarPos(auxVector ?? utils.ChoiceInVec<Vector3>(PhantomConsts.spawns));

            //this.photonView.RPC("mudarPos", RpcTarget.AllViaServer, auxVector ?? utils.ChoiceInVec<Vector3>(PhantomConsts.spawns));

            return;
        }

        utils.SetVisibilidade(true);

        //descobrir sala mais próxima ao player

        float menorDist = float.PositiveInfinity;

        foreach (Vector3 salaPos in PhantomConsts.spawns)
        {
            float dist = utils.GetDistancia(salaPos, targetPlayer.transform.position);

            if (dist < menorDist)
            {
                menorDist = dist;
                auxVector = salaPos;
            }
        }

        mudarPos(auxVector ?? utils.ChoiceInVec<Vector3>(PhantomConsts.spawns));
        //this.photonView.RPC("mudarPos", RpcTarget.AllViaServer, auxVector ?? utils.ChoiceInVec<Vector3>(PhantomConsts.spawns));
    }

    void TocarAudio()
    {
        if (teste)
        {
            Debug.Log("Tocar Audio");
            teste = false;
        }

        // escolher uma das 3 ações

        int acao = utils.RandInt(0, consts.qtdAudio - 1);

        this.photonView.RPC("TocarParaTodos", RpcTarget.AllViaServer, targetPlayer.transform.position, acao);
        EncerrarAcao("TocarAudio");
    }

    void MoverPorta()
    {
        if (teste)
        {
            Debug.Log("Movendo portas");
            teste = false;
        }

        // obtém a porta mais próxima do player

        if (auxPorta == null)
        {
            float menor = float.PositiveInfinity;

            foreach (PortaTrigger porta in ambiente.GetPortas())
            {
                float dist = utils.GetDistancia(targetPlayer.transform.position, porta.Pos());

                if (dist < menor)
                {
                    menor = dist;
                    auxPorta = porta;
                }
            }
        }

        // caso de qualquer erro

        if (auxPorta == null)
        {
            EncerrarAcao("MoverPorta");
            return;
        }

        // esperar a porta parar de se mover

        if (auxPorta.EmMovimento()) return;

        // mudar estado da porta

        if (auxPorta.Aberta()) auxPorta.FecharPorta();
        else auxPorta.AbrirPorta();

        EncerrarAcao("MoverPorta");

        // ir até próximo ao player se cair no dado aleatório isso

        if (utils.RodarDado(consts.probRealocar))
        {
            GetComponent<PhantomCounter>().SetIntervalo(1);

            auxBool = utils.RodarDado(consts.probRealocar);
            currentAction = "TrocarSala";
        }
    }

    void AlterarLuz()
    {
        if (teste)
        {
            Debug.Log("Alterando luz");
            teste = false;
        }

        Debug.Log("Piscando luz");

        // obtém a luz mais próxima do player

        if (auxLuz == null)
        {
            float menor = float.PositiveInfinity;

            foreach (Luz luz in ambiente.GetLuzes())
            {
                float dist = utils.GetDistancia(targetPlayer.transform.position, luz.Pos());

                if (dist < menor)
                {
                    menor = dist;
                    auxLuz = luz;
                }
            }
        }

        // caso de qualquer erro

        if (auxLuz == null)
        {
            EncerrarAcao("AlterarLuz");
            return;
        }

        // mudar estado da luz

        auxLuz.Piscar(consts.timeLimitLuz);

        EncerrarAcao("AlterarLuz");

        // ir até próximo ao player se cair no dado aleatório isso

        if (utils.RodarDado(consts.probRealocar))
        {
            GetComponent<PhantomCounter>().SetIntervalo(1);

            auxBool = utils.RodarDado(consts.probRealocar);
            currentAction = "TrocarSala";
        }
    }

    void MatarPlayer()
    {
        if (!teste)
        {
            float dist = utils.GetDistancia(transform.position, targetPlayer.transform.position);

            if (dist < 5f)
            {
                targetPlayer.GetComponent<PlayerInterface>().MatarPlayer();

                EncerrarAcao("MatarPlayer");

                //auxBool = true;
                //currentAction = "TrocarSala";

                return;
            }
        }

        if (teste)
        {
            Debug.Log("Matando player");
            utils.SetVisibilidade(true);
            teste = false;
        }

        //utils.SetVisibilidade(dist < consts.proximidadeVisivel);
        

        this.photonView.RPC("BaterCoracao", RpcTarget.All, targetId);

        //PiscarLampadasProximas();

        mudarPos(targetPlayer.transform.position);

        //this.photonView.RPC("mudarPos", RpcTarget.AllViaServer, targetPlayer.transform.position);
    }

    // funções gerais

    public bool InAction()
    {
        return currentAction != null;
    }

    public void DefineAction(string actionName, GameObject player, int playerId)
    {
        currentAction = actionName;
        targetPlayer = player;
        targetId = playerId;
        teste = true;
    }

    public List<string> GetActions()
    {
        return Actions.Keys.ToList();
    }

    // Parte do código responsável por escolher ação

    int CalcularFichas(float probabilidade)
    {
        return (int)(Mathf.Max( Mathf.Floor(probabilidade), 1f ));
    }

    public string GetAction(float pTempo, float pRaiva, float pSanidade)
    {
        float tempoRaiva = Mathf.Max(pTempo, pRaiva);
        float probSanidade = 1f - pSanidade;

        pSanidade = Mathf.Max(pSanidade, 0.2f);
        probSanidade = Mathf.Min(probSanidade, 0.8f);

        //criar as fichas de cada ação

        List<Tuple<string, int>> fichas = new List<Tuple<string, int>>
        {
            new Tuple<string, int>("TocarAudio", CalcularFichas(10f * consts.multFichaAudio * pSanidade)),
            new Tuple<string, int>("MoverPorta", CalcularFichas(10f * consts.multFichaPorta * pSanidade)),
            new Tuple<string, int>("AlterarLuz", CalcularFichas(10f * consts.multFichaLuz * pSanidade)),
            new Tuple<string, int>("TrocarSala", CalcularFichas(tempoRaiva * 10f * consts.multFichaAndar)),
            new Tuple<string, int>("MatarPlayer", CalcularFichas(tempoRaiva * 10f * consts.multFichaAndar * probSanidade))
        };

        //obter o total

        int total = 0;

        foreach ((string key, int value) in fichas)
        {
            total += value;
        }

        //probabilidade da ação
        int escolha = (int)UnityEngine.Random.Range(0f, (float)total - 0.001f);

        //buscar ação

        total = 0;

        foreach ((string key, int value) in fichas)
        {
            total += value;

            if (escolha < total)
            {
                return key; //retorna ação
            }
        }

        return fichas[^1].Item1; //caso não encontre, retorna a última ação da lista (matar)
    }


}
