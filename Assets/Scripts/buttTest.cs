using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class buttTest : MonoBehaviour
{
    // Start is called before the first frame update
    private List<string> a = new List<string>();


    public void testButt()
    {
       a.Add("Button pressed");
    }

    public void testButt2()
    {
        print(a.Count);
    }
}
