using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// o que falta fazer:

/*

    player:

    * diminuir a sanidade quando player estiver sozinho ou no escuro
 
    fantasma:

    aumentar velocidade do fantasma de acordo com a raiva
    fechar portas na caçada
    * parar de caçar quando player colocar cruz

    ambiente:

    colocar segundo andar
    mobiliar o máximo possível
    aparecer texto E quando player chegar próximo de portas

    geral:

    ***** sincronizar tudo com outros jogadores
    
    ##
    fazer menu do jogo
    ##
    permitir players entrarem em outras salas

    
    colocar som


 */

public class PhantomConsts : MonoBehaviourPun
{
    //ambiente
    public float distLampToAction = 40f; //distancia para piscar lampadas
    public int qtdAudio = 5;

    //contador
    public float timerInicio = 15f;
    public int tempoMinimoUpdate = 3;
    public int tempoMaximoUpdate = 10;
    public float tempoMaximoPartidaMin = 10;
    public float minRefreshTime = 5; //tempo mínimo de espera depois de executar alguma ação
    public float maxRefreshTime = 10; //tempo máximo de espera depois de executar alguma ação

    //probs
    public float prob = 0.7f; //probabilidade geral
    public float grau = 1.5f; //crescimento da curva
    public float pesoAmbiente = 0.5f; //regulador da importancia do ambiente (sanidade, solidão, etc)
    public float probMax = 0.3f;

    //ambiente
    public float minDist = 60;
    public float maxDist = 700;
    public float minProbAmbiente = 0.4f;
    public float pesoSanidade = 1;
    public float pesoDistancia = 1;

    //action
    public float proximidadeMatar = 15;
    public float proximidadeVisivel = 100;
    public float probRealocar = 0.5f;
    public float timeLimitLuz = 10;
    public float tempoMaxArmadilha = 60f;

    //pegar fichas
    public float multFichaAudio = 1.5f;
    public float multFichaAndar = 1f;
    public float multFichaPorta = 1f;
    public float multFichaLuz = 1f;
    public float multFichaMatar = 0.8f;

    static public Vector3[] spawns = new Vector3[] {
        //primeiro andar

        new Vector3(14.46f,0.8f,-54.0600014f),
        new Vector3(-2.07999992f,0.8f,-53.2000008f),
        new Vector3(-7.42000008f,0.8f,-49.0400009f),
        new Vector3(-14.4899998f,0.8f,-53.5299988f),
        new Vector3(-17.5200005f,0.8f,-53.5299988f),
        new Vector3(-22.7000008f,0.8f,-50.4300003f),
        new Vector3(-37,0.8f,-57.6100006f),
        new Vector3(-30.8400002f,0.8f,-53.5299988f),
        new Vector3(-41.6199989f,0.8f,-55.2900009f),
        new Vector3(-41.6199989f,0.8f,-49.7599983f),
        new Vector3(-38.6399994f,0.8f,-48.0499992f),
        new Vector3(-35.5999985f,0.8f,-46.3199997f),
        new Vector3(-32.7900009f,0.8f,-44.7200012f),
        new Vector3(-26.6200008f,0.8f,-43.3800011f),
        new Vector3(-23.1599998f,0.8f,-43.3800011f),
        new Vector3(-18.5699997f,0.8f,-43.3800011f),
        new Vector3(-10.4499998f,0.8f,-43.3800011f),
        new Vector3(-6.1500001f,0.8f,-43.5299988f),
        new Vector3(-1.99000001f,0.8f,-43.5299988f),
        new Vector3(2.43000007f,0.8f,-43.5299988f),
        new Vector3(8.22999954f,0.8f,-44.1800003f),
        new Vector3(11.6000004f,0.8f,-45.7900009f),
        new Vector3(14.6999998f,0.8f,-47.6899986f),
        new Vector3(18.0900002f,0.8f,-49.2900009f),
        new Vector3(18.0900002f,0.8f,-54.9900017f),
        new Vector3(-8.28999996f,0.8f,-39.6899986f),
        new Vector3(-18.2099991f,0.8f,-39.6899986f),
        new Vector3(-18.2099991f,0.8f,-34.6699982f),
        new Vector3(-17.8600006f,0.8f,-28.9699993f),
        new Vector3(-11.9099998f,0.8f,-35.9199982f),
        new Vector3(-7.38999987f,0.8f,-33.0499992f),
        new Vector3(-18.3700008f,0.8f,-19.4300003f),
        new Vector3(-27.0900002f,0.8f,-19.2000008f),
        new Vector3(-27.0900002f,0.8f,-5.98999977f),
        new Vector3(1.21000004f,0.8f,-5.98999977f),
        new Vector3(-7.71000004f,0.8f,-11.0799999f),
        new Vector3(5.28999996f,0.8f,-11.3999996f),
        new Vector3(-3.25999999f,0.8f,-20.9300003f),

        //segundo andar

        new Vector3(14.46f,5f,-54.0600014f),
        new Vector3(-2.07999992f,5f,-53.2000008f),
        new Vector3(-7.42000008f,5f,-49.0400009f),
        new Vector3(-14.4899998f,5f,-53.5299988f),
        new Vector3(-17.5200005f,5f,-53.5299988f),
        new Vector3(-22.7000008f,5f,-50.4300003f),
        new Vector3(-37,5f,-57.6100006f),
        new Vector3(-30.8400002f,5f,-53.5299988f),
        new Vector3(-41.6199989f,5f,-55.2900009f),
        new Vector3(-41.6199989f,5f,-49.7599983f),
        new Vector3(-38.6399994f,5f,-48.0499992f),
        new Vector3(-35.5999985f,5f,-46.3199997f),
        new Vector3(-32.7900009f,5f,-44.7200012f),
        new Vector3(-26.6200008f,5f,-43.3800011f),
        new Vector3(-23.1599998f,5f,-43.3800011f),
        new Vector3(-18.5699997f,5f,-43.3800011f),
        new Vector3(-10.4499998f,5f,-43.3800011f),
        new Vector3(-6.1500001f,5f,-43.5299988f),
        new Vector3(-1.99000001f,5f,-43.5299988f),
        new Vector3(2.43000007f,5f,-43.5299988f),
        new Vector3(8.22999954f,5f,-44.1800003f),
        new Vector3(11.6000004f,5f,-45.7900009f),
        new Vector3(14.6999998f,5f,-47.6899986f),
        new Vector3(18.0900002f,5f,-49.2900009f),
        new Vector3(18.0900002f,5f,-54.9900017f),
        new Vector3(-8.28999996f,5f,-39.6899986f),
        new Vector3(-18.2099991f,5f,-39.6899986f),
        new Vector3(-18.2099991f,5f,-34.6699982f),
        new Vector3(-17.8600006f,5f,-28.9699993f),
        new Vector3(-11.9099998f,5f,-35.9199982f),
        new Vector3(-7.38999987f,5f,-33.0499992f),
        new Vector3(-18.3700008f,5f,-19.4300003f),
        new Vector3(-27.0900002f,5f,-19.2000008f),
        new Vector3(-27.0900002f,5f,-5.98999977f),
        new Vector3(1.21000004f,5f,-5.98999977f),
        new Vector3(-7.71000004f,5f,-11.0799999f),
        new Vector3(5.28999996f,5f,-11.3999996f),
        new Vector3(-3.25999999f,5f,-20.9300003f)
    };
}
