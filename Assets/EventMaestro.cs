using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

// Handles photon events
// CUIDADO: this class should only handle events for room-owned singleton objects
// 9-7 TODO: maybe RPC calls are the simplest way of keeping clients updated on important data?
public class EventMaestro : MonoBehaviourPunCallbacks
{

    public static EventMaestro instance;
    private void Awake()
    {
        instance = this;
    }
    public int EventTeam;
    Color clor;
    public Heather HUD;
   
    public PlayerSorter sort;

    //stored groder data
    string[,] storedNodes = new string[4,2];

    public bool isTimeUp = false;
    //event codes
    const byte HostMigrationEventCode = 7;
    //const byte ChangeColorEventCode = 2;
    //const byte GroupingEventCode = 3;
    // public const byte RingEventCode = 4;
    //public const byte GoalEventCode = 1;
    //public const byte RingingEventCode = 5;
    //public const byte RingDeathEventCode = 6;
    //public const byte TimeUpEventCode = 8;

    //scoring -> needs HUD access

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void OnEvent(EventData photonEvent)
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
        byte eventCode = photonEvent.Code;
        switch (eventCode)
        {
            case 1:
                {
                    // data[0] = group, data[1] = '''active''' node, data[2] = '''upcoming node'''
                    object[] data = (object[])photonEvent.CustomData;
                    int group = (int)data[0];
                    string active = (string)data[1];
                    string upcoming = (string)data[2];
                    storedNodes[group, 0] = active;
                    storedNodes[group, 1] = upcoming;
                    //Groder.instance.UpdateActiveNode((string)data[0]);
                    break;
                }
            //case 2: Pearlman.SyncTrail(clor); break; 
            case 3: //GroupingEvent: this event should only be recieved by master client, prob groups players 
                { print("group event got");
                    object[] data1 = (object[])photonEvent.CustomData;
                    int viewID = (int)data1[0];
                    PhotonView guy = PhotonView.Find(viewID);
                    sort.SetGroup(guy.OwnerActorNr, guy);
                    break; 
                }
            case 4: //RingEvent broken scoring event
                {
                    object[] data2 = (object[])photonEvent.CustomData;
                    Kart karttz = PhotonView.Find((int)data2[1]).GetComponent<Kart>();
                    int points = (int)data2[0];
                    print("got " + points + " points");
                    //normal_score(karttz.team, points);
                    HUD.PointChange(karttz.team, points);
                    break;
                }
            case 5: //RingingEvent: on ring activation checks if new ring needs to be instanced
                {
                    object[] data3 = (object[])photonEvent.CustomData;
                    int group = (int)data3[0];
                    Vector3 pos = (Vector3)data3[1];
                    Node nod = Norman.instance.GetClosestNode(pos);
                    Groder.instance.RingChecker(group, nod);
                    break;
                }
            case 6: //RingDeathEvent
                {
                    object[] data4 = (object[])photonEvent.CustomData;
                    if (isTimeUp) { return; }
                    int groupa = (int)data4[0];
                    int view = (int)data4[1];
                    if (PhotonView.Find(view))
                    {
                        PhotonNetwork.Destroy(PhotonView.Find(view).gameObject);
                    }
                    if (Groder.instance) { Groder.instance.RingDeader(groupa, view); }
                    break;
                }
            case 7: //HostMigrationEvent: on master client change
                {
                    print("host disconnected");
                    PhotonNetwork.LeaveRoom(false);
                    //PhotonNetwork.NetworkClientState;
                   //TODO: fix next line or so
                    //PhotonNetwork.LoadLevel("shop");
                    break;
                }
            case 8: //TimeUpEvent
                {//slowdown statements
                 //Time.fixedDeltaTime = .05f;
                 //Time.maximumDeltaTime = .05f;
                    isTimeUp = true;
                    // 8/13: this may be okay??? see comment in Groder for likely better alternative
                    // disabling Groder does prevent scoring as well as ring destruction
                    Groder.instance.enabled = false;
                    print("time's up!");
                    break;
                }
            default: break;
        }

    }

    private void Start()
    {
        clor = Random.ColorHSV();
    }

    override public void OnMasterClientSwitched(Player newMasterClient)
    {
        print("switch event");
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(HostMigrationEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("shop");
        base.OnLeftRoom();
    }

}
