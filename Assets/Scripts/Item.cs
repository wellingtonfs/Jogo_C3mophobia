using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour, IUsable
{
    public UnityEvent OnUse => throw new System.NotImplementedException();

    public void Use(GameObject actor)
    {
        GetComponentInChildren<MeshRenderer>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
         
    }

    // Start is called before the first frame update

}
