using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reproduce 'burst'
public class ItBurst : Item
{
    private void Start()
    {
        cost = 100;
    }

    public override IEnumerator UseItem(Kart own)
    {

        int frames = 0;
        if (inUse) { yield break; }
        if (cost > own.fuelAmt) { yield break; }
        own.fuelAmt -= cost;
        print("using item");
        inUse = true;
        // owner kart lunges forward
        if (own.TryGetComponent<Rigidbody>(out Rigidbody ownBod))
        {
                ownBod.AddRelativeForce(new Vector3(0, 0, 175), ForceMode.VelocityChange);
                inUse = true;
        }
        while(frames < 20)
        {
            ExtDebug.DrawBoxCastBox(own.transform.position, new Vector3(5, 5, 5), Quaternion.identity, own.transform.TransformDirection(Vector3.forward), 30f, Color.blue);
            if (Physics.BoxCast(own.transform.position, new Vector3(5, 5, 5), own.transform.TransformDirection(Vector3.forward),
            out RaycastHit info, Quaternion.identity, 30f, LayerMask.GetMask("Karts"), QueryTriggerInteraction.UseGlobal))
            {
                PhotonView photo = info.collider.GetComponent<PhotonView>();
                photo.RPC("RPC_Stun", photo.Owner);
                print("hit");
                break;
            }
            yield return new WaitForFixedUpdate();
            frames += 1;
        }
        inUse = false;
        print("ItBurst");
        // stuns any kart if hit
    }






}
