using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;
using System;


public class ControllerSelf: MonoBehaviour
{
    //string[] seq = new string[4] {"default", "wood", "plastic","glass"};
    private List<Material> myMaterials = new List<Material>();
    private UnityEngine.Object[] ImpactSoundObjects;
    private List<String> myOutputs = new List<string>();

    private List<Material> myTrialTargetMaterials = new List<Material>();
    private List<Material> myTrialSecondaryMaterials = new List<Material>();
    private List<AudioClip> myTrialImpactSounds = new List<AudioClip>();

    //public Animator anim;

    public GameObject target;
    public GameObject SecObj;
    public GameObject CanvasObject;

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
    public string Soundset;

    private AudioSource aSource;

    private float startTime;
    private float stimulusTime;
    private float reactTime;
    private List<float> Reactions = new List<float>();

    /*private Rigidbody rb;
    private Vector3 old_rbvel;
    private Vector3 new_rbvel;
    private Vector3 a;
    private Vector3 old_a;
    private float r_impulse;
    private float r_force_impulse;
    private float r_velocity;
    private float r_force;
    private List<float> Impulses = new List<float>();
    private List<float> Forces_from_Impulse = new List<float>();
    private List<float> Velocities = new List<float>();
    private List<float> Forces = new List<float>();*/

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


    //public GameObject Agent;
    public GameObject Table;

    
    private Boolean checkImpact;

    void Start()
    {
        aSource = GetComponent<AudioSource>();
        if(Soundset =="Exp1")
        {
            ImpactSoundObjects = Resources.LoadAll("Sounds", typeof(AudioClip)); //loding all sounds instead of ImpactSounds.Add lines
        }
        else if (Soundset == "Metal")
        {
            ImpactSoundObjects = Resources.LoadAll("new-metal", typeof(AudioClip));
        }
        else if (Soundset == "Glass")
        {
            ImpactSoundObjects = Resources.LoadAll("glass-rod", typeof(AudioClip));
        }
        else if (Soundset == "Plastic")
        {
            ImpactSoundObjects = Resources.LoadAll("plastic-rod", typeof(AudioClip));
        }
        else
        {
            Debug.Log("Error in selecting sound folder");
        }


        TrialIndex = 0;
        
        myMaterials.Add(metal);
        myMaterials.Add(wood);
        myMaterials.Add(plastic);
        myMaterials.Add(glass);
        myMaterials.Add(uninformative);
        
        CanvasObject.SetActive(false);

        ReadTrials();

        target.GetComponent<MeshRenderer>().material = myTrialTargetMaterials[TrialIndex];
        SecObj.GetComponent<MeshRenderer>().material = myTrialSecondaryMaterials[TrialIndex];

        startTime = Time.time;
        Debug.Log(startTime);
        checkImpact = false;

        /*rb = SecObj.GetComponent<Rigidbody>();
        old_rbvel.Set(0, 0, 0);
        a.Set(0, 0, 0);*/
        old_vel = 0;

    }



    void FixedUpdate()
    {
        /*new_rbvel = rb.velocity;
        old_a = a;
        a = new_rbvel - old_rbvel / Time.fixedDeltaTime;
        old_rbvel = new_rbvel;*/


        new_pos = SecObj.transform.position;
        new_vel = (new_pos - last_pos) / Time.fixedDeltaTime;

        Q_vel.Enqueue(new_vel.magnitude);

        if (Q_vel.Count > 6)
        {
            Q_vel.Dequeue();
        }

        foreach (float v in Q_vel)
        {
            //Debug.Log("rbvel: "+v);
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
    }


    void OnCollisionEnter(Collision collision)
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
            /*r_impulse = collision.impulse.magnitude;
            r_force_impulse = collision.impulse.magnitude / Time.fixedDeltaTime;
            r_velocity = new_rbvel.magnitude;
            r_force = rb.mass * a.magnitude;
            //r_force_1frame = rb.mass * old_a;
            
            Impulses.Add(r_impulse);
            Forces_from_Impulse.Add(r_force_impulse);
            Forces.Add(r_force);
            Velocities.Add(r_velocity);*/
            
            aSource.Play();
            
            Velocities.Add(vel_avg);
            Forces.Add(a);
            Invoke("DisplayOptions", 1.0f);
            //Debug.Log("Material index" + TrialIndex);
            //Debug.Log("Material count" + myTrialTargetMaterials.Count);
            checkImpact = true;
        }

    }

    public void DisplayOptions()
    {
        //anim.Play("Breathing Idle");
        //Debug.Log("Enable canvas");
        CanvasObject.SetActive(true);
        //Agent.SetActive(false);
        Table.SetActive(false);
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

        //Agent.SetActive(true);
        Table.SetActive(true);
        if (TrialIndex < myTrialTargetMaterials.Count)
        {
            CanvasObject.SetActive(false);
            target.GetComponent<MeshRenderer>().material = myTrialTargetMaterials[TrialIndex];
            SecObj.GetComponent<MeshRenderer>().material = myTrialSecondaryMaterials[TrialIndex];
            //anim.Play("Impact");
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
            writer.WriteLine(myTrialTargetMaterials[i].name + ", " + myTrialSecondaryMaterials[i].name + ", " + myTrialImpactSounds[i].name + ", " + myOutputs[i] + ", " + Reactions[i] + ", "  + Velocities[i] + ", " + Forces[i]);
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
        else if (ExperimentalCondition == "DC1")
            return Application.dataPath + "/Data/" + "DC1.csv";
        else if (ExperimentalCondition == "DC3")
            return Application.dataPath + "/Data/" + "DC3.csv";

        else if (ExperimentalCondition == "DC5")
            return Application.dataPath + "/Data/" + "DC5.csv";
        else if (ExperimentalCondition == "DC6")
            return Application.dataPath + "/Data/" + "DC6.csv";
        else if (ExperimentalCondition == "DC7")
            return Application.dataPath + "/Data/" + "DC7.csv";

        else if (ExperimentalCondition == "5")
            return Application.dataPath + "/Data/" + "condition5.csv";
        else if (ExperimentalCondition == "6")
            return Application.dataPath + "/Data/" + "condition6.csv";
        else if (ExperimentalCondition == "7")
            return Application.dataPath + "/Data/" + "condition7.csv";
        
        else if (ExperimentalCondition == "51")
            return Application.dataPath + "/Data/" + "condition51.csv";
        else if (ExperimentalCondition == "61")
            return Application.dataPath + "/Data/" + "condition61.csv";
        else if (ExperimentalCondition == "71")
            return Application.dataPath + "/Data/" + "condition71.csv";
        
        else if (ExperimentalCondition == "52")
            return Application.dataPath + "/Data/" + "condition52.csv";
        else if (ExperimentalCondition == "62")
            return Application.dataPath + "/Data/" + "condition62.csv";
        else if (ExperimentalCondition == "72")
            return Application.dataPath + "/Data/" + "condition72.csv";
        
        else if (ExperimentalCondition == "53")
            return Application.dataPath + "/Data/" + "condition53.csv";
        else if (ExperimentalCondition == "63")
            return Application.dataPath + "/Data/" + "condition63.csv";
        else if (ExperimentalCondition == "73")
            return Application.dataPath + "/Data/" + "condition73.csv";
        else
        {
            Debug.Log("wrong condition entered for self interaction");
            return Application.dataPath + "/Data/" + "Condition2.csv";
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

        for (var i = 0; i < lines.Length-1; i++)
        {
            
            string[] parts = lines[i].Split(',');
            targetMaterialname = parts[0];
            secondaryMaterialname = parts[1];
            soundClipname = parts[2];

            for(int j=0; j< myMaterials.Count; j++)
            {
                if(myMaterials[j].name == secondaryMaterialname.Trim())
                {
                    myTrialSecondaryMaterials.Add(myMaterials[j]);
                }
                if (myMaterials[j].name == targetMaterialname.Trim())
                {
                    myTrialTargetMaterials.Add(myMaterials[j]);
                }
            }

            //Debug.Log("Almost there 2");
            for (int k=0; k< ImpactSoundObjects.Length; k++)
            {
                //Debug.Log("Inside the loop 2");
                if (ImpactSoundObjects[k].name == soundClipname.Trim())
                {
                    //Debug.Log("Found" + ImpactSoundObjects[k].name);
                    myTrialImpactSounds.Add(ImpactSoundObjects[k] as AudioClip);
                }
            }
        }
        Debug.Log("trial impact sounds count"+ myTrialImpactSounds.Count);
        Debug.Log("trial materials count"+ myMaterials.Count);
        Debug.Log("lines" +lines.Length);
        
    }


}
//    public void RestartAnim()
//    {
//        anim.Play("Impact");
//    }

//   public void ChangeScene()
//    {
//        SceneManager.LoadScene("Options");
//    }

//   public void SwitchAnimAndNextTrial()
//   {
//       anim.Play("Breathing Idle");
//       StartCoroutine(NextTrial(MaterialIndex, 1f));
//   }

//IEnumerator NextTrial(int i, float t)
//{
//    yield return new WaitForSeconds(t);
//    target.GetComponent<MeshRenderer>().material = myMaterials[i];
//    MaterialIndex++;
//    anim.Play("Impact");
//}

//public string getPath()
//{
//    return Application.dataPath + "/Data/" + "Saved_Inventory.csv";

//if UNITY_EDITOR
//"Participant " + "   " + DateTime.Now.ToString("dd-MM-yy   hh-mm-ss") + ".csv";
//elif UNITY_ANDROID
//      return Application.persistentDataPath+"Saved_Inventory.csv";
//elif UNITY_IPHONE
//        return Application.persistentDataPath+"/"+"Saved_Inventory.csv";
//else
//        return Application.dataPath + "/" + "Saved_Inventory.csv";
//endif
//}


//public string getPath()
//{
//    return Application.dataPath + "/Data/" + "Saved_Inventory.csv";
//}
