using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wabbit : MonoBehaviour
{
    public float targetDist = 100;
    public int tarnum = 1;
    public GameObject target1;
    public GameObject target2;
    public GameObject target3;
    public GameObject target4;
    public NavMeshAgent agent;





    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(target1.transform.position);
        //print(dest.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < targetDist)
        {
            switch (tarnum)
            {
                case 1:
                    print("1");
                    agent.SetDestination(target2.transform.position);
                    tarnum++;
                    break;
                case 2:
                    print(2);
                    agent.SetDestination(target3.transform.position);
                    tarnum++;
                    break;
                case 3:
                    print(3);
                    agent.SetDestination(target4.transform.position);
                    tarnum = 0;
                    break;
                default: break;
            }
        }
    }

}
