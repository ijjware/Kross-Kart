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
    private PhotonView view;
    // 0 == Either; 1 == Blu; 2 == Red
    public int flagType = 0;
    private int pointValue = 2;
    
    public int getFlagType() { return flagType; }
    public int getPoints() { return pointValue; }


    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

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
                
                view.RPC("grab", RpcTarget.All, grabber.GetComponent<PhotonView>().ViewID);
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        view = gameObject.GetComponent<PhotonView>();
        print("flag start");
        isInGoal = true;
        object[] teamData = info.photonView.InstantiationData;
        SetFlagType((int)teamData[0]);
        SetFlagColor();
    }

    [PunRPC]
    void grab(int id)
    {
        Rigidbody yes = PhotonView.Find(id).GetComponent<Rigidbody>();
        if (!gameObject.TryGetComponent<SpringJoint>(out SpringJoint spingy))
        {
            SpringJoint sping = gameObject.AddComponent<SpringJoint>();
            sping.spring = 40;
            sping.tolerance = .01f;
            sping.connectedBody = yes;
            trigger.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            spingy.connectedBody = yes;
            trigger.GetComponent<BoxCollider>().enabled = false;
        }
        grabbed = true;
        isInGoal = false;
    }

    [PunRPC]
    void ex_grab(Rigidbody bod)
    {
        if (!gameObject.TryGetComponent<SpringJoint>(out SpringJoint spingy))
        {
            SpringJoint sping = gameObject.AddComponent<SpringJoint>();
            sping.spring = 30;
            sping.tolerance = .01f;
            sping.connectedBody = bod;
            trigger.GetComponent<BoxCollider>().enabled = false;
        }
    }
    
    [PunRPC]
    void ungrab()
    {
        if (gameObject.TryGetComponent<SpringJoint>(out SpringJoint spingy))
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
