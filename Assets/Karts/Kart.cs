using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.Animations;
using ExitGames.Client.Photon;

public class Kart : MonoBehaviour/*, IPunObservable*/
{
    public Rigidbody bod;
    public PhotonView enem;
    public PhotonView view;

    //'you' is the HUD child node that represents player location on the minimap
    public GameObject you;
    //OG denotes whether this instance of the kart belongs to the client
    public bool OG = false;

    // IPunObservable vars
    //Vector3 networkPosition;
    //Quaternion networkRotation;

    enum DriftMethod { relative, absolute }
    [SerializeField]
    DriftMethod driMethod = DriftMethod.absolute;
    enum BurstMethod { auto, manual}
    [SerializeField]
    BurstMethod buMethod = BurstMethod.manual;

    //acceleration vars
    public float acceleration = 0.0f;
    public float rate_accel = 1f;
    public float max_accel = 1.5f;

    //boost vars
    public float boostStrength = 1f;
    private float boost = 0f;
    public float fuelAmt = 0;
    
    //drift idk
    public float driftAccel = 0f;
    public float driftTurn = 100;
    public float driftTime = 0f;
    public float driftReduct = .99f;

    public Vector3 move = new Vector3(0, 0, 0);
    public Vector3 drift = new Vector3(0, 0, 0);

    //turning vars
    public float min_turn = 125;
    public float turn_speed = 200;

    //big bool (determines if this instance of the kart can receive input?)
    public bool active = true;
    private PlayerMaestro master;
    public int team = 2;
    public int group;

    //flag vars
    public bool isHoldingFlag = false;
    public Queue<int> heldFlags = new Queue<int>();

    //input vars
    public bool accelerating = false;
    public bool drifting = false;
    public Vector2 left_steering;
    public float roll;
    public bool boosting = false;

    //magnet vars
    //public bool stuck = false;
    //private float maxStuckTime = 5f;
    //private float minStuckTime = 1f;
    //public int mashInputs = 0;

   
    void FixedUpdate()
    {
        //the next few lines are part of IPunObservable interface
        //if (!view.IsMine)
        //{
        //    bod.position = Vector3.MoveTowards(bod.position, networkPosition, Time.fixedDeltaTime);
        //    bod.rotation = Quaternion.RotateTowards(bod.rotation, networkRotation, (Time.fixedDeltaTime * 150.0f));
        //}

        if (!active) { return; }
        if (!OG) { return; }
        if (!boosting) { get_boost(0.012f); }
        //Magnetize();
        if (drifting)
        {
            if (driMethod == DriftMethod.relative) { RelativeDrift(); }
            else { AbsoluteDrift(); }
            drift *= driftReduct;
        } else {
            acccelerating();
            booosting();
            move_kart();
        }
        { master.boost_update(fuelAmt); }
        TrackYou();
    }

    [PunRPC]
    void RPC_ColorTrail(float r, float g, float b)
    {
        if(TryGetComponent<TrailRenderer>(out TrailRenderer trail))
        {
            Color color = new Vector4(r, g, b);
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.75f), new GradientColorKey(color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
                );
            trail.colorGradient = gradient;
        }
        //TODO: make this work in multiplayer
        //if(you.TryGetComponent<MeshRenderer>(out MeshRenderer meshy))
        //{
        //    //meshy.material.SetColor(you.name, new Color(r, g, b));
        //    meshy.material.color = new Color(r, g, b);
        //}
    }

    [PunRPC]
    void SetGroup(int id)
    {
        print("set in group " + id);
        group = id;
    }

    [PunRPC]
    void RPC_Stun()
    {
        StartCoroutine(stun());
    }

    IEnumerator stun()
    {
        print("stune");
        active = false;
        yield return new WaitForSeconds(2);
        active = true;
    }

    void acccelerating()
    {
        if (accelerating)
        {
            if (acceleration < max_accel)
            {
                acceleration += rate_accel;
                if (acceleration > max_accel) { acceleration = max_accel; }
            }
        }
        else
        {
            if (acceleration > 0)
            {
                acceleration = 0;
                //TODO: what do the next two lines do?
                //acceleration -= rate_accel;
                //if (acceleration < 0) { acceleration = 0; }
            }
        }
    }

    //TODO: pls denote diff between absolute and relative methods
    void RelativeDrift()
    {
        Vector3 rot;
        float amt = left_steering.x * (min_turn + (turn_speed * (max_accel - acceleration) / (max_accel * 200)) + driftTurn);
        rot.y = amt;
        amt = left_steering.y * (min_turn + (turn_speed * (max_accel - acceleration) / (max_accel * 200)) + driftTurn);
        rot.x = -amt;
        rot.z = roll * 100;
        Quaternion deltaRotation = Quaternion.Euler(rot * Time.fixedDeltaTime);
        bod.MoveRotation(bod.rotation * deltaRotation);
        driftTime += Time.fixedDeltaTime;
        if (fuelAmt < 125 && buMethod == BurstMethod.manual) { fuelAmt += 0.5f; }
        bod.AddRelativeForce(drift, ForceMode.VelocityChange);
    }

    void AbsoluteDrift()
    {
        Vector3 rot;
        float amt = left_steering.x * (min_turn + (turn_speed * (max_accel - acceleration) / (max_accel * 200)) + driftTurn);
        rot.y = amt;
        amt = left_steering.y * (min_turn + (turn_speed * (max_accel - acceleration) / (max_accel * 200)) + driftTurn);
        rot.x = -amt;
        rot.z = roll * 100;
        Quaternion deltaRotation = Quaternion.Euler(rot * Time.fixedDeltaTime);
        bod.MoveRotation(bod.rotation * deltaRotation);
        driftTime += Time.fixedDeltaTime;
        if (fuelAmt < 125 && buMethod == BurstMethod.manual) { fuelAmt += 1f; }
        bod.AddForce(drift, ForceMode.VelocityChange); 
    }

    void move_kart()
    {

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 10), Color.red, 1f);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down * 10), Color.red, 1f);
        Vector3 rot;
        float amt = left_steering.x * (min_turn + (turn_speed * (max_accel - acceleration) / (max_accel * 200)) + boost);
        rot.y = amt;
        amt = left_steering.y * (min_turn + (turn_speed * ( max_accel - acceleration) / (max_accel * 200)) + boost);
        rot.x = -amt;
        rot.z = roll * 100;
        Quaternion deltaRotation = Quaternion.Euler(rot * Time.fixedDeltaTime); 
        bod.MoveRotation(bod.rotation * deltaRotation);
        move.z = acceleration + boost;
        if (move.z > 50) { move.z = 50; }
        bod.AddRelativeForce(move, ForceMode.VelocityChange);
        boost = 0f;
    }

    public void driift(bool btn)
    {
        float modifier = 0;
        if (btn)
        {
            
            move.z = acceleration;
            if (driMethod == DriftMethod.absolute) { drift = transform.TransformDirection(move); }
            else { drift = move; }
            drifting = true;
        }
        else
        {
            if (driftTime >= 2f) { modifier = 2; }
            else if (driftTime >= 0.3f) { modifier = driftTime; }
            else { modifier = 0; }
            drifting = false;
            driftTime = 0;
        }

    }

    // pickup boost effect
    public void booosting()
    {
        if (boosting && fuelAmt > 0)
        {
            boost = boostStrength;
            fuelAmt -= 1;
        } else if (fuelAmt < 0)
        {
            fuelAmt = 0;
        }
    }

    public void get_boost(float modifier)
    {
        fuelAmt += 50 * modifier;
        if (fuelAmt > 200) { fuelAmt = 200;}
    }

    //setters
    //TODO: must be a way to isolate this so the kart script doesn't have to be aware of it's master
    public void setMaster(PlayerMaestro maestro) { master = maestro; }

    //TODO: maybe same as above, does the kart script NEED to be aware of the HUD child 'you'
    void TrackYou()
    {
        Vector3 pos = transform.position;
        you.transform.localPosition = pos;
    }

    /* 
        fixed a lot of the jitter but the rotation is still weirdly jumpy,
        messing with step values helps but doesn't fix it. 
        regular rigidbody photonview still works slightly better at least with 2 players.
    */

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(this.bod.position);
    //        stream.SendNext(this.bod.rotation);
    //        stream.SendNext(this.bod.velocity);
    //    }
    //    else
    //    {
    //        networkPosition = (Vector3)stream.ReceiveNext();
    //        networkRotation = (Quaternion)stream.ReceiveNext();
    //        bod.velocity = (Vector3)stream.ReceiveNext();

    //        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
    //        networkPosition += (this.bod.velocity * lag);
    //    }
    //}




    //AYO: unfinished magnetize scripts

    //public void Magnetize()
    //{
    //    RaycastHit info;

    //    if (!active) { return; }
    //    //magnetizer
    //    if (isBursting && driftTime > 0)
    //    {
    //        if (Physics.BoxCast(transform.position, new Vector3(5, 5, 5), transform.TransformDirection(Vector3.forward),
    //            out info, Quaternion.identity, 30f, LayerMask.GetMask("Default"), QueryTriggerInteraction.UseGlobal))
    //        {
    //            print(info.collider.name);
    //            if (info.collider.name == "Network Kart(Clone)")
    //            {
    //                //info.collider.SendMessage("stick");
    //                enem = info.collider.GetComponent<PhotonView>();
    //                //enem.TransferOwnership(view.Owner);
    //                if (enem) { enem.RPC("stick", RpcTarget.All, view.ViewID); }
    //                //enem.stick(GetComponent<Rigidbody>());
    //                print("sticky");
    //            }

    //        }
    //        driftTime -= Time.fixedDeltaTime;
    //    }
    //    else if (isDriftBoosting && driftTime <= 0)
    //    {
    //        isDriftBoosting = false;
    //        driftTime = 0;
    //    }
    //}

    //[PunRPC]
    //void stick(int id)
    //{
    //    if (!GetComponent<FixedJoint>())
    //    {
    //        bod.mass = 1;
    //        PhotonView vicker = PhotonView.Find(id);
    //        //bod.constraints = RigidbodyConstraints.FreezeAll;
    //        //bod.isKinematic = true;
    //        FixedJoint sticker = gameObject.AddComponent<FixedJoint>();
    //        sticker.connectedBody = vicker.GetComponent<Rigidbody>();
    //        sticker.enableCollision = false;
    //        sticker.massScale = sticker.connectedBody.mass / bod.mass;
    //        sticker.connectedMassScale = 1;
    //        //Destroy(GetComponent<PhotonView>());
    //        //view.TransferOwnership(vicker.Owner);
    //        GetComponent<PhotonRigidbodyView>().enabled = false;
    //        GetComponent<PhotonTransformView>().enabled = false;
    //        active = false;
    //        stuck = true;
    //    }
    //    print("sticky");

    //}


}
