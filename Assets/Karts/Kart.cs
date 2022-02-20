using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.Animations;


public class Kart : MonoBehaviour
{
    public Rigidbody bod;
    public PhotonView enem;
    public PhotonView view;
    public GameObject you;

    enum DriftMethod { relative, absolute }
    [SerializeField]
    DriftMethod driMethod = DriftMethod.absolute;
    enum BurstMethod { auto, manual}
    [SerializeField]
    BurstMethod buMethod = BurstMethod.manual;

    //acceleration vars
    public float acceleration = 0.0f;
    public float rate_accel = 1f;
    public float max_accel = 25f;

    //boost vars
    public float boostStrength = 50f;
    private float boost = 0f;
    public float fuelAmt = 0;
    
    //drift idk
    public float driftAccel = 0f;
    public float driftTurn = 100;
    public float driftTime = 0f;

    //burst vars
    public float burstAmt = 1500;
    public bool isBursting = false;
    public Vector3 move = new Vector3(0, 0, 0);
    public Vector3 drift = new Vector3(0, 0, 0);
    public float burstTime = 0f;
    public float burstDuration = 0.5f;

    //turning vars
    public float min_turn = 125;
    public float turn_speed = 200;

    //big bool
    public bool active = true;
    private Maestro master;
    private int layermask = 0;
    public int team = 2;

    //flag vars
    public bool isHoldingFlag = false;
    public Flag flag;
    //[SerializeField]
    public Queue<int> heldFlags = new Queue<int>();

    //input vars
    public bool accelerating = false;
    public bool drifting = false;
    public Vector2 left_steering;
    public float roll;
    public bool boosting = false;

    //magnet vars
    public bool stuck = false;
    private float maxStuckTime = 5f;
    private float minStuckTime = 1f;
    public int mashInputs = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!active)
        { 
          if (isHoldingFlag) { flag.SendMessageUpwards("ungrab"); }  
            return;
        }

        //stunRaycastHit info;

        if (!active) { return; }
        if (isBursting && burstTime > 0)
        {
            //Stunner();
            burstTime -= Time.fixedDeltaTime;
        }
        else if (isBursting && burstTime <= 0)
        {
            isBursting = false;
            burstTime = 0;
        }
        //Magnetize();
        if (drifting)
        {
            if (driMethod == DriftMethod.relative) { RelativeDrift(); }
            else { AbsoluteDrift(); }
            //drift *= .99f;
        } else {
            acccelerating();
            booosting();
            move_kart();
        }
        master.boost_update(fuelAmt);
        TrackYou();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<Kart> (out Kart kart)) {
            if (isBursting)
            {
                PhotonView ek =  kart.GetComponent<PhotonView>();
                ek.RPC("RPC_Stun", ek.Owner);
            }
        }
    }

    void Stunner()
    {
        // check for driftboost in update
        RaycastHit info;
        if (Physics.BoxCast(transform.position, new Vector3(5, 5, 5), transform.TransformDirection(Vector3.forward),
            out info, Quaternion.identity, 30f, LayerMask.GetMask("Karts"), QueryTriggerInteraction.UseGlobal))
        {
            print(info.collider.name);
            if (info.collider.gameObject.TryGetComponent<NetworkedPlayer>(out NetworkedPlayer p))
            {
                //stun

                if (PhotonNetwork.InRoom)
                {
                    print("roomy");
                    if (info.collider.TryGetComponent<PhotonView>(out PhotonView viewtoo)) { viewtoo.RPC("RPC_Stun", RpcTarget.All); }
                } else { info.collider.SendMessage("RPC_Stun"); }

            }

        }

    }

    [PunRPC]
    void RPC_ColorTrail(float r, float g, float b)
    {
        if(TryGetComponent<TrailRenderer>(out TrailRenderer trail))
        {
            print("color");
            Color color = new Vector4(r, g, b);
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.75f), new GradientColorKey(color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
                );
            trail.colorGradient = gradient;

        }
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
                acceleration -= rate_accel;
                if (acceleration < 0) { acceleration = 0; }
            }
        }
    }

    void RelativeDrift()
    {
        Vector3 rot;
        float amt = left_steering.x * (min_turn + (turn_speed * (max_accel - acceleration) / (max_accel * 200)) - (driftTurn / 4));
        rot.y = amt;
        amt = left_steering.y * (min_turn + (turn_speed * (max_accel - acceleration) / (max_accel * 200)) - (driftTurn/4));
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
        RaycastHit info;
        if (btn)
        {
            
            move.z = acceleration;
            if (driMethod == DriftMethod.absolute) { drift = transform.TransformDirection(move); }
            else { drift = move; }
            //print("driftu");
            drifting = true;
        }
        else
        {
            if (driftTime >= 2f) { modifier = 2; }
            else if (driftTime >= 0.3f) { modifier = driftTime; }
            else { modifier = 0; }
            drifting = false;
            if (buMethod == BurstMethod.auto) { buursting(modifier); }
            //else if (buMethod == BurstMethod.manual) { get_boost(modifier); }
            driftTime = 0;
        }
        //{
        //    isDriftBoosting = true;
        //    layermask = 1 << 12;
        //    Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out info, Mathf.Infinity, layermask);
        //    //hits a wall
        //    if (!info.collider)
        //    {
        //        isDriftBoosting = false;
        //        drifting = false;
        //        return;
        //    }
        //    if (info.collider && info.distance < 30)
        //    {
        //        print("bonk" + info.collider.name);
        //        layermask = 1 << 11;
        //        isDriftBoosting = false;
        //        driftTime = 0;

    }

    public void remote_buurst(float mod)
    {
        if (buMethod == BurstMethod.manual) { buursting(mod); }
    }

    private void buursting(float reduction)
    {
        //check if there is a wall in front of player
        //check method: if manual, check for enough fuel
        if (buMethod == BurstMethod.manual && fuelAmt < 125) { return; }
        float burst = burstAmt * reduction;
        bod.AddRelativeForce(new Vector3(0, 0, burst), ForceMode.VelocityChange);
        burstTime = burstDuration * reduction;
        isBursting = true;
        if (buMethod == BurstMethod.manual) { fuelAmt -= 125; }
    }

    // pickup boost effect
    public void booosting()
    {
        //RaycastHit info;
        
        if (boosting && fuelAmt > 0)
        {
            //layermask = 1 << 11;
            boost = boostStrength;
            fuelAmt -= 1;
            //infinite boost line
            //boostAmt += 1;
        } else if (fuelAmt == 0)
        {
            print("boost empty");
            fuelAmt = -1;
        }
    }

    public void get_boost(float modifier)
    {
        fuelAmt += 50 * modifier;
        if (fuelAmt > 200) { fuelAmt = 200;}
        print("boost get");
    }

    //setters
    public void setMaster(Maestro maestro) { master = maestro; }

    void TrackYou()
    {
        Vector3 pos = transform.position;
        you.transform.localPosition = pos;
    }




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
