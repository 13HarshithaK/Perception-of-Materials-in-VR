using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;
using System;
using Valve.VR;


public class ControllerNew : MonoBehaviour
{
    //string[] seq = new string[4] {"default", "wood", "plastic","glass"};
    public SteamVR_Input_Sources handType; 
    public SteamVR_Action_Boolean grabPinchAction; 

    private List<Material> myMaterials = new List<Material>();
    private UnityEngine.Object[] ImpactSoundObjects;
    private List<String> myOutputs = new List<string>();

    private List<Material> myTrialTargetMaterials = new List<Material>();
    private List<Material> myTrialSecondaryMaterials = new List<Material>();
    private List<AudioClip> myTrialImpactSounds = new List<AudioClip>();
    
    public Animator anim;

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

    private AudioSource aSource;

    private float startTime;
    private float stimulusTime;
    private float reactTime;
    private List<float> Reactions = new List<float>();

    public GameObject Agent;
    public GameObject Table;



    void Start()
    {
        aSource = GetComponent<AudioSource>();
        ImpactSoundObjects = Resources.LoadAll("Sounds", typeof(AudioClip)); //loding all sounds instead of ImpactSounds.Add lines
        
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
        anim.Play("Breathing Idle");

    }

    void Update()
    {
        if (grabPinchAction.GetLastStateDown(handType))
        {
            anim.Play("Impact");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Trial no. " + TrialIndex);
        stimulusTime = Time.time;
        Debug.Log("Time at impact: " + stimulusTime);
        aSource.clip = myTrialImpactSounds[TrialIndex];
        aSource.Play();
        TrialIndex++;
        Invoke("DisplayOptions", 1.0f);
        //Debug.Log("Material index" + TrialIndex);
        //Debug.Log("Material count" + myTrialTargetMaterials.Count);
    }

    public void DisplayOptions()
    {
        anim.Play("Breathing Idle");
        //Debug.Log("Enable canvas");
        CanvasObject.SetActive(true);
        Agent.SetActive(false);
        Table.SetActive(false);
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

        Agent.SetActive(true);
        Table.SetActive(true);
        if (TrialIndex < myTrialTargetMaterials.Count)
        {
            CanvasObject.SetActive(false);
            target.GetComponent<MeshRenderer>().material = myTrialTargetMaterials[TrialIndex];
            SecObj.GetComponent<MeshRenderer>().material = myTrialSecondaryMaterials[TrialIndex];
            anim.Play("Impact");
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
        writer.WriteLine("Target, Secondary Object, Audio, Option, Reaction Time");
        for (int i = 0; i < myTrialTargetMaterials.Count; i++)
        {
            writer.WriteLine(myTrialTargetMaterials[i].name + ", " + myTrialSecondaryMaterials[i].name + ", " + myTrialImpactSounds[i].name + ", " + myOutputs[i] + ", " + Reactions[i]);
        }
        writer.Flush();
        writer.Close();
    }


    public string getPath()
    {
        if (ExperimentalCondition == "1")
            return Application.dataPath + "/Data/" + "Condition1.csv";
        else if (ExperimentalCondition == "31")
            return Application.dataPath + "/Data/" + "Condition31.csv";
        else if (ExperimentalCondition == "35")
            return Application.dataPath + "/Data/" + "Condition35.csv";
        else if (ExperimentalCondition == "DC1")
            return Application.dataPath + "/Data/" + "DC1.csv";
        else if (ExperimentalCondition == "DC3")
            return Application.dataPath + "/Data/" + "DC3.csv";
        else if (ExperimentalCondition == "DC2")
            return Application.dataPath + "/Data/" + "DC2.csv";
        else if (ExperimentalCondition == "DC4")
            return Application.dataPath + "/Data/" + "DC4.csv";
        else
        {
            Debug.Log("wrong condition entered for self interaction");
            return Application.dataPath + "/Data/" + "Condition31.csv";
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
                if(myMaterials[j].name == secondaryMaterialname)
                {
                    myTrialSecondaryMaterials.Add(myMaterials[j]);
                }
                if (myMaterials[j].name == targetMaterialname)
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
        //Debug.Log(myTrialImpactSounds.Count);
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
