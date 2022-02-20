using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;


//[System.Serializable]
public class Flag : MonoBehaviour, IPunInstantiateMagicCallback
{
    public GameObject trigger;
    public bool grabbed = false;
    public Collider grabber;
    public bool isInGoal = true;

    // 0 == Either; 1 == Blu; 2 == Red
    public int flagType = 0;
    private int pointValue = 2;
    
    public int getFlagType() { return flagType; }
    public int getPoints() { return pointValue; }

    private void OnTriggerExit(Collider other)
    {
        if (grabbed) { return; }
        if(other.TryGetComponent<Kart>(out Kart kart))
        {
            if (kart.enabled)
            {
                if (kart.team == flagType && isInGoal) { print("nono"); return;}
                print("graeb");
                grabber = other;
                kart.isHoldingFlag = true;
                kart.heldFlags.Enqueue(gameObject.GetComponent<PhotonView>().ViewID);
                grabbed = true;
                isInGoal = false;
                grab();
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {

        print("flag start");
        isInGoal = true;
        object[] teamData = info.photonView.InstantiationData;
        SetFlagType((int)teamData[0]);
        SetFlagColor();

    }

    void grab()
    {
        if (!gameObject.TryGetComponent<HingeJoint>(out HingeJoint spingy))
        {
            HingeJoint sping = gameObject.AddComponent<HingeJoint>();
            //sping.spring = 40;
            //sping.tolerance = .01f;
            sping.connectedBody = grabber.attachedRigidbody;
            trigger.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            spingy.connectedBody = grabber.attachedRigidbody;
            trigger.GetComponent<BoxCollider>().enabled = false;
        }
    }

    void grab(Rigidbody bod)
    {
        if (!gameObject.TryGetComponent<HingeJoint>(out HingeJoint spingy))
        {
            HingeJoint sping = gameObject.AddComponent<HingeJoint>();
            //sping.spring = 30;
            //sping.tolerance = .01f;
            sping.connectedBody = bod;
            trigger.GetComponent<BoxCollider>().enabled = false;
        }
    }

    void ungrab()
    {
        if (gameObject.TryGetComponent<HingeJoint>(out HingeJoint spingy))
        {
            print("drep");
            Destroy(spingy);
            trigger.GetComponent<BoxCollider>().enabled = true;
            grabbed = false;
        }
    }

    void SetFlagColor()
    {
        Color flagColor;
        if (flagType == 1) { flagColor = Color.blue; }
        else { flagColor = Color.red; }
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", flagColor);
    }

    void SetFlagType(int type) { flagType = type; }

}
