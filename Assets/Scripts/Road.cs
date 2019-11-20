using UnityEngine;

public class Road : MonoBehaviour
{
    public int m_SequenceId;

    public Road m_NextRoad;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        Gizmos.DrawSphere(transform.position, 1.0f);

        if (m_NextRoad)
            Gizmos.DrawLine(transform.position, m_NextRoad.transform.position);
    }
}
