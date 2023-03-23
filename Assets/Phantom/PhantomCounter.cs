using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PhantomCounter : MonoBehaviourPun
{
    public static bool loaded = false;

    float timer = 0;
    float timerInicio = 0;
    public float timerGeral = 0;
    float interval = 0;

    bool estado = false;
    bool stopped = false;

    PhantomConsts consts;
    PhantomUtils utils;

    void Start()
    {
        consts = GetComponent<PhantomConsts>();
        utils = GetComponent<PhantomUtils>();

        timerInicio = consts.timerInicio;
        interval = consts.tempoMaximoUpdate;
    }

    void Update()
    {
        timerGeral += Time.deltaTime; //tempo total do jogo

        if (timerGeral < 60)
        {
            timerInicio += Time.deltaTime;

            if (timerInicio > 5)
            {
                GetComponent<WalkingDead>().UpdatePlayers();
                timerInicio = 0;
            }
        }

        if (estado || stopped) return;

        timer += Time.deltaTime;

        if (timer > interval)
        {
            estado = true;
        }
    }

    void resetTimer()
    {
        estado = false;

        timer = 0;
        interval = Random.Range(consts.tempoMinimoUpdate, consts.tempoMaximoUpdate);
    }

    public bool NovoEstado()
    {
        if (stopped) Restart();

        if (estado)
        {
            Pause();
            resetTimer();

            return true;
        }

        return false;
    }

    public void Pause()
    {
        stopped = true;
        resetTimer();
    }

    public void PauseParaAcao()
    {
        stopped = true;
        resetTimer();

        interval += PhantomUtils.IntAleatorio((int)consts.minRefreshTime, (int)consts.maxRefreshTime);
    }

    public void Restart()
    {
        stopped = false;
    }

    public float GetProb()
    {
        return timerGeral / (float)(consts.tempoMaximoPartidaMin * 60);
    }

    public void SetIntervalo(float intervalo)
    {
        interval = intervalo;
    }
}
