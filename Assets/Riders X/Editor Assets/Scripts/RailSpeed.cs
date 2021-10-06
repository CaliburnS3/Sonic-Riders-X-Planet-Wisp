using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSpeed : MonoBehaviour
{
    public GameObject RailReference;
    [Header("The Speed added upon entering the Rail")]
    public float Speed = 0.0f;
    private void OnValidate()
    {
        RailReference.name = "Grind-Rail/" + Speed;
        RailReference.name = RailReference.name.Replace(",", ".");
    }
}
