using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Engine.Network.Activation;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.ML.Data.Basic;
using Encog.ML.Data;
using Encog.MathUtil.Randomize;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System;

namespace ArtificialIntelligence
{
    public class NeuralNetwork
    {

        public enum NetworkState
        {
            CONFIGURED,
            TRAINED,
            INITIALIZED
        }

        private BasicNetwork _basicNetwork;
        private BasicNeuralDataSet _trainingSet;
        private List<double> _neuronsWeight;
        private List<double[]> _bufferAnnOutput;
        private List<double[]> _bufferAnnInput;
        private List<double> _bufferTrainError;
        private int _nInputNeurons;
        private int _nHiddenNeurons;
        private int _nOutputNeurons;
        private double _error;
        private double _trainError;
        private double[][] _annInputs;
        private double[][] _annOutputs;
        private double[] _inputToAnalize;
        private double _learnRate;
        private double _momentum;
        private int _annInputDimension;
        private int _annOutputDimension;
        private NormUtil _normMapping = new NormUtil(1, 0, 255, 0);
        private Bitmap _neuralTexture;
        public double LearnRate
        {
            get { return _learnRate; }
            set { _learnRate = value; }
        }

        public double Momentum
        {
            get { return _momentum; }
            set { _momentum = value; }
        }

        public int NInputNeurons
        {
            get { return _nInputNeurons; }
            set { _nInputNeurons = value; }
        }

        public int NHiddenNeurons
        {
            get { return _nHiddenNeurons; }
            set { _nHiddenNeurons = value; }
        }

        public int NOutputNeurons
        {
            get { return _nOutputNeurons; }
            set { _nOutputNeurons = value; }
        }

        public double[][] AnnInputs
        {
            get { return _annInputs; }
            set { _annInputs = value; }
        }

        public double[][] AnnOutputs
        {
            get { return _annOutputs; }
            set { _annOutputs = value; }
        }

        public double Error
        {
            get { return _error; }
            set { _error = value; }
        }

        public double[] InputToAnalize
        {
            get { return _inputToAnalize; }
            set { _inputToAnalize = value; }
            
        }

        public double[] TempAnnInput
        {

            set { _bufferAnnInput.Add(value); }
        }

        public double[] TempAnnOutput
        {

            set { _bufferAnnOutput.Add(value); }
        }

        public int AnnInputDimension
        {
            get { return _annInputDimension; }
            set { _annInputDimension = value; }
        }


        public int AnnOutputDimension
        {
            get { return _annOutputDimension; }
            set { _annOutputDimension = value; }
        }

        public double TrainError
        {
            get { return _trainError; }
            set { _trainError = value; }
        }

        public List<double> BufferTrainError
        {
            get
            {
                return _bufferTrainError;
            }

            set
            {
                _bufferTrainError = value;
            }
        }

        public Bitmap NeuralTexture
        {
            get
            {
                return _neuralTexture;
            }

            
        }

        public NeuralNetwork()
        {

            _bufferAnnInput = new List<double[]>();
            _bufferAnnOutput = new List<double[]>();
            BufferTrainError = new List<double>();
        }

        public NetworkState CommitDataSet()
        {
            _annInputs = new double[_annInputDimension][];
            _annOutputs = new double[_annOutputDimension][];
            for (int idx = 0; idx < _bufferAnnInput.Count; idx++)
            {
                _annInputs[idx] = _bufferAnnInput[idx];
            }
            for (int idx = 0; idx < _bufferAnnOutput.Count; idx++)
            {
                _annOutputs[idx] = _bufferAnnOutput[idx];
            }
            return NetworkState.CONFIGURED;
        }

        public NetworkState TrainNetwork()
        {
            int epoch = 0;

            _trainingSet = new BasicNeuralDataSet(_annInputs, _annOutputs);

            _basicNetwork = new BasicNetwork();
            _basicNetwork.AddLayer(new BasicLayer(null, true, _nInputNeurons));
            _basicNetwork.AddLayer(new BasicLayer(new ActivationSigmoid(), true, _nHiddenNeurons));
            _basicNetwork.AddLayer(new BasicLayer(new ActivationSigmoid(), false, _nOutputNeurons));
            _basicNetwork.Structure.FinalizeStructure();
            _basicNetwork.Reset();

            //Distribuisco numeri casuali[-1,1] lavorando in ambiente stocastico(non deterministico).
            //In questo modo il training avviene in maniera casuale, partendo sempre dallo stesso stato.
            new ConsistentRandomizer(-1, 1, 100).Randomize(_basicNetwork);

            Backpropagation train = new Backpropagation(_basicNetwork, _trainingSet, LearnRate, Momentum);
            train.FixFlatSpot = false;

            do
            {
                train.Iteration();
                epoch++;
                _trainError = train.Error;

                BufferTrainError.Add(_trainError);
            } while (train.Error > _error);

            train.FinishTraining();

            _neuronsWeight = _basicNetwork.Structure.Network.Flat.Weights.Select(x => System.Convert.ToDouble(x)).ToList();
            Make2DNeuronsWeightsMap();

            foreach (IMLDataPair pair in _trainingSet)
            {
                IMLData output = _basicNetwork.Compute(pair.Input);
                Console.WriteLine("Input: " + pair.Input[0] + @" - " + pair.Input[1] + @" - " + pair.Input[2]);
                Console.WriteLine("Output 0: - actual=" + output[0] + @"-ideal=" + pair.Ideal[0]);
                Console.WriteLine("Output 1: - actual=" + output[1] + @"-ideal=" + pair.Ideal[1]);
                Console.WriteLine("Output 2: - actual=" + output[2] + @"-ideal=" + pair.Ideal[2]);
            }
            return NetworkState.TRAINED;
        }


        public double[] ProcessInput()
        {
            List<double> outputs = new List<double>();
            IMLData testInput = new BasicMLData(_inputToAnalize);
            IMLData outputResults;

            outputResults = _basicNetwork.Compute(testInput);
            for (int idx = 0; idx < outputResults.Count; idx++)
            {
                outputs.Add(outputResults[idx]);
            }
            return outputs.ToArray<double>();
        }

        private void Make2DNeuronsWeightsMap()
        {
            ImageConverter imgConverter = new ImageConverter();
            _neuralTexture = imgConverter.NeuronMapToImage(_neuronsWeight.Select(x => (byte)_normMapping.normalize(x)).ToArray(), 0xFF000000);
            
        }
    }
}
