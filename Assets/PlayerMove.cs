using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Photon.Pun;

public class PlayerMove : MonoBehaviour
{
    public float pushforce = 4;
    public CharacterController controller;
    public float speed = 10f;
    public float gravity = -9.81f;

    Vector3 velocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    bool isGrounded;

    new AudioSource audio;

    [SerializeField]
    public PhotonView myView;
    // Start is called before the first frame update
    private void Awake()
    {
        if (!myView.IsMine && GetComponent<PlayerMove>() != null)
        {
            Debug.Log("Destroying player scripts");
            Destroy(GetComponent<PlayerMove>());
            Destroy(GetComponent<Player>());
        }
    }

    private void Start()
    {
        gameObject.GetComponentInChildren<Camera>().enabled = false;
        gameObject.GetComponentInChildren<Camera>().enabled = true;

        audio = gameObject.GetComponents<AudioSource>()[0];
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //configurar audio

        if (x != 0 || z != 0) //se está andando
        {
            if (!audio.isPlaying)
            {
                audio.Play();
            }
        } else if (audio.isPlaying) {
            audio.Stop();
        }

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move *speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody Rb = hit.collider.attachedRigidbody;
        if (Rb != null && !Rb.isKinematic)
        {
            Rb.AddForce(hit.moveDirection * pushforce);
        }
    }
}
