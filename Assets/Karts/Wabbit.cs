using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wabbit : MonoBehaviour
{
    public float targetDist = 50;
    private float wait = 0f;
    public int tarnum = 1;
    public GameObject target1;
    public GameObject target2;
    public GameObject target3;
    public GameObject target4;
    public GameObject target5;
    public GameObject target6;
    public GameObject target7;
    public GameObject target8;
    public GameObject target9;
    public GameObject target10;
    public GameObject target11;
    public GameObject target12;
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
        wait += Time.deltaTime;
        if (agent.remainingDistance < targetDist && wait > 1)
        {
            switch (tarnum)
            {
                case 1:
                    agent.SetDestination(target2.transform.position);
                    tarnum++;
                    break;
                case 2:
                    agent.SetDestination(target3.transform.position);
                    tarnum++;
                    break;
                case 3:
                    agent.SetDestination(target4.transform.position);
                    tarnum++;
                    break;
                case 4:
                    agent.SetDestination(target5.transform.position);
                    tarnum++;
                    break;
                case 5:
                    agent.SetDestination(target6.transform.position);
                    tarnum++;
                    break;
                case 6:
                    agent.SetDestination(target7.transform.position);
                    tarnum++;
                    break;
                case 7:
                    agent.SetDestination(target8.transform.position);
                    tarnum++;
                    break;
                case 8:
                    agent.SetDestination(target9.transform.position);
                    tarnum++;
                    break;
                case 9:
                    agent.SetDestination(target10.transform.position);
                    tarnum++;
                    break;
                case 10:
                    agent.SetDestination(target11.transform.position);
                    tarnum++;
                    break;
                case 11:
                    agent.SetDestination(target12.transform.position);
                    tarnum++;
                    break;
                case 12:
                    agent.SetDestination(target1.transform.position);
                    tarnum = 1;
                    break;
                default: break;
            }
            print(tarnum);
            wait = 0;
        }
    }

}
