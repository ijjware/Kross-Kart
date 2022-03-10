using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class PlayerSorter : MonoBehaviourPunCallbacks
{

    private int numPlayers;
    public int currentGroup = 1;
    private Dictionary<int, List<int>> groupings = new Dictionary<int, List<int>>();

    override public void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(groupings.TryGetValue(currentGroup, out List<int> value))
        {
            value.Add(newPlayer.ActorNumber);
            currentGroup += 1;
        } else
        {
            List<int> vs = new List<int>();
            vs.Add(newPlayer.ActorNumber);
            groupings[currentGroup] = vs;
        }
    }

    void Start()
    {
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player.IsLocal) 
            {
                List<int> vs = new List<int>();
                vs.Add(player.ActorNumber);
                groupings[1] = vs;
            }
        }
    }

    public Dictionary<int, List<int>> getGroupings()
    {
        return groupings;
    }

}
