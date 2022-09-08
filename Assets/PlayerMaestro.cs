using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

//local client script
//attached to Pearlman
//instantiates player karts
//manage local player inputs
//needs to access HUD and Cinemachine cameras
//could isolate HUD access into seperate behavior
public class PlayerMaestro : MonoBehaviourPunCallbacks
{
    public static PlayerMaestro instance;
    private void Awake()
    {
        instance = this;
    }
   
    //local kart instance
    public Kart kart;
    public NPK autokart;
    public Slider boost;

    const byte ChangeColorEventCode = 2;
    const byte GroupingEventCode = 3;

    Color clor;
    GameObject p1kar;
    public GameObject you;
    Vector3 startPos;
    Quaternion startRot;
    public GameObject[] spawns;

    public Item heldItem;
    private bool isStrafing = false;

    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera driftCam;

    private void Start()
    {
        int numPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        spawns = GameObject.FindGameObjectsWithTag("SpawnPoints");
        
        if (PhotonNetwork.InRoom)
        {
            //this is maybe a lil dumb but also maybe not?
            //spawns is a list of predefined transforms
            startPos = spawns[numPlayers - 1].transform.position;
            startRot = spawns[numPlayers - 1].transform.rotation;

            if (NetworkedPlayer.LocalPlayerInstance == null)
            {
                p1kar = PhotonNetwork.Instantiate("Network Kart", startPos, startRot);
                kart = p1kar.GetComponent<Kart>();
                autokart = p1kar.GetComponent<NPK>();
                kart.setMaster(this);
                Setup();
                //sorter.SetGroup(p1kar.GetPhotonView().OwnerActorNr, p1kar);
                //kart.setMaster(pMan); TODO maybe replace with master = playermaestro.instance; in kart.cs
                //kart.setMaster(gameObject.GetComponent<Maestro>());
                //TODO: this is for sure stupid refactor to take into acct current team member counts
                //if (numPlayers % 2 == 0)
                //{
                //    kart.team = 0;

                //} //RED team
                //else
                //{
                //    kart.team = 1;

                //} //BLU team
                kart.you = PhotonNetwork.Instantiate("you", startPos, startRot);
                kart.OG = true;
            }

            //freeze other kart
            if (PhotonNetwork.CurrentRoom.PlayerCount != 1)
            {
                kart.active = false;
            }

        }
        //doesn't edit gradient of karts on foreign clients that are not yet instantiated
        //color of trail is different between local instance and remote instances
        //TODO: figure out whether to leave these as events or make them method calls
        //SendPlayerJoinEvent();
        //SendColorEvent();
        //SendGroupingEvent();
    }

    public void Setup()
    {
        mainCam.Follow = GameObject.Find("follower").transform;
        mainCam.LookAt = kart.gameObject.transform;
        driftCam.Follow = kart.gameObject.transform;
        driftCam.LookAt = kart.gameObject.transform;
        //kart.setMaster(pMan);
    }

    private void SendGroupingEvent()
    {
        print("send group event");
        int kartViewID = p1kar.GetPhotonView().ViewID;
        object[] content = new object[] { kartViewID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(GroupingEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void SendColorEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(ChangeColorEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }
    
    private void SendPlayerJoinEvent()
    {

    }

    public void SyncTrail(Color yes)
    {
        kart.GetComponent<PhotonView>().RPC("RPC_ColorTrail", RpcTarget.All, yes.r, yes.g, yes.b);
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 8)
        {
            autokart.enabled = true;
            kart.enabled = false;
        }
    }


    //Kart input funcs
    public void Steer(InputAction.CallbackContext context)
    {
        if(isStrafing) { return; }
        //if (kart.stuck) { kart.mashInputs += 1; }
        kart.left_steering = context.ReadValue<Vector2>();
        //print(left_steering);
    }

    public void moveY(InputAction.CallbackContext context)
    {
        if (!isStrafing) { return; }
        kart.move.y = context.ReadValue<float>();
    }

    public void moveX(InputAction.CallbackContext context)
    {
        if (!isStrafing) { return; }
        kart.move.x = context.ReadValue<float>();
    }

    public void rollR(InputAction.CallbackContext context)
    {
        //if (kart.stuck) { kart.mashInputs += 1; }
        kart.roll = -context.ReadValue<float>();
    }

    public void rollL(InputAction.CallbackContext context)
    {
        //if (kart.stuck) { kart.mashInputs += 1; }
        kart.roll = context.ReadValue<float>();
    }

    public void Accel(InputAction.CallbackContext context)
    {
        //if (kart.stuck) { kart.mashInputs += 1; }
        bool btn = context.ReadValueAsButton();
        if (btn)
        {
            kart.accelerating = true;
        }
        else
        {
            kart.accelerating = false;
        }
    }

    public void booost(InputAction.CallbackContext context)
    {
        //if (kart.stuck) { kart.mashInputs += 1; }
        bool btn = context.ReadValueAsButton();
        if (btn)
        {
            kart.boosting = true;
        }
        else
        {
            kart.boosting = false;
        }
    }

    public void driift(InputAction.CallbackContext context)
    {
        //if (kart.stuck) { kart.mashInputs += 1; return; }
        bool btn = context.ReadValueAsButton();
        kart.driift(btn);
        if (btn)
            driftCam.Priority = 11;
        else
            driftCam.Priority = 9;
    }

    public void straafe(InputAction.CallbackContext context)
    {
        
        bool btn = context.ReadValueAsButton();
        if (btn)
        {
            if (isStrafing) { return; }
            isStrafing = true;
            kart.move.x = kart.left_steering.x;
            //print(kart.left_steering.x);
            kart.move.y = kart.left_steering.y;
            kart.left_steering = new Vector2();
        }
        else
        {
            isStrafing = false;
            kart.move.x = 0;
            kart.move.y = 0;
        }
    }

    public void buurst(InputAction.CallbackContext context)
    {
        //bool btn = context.ReadValueAsButton();
        //if (btn) { kart.remote_buurst(1); }
        heldItem.StartCoroutine("UseItem", kart);
    }

    public void boost_update(float amt) { boost.value = amt; }

}
