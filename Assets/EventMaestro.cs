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
    public int EventTeam;
    Color clor;
    public Heather HUD;
   
    public PlayerSorter sort;

    public bool isTimeUp = false;
    //event codes
    //const byte GraphInstanceEventCode = 7;
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
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
        byte eventCode = photonEvent.Code;
        
        switch (eventCode)
        {
            //case 2: Pearlman.SyncTrail(clor); break; 
            case 3: //GroupingEvent: this event should only be recieved by master client, prob groups players 
                { print("group event got");
                    object[] data1 = (object[])photonEvent.CustomData;
                    int viewID = (int)data1[0];
                    PhotonView guy = PhotonView.Find(viewID);
                    sort.SetGroup(guy.OwnerActorNr, guy);
                    break; 
                }
            case 4: //RingEvent
                {
                    object[] data2 = (object[])photonEvent.CustomData;
                    Kart karttz = PhotonView.Find((int)data2[1]).GetComponent<Kart>();
                    int points = (int)data2[0];
                    //print("got " + points + " points");
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
            case 7: //GraphInstanceEvent: sets node graph references on master client change (unwanted)
                { //print("print");
                  //object[] data = (object[])photonEvent.CustomData;
                  //int view = (int)data[0];
                  //if (PhotonView.Find(view))
                  //{
                  //    GameObject graph = PhotonView.Find(view).gameObject;
                  //    Norm = graph.GetComponent<Norman>();
                  //    Groder = graph.GetComponent<Groder>();
                  //    Maestro.instance.nodgraf = Norm;
                  //    Maestro.instance.grody = Groder;
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

    //public override void OnMasterClientSwitched(Player newMasterClient)
    //{
    //    print("switch event");
    //    //TODO: hook it up stupid
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        print("new master");
    //    }
    //}

}
