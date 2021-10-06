using UnityEditor;
using UnityEngine;

public class ExtraGizmos
{
    public static void DrawWireCapsule(Vector3 p1, Vector3 p2, float radius)
    {
#if UNITY_EDITOR
        // Special case when both points are in the same position
        if (p1 == p2)
        {
            Gizmos.DrawWireSphere(p1, radius);
            return;
        }
        using (new Handles.DrawingScope(Gizmos.color, Gizmos.matrix))
        {
            Quaternion p1Rotation = Quaternion.LookRotation(p1 - p2);
            Quaternion p2Rotation = Quaternion.LookRotation(p2 - p1);
            // Check if capsule direction is collinear to Vector.up
            float c = Vector3.Dot((p1 - p2).normalized, Vector3.up);
            if (c == 1f || c == -1f)
            {
                // Fix rotation
                p2Rotation = Quaternion.Euler(p2Rotation.eulerAngles.x, p2Rotation.eulerAngles.y + 180f, p2Rotation.eulerAngles.z);
            }
            // First side
            Handles.DrawWireArc(p1, p1Rotation * Vector3.left, p1Rotation * Vector3.down, 180f, radius);
            Handles.DrawWireArc(p1, p1Rotation * Vector3.up, p1Rotation * Vector3.left, 180f, radius);
            Handles.DrawWireDisc(p1, (p2 - p1).normalized, radius);
            // Second side
            Handles.DrawWireArc(p2, p2Rotation * Vector3.left, p2Rotation * Vector3.down, 180f, radius);
            Handles.DrawWireArc(p2, p2Rotation * Vector3.up, p2Rotation * Vector3.left, 180f, radius);
            Handles.DrawWireDisc(p2, (p1 - p2).normalized, radius);
            // Lines
            Handles.DrawLine(p1 + p1Rotation * Vector3.down * radius, p2 + p2Rotation * Vector3.down * radius);
            Handles.DrawLine(p1 + p1Rotation * Vector3.left * radius, p2 + p2Rotation * Vector3.right * radius);
            Handles.DrawLine(p1 + p1Rotation * Vector3.up * radius, p2 + p2Rotation * Vector3.up * radius);
            Handles.DrawLine(p1 + p1Rotation * Vector3.right * radius, p2 + p2Rotation * Vector3.left * radius);
        }
#endif
    }

    public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
#if UNITY_EDITOR
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180.0f + arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180.0f - arrowHeadAngle, 0) * Vector3.forward;
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
#endif
    }
}