using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.AI
{
    public delegate float[] Crossover(float[] dad, float[] mom, Random random);
    public delegate void Mutator(float[] genome, float rate, Random random);

    public class GeneticAlgorithm
    {
        private List<Individual> mIndividuals = new List<Individual>();
        private Dictionary<Mutator, float> mMutators = new Dictionary<Mutator, float>();
        private Crossover mCrossover = null;
        private uint mEliteCount = 0;
        private uint mEliteCopies = 1;
        private bool mMutateElites = false;
        private uint mTournamentCompetitors = 2;

        public List<Individual> Individuals
        {
            get { return mIndividuals; }
        }

        public double PerturbationRate
        {
            get { return mPerturbationRate; }
            set { mPerturbationRate = value; }
        }

        public double MaxPerturbationAmount
        {
            get { return mMaxPerturbationAmount; }
            set { mMaxPerturbationAmount = Math.Abs(value); }
        }

        public double MutationRate
        {
            get { return mMutationRate; }
            set { mMutationRate = value; }
        }

        public double CrossoverRate
        {
            get { return mCrossoverRate; }
            set { mCrossoverRate = value; }
        }

        public uint EliteCount
        {
            get { return mEliteCount; }
            set { mEliteCount = value; }
        }

        public bool PerturbateElites
        {
            get { return mPerturbateElites; }
            set { mPerturbateElites = value; }
        }

        public bool MutateElites
        {
            get { return mMutateElites; }
            set { mMutateElites = value; }
        }

        public uint EliteCopies
        {
            get { return mEliteCopies; }
            set { mEliteCopies = value; }
        }

        public uint TournamentCompetitors
        {
            get { return mTournamentCompetitors; }
            set { mTournamentCompetitors = value == 0 ? 1 : TournamentCompetitors; }
        }

        private static int FitnessSort(Individual individual1, Individual individual2)
        {
            return individual1.Fitness.CompareTo(individual2.Fitness);
        }

        public void RunGeneration(Random random)
        {
            if (mIndividuals.Count == 0) return;

            mIndividuals.Sort(FitnessSort);

            List<Individual> newIndividuals = new List<Individual>();

            // Do some elitism
            uint copiedEliteCount = 0;
            uint eliteCount = Math.Min(mEliteCount, (uint)mIndividuals.Count);
            for (uint i = 0; i < mEliteCopies; ++i)
            {
                for (uint j = 0; j < eliteCount; ++j)
                {
                    if (copiedEliteCount < (uint)mIndividuals.Count)
                    {
                        newIndividuals.Add(mIndividuals[mIndividuals.Count - 1 - (int)j]);
                        newIndividuals[newIndividuals.Count - 1].Elite = true;
                        ++copiedEliteCount;
                    }
                }
            }

            // Generate new individuals by crossing tournament selected individuals
            uint tournamentCompetitorCount = Math.Min(mTournamentCompetitors, (uint)mIndividuals.Count);
            while (newIndividuals.Count < mIndividuals.Count)
            {
                // Semi-randomly pick two good competitors
                Individual[] tournamentCompetitors = new Individual[] { null, null };
                for (uint i = 0; i < 2; ++i)
                {
                    double bestFitnessSoFar = double.MinValue;

                    for (uint j = 0; j < tournamentCompetitorCount; ++j)
                    {
                        Individual randomCompetitor = mIndividuals[random.Next(mIndividuals.Count)];
                        if (randomCompetitor.Fitness > bestFitnessSoFar)
                        {
                            bestFitnessSoFar = randomCompetitor.Fitness;
                            tournamentCompetitors[i] = randomCompetitor;
                        }
                    }
                }

                // Cross their genomes
                newIndividuals.Add(new Individual(Genome.Crossover(random, tournamentCompetitors[0].Genome, tournamentCompetitors[1].Genome)));
                newIndividuals[newIndividuals.Count - 1].Elite = false;
            }

            // Perturbate
            for (uint i = mPerturbateElites ? 0 : copiedEliteCount; i < newIndividuals.Count; ++i)
            {
                newIndividuals[(int)i].Genome.Perturbate(random, mPerturbationRate, mMaxPerturbationAmount);
            }

            // Mutate
            for (uint i = mMutateElites ? 0 : copiedEliteCount; i < newIndividuals.Count; ++i)
            {
                newIndividuals[(int)i].Genome.Randomize(random, mMutationRate);
            }

            // Our new, evolved, individuals
            mIndividuals = newIndividuals;
        }
    }
}
