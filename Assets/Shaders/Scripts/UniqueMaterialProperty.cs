using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class UniqueMaterialProperty : MonoBehaviour
{
    static int uniqueId = 1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        uniqueId = 1;
    }

    private void Start()
    {
        Color c = new Color32((byte)(uniqueId % 256), (byte)(uniqueId / 256), 255, 255);
        GetComponent<Renderer>().material.SetColor("_ID", c);
        uniqueId++;
    }
}
