using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGateData : MonoBehaviour
{
    public GameObject StageGateDataRef;
    public bool HideStageGate;
    private int Value;
    private void OnValidate()
    {
        if (HideStageGate)
        {
            Value = 0;
        }
        if (!HideStageGate)
        {
            Value = 1;
        }
        StageGateDataRef.name = "StartPlayerGate/" + Value;
     //   StageGateDataRef.name = StageGateDataRef.name.Replace(",", ".");
    }
}
