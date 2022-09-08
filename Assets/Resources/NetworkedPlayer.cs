using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun;

public class NetworkedPlayer : MonoBehaviourPunCallbacks
{
    public static GameObject LocalPlayerInstance;
    private void Awake()
    {
        LocalPlayerInstance = this.gameObject;
    }
    public GameObject playerNamePrefab;
    public Rigidbody rb;
    public Renderer kartMesh;

}