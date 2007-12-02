using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.AI
{
    internal class NeuronLayer
    {
        public Neuron[] Neurons;

        public uint NeuronCount
        {
            get { return (uint)Neurons.Length; }
        }

        public uint GenomeLength
        {
            get { return Neurons[0].GenomeLength * (uint)Neurons.Length; }
        }

        public NeuronLayer(uint neuronCount, uint inputsPerNeuron)
        {
            Neurons = new Neuron[neuronCount];
            for (uint i = 0; i < Neurons.Length; ++i)
                Neurons[i] = new Neuron(inputsPerNeuron);
        }

        public void Randomize(Random random, bool randomizeBiases)
        {
            for (uint i = 0; i < Neurons.Length; ++i)
                Neurons[i].Randomize(random, randomizeBiases);
        }
    }
}
