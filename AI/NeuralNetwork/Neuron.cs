using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.AI
{
    internal struct Neuron
    {
        public double[] InputWeights;
        public double Bias;

        public uint GenomeLength
        {
            get { return (uint)InputWeights.Length + 1; }
        }

        public Neuron(uint inputCount)
        {
            InputWeights = new double[inputCount];
            for (uint i = 0; i < InputWeights.Length; ++i)
                InputWeights[i] = 1.0;
            Bias = 0;
        }

        public void Randomize(Random random, bool randomizeBias)
        {
            for (uint i = 0; i < InputWeights.Length; ++i)
                InputWeights[i] = random.NextDouble()*2.0-1.0;
            if (randomizeBias) Bias = random.NextDouble() * 2.0 - 1.0;
            else Bias = 0;
        }
    }
}
