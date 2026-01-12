using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class NewGrab : MonoBehaviour
{
    public SteamVR_Input_Sources handType; // 1

    public SteamVR_Action_Boolean grabPinchAction; // 3
    public SteamVR_Action_Boolean teleportAction;
    public SteamVR_Behaviour_Pose controllerPose;

    private GameObject collidingObject; // 1
    private GameObject objectInHand; // 2

    public GameObject controller;
    public GameObject SecObj;
    private GameObject R_SecObj;
    public GameObject MoveObj;
    public GameObject TipObj;



    /*   private void SetCollidingObject(Collider col)
       {
           // 1
           if (collidingObject || !col.GetComponent<Rigidbody>())
           {
               return;
           }
           // 2
           collidingObject = col.gameObject;
       }

       // 1
       public void OnTriggerEnter(Collider other)
       {
           SetCollidingObject(other);
       }

       // 2
       public void OnTriggerStay(Collider other)
       {
           SetCollidingObject(other);
       }

       // 3
       public void OnTriggerExit(Collider other)
       {
           if (!collidingObject)
           {
               return;
           }

           collidingObject = null;
       }*/

    private void GrabPinchObject()
    {
        // 1
        objectInHand = collidingObject;
        collidingObject = null;
        // 2
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    public void spawnobj()
    {
        Vector3 pos = new Vector3(controller.transform.position.x, controller.transform.position.y, controller.transform.position.z);
        Vector3 offset = new Vector3(0, 0, 0);
        R_SecObj = Instantiate(SecObj, pos+offset, controller.transform.rotation);
        R_SecObj.transform.Translate(0, 0, 0.1f);
        Vector3 rotationToAdd = new Vector3(90, 0, 0);
        R_SecObj.transform.Rotate(rotationToAdd);

        objectInHand = R_SecObj;
        //collidingObject = null;
        // 2
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
        //will need to destroy the object when resetting 
    }

    public void setobj()
    {
        MoveObj.transform.position = controller.transform.position;
        MoveObj.transform.rotation = controller.transform.rotation;
        Vector3 rotationToAdd = new Vector3(90, 0, 0);
        MoveObj.transform.Rotate(rotationToAdd);
        MoveObj.transform.Translate(0, 0.15f, 0);
        objectInHand = MoveObj;
        //collidingObject = null;
        // 2
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
        //var joint2 = AddFixedJoint();
        //joint2.connectedBody = TipObj.GetComponent<Rigidbody>();
        //will need to destroy the object when resetting 
    }

    // 3
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 200000;
        fx.breakTorque = 200000;
        return fx;
    }

    private void ReleaseObject()
    {
        // 1
        if (GetComponent<FixedJoint>())
        {
            // 2
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            // 3
            objectInHand.GetComponent<Rigidbody>().velocity = controllerPose.GetVelocity();
            objectInHand.GetComponent<Rigidbody>().angularVelocity = controllerPose.GetAngularVelocity();

        }
        // 4
        objectInHand = null;
        
        /*use if spawnobj instead of setobj
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("Clone");
        foreach (GameObject go in objectsToDestroy)
        {
            Destroy(go);
        }*/

    }

   


    void Update()
    {
        // 1
        if (grabPinchAction.GetLastStateDown(handType))
        {
            /*if (collidingObject)
            {
                spawnobj();
            }

            spawnobj();
            */
            setobj();

        }

        if (teleportAction.GetLastStateDown(handType))
        {
            if (objectInHand)
            {
                ReleaseObject();
            }


        }

    }
}
