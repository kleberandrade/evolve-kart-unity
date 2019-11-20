using UnityEngine;

public class Track : MonoBehaviour
{
    public static Track Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private Road[] roads;

    public int Length => roads == null ? 1 : roads.Length;

    private void Start()
    {
        roads = GetComponentsInChildren<Road>();
        for (int i = 0; i < roads.Length; i++)
        {
            roads[i].m_SequenceId = i + 1;
            roads[i].m_NextRoad = roads[(i + 1) % roads.Length];
        }
    }

}
