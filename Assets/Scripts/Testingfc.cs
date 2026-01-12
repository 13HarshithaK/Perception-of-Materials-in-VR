using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testingfc : MonoBehaviour
{
    // Start is called before the first frame update
    private float startTime;
    private float impactTime;
    private Rigidbody rb;
    private float mass;
    private float acc;
    private float force;
    private int count = 0;
    private float rbvel;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mass= rb.mass;
        rbvel = rb.velocity.magnitude;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("RODWe hit the table with the rod");
        impactTime = Time.time - startTime;
       
        Debug.Log("RODNew time: " + impactTime);
        /*Debug.Log("RODImpulse is: " + collision.impulse);
        Debug.Log("RODMagnitude of Impulse: " + collision.impulse.magnitude);
        Debug.Log("RODForce is : " + collision.impulse / Time.fixedDeltaTime);
        Debug.Log("RODForce calculated through magnitude: " + collision.impulse.magnitude / Time.fixedDeltaTime);
        Debug.Log("RODfixedDeltaTime is: " + Time.fixedDeltaTime);
        Debug.Log("RODSpeed is: " + collision.relativeVelocity.magnitude);
        Debug.Log("RODWithout magnitude speed is: " + collision.relativeVelocity);
        //3.0f is the number of seconds in float */
        Debug.Log(".");
        acc = collision.relativeVelocity.magnitude / Time.fixedDeltaTime;
        force = mass * acc;
        Debug.Log(count);
        //Debug.Log("mass: " + mass);
        Debug.Log("col accln: "+ acc);
        Debug.Log("rb velocity: " + rbvel);
        Debug.Log(Time.fixedDeltaTime);
        Debug.Log("RODForce is mass*acceleration = " + mass * collision.relativeVelocity.magnitude / Time.fixedDeltaTime);
        Debug.Log("RODForce = " + force);
        Debug.Log("rb force = " + mass * rbvel / Time.fixedDeltaTime);
        Debug.Log(".");
        count++;
    }
    //do frame by frame force : https://discussions.unity.com/t/getcomponent-velocity-always-returns-0/146898
}
