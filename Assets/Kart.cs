using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class Kart : MonoBehaviour
{
    public GameObject self;
    public Rigidbody bod;
    public Canvas can;
    public Text tex;

    //acceleration vars
    private bool accelerating = false;
    public float acceleration = 0.0f;
    public float rate_accel = 10f;
    public float max_accel = 500f;

    //drift idk
    private bool drifting = false;
    public float driftAccel = 0f;
    public float driftTurn = 2;
    public float boost = 700f;
    public Vector3 move = new Vector3(0, 0, 0);
    public float driftTime = 0f;

    private Vector2 left_steering;
    private GameObject home;
    public int lap = 1;

    //turning vars
    public float min_turn = 1;
    public float turn_speed = 3;

    //big bool
    public bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        tex = can.GetComponent<Text>();
        home = new GameObject();
        self = this.gameObject;
        home.transform.localPosition = self.transform.localPosition;
        home.transform.eulerAngles = self.transform.eulerAngles;
        self = this.gameObject;
        bod = self.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(active)
        {
            acccelerating();
            move_kart();
        }
        if (drifting)
        {
            drift_kart();
        } else
        {
            acccelerating();
            move_kart();
        }
        
    }

    public void GoHome()
    {
        bod.Sleep();
        self.transform.localPosition = home.transform.localPosition;
        Quaternion ass = new Quaternion();
        ass.eulerAngles = home.transform.localEulerAngles;
        self.transform.localRotation = ass;
        bod.WakeUp();
        tex.text = "1/2";
    }
    void acccelerating()
    {
        if (accelerating)
        {
            if (acceleration < max_accel)
            {
                acceleration += rate_accel;
                if (acceleration > max_accel) { acceleration = max_accel; }
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
    }

    public void Accel(InputAction.CallbackContext context)
    {
        bool btn = context.ReadValueAsButton();
        //Debug.Log(context.ReadValueAsButton());
        if (btn)
        {
            accelerating = true;
        } else {
            accelerating = false;
        }
    }

    public void Steer(InputAction.CallbackContext context)
    {
        left_steering = context.ReadValue<Vector2>();
        print(left_steering);
    }

    public void testinp(InputAction.CallbackContext context)
    {
       // print(context.ReadValue<Vector2>());
    }

    public void set_lap()
    {
        switch (lap)
        {
            case 1:
                tex.text = "2/2";
                lap = 2;
                break;
            case 2:
                tex.text = "Done";
                lap = 3;
                break;
            default: break;
        }

    }

    void drift_kart()
    {
        Vector3 rot = self.transform.rotation.eulerAngles;
        Quaternion but = new Quaternion();
        float amt = left_steering.x * (driftTurn + (turn_speed * (max_accel - acceleration) / max_accel));
        rot.y += amt;
        amt = left_steering.y * (driftTurn + (turn_speed * (max_accel - acceleration) / max_accel));
        rot.x += -amt;
        but.eulerAngles = rot;
        self.transform.rotation = but;
        driftTime += Time.deltaTime;
        bod.AddForce(move, ForceMode.Acceleration);
    }

    void move_kart()
    {
        Vector3 rot = self.transform.rotation.eulerAngles;
        //Vector3 frontpos = front.transform.position;
        Quaternion but = new Quaternion();

        //rot.y = rot.y + ang;
        float amt = left_steering.x * (min_turn + (turn_speed * (max_accel - acceleration) / max_accel));
        //print(amt);
        rot.y += amt;
        amt = left_steering.y * (min_turn + (turn_speed * (max_accel - acceleration) / max_accel));
        //if (acceleration < max_accel) {amt = (left_steering * turn_speed) * ((max_accel - acceleration)/max_accel); }
        //else { amt = left_steering * min_turn;}
        //rot.y += left_steering.x;
        rot.x += -amt;
        but.eulerAngles = rot;
        transform.rotation = but;
        //Vector3 move = Vector3.Lerp(kartpos, frontpos, acceleration);
        move.z = acceleration;
        bod.AddRelativeForce(move, ForceMode.Acceleration);
        //self.transform.position = move;
    }

    public void driift(InputAction.CallbackContext context)
    {
        
        bool btn = context.ReadValueAsButton();
        //Debug.Log(context.ReadValueAsButton());
        if (btn)
        {
            move.z = acceleration;
            print("driftu");
            move = transform.TransformDirection(move);
            drifting = true;
        }
        else
        {
            move.y = 0;
            move.x = 0;
            if (driftTime >= 1.5f)
            {
                move.z = 700f;
                bod.AddRelativeForce(move, ForceMode.Impulse);
            } else if (driftTime >= 1f) {
                move.z = 450;
                bod.AddRelativeForce(move, ForceMode.Impulse);
            } else if (driftTime >= 0.5f) {
                move.z = 200;
                bod.AddRelativeForce(move, ForceMode.Impulse);
            }
            driftTime = 0;
            print("undrift");
            drifting = false;
        }
    }
}
