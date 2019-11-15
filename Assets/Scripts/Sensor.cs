using UnityEngine;

public class Sensor : MonoBehaviour
{
    public float m_MaxDistance;
    public LayerMask m_Layer;
    public float m_Distance;
    public bool m_Detect => m_Distance < m_MaxDistance;
    private RaycastHit m_Hit;

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out m_Hit, m_MaxDistance, m_Layer))
        {
            m_Distance = Vector3.Distance(transform.position, m_Hit.point);
            Debug.DrawLine(transform.position, m_Hit.point, Color.red);
        }
        else
        {
            m_Distance = m_MaxDistance;
            Debug.DrawLine(transform.position, transform.position + transform.forward * m_Distance, Color.green);
        }
    }

}
