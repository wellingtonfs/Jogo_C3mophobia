using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class GameConnection : MonoBehaviourPunCallbacks
{
    public Text chatlog;
    GameObject phantom = null;

    private void Awake()
    {
        PhotonNetwork.LocalPlayer.NickName = "" + Random.Range(0,1000);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();


        if(PhotonNetwork.InLobby == false)
        {
            PhotonNetwork.JoinLobby();
        }

    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();


        PhotonNetwork.JoinRoom("gameloot");

    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        List<Vector3> posicoes1 = new List<Vector3>() {new Vector3(-9.96535683f, 0.0659999996f, -10.6684017f), new Vector3(5.20622301f, 3.72000003f, -9.02715969f), new Vector3(-4.45328999f, 3.74000001f, -21.0567188f)};
        List<Vector3> posicoes2 = new List<Vector3>() { new Vector3(-8.5660429f, 0.257999986f, -45.0779495f), new Vector3(-11.0004673f, 0.167999998f, -37.6721115f), new Vector3(-17.7490196f, 3.56999993f, -26.5100002f) };
        List<Vector3> posicoes3 = new List<Vector3>() {new Vector3(-15.7346106f, 3.7650001f, -24.9439354f), new Vector3(-15.7026558f, 3.64299989f, -54.5365295f), new Vector3(-24.7794666f, 3.69000006f, -47.8812599f) };

        int p1 = Random.Range(0, posicoes1.Count);
        int p2 = Random.Range(0, posicoes2.Count);
        int p3 = Random.Range(0, posicoes3.Count);

        Vector3 pos1 = new Vector3(10, 0, 1);
        Vector3 pos2 = new Vector3(8, 0, 1);
        Vector3 pos3 = new Vector3(6, 0, 1);
        Quaternion rot = quaternion.Euler(Vector3.up);

        PhotonNetwork.InstantiateRoomObject("bolsa armadilha", pos1, rot);
        PhotonNetwork.InstantiateRoomObject("bolsa arma", pos2, rot);
        PhotonNetwork.InstantiateRoomObject("bolsa cruz", pos3, rot);

        phantom = PhotonNetwork.InstantiateRoomObject("Phantom", PhantomUtils.GetPosAleatoria(), rot);

        PhotonNetwork.InstantiateRoomObject("pedra azul", posicoes1[p1], rot);
        PhotonNetwork.InstantiateRoomObject("pedra vermelha", posicoes2[p2], rot);
        PhotonNetwork.InstantiateRoomObject("pedra amarela", posicoes3[p3], rot);

        
    }

    public override void OnJoinedRoom()
    {

        Vector3 pos = new Vector3(Random.Range(-1.0f, 2.0f), 3, Random.Range(-25.0f, -30.0f));
        Quaternion rot = quaternion.Euler(Vector3.up * Random.Range(0.0f, 360.0f));
        GameObject Myplayer =  PhotonNetwork.Instantiate("PlayerLanternaTeste1",pos,rot);
        Myplayer.GetComponent<PlayerMove>().enabled = true;
        Myplayer.gameObject.transform.Find("Rot").GetComponent<rot>().enabled = true;
        Myplayer.gameObject.transform.Find("Rot").GetComponent<Player>().enabled = true;
        Myplayer.gameObject.transform.Find("MainCamera").gameObject.SetActive(true);

        if (phantom != null)
        {
            phantom.GetComponent<WalkingDead>().UpdatePlayers();
        }

        //atualizar lista de players
        //GameObject phantom;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {

        if (returnCode == ErrorCode.GameDoesNotExist)
        {
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
            PhotonNetwork.CreateRoom("gameloot", roomOptions, null);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
    }

    public override void OnLeftRoom()
    {
    }

    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
    }


}
