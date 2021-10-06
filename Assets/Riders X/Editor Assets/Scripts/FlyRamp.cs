using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyRamp : MonoBehaviour
{
    public GameObject FlyRampReference;

    public float StartSpeed = 55.55555f;
    public Vector3 ExitDirection;

    [Header("Trigger")]
    public BoxCollider Collider;
    private void OnValidate()
    {
        float X = 0f;
        float Y = 0f;
        float Z = 0f;
        X = Collider.size.x;
        Y = Collider.size.y;
        Z = Collider.size.z;
        float startSpeed = StartSpeed;
        Vector3 ExitDir = ExitDirection;
        FlyRampReference.name = "FlyCharacter/" + X + "/" + Y + "/" + Z + "/" + startSpeed + "/" + ExitDir.x + "/" + ExitDir.y + "/" + ExitDir.z;
        FlyRampReference.name = FlyRampReference.name.Replace(",", ".");
    }
    private void OnDrawGizmosSelected()
    {
        ExtraGizmos.DrawArrow(transform.position, transform.TransformDirection(ExitDirection) * 2.0f, 1.5f);
    }
}

