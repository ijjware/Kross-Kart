using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class Maestro : MonoBehaviourPunCallbacks
{
    public static Maestro instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("NodeGraph", new Vector3(), new Quaternion());
        }
    }

}
