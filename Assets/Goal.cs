using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class Goal : MonoBehaviour
{
    public const byte GoalEventCode = 1;
    public int goalType = 1;
    public Rigidbody bod;

    private void Start()
    {
        bod = GetComponent<Rigidbody>();
    }

    private void SendGoalEvent(int kartViewID, int flagViewID)
    {
        print("Goal send");
        object[] content = new object[] { kartViewID, flagViewID, goalType}; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(GoalEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Kart>(out Kart kart))
        {
            print("kart");
            if (kart.enabled && kart.isHoldingFlag)
            {
                int Kartid = kart.GetComponent<PhotonView>().ViewID;
                int Flagid = kart.flag.GetComponent<PhotonView>().ViewID;
                print("local car");
                SendGoalEvent(Kartid, Flagid);
                kart.isHoldingFlag = false;

                //room objects can only be destroyed by master client
                // so find a work-around idk
                //PhotonNetwork.Destroy(kart.flag.gameObject);
            }
        }
    }

    public void FixedUpdate()
    {
        //rotate along y axis
        Vector3 rot = new Vector3(0, 100, 0);
        Quaternion deltaRotation = Quaternion.Euler(rot * Time.fixedDeltaTime);
        bod.MoveRotation(bod.rotation * deltaRotation);
    }

}
