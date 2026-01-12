using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class StartTrial : MonoBehaviour
{
    public SteamVR_Input_Sources handType; // 1
    public SteamVR_Action_Boolean grabPinchAction; // 3

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (grabPinchAction.GetLastStateDown(handType))
        {
            


        }
    }

    private void GrabPinchObject()
    {
        // 1
        
    }
}
