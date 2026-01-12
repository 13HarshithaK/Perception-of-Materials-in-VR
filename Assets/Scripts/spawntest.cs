using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawntest : MonoBehaviour
{
    public GameObject controller;
    public GameObject SecObj;
    private GameObject R_SecObj;
    // Start is called before the first frame update
    void Start()
    {
        spawnobj();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnobj()
    {
        Vector3 pos = new Vector3(controller.transform.position.x, controller.transform.position.y, controller.transform.position.z + 0.15f);
        R_SecObj = Instantiate(SecObj, pos, controller.transform.rotation);
        Vector3 rotationToAdd = new Vector3(90, 0, 0);
        R_SecObj.transform.Rotate(rotationToAdd);
    }
}
