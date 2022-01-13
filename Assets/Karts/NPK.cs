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
    public float maxX;
    public float maxY;
    public float maxZ;
    public List<float> distancesX;
    public List<float> distancesY;
     private float wait = 0f;

    private void Start()
    {
        bod = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
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
            sniff();

            wait = 0;
        }

        acccelerating();
            //acceleration = Mathf.Abs(max) * max_accel;

        move_kart();

    }

    void move_kart()
    {
        
        Vector3 rot = transform.rotation.eulerAngles;
        float amt = turn.x * (min_turn + (turn_speed * (max_accel - acceleration) / max_accel));
        rot.y = amt;
        amt = turn.y * (min_turn + (turn_speed * (max_accel - acceleration) / max_accel));
        rot.x = -amt;
        transform.Rotate(rot.x, rot.y, 0, Space.Self);
        move.z = acceleration;
        bod.AddRelativeForce(move, ForceMode.Acceleration);
    }

    void find_direction()
    {
        int indexX = distancesX.IndexOf(maxX);
        int indexY = distancesY.IndexOf(maxY);
        switch(indexX)
        {
            case 0:
                //E
                turn.x = 1;
                break;
            case 1:
                //W
                turn.x = -1;
                break;
            case 2:
                //F
                turn.x = 0;
                break;
            default:
                //print("ope" + indexX);
                break;
        }
        switch (indexY)
        {
            case 0:
                //N
                turn.y = 1;
                break;
            case 1:
                //S
                turn.y = -1;
                break;
            case 2:
                //F
                //print("FY" + maxY);
                turn.y = 0;
                break;
            default:
                //print("ope" + indexY);
                break;
        }
    }

    void path()
    {
    maxY = 0;
    maxX = 0;
    
    foreach (float dist in distancesX) {if (dist > maxX) { maxX = dist; } }
    //if (maxX < maxZ) { maxX = maxZ; }
    foreach (float dist in distancesY) {if (dist > maxY) { maxY = dist; } }
    //if (maxY < maxZ) { maxY = maxZ; }

    }

    void vomit_rays()
    {
        int index = 0;
        var veck = new Vector3(0, 0, rayLength);
        var veck2 = new Vector3(0, 0, 0);
        float maxDistance = 0f;
        RaycastHit info;
        distancesX.Clear();
        distancesY.Clear();
        while (index < 4)
        {
            switch (index)
            {
                case 0:
                    //up
                    veck2.y = 1;
                    break;
                case 1:
                    //down
                    veck2.y = -1;
                    break;
                case 2:
                    //right
                    veck2.x = 1f;
                    break;
                case 3:
                    //left
                    veck2.x = -1f;
                    break;
                default:

                    break;
            }
            for (int i = 0; i < 19; i++)
            {
                veck += veck2;
                if (Physics.Raycast(pos, transform.TransformDirection(veck * i), out info))
                { //Debug.DrawLine(pos, info.point, Color.yellow, 1f); 
                }
                if (maxDistance < info.distance)
                {
                    maxDistance = info.distance;
                }
            }
            if (index < 2) { distancesY.Add(maxDistance); }
            else{ distancesX.Add(maxDistance); }
            maxDistance = 0;
            veck = new Vector3(0, 0, rayLength);
            veck2 = new Vector3(0, 0, 0);
            index++;
        }
        if (Physics.Raycast(pos, transform.TransformDirection(veck), out info))
        { 
            //Debug.DrawLine(pos, info.point, Color.yellow, 1f);
            maxZ = info.distance;
            //distancesX.Add(maxZ);
            //distancesY.Add(maxZ);
        }

    }

    void acccelerating()
    {
        RaycastHit info;
        float buffer = 0;
        if (Physics.Raycast(pos, transform.TransformDirection(Vector3.forward), out info))
        {
            buffer = info.distance;
            //acceleration = info.distance * ((max_accel - acceleration) / max_accel);
            
            //if (acceleration > max_accel) { acceleration = max_accel; }
            //print(info.distance);
        }

        if (buffer > 100)
        {
            if (acceleration < max_accel)
            {
                acceleration += rate_accel;
                if (acceleration > max_accel) { acceleration = max_accel; }
            }

        }else if ( buffer > 10)
        {
            if (acceleration > (max_accel/10))
            {
                acceleration -= rate_accel;
                if (acceleration < (max_accel / 10)) { acceleration = 0; }
            }
        }
        else
        {
            if (acceleration > 0)
            {
                acceleration -= rate_accel;
                if (acceleration < 0) { acceleration = 0; }
            }

        }
        //print(acceleration);

    }

    void sniff()
    {
       if (!Physics.Linecast(transform.position, wabbit.transform.position))
        {
            turn.x = Mathf.Sign(difff.x);
            turn.y = Mathf.Sign(difff.y);
        }
    }

    void update_directions()
    {
        //F = caster.transform.TransformDirection(Vector3.forward * rayLength);
        //N = caster.transform.TransformDirection(new Vector3(0, 0.5f, 1) * rayLength);
        //S = caster.transform.TransformDirection(new Vector3(0, -0.5f, 1) * rayLength);
        //E = caster.transform.TransformDirection(new Vector3(0.5f, 0, 1) * rayLength);
        //W = caster.transform.TransformDirection(new Vector3(-0.5f, 0, 1) * rayLength);
        //NE = caster.transform.TransformDirection((Vector3.forward + Vector3.up + Vector3.right) * rayLength);
        //NW = caster.transform.TransformDirection((Vector3.forward + Vector3.up + Vector3.left) * rayLength);
        //SE = caster.transform.TransformDirection((Vector3.forward + Vector3.down + Vector3.right) * rayLength);
        //SW = caster.transform.TransformDirection((Vector3.forward + Vector3.down + Vector3.left) * rayLength);
        //directions = new Vector3[] { F, N, E, S, W };
        //clearDirs.Clear();
        //foreach (Vector3 direction in directions)
        //{
        //    if (!Physics.Raycast(pos, direction, rayLength))
        //    {
        //        clearDirs.Add(direction);
        //    }

        //}
    }
}
