using System;
using System.Collections.Generic;

public class Genome : ICloneable
{
    private List<Gene> m_Genes;

    public Gene this[int index]
    {
        get { return m_Genes[index]; }
        set { m_Genes[index] = value; }
    }

    public float Length => m_Genes != null ? m_Genes.Count : 0;

    public float Fitness { get; set; }

    public Genome()
    {
        Fitness = 0.0f;
        m_Genes = new List<Gene>();
    }

    public Genome(int actionNumber) : this()
    {
        for (int i = 0; i < actionNumber; i++)
            Growth();
    }

    public void Growth()
    {
        m_Genes.Add(new Gene());
    }

    public Genome(Genome source) : this()
    {
        for (int i = 0; i < source.Length; i++)
            m_Genes.Add((Gene)source[i].Clone());
    }

    public object Clone()
    {
        return new Genome(this);
    }
}
