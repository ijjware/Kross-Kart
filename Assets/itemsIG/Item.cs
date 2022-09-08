using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

//superclass that defines common methods/properties of all items
public abstract class Item : MonoBehaviour
{
    //attached to gameobject childed to owner kart
    protected int cost = 0;
    protected bool inUse = false;
    //void 'use item' method
    public abstract IEnumerator UseItem(Kart Owner);

    
}
