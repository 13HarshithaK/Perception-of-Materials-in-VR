using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Linq;

public class oncolideRec : MonoBehaviour
{
    private AudioSource metal;
    public AudioClip sound;
    //private UnityEngine.Object[] ImpactSoundObjects;
    //private List<AudioClip> myTrialImpactSounds = new List<AudioClip>();
    private float startTime;
    private float impactTime;

    public Rigidbody rb;
    public GameObject go;
    
    private Queue Q_rbvel = new Queue();
    private Vector3 new_pos;
    private Vector3 last_pos;
    private Vector3 new_vel;
    private Vector3 lastlast_pos;
    private float old_vel;
    private float vel_avg;
    private float a;
    private float othera;

    //private Boolean checkImpact;


    void Start()
    {
        //ImpactSoundObjects = Resources.LoadAll("Sounds", typeof(AudioClip));
        metal = GetComponent<AudioSource>();
        metal.GetComponent<AudioSource>().clip = sound;
        startTime = Time.time;
        //Debug.Log("Start: "+ startTime);
        //checkImpact = false;

        //rb = GetComponent<Rigidbody>();

        old_vel = 0;
        new_pos.Set(0, 0, 0);
        last_pos.Set(0, 0, 0);
    }

    void FixedUpdate()
    {
        /*new_rbvel = rb.velocity;
        old_a = a;
        a = new_rbvel - old_rbvel / Time.fixedDeltaTime;
        //Debug.Log("acceleration = " + a.magnitude);
        //g_force = (((velocity_x - last_velocity_x) / delta_time) + ((velocity_y - last_velocity_y) / delta_time) + ((velocity_z - last_velocity_z) / delta_time)) / 9.81f;
        //Vector3 a2 = new Vector3(new_rbvel.x - old_rbvel.x / Time.fixedDeltaTime, new_rbvel.y - old_rbvel.y / Time.fixedDeltaTime, new_rbvel.z - old_rbvel.z / Time.fixedDeltaTime);
        //Debug.Log("acceleration 2 = " + a2.magnitude);
        old_rbvel = new_rbvel;*/

        /*rbvel = rb.velocity.magnitude;
        Q_rbvel.Enqueue(rbvel);

        if (Q_rbvel.Count > 6)
        {
            Q_rbvel.Dequeue();
        }

        foreach (float v in Q_rbvel)
        {
            //Debug.Log("rbvel: "+v);
            rbvel_avg += v;

        }
        rbvel_avg -= rbvel;
        rbvel_avg /= Q_rbvel.Count;*/

        //Debug.Log("Average: " +rbvel_avg);
        
        new_pos = go.transform.position;
        new_vel = (new_pos - last_pos)/ Time.fixedDeltaTime;
               
        Q_rbvel.Enqueue(new_vel.magnitude);

        if (Q_rbvel.Count > 6)
        {
            Q_rbvel.Dequeue();
        }

        foreach (float v in Q_rbvel)
        {
            //Debug.Log("rbvel: "+v);
            vel_avg += v;
        }
        vel_avg -= new_vel.magnitude;
        vel_avg /= Q_rbvel.Count;
        a = (vel_avg - old_vel) / Time.fixedDeltaTime;
        othera = (old_vel - vel_avg) / Time.fixedDeltaTime;
        //Debug.Log("last pos = " + last_pos + "  new pos = " + new_pos + "  vel = " + vel);
        lastlast_pos = last_pos;
        last_pos = new_pos;
        old_vel = vel_avg;

    }


    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("We hit the table with glass");
        // metal.clip = ImpactSoundObjects[0] as AudioClip;
        
        if (vel_avg > 0 && vel_avg <= 1)
        {
            metal.volume = 0.2f;
            Debug.Log("Volume: " + 0.2);

        }
        else if( 1 < vel_avg && vel_avg < 2 )
        {
            metal.volume = 0.75f;
            Debug.Log("Volume: " + 0.75);
        }
        else
        {
            metal.volume = 1f;
            Debug.Log("Volume: " + 1);
        }


        metal.Play();
        //impactTime = Time.time - startTime;
        //Debug.Log("F = ma: " + rb.mass * rbvel_avg);
        //Debug.Log("Velocity: " + rbvel_avg);
        Debug.Log("Vel on impact: " + vel_avg);
        //Debug.Log("F = ma 1 frame before: " + rb.mass * old_a.magnitude);
        //Debug.Log("Impulse is: " + collision.impulse);
        Debug.Log("last pos = " + lastlast_pos + "  new pos = " + new_pos + "  vel = " + vel_avg + "  a  = " + Mathf.Abs(a));
        //Debug.Log("Time.fixedDeltaTime: " + Time.fixedDeltaTime);
        
        //Debug.Log("Magnitude of Impulse: " + collision.impulse.magnitude);
        //Debug.Log("Force is : " + collision.impulse / impactTime);
        //Debug.Log("Force calculated through magnitude: " + collision.impulse.magnitude / Time.fixedDeltaTime);
        /*Invoke("ChangeScene", 3.0f);
        impactTime = Time.time - startTime;
        Debug.Log("New time: " + impactTime);
        Debug.Log("Impulse is: " + collision.impulse);
        Debug.Log("Magnitude of Impulse: " + collision.impulse.magnitude);
        Debug.Log("Force is : " + collision.impulse / Time.fixedDeltaTime);
        Debug.Log("Force calculated through magnitude: " + collision.impulse.magnitude / Time.fixedDeltaTime);
        Debug.Log("fixedDeltaTime is: " + Time.fixedDeltaTime);
        Debug.Log("Speed is: " + collision.relativeVelocity.magnitude);
        Debug.Log("Without magnitude speed is: " + collision.relativeVelocity);
        //3.0f is the number of seconds in float*/

    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("Options");
    }



}
