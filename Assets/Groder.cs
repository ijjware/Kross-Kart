using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class Groder : MonoBehaviourPunCallbacks
{
    float spawnTimer = 2;
    public Node[] allNodes;
    public bool ready = false;

    Dictionary<int, Node> dactive = new Dictionary<int, Node>();
    Dictionary<int, Node> dupcoming = new Dictionary<int, Node>();

    private void Start()
    {
        allNodes = gameObject.GetComponentsInChildren<Node>();
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0 && ready)
        {

            int i = dactive.Count;
            while (i > 0)
            {
                SetNextNodes(i);
                MoveRings(i);
                i--;
            }
            spawnTimer = 5;
        }
    }
    // pretend we already have a active and upcoming node
    // assign new nodes
    private void SetNextNodes(int group)
    {
        Node next = dupcoming[group];
        foreach(Node nod in next.neighbours)
        {
            if(nod != dactive[group])
            {
                dactive[group] = next;
                dupcoming[group] = nod;
                return;
            }
        }
    }

    public Node GetNextNode(int group)
    {
        Node next = dupcoming[group];
        foreach (Node nod in next.neighbours)
        {
            if (nod != dactive[group])
            {
                return nod;
            }
        }
        print("GetNextNode oops");
        return null;
    }


    //determine starting nodes based on spawn
    // NEEDS: kart position and forward vector in global space
    public void SetStartingNodes(Vector3 spawnPos, Vector3 spawnDir, int group)
    {
        allNodes = gameObject.GetComponentsInChildren<Node>();
        print("setStart");
        List<Node> valids = new List<Node>();
        foreach (Node x in allNodes)
        {
            if (Vector3.Dot(spawnDir, x.transform.position) == 0)
            {
                print("valids added");
                valids.Add(x);
            } else { print("invalid"); }
        }
        print("uper");
        List<Node> uperValids = new List<Node>();
        //now have all nodes in front of kart
        //sort by threshold distance
        foreach (Node y in valids)
        {
            if (Vector3.Distance(spawnPos, y.transform.position) < 200)
            {
                print("inThresh");
                uperValids.Add(y);
            } else { print("outThresh"); }
        }
        //pick an valid node
        //TODO: make it random
        Node guy = uperValids[0];
        Node nextGuy = guy.neighbours[0];
        print("guy");
        foreach (Node z in guy.neighbours)
        {
            // sort neighbours by 'most in that direction'
            if (Vector3.Dot(spawnDir, z.transform.position) > Vector3.Dot(spawnDir, nextGuy.transform.position))
            {
                nextGuy = z;
            }
        }
        dactive[group] = guy;
        dupcoming[group] = nextGuy;
        ready = true;
        AddRing(group, guy);
    }


    //add next ring method
    //try rendering line
    private void MoveRings(int group)
    {
        Node next = dupcoming[group];
        AddRing(group, next);
    }

    private void AddRing(int group, Node node)
    {
        
        object[] team = new object[3] { 0, 0, 0 };
        team[0] = group;
        team[1] = 5;
        team[2] = 4;
        PhotonNetwork.Instantiate("Ring", node.transform.position,
            node.transform.rotation, 0, team);
    }



    
}
