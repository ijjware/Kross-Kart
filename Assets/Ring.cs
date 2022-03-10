using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class Ring : MonoBehaviour, IPunInstantiateMagicCallback
{
    List<GameObject> visitors = new List<GameObject>();
    public float existTime = 5;
    int group;
    int lives =2;
    int points =4;
    //TODO: visual group identifier (i.e. color, symbol)

    //info is: group, existTime, points 
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] stuff = info.photonView.InstantiationData;
        group = (int)stuff[0];
        existTime = (int)stuff[1];
        points = (int)stuff[2];
    }

    private void Update()
    {
        existTime -= Time.deltaTime;
        if (existTime <= 0 || lives == 0) { Destroy(gameObject); }
    }

    private void OnTriggerExit(Collider other)
    {
        if(visitors.Contains(other.gameObject)) { return; }
        if (TryGetComponent<Kart>(out Kart kar))
        {
            if (kar.group == group)
            {
                //raise score event
                //decrement lives + points
                lives -= 1;
                if (lives == 0) { Destroy(gameObject); }
                points /= 2;
                print(points);
            } else
            {
                //raise minor score event
            }
            visitors.Add(other.gameObject);
        }
    }


    
}
