using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashpanel : MonoBehaviour
{
    public GameObject DashpanelReference;
    [Header("The Speed added upon entering the Dash Panel")]
    public float MinSpeed = 0.0f;
    private void OnValidate()
    {
            DashpanelReference.name = "Dashpanel-Spawner/" + MinSpeed;
            DashpanelReference.name = DashpanelReference.name.Replace(",", ".");
    }
}
