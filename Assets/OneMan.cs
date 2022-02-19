using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OneMan : MonoBehaviour
{

    public GameObject t1;
    public GameObject t1Point;
    public GameObject t2;
    public GameObject t2Point;
    public GameObject t3;
    public GameObject t3Point;
    public TrackBase curr;
    public Kart kart;

    public GameObject clone1;
    public GameObject clone2;
    public GameObject clone3;

    private int track = 1;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(t1, new Vector3(0, 0, 0), Quaternion.identity);
        //t1.SetActive(true);
        kart = FindObjectOfType<Kart>();
        track_one();
    }

    // Update is called once per frame
    void Update()
    {
    
        if (curr)
        {
            if (curr.finished)
            {
                print("inst");
                curr.finished = false;
                curr = null;
                switch (track)
                {
                    case 1:
                        StartCoroutine(track_two());
                        break;
                    case 2:
                        StartCoroutine(track_three());
                        break;
                    case 3:
                        win_msg();
                        deact();
                        print("go home now");
                        break;
                    default: break;
                }
            }
        }
    }

    void deact()
    {
        kart.active = false;
    }

    void start_race()
    {
        // print("start");
        kart.active = true; 
    }

    void track_one()
    {
        start_race();
        clone1 = Instantiate(t1, t1Point.transform.position, t1Point.transform.rotation);
        curr = FindObjectOfType<TrackBase>();
    }

    IEnumerator track_two()
    {
        //stop karts
        deact();
        yield return new WaitForSeconds(1);
        Destroy(clone1);
        yield return new WaitForSeconds(1);
        //get rid of t1
        clone2 = Instantiate(t2, t2Point.transform.position, t2Point.transform.rotation);
        curr = FindObjectOfType<TrackBase>();
        track = 2;
        yield return new WaitForSeconds(2);
        start_race();
    }

    IEnumerator track_three()
    {
        //stop karts
        deact();
        yield return new WaitForSeconds(1);
        Destroy(clone2);
        yield return new WaitForSeconds(1);
        //get rid of t1
  
        clone3 = Instantiate(t3, t3Point.transform.position, t3Point.transform.rotation);
        curr = FindObjectOfType<TrackBase>();
        track = 3;
        yield return new WaitForSeconds(2);
        start_race();
    }



    void win_msg()
    {
        //kart.tex.text = "Goodbye, My Love!";
    }

}
