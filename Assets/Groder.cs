using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

//adds and tracks current rings
// 9-7 TODO: should be able to toggle ring spawning and scoring seperately probably?
public class Groder : MonoBehaviourPunCallbacks
{
    public static Groder instance;
    private void Awake()
    {
        instance = this;
    }
    float spawnTimer = 2;
    public Node[] allNodes;
    public Node activeNode;
    public bool ready = false;

    Dictionary<int, Node> dactive = new Dictionary<int, Node>();
    Dictionary<int, Node> dupcoming = new Dictionary<int, Node>();

    public HashSet<GameObject> ringlist = new HashSet<GameObject>();
    Dictionary<int, HashSet<GameObject>> activeRings = new Dictionary<int, HashSet<GameObject>>();

    private void Start()
    {
        allNodes = gameObject.GetComponentsInChildren<Node>();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
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
            spawnTimer = 2;
        }
    }

    // pretend we already have an active and upcoming node
    // assign new nodes
    private void SetNextNodes(int group)
    {
        Node next = dupcoming[group];
        //make new list of valid nodes
        List<Node> noxts = new List<Node>();
        foreach (Node nod in next.neighbours)
        {
            if(nod != dactive[group])
            {
                noxts.Add(nod);
            }
        }
        //randomly select from list
        int index = (int)Random.Range(0f, (float)noxts.Count);
        if (index < 0) { index = 0; }
        dactive[group] = next;
        dupcoming[group] = noxts[index];
        //TODO: weight node selection based on past routes
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
        //print("GetNextNode oops");
        return null;
    }

    //determine starting nodes based on spawn
    public void SetStartingNodes(Vector3 spawnPos, Vector3 spawnDir, int group)
    {
        Debug.DrawRay(spawnPos, spawnDir * 100, Color.blue, float.PositiveInfinity);
        allNodes = gameObject.GetComponentsInChildren<Node>();
        //print("setStart");
        List<Node> valids = new List<Node>();
        foreach (Node x in allNodes)
        {
            float dot = Vector3.Dot(spawnDir, x.transform.position - spawnPos);
            if ( dot > 0)
            {
                valids.Add(x);
            } //else { print(x.name + "invalid"); print(dot); }
        }
        List<Node> uperValids = new List<Node>();
        //now have all nodes in front of kart
        //sort by threshold distance
        foreach (Node y in valids)
        {
            if (Vector3.Distance(spawnPos, y.transform.position) < 100)
            {
                uperValids.Add(y);
            }
        }
        //pick an valid node
        int index = (int)Random.Range(0f, (float)uperValids.Count);
        if (index < 0) { index = 0; }
        Node guy = uperValids[index];
        Node nextGuy = guy.neighbours[0];
        
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
        activeRings[group] = new HashSet<GameObject>();
        ready = true;
        AddRing(group, guy);
        //print(guy.name);
    }

    //add next ring method
    private void MoveRings(int group)
    {
        //actually moving code
        Node next = dupcoming[group];
        AddRing(group, next);
    }

    private void AddRing(int group, Node node)
    {
        activeNode = node;
        object[] team = new object[3] { 0, 0, 0 };
        team[0] = group;
        team[1] = 5;
        team[2] = 4;
        GameObject newRing = PhotonNetwork.InstantiateRoomObject("Ring", node.transform.position,
            node.transform.rotation, 0, team);
        activeRings[group].Add(newRing);
        
    }

    //what does this do??
    //intended to update rings based on player position so player doesn't run out of rings
    public void RingChecker(int group, Node node)
    {
        if (dupcoming[group] == node)
        {
            SetNextNodes(group);
            MoveRings(group);
            spawnTimer = 2;
        }
    }


    //updates active ring list upon ring death
    public void RingDeader(int group, int num)
    {
        
        bool repeat = true;
        HashSet<GameObject> rings = activeRings[group];
        while(repeat)
        {
            repeat = false;
            foreach (GameObject rin in rings)
            {
                if (!rin)
                {
                    rings.Remove(rin);
                    repeat = true;
                    break;
                }
            }
        }
    }
    
}
