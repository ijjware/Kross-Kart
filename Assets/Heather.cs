using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

//(intended to be) bottleneck for HUD element access
public class Heather : MonoBehaviour, IPunInstantiateMagicCallback
{

    public static Heather instance;
    private void Awake()
    {
        instance = this;
    }
    public int UsTeam = 1;
    public Text UsText;
    public Text ThemText;
    public GameObject TimeCanvas;
    private int UsPts = 0;
    private int ThemPts = 0;

    public Slider boost;
    public Text timer;
    public int duration = 10;
    public int secs = 0;
    public int mins = 0;

    float change;
    public GameObject miniPivot;
    public const byte TimeUpEventCode = 8;

    private void Update()
    {
        Vector3 rot = miniPivot.transform.rotation.eulerAngles;
        rot.x += change;
        Quaternion nov = Quaternion.identity;
        nov.eulerAngles = rot;
        miniPivot.transform.Rotate(change, 0, 0, Space.Self);
        if (secs == 0 && mins == 0) { return; }
        TimerUpdate();
    }

    public void PointChange(int team, int change)
    {
        if (UsTeam == team)
        {
            UsPts += change;
        } else { ThemPts += change; }
        score_update();
    }

    private void score_update()
    {
        UsText.text = "" + UsPts;
        ThemText.text = ThemPts + "";
    }

    public void pivot(InputAction.CallbackContext context)
    {
        //change = context.ReadValue<float>();
        //change *= 2;
    }

    public void boost_update(float amt) { boost.value = amt; }

    private void TimerUpdate()
    {
        //increment timer
        secs = duration - (int)Time.fixedTime;
        mins = secs / 60;
        secs -= (mins * 60);
        string secString = "00";
        string minString = "00    ";
        if (secs < 10) { secString = "0" + secs; }
        else if (secs < 60) { secString = secs.ToString(); }
        if (mins > 9) { minString = mins + "    "; }
        else if (mins > 0) { minString = "0" + mins + "    "; }
        timer.text = minString + secString;
        if (secs == 0 && mins == 0) { TimeUp(); }
    }

    private void TimeUp()
    {
        //manage state when time is up
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(TimeUpEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        TimeCanvas.SetActive(true);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
