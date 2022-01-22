using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    private BoxCollider box;

    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("boost");
        other.SendMessage("get_boost");
        box.enabled = false;

    }

    private void get()
    {
        print("cooldown");
        
    }

}
