using System;
using System.Collections.Generic;
using UnityEngine;

namespace RidersX.Objects
{
    [RequireComponent(typeof(Collider))]
    public class TrickRamp : MonoBehaviour
    {
        public enum LaunchDirection
        {
            Down, Up, Side
        }

        public List<TrickInterval> UpTrajectories;
        public List<TrickInterval> SideTrajectories;
        public List<TrickInterval> DownTrajectories;
        public Transform AxisOverride;

        [Space]
        public AnimationCurve GravityOverJumpStrength;

        [Space]
        public AnimationCurve FlipDurationOverJumpStrength = new AnimationCurve(new Keyframe(0.0f, 1.5f), new Keyframe(1.0f, 0.6f));

        [Header("Failure")]
        public bool IsManual = false;
        public Vector3 ProperExitDirection;

        private Transform _transform { get { return AxisOverride == null ? transform : AxisOverride; } }

        private Vector3 GetVelocityToPoint(Vector3 from, Vector3 to, float arcHeight, float gravity)
        {
            float heightDifference = to.y - from.y;
            Vector3 lateralDifference = new Vector3(to.x - from.x, 0.0f, to.z - from.z);

            float time = Mathf.Sqrt(-2.0f * arcHeight / gravity) + Mathf.Sqrt(2.0f * (heightDifference - arcHeight) / gravity);

            Vector3 verticalVelocity = Vector3.up * Mathf.Sqrt(-2.0f * gravity * arcHeight);
            Vector3 lateralVelocity = lateralDifference / time;

            return lateralVelocity + verticalVelocity * -Mathf.Sign(gravity);
        }

        public enum TrajectoryPreviews
        {
            None = 0,
            Up = 1,
            Down = 1 << 1,
            Side = 1 << 2
        }

        [Header("Editor only")]
        public TrajectoryPreviews ShowTrajectories = TrajectoryPreviews.Up | TrajectoryPreviews.Side | TrajectoryPreviews.Down;
        [Range(0.0f, 1.0f)]
        public float PreviewJumpStrength;
        public int PreviewSteps = 256;
        [Min(0.0f)]
        public float PreviewWidth = 0.0f;

        public Vector3 PreviewPosition;

        private Vector3[] _downPreviewPoints;
        private Vector3[] _upPreviewPoints;
        private Vector3[] _sidePreviewPoints;
        private Vector3 UpTrajectorVector;
        private Vector3 DownTrajectorVector;
        private Vector3 SideTrajectorVector;

        private void OnValidate()
        {
            _downPreviewPoints = GeneratePreviewPoints(DownTrajectories, PreviewSteps);
            _upPreviewPoints = GeneratePreviewPoints(UpTrajectories, PreviewSteps);
            _sidePreviewPoints = GeneratePreviewPoints(SideTrajectories, PreviewSteps);

            ProperExitDirection.Normalize();
            int IsManualInt = 0;
            if (IsManual)
            {
                IsManualInt = 1;
            }
            if (!IsManual)
            {
                IsManualInt = 0;
            }
            Vector3 ExitDir = new Vector3(ProperExitDirection.x, ProperExitDirection.y, ProperExitDirection.z);
            TrickInterval SetUp = GetComponent<TrickRamp>().UpTrajectories[0];
            TrickInterval SetDown = GetComponent<TrickRamp>().SideTrajectories[0];
            TrickInterval SetSide = GetComponent<TrickRamp>().DownTrajectories[0];
            Transform Up = transform.Find("Up Targets/Target");
            Transform Down = transform.Find("Down Targets/Target");
            Transform Side = transform.Find("Side Targets/Target");
            SetUp.Target = Up;
            SetDown.Target = Down;
            SetSide.Target = Side;
            float ArcHeightUP = SetUp.ArcHeight;
            float ArcHeightDown = SetDown.ArcHeight;
            float ArcHeightSide = SetSide.ArcHeight;
            float BOX_X = GetComponent<BoxCollider>().size.x;
            float BOX_Y = GetComponent<BoxCollider>().size.y;
            float BOX_Z = GetComponent<BoxCollider>().size.z;
            UpTrajectorVector = Up.transform.position;
            DownTrajectorVector = Down.transform.position;
            SideTrajectorVector = Side.transform.position;
            base.name = "TrickRamp-Spawner/" + IsManualInt + "/" + ExitDir.x + "/" + ExitDir.y + "/" + ExitDir.z + "/" + ArcHeightUP + "/" + ArcHeightDown + "/" + ArcHeightSide + "/" + BOX_X + "/" + BOX_Y + "/" + BOX_Z + "/" + UpTrajectorVector.x + "/" + UpTrajectorVector.y + "/" + UpTrajectorVector.z + "/" + DownTrajectorVector.x + "/" + DownTrajectorVector.y + "/" + DownTrajectorVector.z + "/" + SideTrajectorVector.x + "/" + SideTrajectorVector.y + "/" + SideTrajectorVector.z;
            name = name.Replace(",", ".");
        }
    


        private void OnDrawGizmos()
        {
            if (transform.hasChanged || (AxisOverride != null && AxisOverride.hasChanged))
            {
                OnValidate();
            }

            for (int i = 0; i < DownTrajectories.Count; i++)
            {
                if (DownTrajectories[i].Target.hasChanged)
                {
                    _downPreviewPoints = GeneratePreviewPoints(DownTrajectories, PreviewSteps);
                }
            }
            for (int i = 0; i < UpTrajectories.Count; i++)
            {
                if (UpTrajectories[i].Target.hasChanged)
                {
                    _upPreviewPoints = GeneratePreviewPoints(UpTrajectories, PreviewSteps);
                }
            }
            for (int i = 0; i < SideTrajectories.Count; i++)
            {
                if (SideTrajectories[i].Target.hasChanged)
                {
                    _sidePreviewPoints = GeneratePreviewPoints(SideTrajectories, PreviewSteps);
                }
            }

            if (ShowTrajectories != TrajectoryPreviews.Down)
            {
                Vector3 offset = _transform.TransformDirection(Vector3.right * PreviewWidth);
                TrickInterval downInterval = DownTrajectories.Find(x => x.ToJumpStrength >= PreviewJumpStrength);
                TrickInterval upInterval = UpTrajectories.Find(x => x.ToJumpStrength >= PreviewJumpStrength);
                TrickInterval sideInterval = SideTrajectories.Find(x => x.ToJumpStrength >= PreviewJumpStrength);

                for (int i = 0; i < PreviewSteps - 1; i++)
                {
                    if (ShowTrajectories.HasFlag(TrajectoryPreviews.None))
                    {
                        Gizmos.color = Color.yellow;
                        DrawPreview(_downPreviewPoints, offset, i, downInterval);
                    }

                    if (ShowTrajectories.HasFlag(TrajectoryPreviews.None))
                    {
                        Gizmos.color = Color.blue;
                        DrawPreview(_upPreviewPoints, offset, i, upInterval);
                    }

                    if (ShowTrajectories.HasFlag(TrajectoryPreviews.None))
                    {
                        Gizmos.color = Color.red;
                        DrawPreview(_sidePreviewPoints, offset, i, sideInterval);
                    }
                }
            }

            Gizmos.color = Color.white;
            ExtraGizmos.DrawArrow(_transform.TransformPoint(PreviewPosition), ProperExitDirection * 2.0f, 1.5f);
        }

        private void DrawPreview(Vector3[] points, Vector3 offset, int i, TrickInterval interval)
        {
            if (interval.IgnoreAxis.HasFlag(Axis.X) && PreviewWidth > 0.0f)
            {
                Gizmos.DrawLine(points[i] + offset, points[i + 1] + offset);
                Gizmos.DrawLine(points[i] - offset, points[i + 1] - offset);
            }
            else
            {
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
        }

        private Vector3[] GeneratePreviewPoints(List<TrickInterval> trajectories, int steps)
        {
            Vector3[] points = new Vector3[steps];
            Vector3 currentPosition = _transform.TransformPoint(PreviewPosition);

            TrickInterval interval = trajectories.Find(x => x.ToJumpStrength >= PreviewJumpStrength);
            if (interval.Target == null)
            {
                return points;
            }

            float gravityStrength = GravityOverJumpStrength.Evaluate(PreviewJumpStrength);
            Vector3 gravity = Vector3.down * gravityStrength;
            Vector3 velocity = GetVelocityToPoint(currentPosition, interval.Target.position, interval.ArcHeight, -gravityStrength);

            for (int i = 0; i < steps; i++)
            {
                points[i] = currentPosition;

                velocity += gravity * Time.fixedDeltaTime;
                currentPosition = currentPosition + (velocity * Time.fixedDeltaTime);
            }

            return points;
        }
    }
}

    
