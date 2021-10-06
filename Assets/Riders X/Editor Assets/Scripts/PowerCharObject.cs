using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCharObject : MonoBehaviour
{
    public GameObject PowerObjectRef;
    public BoxCollider TriggerBoxCollider;
    public BoxCollider NonTriggerBoxCollider;
    private float X;
    private float Y;
    private float Z;
    public bool ApplyValues;
    private void OnValidate()
    {
    
            float X = 0f;
            float Y = 0f;
            float Z = 0f;
            X = TriggerBoxCollider.size.x;
            Y = TriggerBoxCollider.size.y;
            Z = TriggerBoxCollider.size.z;
            PowerObjectRef.name = "PowerObject-Spawner/" + X + "/" + Y + "/" + Z;
            PowerObjectRef.name = PowerObjectRef.name.Replace(",", ".");
            NonTriggerBoxCollider.size = TriggerBoxCollider.size / 1.2f;
            ApplyValues = false;
        
    }
}
