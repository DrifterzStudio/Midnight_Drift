using UnityEngine;

// marks the start/finish line. drop it on the track, blue (forward) axis pointing the way you drive.
// RaceManager reads its pose for the lap detection. in multi it's one shared line for everyone.
public class FinishLine : MonoBehaviour
{
    [Tooltip("Just the editor gizmo size. The real detection width is lineHalfWidth on RaceManager.")]
    public float gizmoWidth = 20f;

    void OnDrawGizmos()
    {
        Vector3 fwd = transform.forward;
        fwd.y = 0f;
        fwd = fwd.sqrMagnitude > 0.0001f ? fwd.normalized : Vector3.forward;
        Vector3 right = Vector3.Cross(Vector3.up, fwd);
        Vector3 p = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(p - right * gizmoWidth * 0.5f, p + right * gizmoWidth * 0.5f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(p, p + fwd * 3f);
    }
}
