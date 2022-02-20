using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Norman : MonoBehaviour
{
    //norman would be attached to the parent object of all the nodes in the graph
    public Node[] allNodes;
    public Node enterer;
    public List<Node> checks;
    public GameObject test;

    private void Start()
    {
        allNodes = gameObject.GetComponentsInChildren<Node>();
        //checks = GetNConnects(enterer, 4);
        //ConnectNodes();
        //enterer = GetClosestNode(test.transform.position);
    }


    
    public Node GetClosestNode(Vector3 point)
    {
        Node closest = allNodes[0];
        foreach (Node node in allNodes)
        {
            float distSmall = Vector3.Distance(closest.transform.position, point);
            float distOther = Vector3.Distance(node.transform.position, point);
            if (distSmall > distOther )
            {
                closest = node;
            }
        }
        return closest;
    }

    public Node GetFurthestNode(Vector3 point)
    {
        Node closest = allNodes[0];
        foreach (Node node in allNodes)
        {
            float distSmall = Vector3.Distance(closest.transform.position, point);
            float distOther = Vector3.Distance(node.transform.position, point);
            if (distSmall < distOther)
            {
                closest = node;
            }
        }
        return closest;
    }

    public void ConnectNodes()
    {

        //Node guy;
        foreach(Node x in allNodes)
        {
            foreach(Node y in allNodes)
            {
                if (x.Equals(y) ) { continue; }
                if (!Physics.Linecast(x.transform.position, y.transform.position))
                {
                    //print("linecast");
                    if (x.neighbours.Contains(y)) { continue; }
                    x.neighbours.Add(y);
                    y.neighbours.Add(x);
                    //Gizmos.DrawLine(x.transform.position, y.transform.position);
                    

                }
            }
        }


    }





    public List<Node> GetNConnects(Node nod, int n)
    {
        print("GetNConnects");
        Queue<Node> nodes = new Queue<Node>();
        Queue<Node> noddies = new Queue<Node>();
        List<Node> nots = new List<Node>();
        nots.Add(nod);
        nodes.Enqueue(nod);
        Node nodder = nod;
    Queue1:
        print("Queue1");
        while (nodes.Count > 0)
        {
            nodder = nodes.Dequeue();
            nots.Add(nodder);
            foreach (Node nods in nodder.neighbours)
            {
                print("add noddies");
                noddies.Enqueue(nods);
            }
        }
        n--;
        if (n != 0) { goto Queue2; }
        else { return CheckNConnects(nots, noddies); }
    Queue2:
        print("Queue2");
        while (noddies.Count > 0)
        {
            nodder = noddies.Dequeue();
            nots.Add(nodder);
            foreach (Node nods in nodder.neighbours)
            {
                nodes.Enqueue(nods);
            }
        }
        n--;
        if (n != 0) { goto Queue1; }
        else { return CheckNConnects(nots, nodes); }
    }

    public List<Node> CheckNConnects(List<Node> nots, Queue<Node> nodes)
    {
        print("CheckNConnects");
        List<Node> haves = new List<Node>();
        while (nodes.Count > 0)
        {
            Node node = nodes.Dequeue();
            if (!nots.Contains(node) ) { haves.Add(node); }
        }
        return haves;
    }

    /* bfs(g, v) 
     {
        queue.push(v)
        while (queue.len > 0) 
        {
            v = queue.pop
            if not visit: visit v
                for w in v.neighbours:
                    if not visit: queue.push w
        }
    }

     */






}
