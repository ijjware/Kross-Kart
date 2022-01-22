using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public TrackBase dad;
    public GameObject self;

    // Start is called before the first frame update
    void Start()
    {
        self = this.gameObject;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        dad.checkcheck(other, self);
    }

}
