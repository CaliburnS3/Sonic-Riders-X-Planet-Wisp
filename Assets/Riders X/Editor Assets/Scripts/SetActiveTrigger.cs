using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveTrigger : MonoBehaviour
{
    public GameObject Activate;
    public GameObject Deactivate;

    public void OnTriggerEnter(Collider character)
    {
        if (character.CompareTag("Player"))
        {
            if (Activate != null)
            {
                Activate.SetActive(true);
            }
            if (Deactivate != null)
            {
                Deactivate.SetActive(false);
            }
        }
    }
}

