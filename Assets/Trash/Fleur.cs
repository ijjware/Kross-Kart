using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using Photon.Pun;

public class Fleur : MonoBehaviour
{
    private Rigidbody bod;
    private PhotonView view;
    public float spin = 100;

    // Start is called before the first frame update
    void Start()
    {
        bod = GetComponent<Rigidbody>();
        //view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 rot = new Vector3(0, 0, spin);
            Quaternion deltaRotation = Quaternion.Euler(rot * Time.fixedDeltaTime);
            bod.MoveRotation(bod.rotation * deltaRotation);
        }
    }

}
