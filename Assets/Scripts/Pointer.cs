using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    public float m_defaultLength = 5.0f;
    public GameObject m_Dot;
    public VRInputModule m_Input_Module;
    private LineRenderer m_LineRenderer = null;

    
    private void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        //use default or distance
        PointerEventData data = m_Input_Module.GetData();
        //set the pointer length to default length if it isnt hitting anything else set it to the distance between controler and object
        float targetLength = data.pointerCurrentRaycast.distance == 0 ? m_defaultLength : data.pointerCurrentRaycast.distance;

        //Raycast
        RaycastHit hit = CreateRaycast(targetLength);

        //Default if not hitting
        Vector3 endPosition = transform.position + (transform.forward * targetLength);

        //Update if hitting something
        if(hit.collider !=null)
        {
            endPosition = hit.point;
        }

        //Set position of dot
        m_Dot.transform.position = endPosition;

        //set position of linerenderer
        m_LineRenderer.SetPosition(0, transform.position);
        m_LineRenderer.SetPosition(1, endPosition);

    }

    private RaycastHit CreateRaycast(float Length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, m_defaultLength);
        return hit; 
    }
}
