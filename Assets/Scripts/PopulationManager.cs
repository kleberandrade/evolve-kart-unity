using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopulationManager : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject m_KartPrefab;
    public Transform m_Spawner;

    [Header("Save")]
    public string m_FileName = "save.xls";
    public bool m_Save = true;

    [Header("Parameters")]
    public int m_PopulationSize = 20;
    public int m_StartActionNumber = 5;
    public int m_GrowthActions = 1;
    public float m_TrialTime = 20.0f;

    [Header("UI (User Interface)")]
    public Text m_GenerationText;
    public Text m_ActionsText;
    public Text m_DeadsText;
    public Text m_ElapsedTimeText;
    public Text m_TotalTimeText;

    private List<Brain> m_Population = new List<Brain>();
    private float m_ElapsedTime = 0;
    private int m_Deads = 0;
    private int m_Generation = 1;
    private int m_CurrentActionNumber;

    private bool m_Initialized = false;

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        m_CurrentActionNumber = m_StartActionNumber;

        for (int i = 0; i < m_PopulationSize; i++)
        {
            Brain brain = CreateKartWithBrain(new Genome(m_CurrentActionNumber));
            m_Population.Add(brain);
        }

        InitializeBrains();
        m_Initialized = true;
    }

    public void InitializeBrains()
    {
        foreach (Brain brain in m_Population)
            brain.Initialize();
    }

    public Brain CreateKartWithBrain(Genome genome)
    {
        GameObject kart = Instantiate(m_KartPrefab, m_Spawner.position, m_Spawner.rotation);
        Brain brain = kart.GetComponent<Brain>();
        brain.Genome = genome;
        return brain;
    }

    private void UpdateUI()
    {
        if (m_GenerationText)
            m_GenerationText.text = $"GENERATION: {m_Generation}";

        if (m_ElapsedTimeText)
            m_ElapsedTimeText.text = $"TRIAL TIME: {(int)m_ElapsedTime} / {m_TrialTime}";

        if (m_DeadsText)
            m_DeadsText.text = $"DEADS: {m_PopulationSize - m_Deads}/{m_PopulationSize}";

        if (m_ActionsText)
            m_ActionsText.text = $"ACTIONS: {m_CurrentActionNumber}";

        if (m_TotalTimeText)
            m_TotalTimeText.text = $"TOTAL TIME: {TimeSpan.FromSeconds(Time.time).ToString(@"hh\:mm\:ss") }";
    }

    private void Update()
    {
        if (!m_Initialized) return;

        m_ElapsedTime += Time.deltaTime;
        m_Deads = m_Population.Where(x => x.Alive).ToList().Count;

        if (m_ElapsedTime > m_TrialTime || m_Deads == 0)
        {
            NewPopulation();
            m_ElapsedTime = 0;
        }

        UpdateUI();
    }

    public void KillAll()
    {
        foreach (var brain in m_Population)
        {
            if (brain.Alive)
                brain.Kill();
        }
    }

    public List<Genome> GetAllGenomes()
    {
        var genomes = new List<Genome>();
        for (int i = 0; i < m_PopulationSize; i++)
            genomes.Add(m_Population[i].Genome);

        return genomes;
    }

    private void NewPopulation()
    {
        KillAll();

        var genomes = GetAllGenomes();
        var newPopulation = new List<Brain>();

        if (m_Save)
            Save(m_Generation != 1, genomes);

        var elite = GeneticOperator.Elitism(genomes);
        foreach (var genome in elite)
        {
            for (int j = 0; j < m_GrowthActions; j++)
                genome.Growth();

            newPopulation.Add(CreateKartWithBrain(genome));
        }

        for (int i = 0; i < (m_PopulationSize - GeneticOperator.ELITISM_NUMBER) / 2; i++)
        {
            Genome parent1 = GeneticOperator.TournamentSelection(genomes);
            Genome parent2 = GeneticOperator.TournamentSelection(genomes);
            Genome parent3 = GeneticOperator.TournamentSelection(genomes);

            Genome offspring1, offspring2;

            GeneticOperator.BlendCrossover(parent1, parent2, out offspring1);
            GeneticOperator.BlendCrossover(parent1, parent3, out offspring2);

            /*
            Genome parent1 = GeneticOperator.TournamentSelection(genomes);
            Genome parent2 = GeneticOperator.TournamentSelection(genomes);

            Genome offspring1, offspring2;

            GeneticOperator.UniformCrossover(parent1, parent2, out offspring1, out offspring2);
            */

            GeneticOperator.Mutate(ref offspring1);
            GeneticOperator.Mutate(ref offspring2);

            for (int j = 0; j < m_GrowthActions; j++)
            {
                offspring1.Growth();
                offspring2.Growth();
            }

            newPopulation.Add(CreateKartWithBrain(offspring1));
            newPopulation.Add(CreateKartWithBrain(offspring2));
        }

        for (int i = 0; i < m_PopulationSize; i++)
        {
            Destroy(m_Population[i].gameObject);
        }

        m_Population.Clear();
        m_Population = newPopulation;
        InitializeBrains();

        m_CurrentActionNumber += m_GrowthActions;
        m_Generation++;
    }

    public void Save(bool append, List<Genome> genomes)
    {
        if (string.IsNullOrEmpty(m_FileName))
        {
            return;
        }

        using (StreamWriter file = new StreamWriter(m_FileName, append))
        {
            float bestFitness = 0.0f;
            float averageFitness = 0.0f;
            for (int i = 0; i < m_PopulationSize; i++)
            {
                averageFitness += genomes[i].Fitness;
                if (bestFitness < genomes[i].Fitness)
                {
                    bestFitness = genomes[i].Fitness;
                }
            }

            averageFitness /= (float)m_PopulationSize;
            file.WriteLine("{0}\t{1}", averageFitness, bestFitness);
        }
    }
}
