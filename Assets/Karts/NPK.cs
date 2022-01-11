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
    public GameObject caster;

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
    public float max;
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
        pos = caster.transform.position;
        difff = transform.InverseTransformPoint(wabbit.transform.position);
        if (wait > 0)
        {
            //update_directions();
            //debug_rays(); 
            vomit_rays();
            path();
            find_direction();
            wait = 0;
        }
        
        //turn = difff.normalized;
        acceleration = Mathf.Abs(difff.normalized.z) * max_accel;
        //if (!clearDirs.Contains(F)) {
        //    acceleration = 0;
        //}
        move_kart();

    }

    void move_kart()
    {
        
        Vector3 rot = transform.rotation.eulerAngles;
        Quaternion but = new Quaternion();
        float amt = turn.x * (min_turn + (turn_speed * (max_accel - acceleration) / max_accel));
        rot.y += amt;
        amt = turn.y * (min_turn + (turn_speed * (max_accel - acceleration) / max_accel));
        rot.x += amt;
        but.eulerAngles = rot;
        transform.rotation = but;
        move.z = acceleration;
        bod.AddRelativeForce(move, ForceMode.Acceleration);
    }

    void find_direction()
    {
        //directions = new Vector3[] { F, N, E, S, W };
        int index = distances.IndexOf(max);
        switch(index)
        {
            case 4:
                //F
                turn.x = 0;
                turn.y = 0;
                break;
            case 0:
                //N
                turn.x = 0;
                turn.y = 1;
                break;
            case 2:
                //E
                turn.x = 1;
                turn.y = 0;
                break;
            case 1:
                //S
                turn.x = 0;
                turn.y = -1;
                break;
            case 3:
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
        F = caster.transform.TransformDirection(Vector3.forward * rayLength);
        N = caster.transform.TransformDirection(new Vector3(0, 0.5f, 1) * rayLength);
        S = caster.transform.TransformDirection(new Vector3(0, -0.5f, 1) * rayLength);
        E = caster.transform.TransformDirection(new Vector3(0.5f, 0, 1) * rayLength);
        W = caster.transform.TransformDirection(new Vector3(-0.5f, 0, 1) * rayLength);
        //NE = caster.transform.TransformDirection((Vector3.forward + Vector3.up + Vector3.right) * rayLength);
        //NW = caster.transform.TransformDirection((Vector3.forward + Vector3.up + Vector3.left) * rayLength);
        //SE = caster.transform.TransformDirection((Vector3.forward + Vector3.down + Vector3.right) * rayLength);
        //SW = caster.transform.TransformDirection((Vector3.forward + Vector3.down + Vector3.left) * rayLength);
        directions =  new Vector3[] { F, N, E, S, W };
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
        //compare dist to available directions
        try
        {
            max = 0;
            //distances.Clear();
            foreach (float dist in distances)
            {
                if (dist > max)
                {
                    max = dist;
                }
                //distances.Add(Vector3.Distance(difff, dir));
                //if (Vector3.Distance(difff, min) > Vector3.Distance(difff, dir))
                //{
                //    min = dir;
                //}
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
        //Debug.DrawRay(pos, NW, Color.red, wait);
        //Debug.DrawRay(pos, NE, Color.red, wait);
        //Debug.DrawRay(pos, SE, Color.red, wait);
        //Debug.DrawRay(pos, SW, Color.red, wait);
        Debug.DrawRay(pos, W, Color.red, wait);
        Debug.DrawRay(pos, E, Color.red, wait);
        Debug.DrawRay(pos, N, Color.red, wait);
        Debug.DrawRay(pos, S, Color.red, wait);
        Debug.DrawRay(pos, F, Color.red, wait);
    }

    void vomit_rays()
    {
        int index = 0;
        var veck = new Vector3(0, 0, rayLength);
        var veck2 = new Vector3(0, 0, 0);
        float avgDistance = 0f;
        RaycastHit info;
        distances.Clear();
        while (index < 4)
        {
            switch(index)
            {
                case 0:
                    //up
                    veck2.y = 1.5f;
                    break;
                case 1:
                    //down
                    veck2.y = -1.5f;
                    break;
                case 2:
                    //right
                    veck2.x = 1.5f;
                    break;
                case 3:
                    //left
                    veck2.x = -1.5f;
                    break;
                default:
                    break;
            }
            for(int i = 0; i < 19; i++)
            {
                veck += veck2;
                Physics.Raycast(pos, transform.TransformDirection(veck), out info);
                avgDistance += info.distance;
            }
            distances.Add(avgDistance / 20);
            avgDistance = 0;
            veck = new Vector3(0, 0, rayLength);
            veck2 = new Vector3(0, 0, 0);
            index++;
        }
        Physics.Raycast(pos, transform.TransformDirection(veck), out info);
        distances.Add(info.distance);

    }

}
