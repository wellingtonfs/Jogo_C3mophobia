using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwicher : MonoBehaviour
{
    int index = 0;

    [SerializeField] List<GameObject> players = new List<GameObject>();
    PlayerInputManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<PlayerInputManager>();
        index = Random.Range(0, players.Count);
        manager.playerPrefab = players[index];
        
    }

    // Update is called once per frame
    public void SwitchNext(PlayerInput input)
    {
        index = Random.Range(0, players.Count);
        manager.playerPrefab = players[index];

    }
}
