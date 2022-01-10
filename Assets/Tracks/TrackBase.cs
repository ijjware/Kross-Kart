using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBase : MonoBehaviour
{
    public bool finished = false;
    public int KartCheck = 1;
    public int KartLap = 0;
    public int numChecks;
    public int numLaps = 3;
    public GameObject self;
    public GameObject[] checks;

    // Start is called before the first frame update
    void Start()
    {
        //print("start1");
        self = this.gameObject;
        checks =  GameObject.FindGameObjectsWithTag("Checks");
        clear_checks();
        numChecks = checks.Length - 1;
        //Debug.Log(checks.Length);
    }

    void clear_checks()
    {
        int size = 0;
        foreach(GameObject check in checks)
        {
            print(check.name);
            if (check != null) { size += 1;}
        }
        GameObject[] newchex = new GameObject[size];
        size = 0;
        foreach (GameObject check in checks)
        {
            if (check != null) { 
                newchex[size] = check;
                size += 1;
            }
        }
        checks = newchex;
    }

    public void checkcheck(Collider car, GameObject point)//Checkpoint point)
    {
        int num = 0;
        //print(car.name);
        //print(point.name);
        foreach (GameObject check in checks)
        {
            if (check.Equals(point))
            {
                //print("break");
                num = int.Parse(check.name);
                break;
            }
            //num += 1;
        }
        //print(num);
        switch (car.name) {
            case "Kart":

                if (num == KartCheck)
                {
                    //print("if num ==");
                    if (KartCheck == 0)
                    {
                        if (KartLap != numLaps)
                        {
                            //print("lap");
                            car.SendMessage("set_lap");
                            KartLap += 1;
                        }
                        KartCheck = 1;
                    }

                    //print(point.name);
                    //point.lightBarch();
                    KartCheck += 1;
                    if (KartCheck > numChecks) { KartCheck = 0; }
                }
                else if (num == KartCheck - 1 && num != 0) { } //print("wrong way Kart"); }
                break;
            
            default: break;
        }
        if (KartLap == numLaps)
        {
            finished = true;
            print(" you win gj man proud of u");
        }
        
    }
}
