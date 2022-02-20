using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleur : MonoBehaviour
{
    private Rigidbody bod;
    public float spin = 100;

    // Start is called before the first frame update
    void Start()
    {
        bod = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rot = new Vector3(0, 0, spin);
        Quaternion deltaRotation = Quaternion.Euler(rot * Time.fixedDeltaTime);
        bod.MoveRotation(bod.rotation * deltaRotation);
    }

}
