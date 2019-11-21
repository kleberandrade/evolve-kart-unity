using System.Collections.Generic;
using System.Linq;

public static class GeneticOperator
{
    private static float MUTATION_PROBABILITY = 0.05f;
    private static float CROSSOVER_PROBABILITY = 0.5f;
    private static int TOURNAMENT_SELECTION_SIZE = 3;
    private static float ALPHA = 0.2f;
    public static int ELITISM_NUMBER = 10;

    public static List<Genome> Elitism(List<Genome> genomes)
    {
        List<Genome> elite = new List<Genome>();
        var candidates = genomes.OrderByDescending(g => g.Fitness).ToList();
        for (int i = 0; i < ELITISM_NUMBER; i++)
            elite.Add((Genome)candidates[i].Clone());

        return elite;
    }

    public static void Mutate(ref Genome genome)
    {
        float rate = MUTATION_PROBABILITY / (float)genome.Length;

        for (int i = 0; i < genome.Length; i++)
        {
            if (Helper.Next(1.0f) < rate * (i + 1))
            {
                genome[i].Horizontal = Helper.Next(Gene.MIN_HORIZONTAL, Gene.MAX_HORIZONTAL);
                genome[i].Vertical = Helper.Next(Gene.MIN_VERTICAL, Gene.MAX_VERTICAL);
                genome[i].Duration = Helper.Next(Gene.MIN_DURATION, Gene.MAX_DURATION);
            }
        }
    }

    public static void UniformCrossover(Genome parent1, Genome parent2, out Genome offspring1, out Genome offspring2)
    {
        offspring1 = (Genome)parent1.Clone();
        offspring2 = (Genome)parent2.Clone();

        for (int i = 0; i < parent1.Length; i++)
        {
            float rnd = Helper.Next(1.0f);
            offspring1[i] = rnd < CROSSOVER_PROBABILITY ? parent1[i] : parent2[i];
            offspring2[i] = rnd < CROSSOVER_PROBABILITY ? parent2[i] : parent1[i];
        }
    }

    public static void BlendCrossover(Genome parent1, Genome parent2, out Genome offspring)
    {
        offspring = new Genome((int)parent1.Length);

        for (int i = 0; i < parent1.Length; i++)
        {
            offspring[i].Vertical = parent1[i].Vertical + ALPHA * (parent2[i].Vertical - parent1[i].Vertical);
            offspring[i].Horizontal = parent1[i].Horizontal + ALPHA * (parent2[i].Horizontal - parent1[i].Horizontal);
            offspring[i].Duration = parent1[i].Duration + ALPHA * (parent2[i].Duration - parent1[i].Duration);
        }
    }

    public static Genome TournamentSelection(List<Genome> population)
    {
        List<Genome> candidates = new List<Genome>();

        for (int i = 0; i < TOURNAMENT_SELECTION_SIZE; i++)
        {
            int index = Helper.Next(0, population.Count - 1);
            candidates.Add(population[index]);
        }

        return candidates.OrderByDescending(g => g.Fitness).ToList()[0];
    }
}