using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterface : MonoBehaviour
{
    float sanidade = 1f;
    float timer = 0f;
    new AudioSource audio; //coração

    // Start is called before the first frame update
    void Start()
    {
        audio = gameObject.GetComponents<AudioSource>()[1];
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 60f)
        {
            sanidade = Mathf.Max(0f, sanidade - 0.2f);
            timer = 0f;
        }
    }

    public void SetPosition(Vector3 newPos)
    {
        GetComponent<CharacterController>().enabled = false;

        transform.position = newPos;

        GetComponent<CharacterController>().enabled = true;
    }

    public void ReduzirSanidade(float qtd = 0.1f)
    {
        sanidade -= qtd;
    }

    public void AumentarSanidade(float qtd = 0.1f)
    {
        sanidade += qtd;
    }

    public float GetSanidade()
    {
        return sanidade;
    }

    public void MatarPlayer()
    {
        
        gameObject.transform.GetChild(4).GetComponent<Player>().morrer();
        SetPosition(new Vector3(-2.25999999f, 2.1400001f, -27.6200008f));
    }

    public void Caçando(bool status)
    {
        if (status && !audio.isPlaying) audio.Play();
        else if(!status && audio.isPlaying) audio.Stop();
    }
}
