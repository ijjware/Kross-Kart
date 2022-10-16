using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks
{
    byte maxPlayersRoom = 0;
    bool isConnecting;
    public InputField playerName;
    string gameVersion = "1";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
       // if (PlayerPrefs.HasKey("PlayerName"))
           // playerName.text = PlayerPrefs.GetString("PlayerName");
    }

    void Start()
    {
        isConnecting = true;
        PhotonNetwork.NickName = "mobu"; //playerName.text;
        //if (PhotonNetwork.IsConnected)
        //{
        //    print("Joining room...");
        //    PhotonNetwork.JoinRandomRoom();
        //} else
        //{
            print("connecting...");
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        //}
    }

    // Network Callbacks
    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            print("OnConnectedToMaster...");
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("failed to join random room...");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayersRoom;
        options.IsOpen = true;
        options.IsVisible = true;
        //options.CleanupCacheOnLeave = false; // ??
        PhotonNetwork.CreateRoom("poo", options);
        //PhotonNetwork.CreateRoom(null, new RoomOptions{ MaxPlayers = this.maxPlayersRoom });
        //base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("disconnected because " + cause);
        isConnecting = false;
        //base.OnDisconnected(cause);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2) // modify this int to change number of players in a game
        {
            PhotonNetwork.LoadLevel("tst");
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print("joined room with " + PhotonNetwork.CurrentRoom.PlayerCount + " players");
        //PhotonNetwork.LoadLevel("tst");
    }

}
