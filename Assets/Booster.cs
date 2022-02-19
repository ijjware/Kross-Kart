using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    private SphereCollider box;
    private MeshRenderer icon;
    public float cooldown = 5;
    private float time = 5;

    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<SphereCollider>();
        icon = GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (time >= cooldown)
        {
            box.enabled = true;
            icon.enabled = true;
        } else { time += Time.fixedDeltaTime; }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("boost");
        if (other.TryGetComponent<Kart>(out Kart kart))
        {
            kart.get_boost(1f);
        }
        box.enabled = false;
        icon.enabled = false;
        time = 0;
    }

    private void get()
    {
        print("cooldown");
        
    }

}
