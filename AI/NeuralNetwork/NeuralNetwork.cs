using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.AI.BasicNeuralNetwork
{
    public class NeuralNetwork
    {
        public delegate double ComputeOutputDelegate(double netInput);

        public static double Linear(double netInput)
        {
            return netInput;
        }

        public static double Sigmoid(double netInput)
        {
            const double activationResponse = 1.0;
            return (1.0 / (1.0 + Math.Exp(-netInput / activationResponse)));
        }

        private uint mInputCount;
        private NeuronLayer[] mNeuronLayers;
        private uint mGenomeLength;
        private double mBiasFactor = 1.0;
        private ComputeOutputDelegate mOutputComputingFunction = Linear;

        public uint InputCount
        {
            get { return mInputCount; }
        }

        public uint HiddenLayerCount
        {
            get { return (uint)mNeuronLayers.Length; }
        }

        public uint NeuronPerHiddenLayer
        {
            get
            {
                if (mNeuronLayers.Length == 1) return 0;
                return (uint)mNeuronLayers[0].Neurons.Length;
            }
        }

        public uint OutputCount
        {
            get { return (uint)mNeuronLayers[mNeuronLayers.Length-1].Neurons.Length; }
        }

        public uint GenomeLength
        {
            get { return mGenomeLength; }
        }

        public double BiasFactor
        {
            get { return mBiasFactor; }
            set { mBiasFactor = value; }
        }

        public ComputeOutputDelegate OutputComputingFunction
        {
            get { return mOutputComputingFunction; }
            set { mOutputComputingFunction = value; }
        }

        public NeuralNetwork(uint inputCount, uint hiddenLayerCount, uint neuronsPerHiddenLayer, uint outputCount)
        {
            if (inputCount == 0) throw new ArgumentException("Cannot have 0 inputs");
            if (inputCount == 0) throw new ArgumentException("Cannot have 0 inputs");
            if (hiddenLayerCount != 0 && neuronsPerHiddenLayer == 0) throw new ArgumentException("Cannot have hidden layers with 0 neurons");

            mInputCount = inputCount;
            mNeuronLayers = new NeuronLayer[hiddenLayerCount + 1];
            if (hiddenLayerCount == 0)
            {
                mNeuronLayers[0] = new NeuronLayer(outputCount, inputCount);
            }
            else
            {
                for (int i = 0; i < hiddenLayerCount; ++i)
                {
                    if (i == 0)
                        mNeuronLayers[i] = new NeuronLayer(neuronsPerHiddenLayer, inputCount);
                    else
                        mNeuronLayers[i] = new NeuronLayer(neuronsPerHiddenLayer, (uint)mNeuronLayers[i - 1].Neurons.Length);
                }
                mNeuronLayers[mNeuronLayers.Length - 1] = new NeuronLayer(outputCount, neuronsPerHiddenLayer);
            }

            mGenomeLength = 0;
            foreach (NeuronLayer neuronLayer in mNeuronLayers)
                mGenomeLength += neuronLayer.GenomeLength;
        }

        public void Randomize(Random random) { Randomize(random, true); }
        public void Randomize(Random random, bool randomizeBiases)
        {
            // Iterate through neuron layers
            for (uint i = 0; i < mNeuronLayers.Length; ++i)
            {
                // Randomize layer
                mNeuronLayers[i].Randomize(random, randomizeBiases);
            }
        }

        public Genome Genome
        {
            get
            {
                Genome genome = new Genome(mGenomeLength);
                int geneIndex = 0;
                // Iterate through neuron layers
                for (uint i = 0; i < mNeuronLayers.Length; ++i)
                {
                    // Iterate through neurons
                    for (uint j = 0; j < mNeuronLayers[i].Neurons.Length; ++j)
                    {
                        // Iterate through input weights
                        for (uint k = 0; k < mNeuronLayers[i].Neurons[j].InputWeights.Length; ++k)
                        {
                            // Write weight to genome
                            genome.Genes[geneIndex] = mNeuronLayers[i].Neurons[j].InputWeights[k];
                            ++geneIndex;
                        }

                        // Write bias to genome
                        genome.Genes[geneIndex] = mNeuronLayers[i].Neurons[j].Bias;
                        ++geneIndex;
                    }
                }
                return genome;
            }
            set
            {
                if (value.Length != mGenomeLength) throw new ArgumentException("Invalid genome length", "genome");
                int geneIndex = 0;
                // Iterate through neuron layers
                for (uint i = 0; i < mNeuronLayers.Length; ++i)
                {
                    // Iterate through neurons
                    for (uint j = 0; j < mNeuronLayers[i].Neurons.Length; ++j)
                    {
                        // Iterate through input weights
                        for (uint k = 0; k < mNeuronLayers[i].Neurons[j].InputWeights.Length; ++k)
                        {
                            // Write weight from genome
                            mNeuronLayers[i].Neurons[j].InputWeights[k] = value.Genes[geneIndex];
                            ++geneIndex;
                        }

                        // Write bias from genome
                        mNeuronLayers[i].Neurons[j].Bias = value.Genes[geneIndex];
                        ++geneIndex;
                    }
                }
            }
        }

        public List<double> Process(double[] inputs)
        {
            if ((uint)inputs.Length != mInputCount) throw new ArgumentException("Invalid input count", "inputs");
            List<double> inputList = new List<double>(inputs);
            List<double> outputList = null;

            // Iterate through layers
            for (uint i = 0; i < mNeuronLayers.Length; ++i)
            {
                outputList = new List<double>();

                // Iterate through neurons
                for (uint j = 0; j < mNeuronLayers[i].Neurons.Length; ++j)
                {
                    double netInput = 0.0;

                    // Iterate through input weights
                    for (uint k = 0; k < (uint)inputList.Count; ++k)
                    {
                        // Treat input
                        netInput += mNeuronLayers[i].Neurons[j].InputWeights[k] * inputList[(int)k];
                    }

                    // Add bias
                    netInput += mNeuronLayers[i].Neurons[j].Bias * mBiasFactor;

                    // Save output
                    outputList.Add(mOutputComputingFunction(netInput));
                }

                inputList = outputList;
            }

            return outputList;
        }
    }
}
