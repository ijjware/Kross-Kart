using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPK : MonoBehaviour
{

    public Vector3 move = new Vector3(0, 0, 0);
    public Vector2 turn = new Vector3(0, 0);

    private Vector3 pos;
    public float rayLength = 1;

    public Vector3 difff;
    public Vector3 target;
    public Rigidbody bod;
    public GameObject wabbit;

    //acceleration vars
    public float acceleration = 0.0f;
    public float rate_accel = 10f;
    public float max_accel = 100f;

    //turning vars
    public float min_turn = 1;
    public float turn_speed = 3;

    //direction vars
    private Vector3 N;
    private Vector3 NE;
    private Vector3 NW;
    private Vector3 E;
    private Vector3 S;
    private Vector3 SE;
    private Vector3 SW;
    private Vector3 W;
    private Vector3 F;
    public Vector3[] directions;
    public List<Vector3> clearDirs;
    public Vector3 min;
    public List<float> distances;
     private float wait = 0f;

    private void Start()
    {
        bod = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //bod.AddForce();
        wait += Time.deltaTime;
        pos = transform.position;
        difff = transform.InverseTransformPoint(wabbit.transform.position);
        if (wait > .3)
        {
            update_directions();
            debug_rays();
            path();
            find_direction();
            wait = 0;
        }
        acceleration = Mathf.Abs(difff.normalized.z) * max_accel;
        if (!clearDirs.Contains(F)) {
            acceleration = 0;
        }
        move_kart();

    }

    void move_kart()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        Quaternion but = new Quaternion();
        float amt = turn.x * (min_turn + (turn_speed * (max_accel - acceleration) / max_accel));
        rot.y += amt;
        amt = turn.y * (min_turn + (turn_speed * (max_accel - acceleration) / max_accel));
        rot.x += -amt;
        but.eulerAngles = rot;
        transform.rotation = but;
        move.z = acceleration;
        bod.AddRelativeForce(move, ForceMode.Acceleration);
    }

    void find_direction()
    {
        //directions = new Vector3[] { F, N, NE, NW, E, SE, S, SW, W };
        int index = Array.IndexOf(directions, min);
        switch(index)
        {
            case 0:
                //F
                turn.x = 0;
                turn.y = 0;
                break;
            case 1:
                //N
                turn.x = 0;
                turn.y = 1;
                break;
            case 2:
                //NE
                turn.x = 1;
                turn.y = 1;
                break;
            case 3:
                //NW
                turn.x = -1;
                turn.y = 1;
                break;
            case 4:
                //E
                turn.x = 1;
                turn.y = 0;
                break;
            case 5:
                //SE
                turn.x = 1;
                turn.y = -1;
                break;
            case 6:
                //S
                turn.x = 0;
                turn.y = -1;
                break;
            case 7:
                //SW
                turn.x = -1;
                turn.y = -1;
                break;
            case 8:
                //W
                turn.x = -1;
                turn.y = 0;
                break;
            default:
                print("ope");
                break;
        }
    }

    void update_directions()
    {
        F = transform.TransformDirection(Vector3.forward * rayLength);
        N = transform.TransformDirection(Vector3.up * rayLength);
        S = transform.TransformDirection(Vector3.down * rayLength);
        E = transform.TransformDirection(Vector3.right * rayLength);
        W = transform.TransformDirection(Vector3.left * rayLength);
        NE = transform.TransformDirection((Vector3.forward + Vector3.up + Vector3.right) * rayLength);
        NW = transform.TransformDirection((Vector3.forward + Vector3.up + Vector3.left) * rayLength);
        SE = transform.TransformDirection((Vector3.forward + Vector3.down + Vector3.right) * rayLength);
        SW = transform.TransformDirection((Vector3.forward + Vector3.down + Vector3.left) * rayLength);
        directions =  new Vector3[] { F, N, NE, NW, E, SE, S, SW, W };
        clearDirs.Clear();
        foreach (Vector3 direction in directions) {
            if (!Physics.Raycast(pos, direction, rayLength))
            {
                clearDirs.Add(direction);
            }

        }
    }

    void path()
    {
        //desired direction difff?
        target = wabbit.transform.position;
        //compare dist to available directions
        try
        {
            min = clearDirs[0];
            distances.Clear();
            foreach (Vector3 dir in clearDirs)
            {
                distances.Add(Vector3.Distance(difff, dir));
                if (Vector3.Distance(difff, min) > Vector3.Distance(difff, dir))
                {
                    min = dir;
                }
                
            }
        } catch (Exception ex)
        {
            min = new Vector3(0, 0, 0);
            print("poop");
        }
        finally {  }
    }

    void debug_rays()
    {
        Debug.DrawRay(pos, NW);
        Debug.DrawRay(pos, NE);
        Debug.DrawRay(pos, SE);
        Debug.DrawRay(pos, SW);
        Debug.DrawRay(pos, W);
        Debug.DrawRay(pos, E);
        Debug.DrawRay(pos, N);
        Debug.DrawRay(pos, S);
        Debug.DrawRay(pos, F);
    }

}
