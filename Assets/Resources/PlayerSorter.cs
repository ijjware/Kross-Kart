using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;


//intended to handle player groupings + spawn locations
public class PlayerSorter : MonoBehaviourPunCallbacks
{
    public static PlayerSorter instance;
    private void Awake()
    {
        instance = this;
    }
    private int numPlayers;
    public int currentGroup = 1;
    // dict where key == group num and value == player num
    private Dictionary<int, List<int>> groupings = new Dictionary<int, List<int>>();
    // dict where key == player num and value == group num
    private Dictionary<int, int> reverseGroupings = new Dictionary<int, int>();

    override public void OnPlayerEnteredRoom(Player newPlayer) // event fires for remote players? not local?
    {
        print("player entered");
        if(groupings.TryGetValue(currentGroup, out List<int> value))
        {
            value.Add(newPlayer.ActorNumber);
            reverseGroupings[newPlayer.ActorNumber] = currentGroup;
            currentGroup += 1;
        } else
        {
            List<int> vs = new List<int>();
            vs.Add(newPlayer.ActorNumber);
            groupings[currentGroup] = vs;
            reverseGroupings[newPlayer.ActorNumber] = currentGroup;
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
                reverseGroupings[player.ActorNumber] = 1;
                groupings[1] = vs;
            }
        }
    }

    public Dictionary<int, List<int>> getGroupings()
    {
        return groupings;
    }

    public void SetGroup(int ActorID, PhotonView view)
    {
        print("SetGroup try");
        if (reverseGroupings.TryGetValue(ActorID, out int group))
        {
            print("SetGroup match");
            view.RPC("SetGroup", RpcTarget.All, group);
        }
    }



    //statements to prevent players from joining room mid game
    //PhotonNetwork.CurrentRoom.IsOpen = false;
    //PhotonNetwork.CurrentRoom.IsVisible = false;
}
