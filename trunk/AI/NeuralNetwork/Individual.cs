using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.AI
{
    public class Individual : ICloneable
    {
        private Genome mGenome;
        private double mFitness = 0.0;
        private bool mElite = false;

        public Genome Genome
        {
            get { return mGenome; }
            set
            {
                if(value == null) throw new ArgumentNullException();
                mGenome = value;
            }
        }

        public double Fitness
        {
            get { return mFitness; }
            set { mFitness = value; }
        }

        public bool Elite
        {
            get { return mElite; }
            set { mElite = value; }
        }

        public Individual(Individual original)
        {
            mGenome = original.Genome;
            mFitness = original.mFitness;
        }

        public Individual(Genome genome)
        {
            mGenome = genome;
        }

        public Individual(Genome genome, double fitness)
        {
            mGenome = genome;
            mFitness = fitness;
        }

        object ICloneable.Clone()
        {
            Individual clone = new Individual((Genome)(((ICloneable)mGenome).Clone()), mFitness);
            return (object)clone;
        }
    }
}
