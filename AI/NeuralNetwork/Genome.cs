using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.AI.BasicNeuralNetwork
{
    public class Genome : ICloneable
    {
        private double[] mGenes;

        public double[] Genes
        {
            get { return mGenes; }
            set { mGenes = value; }
        }

        public uint Length
        {
            get { return (uint)mGenes.Length; }
        }

        public Genome() { }

        public Genome(uint length)
        {
            mGenes = new double[length];
        }

        public Genome(Genome original)
        {
            mGenes = original.mGenes;
        }

        public void Randomize(Random random)
        {
            for (int i = 0; i < mGenes.Length; ++i)
                mGenes[i] = random.NextDouble() * 2.0 - 1.0;
        }

        public void Randomize(Random random, double ratio)
        {
            for(int i = 0 ; i < mGenes.Length ; ++i)
                if (random.NextDouble() < ratio)
                    mGenes[i] = random.NextDouble() * 2.0 - 1.0;
        }

        public void Perturbate(Random random, double maxAmount)
        {
            for (int i = 0; i < mGenes.Length; ++i)
                mGenes[i] += (random.NextDouble() * 2.0 - 1.0) * maxAmount;
        }

        public void Perturbate(Random random, double ratio, double maxAmount)
        {
            for (int i = 0; i < mGenes.Length; ++i)
                if (random.NextDouble() < ratio)
                    mGenes[i] += (random.NextDouble() * 2.0 - 1.0) * maxAmount;
        }

        public static Genome Crossover(Random random, Genome genome1, Genome genome2)
        {
            if (genome1.Length != genome2.Length) throw new ArgumentException("Genomes must have equal length");
            Genome child = new Genome(genome1.Length);
            for (int i = 0; i < child.Length; ++i)
                child.Genes[i] = (random.Next(2) == 1) ? genome1.Genes[i] : genome2.Genes[i];
            return child;
        }

        object ICloneable.Clone()
        {
            Genome clone = new Genome((uint)mGenes.Length);
            mGenes.CopyTo(clone.mGenes, 0);
            return (object)clone;
        }
    }
}
