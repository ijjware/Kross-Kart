using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public TrackBase dad;
    public MeshRenderer bluMesh;
    public MeshRenderer redMesh;
    public GameObject self;
    private Material bluMat;
    private Material redMat;
    private Color blu;
    private Color red;
    private Color lightblu;
    private Color lightred;

    // Start is called before the first frame update
    void Start()
    {
        self = this.gameObject;
        // #001D8E
        ColorUtility.TryParseHtmlString(("#6987FF"), out lightblu);
        ColorUtility.TryParseHtmlString(("#FB5959"), out lightred);
        ColorUtility.TryParseHtmlString(("#7D0303"), out red);
        ColorUtility.TryParseHtmlString(("#001D8E"), out blu);
        dim();
    }

    private void OnTriggerEnter(Collider other)
    {
        dad.checkcheck(other, self);
    }

    public void lightBarch()
    {
        // dark blue #001D8E
        // light blue #6987FF
        bluMesh.material.SetColor("_Color", lightblu);
    }

    public void lightRarch()
    {
        // dark red #7D0303
        // light red #FB5959
        redMesh.material.SetColor("_Color", lightred);
    }

    public void dim()
    {
        //print(blu.ToString());
        //print(red.ToString());
        ColorUtility.TryParseHtmlString(("#001D8E"), out blu);
        bluMesh.material.SetColor(Shader.PropertyToID("_Color"), Color.black);
        ColorUtility.TryParseHtmlString(("#7D0303"), out red);
        redMesh.material.SetColor("_Color", Color.black);
    }
}
