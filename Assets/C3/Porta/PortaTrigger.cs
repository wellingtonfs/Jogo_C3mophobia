using Photon.Pun;
using UnityEngine;

public class PortaTrigger : MonoBehaviourPun
{
    public short spring = 9;
    public short damper = 3;
    public bool portaEsquerda = false;

    private HingeJoint hinge;
    private bool playerIn = false;
    private short inProgress = 0;

    private short minAngle = 0;
    private int maxAngle = 90;
    new AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        hinge = GetComponent<HingeJoint>();

        //criar audio

        audio = gameObject.AddComponent<AudioSource>();
        audio.clip = Resources.Load("porta_abrindo") as AudioClip;
        audio.playOnAwake = false;
        audio.transform.position = transform.position;
        audio.spatialBlend = 1;
        audio.rolloffMode = AudioRolloffMode.Linear;
        audio.minDistance = 0;
        audio.maxDistance = 10;

        if (portaEsquerda) maxAngle = -maxAngle;
    }

    void OnPressKeyInteract()
    {
        if (playerIn)
        {
            if ((portaEsquerda && hinge.angle > -1) || (!portaEsquerda && hinge.angle < 1) )
            {
                AbrirPorta();
            }
            else
            {
                FecharPorta();
            }
        }
    }

    [PunRPC]
    void abrirPorta()
    {
        JointSpring hingeSpring = hinge.spring;
        hingeSpring.spring = spring;
        hingeSpring.damper = damper;
        hingeSpring.targetPosition = maxAngle;

        if (!audio.isPlaying)
        {
            audio.Play();
        }

        hinge.spring = hingeSpring;
        hinge.useSpring = true;
        inProgress = 1;
    }

    [PunRPC]
    void fecharPorta()
    {
        JointSpring hingeSpring = hinge.spring;
        hingeSpring.spring = spring;
        hingeSpring.damper = damper;
        hingeSpring.targetPosition = minAngle;

        if (!audio.isPlaying)
        {
            audio.Play();
        }

        hinge.spring = hingeSpring;
        hinge.useSpring = true;
        inProgress = 2;
    }

    public void AbrirPorta()
    {
        this.photonView.RPC("abrirPorta", RpcTarget.All);
    }

    public void FecharPorta()
    {
        this.photonView.RPC("fecharPorta", RpcTarget.All);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnPressKeyInteract();
        }
      

        if (inProgress == 1)
        {
            if ((portaEsquerda && hinge.angle < (maxAngle + 1)) || (!portaEsquerda && hinge.angle > (maxAngle - 1)))
                hinge.useSpring = false;
        }
        else if (inProgress == 2)
        {
            if ((portaEsquerda && hinge.angle > (minAngle - 1)) || (!portaEsquerda && hinge.angle < (minAngle + 1)))
                hinge.useSpring = false;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag != "Player") return;

        playerIn = true;
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag != "Player") return;

        playerIn = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player") return;

        hinge.useSpring = false;
    }

    //metodos públicos

    public bool EmMovimento()
    {
        return hinge.useSpring;
    }

    public bool Aberta()
    {
        return ((portaEsquerda && hinge.angle < (maxAngle + 1)) || (!portaEsquerda && hinge.angle > (maxAngle - 1)));
    }

    public Vector3 Pos()
    {
        return transform.position;
    }
}
