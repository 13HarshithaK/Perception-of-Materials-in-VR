using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;
using System;


public class BoundingSelf : MonoBehaviour
{
    private List<Material> myMaterials = new List<Material>();
    private UnityEngine.Object[] ImpactSoundObjects;
    private List<String> myOutputs = new List<string>();

    private List<Material> myTrialTargetMaterials = new List<Material>();
    private List<Material> myTrialSecondaryMaterials = new List<Material>();
    private List<AudioClip> myTrialImpactSounds = new List<AudioClip>();

    public GameObject target;
    public GameObject SecObj;
    public GameObject CanvasObject;
    public GameObject WarningCanvas;

    public Material metal;
    public Material wood;
    public Material plastic;
    public Material glass;
    public Material uninformative;

    public int MaterialIndex;
    public int TrialIndex;

    public string User;
    public string Age;
    public string ExperimentalCondition;

    private AudioSource aSource;

    private float startTime;
    private float stimulusTime;
    private float reactTime;
    private List<float> Reactions = new List<float>();


    private List<float> Velocities = new List<float>();
    private List<float> Forces = new List<float>();

    private Vector3 new_pos;
    private Vector3 last_pos;
    private Vector3 new_vel;
    private Vector3 lastlast_pos;
    private float old_vel;
    private float vel_avg;
    private float a;
    private Queue Q_vel = new Queue();

    public GameObject Bounding_Box;
    public GameObject Table;

    private Boolean checkImpact;
    // private Boolean checkBounding;

    bool m_Started;
    public LayerMask m_LayerMask;


    // Start is called before the first frame update
    void Start()
    {
        m_Started = true;

        aSource = GetComponent<AudioSource>();
        ImpactSoundObjects = Resources.LoadAll("Sounds", typeof(AudioClip)); //loding all sounds instead of ImpactSounds.Add lines

        TrialIndex = 0;

        myMaterials.Add(metal);
        myMaterials.Add(wood);
        myMaterials.Add(plastic);
        myMaterials.Add(glass);
        myMaterials.Add(uninformative);

        CanvasObject.SetActive(false);
        WarningCanvas.SetActive(false);


        ReadTrials();

        target.GetComponent<MeshRenderer>().material = myTrialTargetMaterials[TrialIndex];
        //SecObj.GetComponent<MeshRenderer>().material = myTrialSecondaryMaterials[TrialIndex];

        startTime = Time.time;
        Debug.Log(startTime);
        checkImpact = false;

        old_vel = 0;

        Bounding_Box.SetActive(false);
        //checkBounding = false;

    }

    void FixedUpdate()
    {
        new_pos = SecObj.transform.position;
        new_vel = (new_pos - last_pos) / Time.fixedDeltaTime;

        Q_vel.Enqueue(new_vel.magnitude);

        if (Q_vel.Count > 6)
        {
            Q_vel.Dequeue();
        }

        foreach (float v in Q_vel)
        {
            vel_avg += v;
        }
        vel_avg -= new_vel.magnitude;
        vel_avg /= Q_vel.Count;
        a = (vel_avg - old_vel) / Time.fixedDeltaTime;
        //othera = (old_vel - vel_avg) / Time.fixedDeltaTime;
        //Debug.Log("last pos = " + last_pos + "  new pos = " + new_pos + "  vel = " + new_vel + "  vel avg = " + vel_avg);
        lastlast_pos = last_pos;
        last_pos = new_pos;
        old_vel = vel_avg;

        MyCollisions();

    }



    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == Bounding_Box)
        {
            //checkBounding = true;
            WarningCanvas.SetActive(true);
            Table.SetActive(false);
            Debug.Log("Box");
        }

        else
        {
            Debug.Log("Trial no. " + TrialIndex);
            stimulusTime = Time.time;
            Debug.Log("Time at impact: " + stimulusTime);
            aSource.clip = myTrialImpactSounds[TrialIndex];

            if (vel_avg > 0 && vel_avg <= 1)
            {
                aSource.volume = 0.3f;
                //Debug.Log("Volume: " + 0.3);

            }
            else if (1 < vel_avg && vel_avg < 2)
            {
                aSource.volume = 0.8f;
                //Debug.Log("Volume: " + 0.8);
            }
            else
            {
                aSource.volume = 1f;
                //Debug.Log("Volume: " + 1);
            }

            //aSource.Play();

            if (checkImpact == false)
            {
                aSource.Play();

                Velocities.Add(vel_avg);
                Forces.Add(a);
                Invoke("DisplayOptions", 1.0f);
                checkImpact = true;
            }
        }        

    }

    void OnCollisionExit(Collision collision)
    {
        print("No longer in contact with " + collision.transform.name);
        if (collision.gameObject == Bounding_Box)
        {
            WarningCanvas.SetActive(false);

            //enable table
            //disable box
        }
    }

    void MyCollisions()
    {
        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        Collider[] hitColliders = Physics.OverlapBox(Bounding_Box.transform.position, Bounding_Box.transform.localScale / 2, Quaternion.identity, m_LayerMask);
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            //Output all of the collider names
            Debug.Log("Hit : " + hitColliders[i].name + i);
            //Increase the number of Colliders in the array
            i++;
            if (hitColliders[i].gameObject == Bounding_Box)
            {
                WarningCanvas.SetActive(true);

            }
        }
    }

    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (m_Started)
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(Bounding_Box.transform.position, Bounding_Box.transform.localScale);
    }

    public void DisplayOptions()
    {
        //Debug.Log("Enable canvas");
        CanvasObject.SetActive(true);
        Table.SetActive(false);
        Bounding_Box.SetActive(true);
        //SecObj.GetComponent<MeshRenderer>().material = myMaterials[4];
        TrialIndex++;
        checkImpact = false;

    }

    public void SaveOutputWood()
    {
        myOutputs.Add("Wood");
        reactTime = Time.time - stimulusTime;
        //Debug.Log("it took this much time: " + reactTime);
        Reactions.Add(reactTime);
        //next trial
        NextTrialtest();
    }

    public void SaveOutputPlastic()
    {
        myOutputs.Add("Plastic");
        reactTime = Time.time - stimulusTime;
        //Debug.Log("it took this much time: " + reactTime);
        Reactions.Add(reactTime);
        //next trial
        NextTrialtest();
    }

    public void SaveOutputGlass()
    {
        myOutputs.Add("Glass");
        reactTime = Time.time - stimulusTime;
        //Debug.Log("it took this much time: " + reactTime);
        Reactions.Add(reactTime);
        //next trial
        NextTrialtest();
    }

    public void SaveOutputMetal()
    {
        myOutputs.Add("Metal");
        reactTime = Time.time - stimulusTime;
        //Debug.Log("it took this much time: " + reactTime);
        Reactions.Add(reactTime);
        //next trial
        NextTrialtest();
    }

    public void NextTrialtest()
    {
        Table.SetActive(true);
        Bounding_Box.SetActive(false);
        if (TrialIndex < myTrialTargetMaterials.Count)
        {
            CanvasObject.SetActive(false);
            target.GetComponent<MeshRenderer>().material = myTrialTargetMaterials[TrialIndex];
            //SecObj.GetComponent<MeshRenderer>().material = myTrialSecondaryMaterials[TrialIndex];
        }
        else
        {
            makeOutput();
            SceneManager.LoadScene("Options");
        }

    }

    public void makeOutput()
    {
        StreamWriter writer = new StreamWriter(Application.dataPath + "/Data/" + User + ExperimentalCondition + "table.csv");
        writer.WriteLine("Name/ID: " + User);
        writer.WriteLine("Age: " + Age);
        writer.WriteLine("Condition being tested: " + ExperimentalCondition);
        writer.WriteLine("Target, Secondary Object, Audio, Option, Reaction Time, Velocity, Force");
        for (int i = 0; i < myTrialTargetMaterials.Count; i++)
        {
            writer.WriteLine(myTrialTargetMaterials[i].name + ", " + myTrialSecondaryMaterials[i].name + ", " + myTrialImpactSounds[i].name + ", " + myOutputs[i] + ", " + Reactions[i] + ", " + Velocities[i] + ", " + Forces[i]);
        }
        writer.Flush();
        writer.Close();
    }


    public string getPath()
    {
        if (ExperimentalCondition == "2")
            return Application.dataPath + "/Data/" + "Condition2.csv";
        else if (ExperimentalCondition == "41")
            return Application.dataPath + "/Data/" + "Condition41.csv";
        else if (ExperimentalCondition == "45")
            return Application.dataPath + "/Data/" + "Condition45.csv";
        else
        {
            Debug.Log("wrong condition entered for self interaction");
            return Application.dataPath + "/Data/" + "Condition4.csv";
        }
    }

    public void ReadTrials()
    {
        string file = "";

        if (File.Exists(getPath()))
        {
            FileStream fileStream = new FileStream(getPath(), FileMode.Open, FileAccess.ReadWrite);
            StreamReader read = new StreamReader(fileStream);
            file = read.ReadToEnd();
        }
        else
        {
            Debug.Log("File at Location does not exist");
        }

        string targetMaterialname = "";
        string secondaryMaterialname = "";
        string soundClipname = "";

        string[] lines = file.Split('\n');

        for (var i = 0; i < lines.Length - 1; i++)
        {

            string[] parts = lines[i].Split(',');
            targetMaterialname = parts[0];
            secondaryMaterialname = parts[1];
            soundClipname = parts[2];

            for (int j = 0; j < myMaterials.Count; j++)
            {
                if (myMaterials[j].name == secondaryMaterialname)
                {
                    myTrialSecondaryMaterials.Add(myMaterials[j]);
                }
                if (myMaterials[j].name == targetMaterialname)
                {
                    myTrialTargetMaterials.Add(myMaterials[j]);
                }
            }

            //Debug.Log("Almost there 2");
            for (int k = 0; k < ImpactSoundObjects.Length; k++)
            {
                //Debug.Log("Inside the loop 2");
                if (ImpactSoundObjects[k].name == soundClipname.Trim())
                {
                    //Debug.Log("Found" + ImpactSoundObjects[k].name);
                    myTrialImpactSounds.Add(ImpactSoundObjects[k] as AudioClip);
                }
            }
        }
        //Debug.Log(myTrialImpactSounds.Count);
    }

}
