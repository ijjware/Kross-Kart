using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class Maestro : MonoBehaviourPunCallbacks
{
    Color clor;
    GameObject p1kar;
    public Kart kart;
    public GameObject you;
    Vector3 startPos;
    Quaternion startRot;
    public GameObject[] spawns;
    public Slider boost;
    public Text timer;
    public int secs = 0;
    public int mins = 0;

    float change;
    public GameObject miniPivot;

    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera driftCam;
    //public GameObject follower;
    //public GameObject aimee; 
    
    public Text UsText;
    public Text ThemText;
    private int UsPts = 0;
    private int ThemPts = 0;
    public GameObject bluGoal;
    public GameObject redGoal;
    const byte  ChangeColorEventCode = 2;

    public bool isActive = false;

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
        if (eventCode == 1)
        {
            //goal event
            object[] data = (object[])photonEvent.CustomData;
            print("Goal receive");
            //find Flag
            Flag flaggy = PhotonView.Find((int)data[1]).GetComponent<Flag>();
            //find Kart
            Kart kartty = PhotonView.Find((int)data[0]).GetComponent<Kart>();
            //goal type
            int goalType = (int)data[2];

            if (goalType == flaggy.getFlagType())
            {
                //return flag unless nutral
                if (goalType == 0) { }// neutral scoring
                else { flag_reset(flaggy); }//return flag
            } else { normal_score(flaggy); } // normal scoring
            
            //int flaggyType = flaggy.getFlagType();
        }
        else if (eventCode == 2)
        {
            kart.GetComponent<PhotonView>().RPC("RPC_ColorTrail", RpcTarget.All, clor.r, clor.g, clor.b);
        }
    }

    private void normal_score(Flag felg)
    {
        // score based on flag type
        if (kart.team == felg.getFlagType()) { ThemPts += felg.getPoints(); }
        else { UsPts += felg.getPoints(); print("us"); }

        //switch (felg.getFlagType())
        //{
        //    case 1:
        //        // red point
        //        redPts += felg.getPoints();
        //        break;
        //    case 2:
        //        // blu point
        //        bluPts += felg.getPoints();
        //        break;
        //    default: break;
        //}
        flag_reset(felg);
        score_update();
    }

    private void flag_reset(Flag fleg)
    {
        // flag destroyed and reinstantiated
        if (PhotonNetwork.IsMasterClient)
        {
            switch (fleg.getFlagType())
            {
                case 1:
                    // blu flag
                    PhotonNetwork.InstantiateRoomObject("Flag", bluGoal.transform.position, 
                        bluGoal.transform.rotation, 0 , new object[] {1});
                    break;
                case 2:
                    // red flag
                    PhotonNetwork.InstantiateRoomObject("Flag", redGoal.transform.position, 
                        redGoal.transform.rotation, 0, new object[] {2});
                    break;
                default: break;
            }
            PhotonNetwork.Destroy(fleg.gameObject);
        }
    }

    private void FlagInstance()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //red flag
            object[] team = new object[] { 2 };
            PhotonNetwork.Instantiate("Flag", redGoal.transform.position,
                redGoal.transform.rotation, 0, team);
            //blu flag
            team[0] = 1;
            PhotonNetwork.Instantiate("Flag", bluGoal.transform.position,
                bluGoal.transform.rotation, 0, team);
        }
    }


    private void score_update()
    {
        UsText.text = "" + UsPts;
        ThemText.text = ThemPts + "";
    }

    public void boost_update(float amt) { boost.value = amt; }

    public GameObject spawn;
    // Start is called before the first frame update
    void Start()
    {
        clor = Random.ColorHSV();
        int numPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        spawns = GameObject.FindGameObjectsWithTag("SpawnPoints");
        if (PhotonNetwork.InRoom)
        {
            startPos = spawns[numPlayers - 1].transform.position;
            startRot = spawns[numPlayers - 1].transform.rotation;

            if (NetworkedPlayer.LocalPlayerInstance == null)
            {
                p1kar = PhotonNetwork.Instantiate("Network Kart", startPos, startRot);
                kart = p1kar.GetComponent<Kart>();
                mainCam.Follow = GameObject.Find("follower").transform;
                mainCam.LookAt = p1kar.transform;
                driftCam.Follow = p1kar.transform;
                driftCam.LookAt = p1kar.transform;
                kart.setMaster(gameObject.GetComponent<Maestro>());
                if (numPlayers%2 == 0) { kart.team = 0; } //RED team
                else { kart.team = 1; } //BLU team
                kart.you = you;
                
            }

            ////freeze other kart
            //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            //{
            //    kart.enabled = true;
            //}
        }

        //enterer.GetNConnects(senders, nodes, 4);
        
        //foreach(GameObject node in nodes)
        
        kart.enabled = true;
        FlagInstance();


        //doesn't edit gradient of karts on foreign clients that are not yet instantiated
        //color of trail is different between local instance and remote instances
        // store color value
        // on player joined rpc call
        SendColorEvent();
    }

    private void SendColorEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(ChangeColorEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    //Kart input funcs
    public void Steer(InputAction.CallbackContext context)
    {
        if (kart.stuck) { kart.mashInputs += 1; }
        kart.left_steering = context.ReadValue<Vector2>();
        //print(left_steering);
    }

    public void rollR(InputAction.CallbackContext context)
    {
        if (kart.stuck) { kart.mashInputs += 1; }
        kart.roll = -context.ReadValue<float>();
    }

    public void rollL(InputAction.CallbackContext context)
    {
        if (kart.stuck) { kart.mashInputs += 1; }
        kart.roll = context.ReadValue<float>();
    }

    public void Accel(InputAction.CallbackContext context)
    {
        if (kart.stuck) { kart.mashInputs += 1; }
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
        if (kart.stuck) { kart.mashInputs += 1; }
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
        if (kart.stuck) { kart.mashInputs += 1; return; }
        bool btn = context.ReadValueAsButton();
        kart.driift(btn);
        if (btn)
            driftCam.Priority = 11;
        else
            driftCam.Priority = 9;
    }

    public void buurst(InputAction.CallbackContext context)
    {
        bool btn = context.ReadValueAsButton();
        if (btn) { kart.remote_buurst(1); }
    }

    public void pivot(InputAction.CallbackContext context)
    {
        change = context.ReadValue<float>();
        change *= 2;
        //print("uh" + change);
        //print(left_steering);
    }

    private void FixedUpdate()
    {
        Vector3 rot = miniPivot.transform.rotation.eulerAngles;
        rot.x += change;
        Quaternion nov = Quaternion.identity;
        nov.eulerAngles = rot;
        miniPivot.transform.Rotate(change, 0, 0, Space.Self);

        //increment timer
        secs = (int)Time.fixedTime;
        mins = secs / 60;
        secs -= (mins * 60);
        string secString = "00";
        string minString = "00    ";
        if (secs < 10) { secString = "0" + secs; }
        else if (secs < 60 ) { secString = secs.ToString(); }
        if (mins > 9) { minString = mins + "    "; }
        else if ( mins > 0) { minString = "0" + mins + "    "; }
        timer.text = minString + secString;

    }

}
