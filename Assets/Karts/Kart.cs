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
    public float rate_accel = 1f;
    public float max_accel = 25f;

    //boost vars
    public float boostStrength = 50f;
    private float boost = 0f;
    public int boostAmt = 0;
    private bool boosting = false;
    
    //drift idk
    private bool drifting = false;
    public float driftAccel = 0f;
    public float driftTurn = 2;
    public float driftBoost = 3000;
    public Vector3 move = new Vector3(0, 0, 0);
    public Vector3 drift = new Vector3(0, 0, 0);
    public float driftTime = 0f;

    private Vector2 left_steering;
    private float roll;
    private GameObject home;
    public int lap = 1;

    //turning vars
    public float min_turn = 125;
    public float turn_speed = 200;

    //big bool
    public bool active = false;
    private int layermask = 1 << 10;

    // Start is called before the first frame update
    void Start()
    {
        //tex = can.GetComponent<Text>();
        home = new GameObject();
        self = this.gameObject;
        home.transform.localPosition = self.transform.localPosition;
        home.transform.eulerAngles = self.transform.eulerAngles;
        self = this.gameObject;
        //bod = self.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //RaycastHit info;
        //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down * 10), out info, 30, layermask))
        //{
        //    boostAmt += 100;
        //    print("boost get");
        //    info.collider.SendMessage("get");
        //}
        if (drifting)
        {
            drift_kart();
        } else
        {
            acccelerating();
            booosting();
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
        //tex.text = "1/2";
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
        //print(left_steering);
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
        Vector3 rot = self.transform.localRotation.eulerAngles;
        float amt = left_steering.x * (min_turn + (turn_speed * (max_accel - acceleration) / (max_accel * 200)) + driftTurn);
        rot.y = amt;
        amt = left_steering.y * (min_turn + (turn_speed * (max_accel - acceleration) / (max_accel * 200)) + driftTurn);
        rot.x = -amt;
        rot.z = roll * 100;
        Quaternion deltaRotation = Quaternion.Euler(rot * Time.fixedDeltaTime);
        bod.MoveRotation(bod.rotation * deltaRotation);
        driftTime += Time.deltaTime;
        bod.AddForce(drift, ForceMode.VelocityChange);
    }

    void move_kart()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 10), Color.red, 1f);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down * 10), Color.red, 1f);
        Vector3 rot;
        float amt = left_steering.x * (min_turn + (turn_speed * (max_accel - acceleration) / (max_accel * 200)) + boost);
        rot.y = amt;
        amt = left_steering.y * (min_turn + (turn_speed * ( max_accel - acceleration) / (max_accel * 200)) + boost);
        rot.x = -amt;
        rot.z = roll * 100;
        Quaternion deltaRotation = Quaternion.Euler(rot * Time.fixedDeltaTime); 
        bod.MoveRotation(bod.rotation * deltaRotation);
        move.z = acceleration + boost;
        if (move.z > 50) { move.z = 50; }
        bod.AddRelativeForce(move, ForceMode.VelocityChange);
        boost = 0f;
    }

    public void driift(InputAction.CallbackContext context)
    {
        bool btn = context.ReadValueAsButton();
        if (btn)
        {
            move.z = acceleration;
            drift = transform.TransformDirection(move);
            print("driftu");
            drifting = true;
        }
        else
        {
            if (driftTime >= 1.5f)
            {
                move.z = driftBoost;
                bod.AddRelativeForce(move, ForceMode.VelocityChange);
            } else if (driftTime >= 1f) {
                move.z = driftBoost/2;
                bod.AddRelativeForce(move, ForceMode.VelocityChange);
            } else if (driftTime >= 0.5f) {
                move.z = driftBoost/3;
                bod.AddRelativeForce(move, ForceMode.VelocityChange);
            }
            driftTime = 0;
            print("undrift");
            drifting = false;
        }
    }

    public void rollR(InputAction.CallbackContext context)
    {
        roll = -context.ReadValue<float>();
    }

    public void rollL(InputAction.CallbackContext context)
    {
        roll = context.ReadValue<float>();
    }

    public void booosting()
    {
        if (boosting && boostAmt > 0)
        {
            boost = boostStrength;
            boostAmt -= 1;
            //infinite boost line
            //boostAmt += 1;
        } else if (boostAmt == 0)
        {
            print("boost empty");
            boostAmt = -1;
        }
    }

    public void get_boost()
    {
        boostAmt += 100;
        print("boost get");
        //info.collider.SendMessage("get");
    }

    public void booost(InputAction.CallbackContext context)
    {
        bool btn = context.ReadValueAsButton();
        //Debug.Log(context.ReadValueAsButton());
        if (btn)
        {
            boosting = true;
        }
        else
        {
            boosting = false;
        }
    }
}
