using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

// TODO: pls god wtf does this do help me out here
public class Ring : MonoBehaviour, IPunInstantiateMagicCallback
{
    List<GameObject> visitors = new List<GameObject>();
    public float existTime = 5;
    int group;
    int lives =2;
    int points = 4;
    int view;
    public const byte RingEventCode = 4;
    public const byte RingingEventCode = 5;
    public const byte RingDeathEventCode = 6;
    // TODO: visual group identifier (i.e. color, symbol)

    // info is: group, existTime, points 
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] stuff = info.photonView.InstantiationData;
        group = (int)stuff[0];
        existTime = (int)stuff[1];
        points = (int)stuff[2];
        view = GetComponent<PhotonView>().ViewID;
        print("instance ring " + view);
    }

    private void Update()
    {
        existTime -= Time.deltaTime;
        if (existTime <= 0 || lives == 0) { SendRingDeathEvent(); enabled = false; }

    }

    //TODO: RPC decrement points lives
    private void OnTriggerExit(Collider other)
    {
        // print("ring exit trigger");
        if(visitors.Contains(other.gameObject)) { return; }
        if (other.TryGetComponent<Kart>(out Kart kar))
        {
            // print("ring get kar");
            //why kar.enabled?
            if (kar.group == group && kar.enabled)
            {
                // raise score event
                SendRingEvent(points, kar.GetComponent<PhotonView>().ViewID);
                SendRingingEvent();
                // decrement lives + points
                lives -= 1;
                if (lives == 0) { SendRingDeathEvent(); }
                points /= 2;
                // print(points);
            } else
            {
                // raise minor score event
            }
            visitors.Add(other.gameObject);
        }
    }

    // scoring event
    private void SendRingEvent(int pints, int view)
    {
        // print("ring event sent");
        // print("sent " + pints + " points");
        object[] content = new object[] { pints, view }; 
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(RingEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void SendRingingEvent()
    {
        Vector3 pos = transform.position;
        object[] content = new object[] { group, pos};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(RingingEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void SendRingDeathEvent()
    {
        object[] content = new object[] { group, view};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(RingDeathEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

}
