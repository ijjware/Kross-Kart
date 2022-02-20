using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    public List<Node> neighbours;

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
