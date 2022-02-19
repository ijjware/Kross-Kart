using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    public Node[] neighbours;

    //get a(n) node(s) n connections away from a given node
    //call method on each neighbour
    //method would have sender node and n-1 as params
    //when n == 0 those nodes are n away from the first node
    //public void GetNConnects(List<GameObject> senders, List<GameObject> list, int n)
    //{
    //    if (n == 0) { return; }
    //    foreach (GameObject node in neighbours)
    //    {
    //        if (n - 1 == 0)
    //        {
    //            list.Add(node);
    //        }
    //        else { senders.Add(node); }
    //    }
    //    foreach (GameObject node in neighbours)
    //    {
    //       if (TryGetComponent<Node>(out Node nod))
    //        {
    //            nod.GetNConnects(senders, list, n - 1);
    //        }
    //    }




    //}

    private void OnDrawGizmos()
    {
        foreach (Node node in neighbours)
        {
            Gizmos.DrawLine(gameObject.transform.position, node.transform.position);
        }
    }
    //if (n == 0)
    //{
    //    if (!senders.Contains(gameObject)) { list.Add(gameObject); }
    //    return;
    //}
    //else
    //{
    //    senders.Add(gameObject);
    //}

    //foreach (GameObject node in neighbours)
    //{
    //    if (node.TryGetComponent<Node>(out Node nod))
    //    {
    //        if (!senders.Contains(node)) { nod.GetNConnects(senders, list, n - 1); }
    //    }
    //}
}
