using PathCreation;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static PathCreation.BezierPath;

public class SplineTool : EditorWindow
{
    public enum SplineCreationType
    {
        Points, Bezier
    }

    public GameObject TargetGameObject;
    public GameObject PreviousTarget;
    public PathCreator Spline;
    public SplineCreationType SplineType;
    public Transform[] Points;
    public Transform Parent;

    public Vector3 ColliderDimensions = Vector3.one;
    public Vector3 ColliderOffset = Vector3.zero;
    public bool FixEndCaps = true;

    [MenuItem("Riders X/Spline Creator")]
    public static void Open()
    {
        SplineTool window = (SplineTool)CreateInstance(typeof(SplineTool));
        window.Show();
    }

    private void OnEnable()
    {
        TargetGameObject = Selection.activeGameObject;
        Points = new Transform[0];
        CheckForPath();
    }

    private void OnGUI()
    {
        ScriptableObject target = this;
        SerializedObject serializedObject = new SerializedObject(target);

        TargetGameObject = Selection.activeGameObject;
        if (TargetGameObject != null && TargetGameObject != PreviousTarget)
        {
            CheckForPath();
            PreviousTarget = TargetGameObject;
        }

        serializedObject.ApplyModifiedProperties();

        if (TargetGameObject != null)
        {
            EditorGUILayout.LabelField("Selected: " + TargetGameObject.name);

            if (Spline != null)
            {
                EditorGUILayout.Space();

                DrawSplineCreation(serializedObject);

                EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);

                DrawColliderCreation(serializedObject);
            }
            else
            {
                CheckForPath();
                if (Spline == null)
                {
                    EditorGUILayout.HelpBox(new GUIContent("Target has no path creator component."));
                    if (GUILayout.Button("Add component"))
                    {
                        TargetGameObject.AddComponent<PathCreator>();
                    }
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox(new GUIContent("Please assign a gameobject to modify."));
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void CheckForPath()
    {
        if (TargetGameObject != null)
        {
            Spline = TargetGameObject.GetComponent<PathCreator>();
        }
    }

    private void DrawColliderCreation(SerializedObject serializedObject)
    {
        EditorGUILayout.LabelField("Collider Creation");

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(ColliderDimensions)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(ColliderOffset)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FixEndCaps)));

        EditorGUILayout.Space();

        float colliderCount = Mathf.Round((Spline.path.length * TargetGameObject.transform.lossyScale.x) / ColliderDimensions.z);
        EditorGUILayout.LabelField("Will generate: " + colliderCount + " collider(s)");

        if (GUILayout.Button("Generate Colliders"))
        {
            ClearColliders();

            Transform transform = TargetGameObject.transform;

            float iteration = 0.0f;
            int colliderNumber = 1;
            float splineLength = Spline.path.length * transform.lossyScale.x;
            while (iteration < splineLength)
            {
                Vector3 point = Spline.path.GetPointAtDistance(iteration / transform.lossyScale.x);
                float colliderLength = ColliderDimensions.z;

                GameObject newCollider = new GameObject("Collider " + colliderNumber);
                BoxCollider boxCollider = newCollider.AddComponent<BoxCollider>();
                //In case the last collider is too large, make it fit
                if (FixEndCaps && iteration + ColliderDimensions.z > splineLength)
                {
                    colliderLength = ((iteration + ColliderDimensions.z) - splineLength) * 2.0f;
                    boxCollider.size = new Vector3(ColliderDimensions.x, ColliderDimensions.y, colliderLength);
                }
                else
                {
                    boxCollider.size = ColliderDimensions;
                }
                boxCollider.isTrigger = true;
                newCollider.transform.SetParent(transform, true);

                float beginningDistance = Spline.path.GetClosestDistanceAlongPath(point);
                Vector3 beginningDirection = Spline.path.GetDirectionAtDistance(beginningDistance, EndOfPathInstruction.Stop);
                Vector3 beginningNormal = Spline.path.GetNormalAtDistance(beginningDistance, EndOfPathInstruction.Stop);

                float endDistance = Spline.path.GetClosestDistanceAlongPath(point + beginningDirection * ColliderDimensions.z + newCollider.transform.InverseTransformDirection(ColliderOffset));
                Vector3 endNormal = Spline.path.GetNormalAtDistance(endDistance, EndOfPathInstruction.Stop);
                Vector3 endDirection = Spline.path.GetDirectionAtDistance(endDistance, EndOfPathInstruction.Stop);

                //Average direction and normal
                newCollider.transform.rotation = Quaternion.LookRotation((beginningDirection + endDirection) / 2.0f, (beginningNormal + endNormal) / 2.0f);
                newCollider.transform.position = point;

                boxCollider.center = Vector3.forward * colliderLength * 0.5f + ColliderOffset;
                EditorUtility.SetDirty(boxCollider);

                iteration += ColliderDimensions.z;
                colliderNumber++;
            }
        }
        if (GUILayout.Button("Clear Colliders"))
        {
            ClearColliders();
        }
    }

    private void ClearColliders()
    {
        foreach (Collider collider in TargetGameObject.GetComponentsInChildren<Collider>())
        {
            DestroyImmediate(collider.gameObject);
        }
    }

    private void DrawSplineCreation(SerializedObject serializedObject)
    {
        EditorGUILayout.LabelField("Spline Creation");

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SplineType)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Parent)));
        if (Parent != null)
        {
            if (GUILayout.Button("Use children as points"))
            {
                List<Transform> children = new List<Transform>();
                for (int i = 0; i < Parent.childCount; i++)
                {
                    Transform child = Parent.GetChild(i);
                    if (child)
                    {
                        children.Add(child);
                    }
                }

                Points = children.ToArray();
            }
            EditorGUILayout.Space();
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Points)), true);

        if (Points.Length > 1 && Points[0] != null && Points[1] != null)
        {
            EditorGUILayout.Space();
            if (SplineType == SplineCreationType.Points)
            {
                if (GUILayout.Button("Generate Spline (Point based)"))
                {
                    CreatePointSpline();
                }
            }
            else if (SplineType == SplineCreationType.Bezier)
            {
                EditorGUILayout.HelpBox(new GUIContent("Bezier spline creation will assume the structure of points is as follows:\n" +
                    "Point 1\n   Handle (Right)\n   Handle (Left)\n" +
                    "Point 2\n   Handle (Right)\n   Handle (Left)\n..."));
                if (GUILayout.Button("Generate Spline (Bezier based)"))
                {
                    CreateBezierSpline();
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox(new GUIContent("Please assign at least two points to create a spline."));
        }
    }

    private void CreatePointSpline()
    {
        List<Vector3> localPoints = new List<Vector3>();
        for (int i = 0; i < Points.Length; i++)
        {
            localPoints.Add(TargetGameObject.transform.InverseTransformPoint(Points[i].position));
        }
        Spline.bezierPath = new BezierPath(localPoints, false, PathSpace.xyz);
        Spline.bezierPath.ControlPointMode = ControlMode.Automatic;
    }

    private void CreateBezierSpline()
    {
        Transform transform = TargetGameObject.transform;

        Spline.bezierPath = new BezierPath(Points, false, PathSpace.xyz);
        Spline.bezierPath.ControlPointMode = ControlMode.Free;

        int anchorIndex = 0;
        for (int i = 1; i < Spline.bezierPath.NumPoints - 1; i += 3)
        {
            // Anchor
            Spline.bezierPath.SetPoint(i - 1, transform.InverseTransformPoint(Points[anchorIndex].position));
            // Left handle
            Spline.bezierPath.SetPoint(i, transform.InverseTransformPoint(Points[anchorIndex].GetChild(1).position));
            // Right handle
            Spline.bezierPath.SetPoint(i + 1, transform.InverseTransformPoint(Points[anchorIndex + 1].GetChild(0).position));
            anchorIndex++;
        }

        Spline.bezierPath.SetPoint(Spline.bezierPath.NumPoints - 1, transform.InverseTransformPoint(Points[anchorIndex].position));
    }
}
