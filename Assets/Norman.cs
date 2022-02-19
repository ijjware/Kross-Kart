using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Norman : MonoBehaviour
{
    //norman would be attached to the parent object of all the nodes in the graph
    public Node enterer;
    public List<Node> checks;

    private void Start()
    {
       checks = GetNConnects(enterer, 4);
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
