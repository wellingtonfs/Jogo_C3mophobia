using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class CameraScript : MonoBehaviour
{
    public float senX;
    public float senY;

    float xRotation;
    float yRotation;

    public Transform orientation;

    [SerializeField]
    private Camera MyCamera;

    public CharacterController controller;

    public float speed = 12f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * senX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * senY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        MyCamera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.Rotate(Vector3.up * mouseX);


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
        
    }
}
