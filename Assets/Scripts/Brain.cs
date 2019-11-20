using UnityEngine;

public class Brain : MonoBehaviour
{
    public Genome Genome { get; set; }
    public bool Alive { get; set; } = false;
    public Material m_DeadMaterial;
    private CarKinematics m_Controller;
    private int m_CurrentGene;
    private float m_StartTime;
    private Renderer[] m_Renderers;
    private Road m_CurrentRoad;
    private int m_RoadNumbers;

    private void Awake()
    {
        m_Controller = GetComponent<CarKinematics>();
        m_Renderers = GetComponentsInChildren<Renderer>();
    }

    public void Initialize()
    {
        Alive = true;
        m_StartTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (!Alive) return;

        m_Controller.VerticalInput = Genome[m_CurrentGene].Vertical;
        m_Controller.HorizontalInput = Genome[m_CurrentGene].Horizontal;

        if (Time.time - m_StartTime >= Genome[m_CurrentGene].Duration)
        {
            m_CurrentGene++;
            if (m_CurrentGene == Genome.Length)
                Kill();
            else
                m_StartTime = Time.time;
        }
    }

    public void Kill()
    {
        Alive = false;
        foreach (Renderer render in m_Renderers)
            render.material = m_DeadMaterial;

        m_Controller.Disable = true;

        EvaluateFitness();
    }

    public void EvaluateFitness()
    {
        var totalDistance = Vector3.Distance(m_CurrentRoad.transform.position, m_CurrentRoad.m_NextRoad.transform.position);
        var distanceToNextPosition = Vector3.Distance(transform.position, m_CurrentRoad.m_NextRoad.transform.position);
        Genome.Fitness = (m_RoadNumbers * 100.0f) + (totalDistance - distanceToNextPosition);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Wall"))
        {
            Kill();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Road"))
        {
            Road road = other.GetComponent<Road>();
            if (m_CurrentRoad == null)
            {
                m_CurrentRoad = road;
            }
            else
            {
                if (road.m_SequenceId > m_CurrentRoad.m_SequenceId || road.m_SequenceId == 1 && m_CurrentRoad.m_SequenceId == 20)
                {
                    m_RoadNumbers++;
                }

                m_CurrentRoad = road;
            }
        }
    }
}
