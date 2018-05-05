using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtificialIntelligence;
using System.Collections.Concurrent;
using SpiderBot.Model;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Globalization;

namespace SpiderBot.Controller
{
    class AIManager
    {

        private NeuralNetwork _neuralNetwork;
        private NormUtil _normUtilPosition;
        private NormUtil _normUtilDistance;
        private NormUtil _normUtilLux;
        private NormUtil _normUtilSpeed;

        private NeuralNetwork.NetworkState _networkState = NeuralNetwork.NetworkState.INITIALIZED;
        private bool _requestStop;
        private ConcurrentQueue<SensorData> _sensorDataQueue;
        private SensorData _tempSensorData;
        private Thread _tAnn;
        private double[] _processOutput;
        private SerialManger _serialManger;
        private SocketManager _socketManager;

        #region Proprieties
        public NeuralNetwork.NetworkState NetworkState
        {
            get
            {
                return _networkState;
            }
        }

        public double TrainError
        {
            get
            {
                return _neuralNetwork.TrainError;
            }
        }
        public List<double> BufferTrainError
        {
            get { return _neuralNetwork.BufferTrainError; }
        }

        public double[] ProcessOutput
        {
            get
            {
                return _processOutput;
            }

        }

        public double[] InputToAnalize
        {
            get
            {
                return _neuralNetwork.InputToAnalize;
            }

        }

        public Bitmap NeuralTexture
        {
            get
            {
                return _neuralNetwork.NeuralTexture;
            }

        }
        #endregion
        /// <summary>
        /// Costruttore di default
        /// </summary>
        public AIManager()
        {
            _sensorDataQueue = new ConcurrentQueue<SensorData>();
            _normUtilPosition = new NormUtil(4, 0, 1, 0);
            _normUtilDistance = new NormUtil(3, 0, 1, 0);
            _normUtilLux = new NormUtil(1, 0, 1, 0);
            _normUtilSpeed = new NormUtil(4, 0, 1, 0);

        }

        public void Configure(int nInputNeuron, int nHiddenNeuron, int nOutputNeuron, double learnRate, double momentum, double error, string inputDatasetFilePath, string outputDatasetFilePath, string serialPort)
        {
            _neuralNetwork = new NeuralNetwork();
            _neuralNetwork.NInputNeurons = nInputNeuron;
            _neuralNetwork.NHiddenNeurons = nHiddenNeuron;
            _neuralNetwork.NOutputNeurons = nOutputNeuron;
            _neuralNetwork.AnnInputDimension = 13; //TODO: Da aggiustare in base al file del dataset
            _neuralNetwork.AnnOutputDimension = 13; //TODO: Da aggiustare in base al file del dataset
            _neuralNetwork.LearnRate = learnRate;
            _neuralNetwork.Momentum = momentum;
            _neuralNetwork.Error = error;
            LoadDatasetInputOutput(inputDatasetFilePath, outputDatasetFilePath);
            _networkState = _neuralNetwork.CommitDataSet();

            //_serialManger = new SerialManger(serialPort);
            _socketManager = new SocketManager("192.168.4.1", 65535);
        }

        public void Start()
        {

            _tAnn = new Thread(new ThreadStart(Action));
            _tAnn.Priority = ThreadPriority.Normal;
            _tAnn.Start();

            //_serialManger.StartCommunication();
            _socketManager.StartCommunication();

        }

        public void Stop()
        {
            if (_tAnn != null)
            {
                _requestStop = true;
                _tAnn.Join();
                _tAnn = null;
            }

            if (_serialManger != null)
            {
                /*_serialManger.StopCommunication();
                _serialManger = null;*/
                _socketManager.StopCommunication();
                _socketManager = null;

            }
        }

        /// <summary>
        /// Carica in memoria i dataset
        /// </summary>
        /// <param name="datasetFilePath"></param>
        private void LoadDatasetInputOutput(string datasetInputFilePath, string datasetOutputFilePath)
        {
            string[] datasetInputRows = File.ReadAllLines(datasetInputFilePath);
            string[] datasetOutputRows = File.ReadAllLines(datasetOutputFilePath);
            string[] fieldsRow = null;

            for (int idxRow = 0; idxRow < datasetInputRows.Length; idxRow++)
            {
                fieldsRow = datasetInputRows[idxRow].Split(';');
                _neuralNetwork.TempAnnInput = new double[] { double.Parse(fieldsRow[0].Trim()), double.Parse(fieldsRow[1].Trim()), double.Parse(fieldsRow[2].Trim()) };

                fieldsRow = datasetOutputRows[idxRow].Split(';');
                _neuralNetwork.TempAnnOutput = new double[] { double.Parse(fieldsRow[0].Trim()), double.Parse(fieldsRow[1].Trim()), double.Parse(fieldsRow[2].Trim()) };
            }


        }

        private void Action()
        {
            if (NetworkState == NeuralNetwork.NetworkState.CONFIGURED)
            {
                _networkState = _neuralNetwork.TrainNetwork();
                if (NetworkState == NeuralNetwork.NetworkState.TRAINED)
                {
                    while (!_requestStop)
                    {
                        _tempSensorData = DequeueElementToSensorDataQueue();

                        if (_tempSensorData != null)
                        {

                            _neuralNetwork.InputToAnalize = new double[] { _normUtilPosition.normalize((int)_tempSensorData.ProximityDirection),
                                                                           _normUtilDistance.normalize((int)_tempSensorData.ProximityDistance),
                                                                           _normUtilLux.normalize((int)_tempSensorData.LuxIntensity) };

                            _processOutput = _neuralNetwork.ProcessInput();

                            //Effettuo la denormalizzazione degli elementi
                            _processOutput[0] = _normUtilPosition.denormalize(_processOutput[0]);
                            _processOutput[1] = _normUtilSpeed.denormalize(_processOutput[1]);
                            _processOutput[2] = _normUtilLux.denormalize(_processOutput[2]);

                            
                            PrepareDataForSerialTrasmission(ref _processOutput);
                            Console.WriteLine(_processOutput[0] + " " + _processOutput[1] + " " + _processOutput[2]);
#if DEBUG
                            //Console.WriteLine(_processOutput[0] + " " + _processOutput[1] + " " + _processOutput[2]);
#endif
                            //_serialManger.EnqueueElementToRS232DataQueue(new RS232Data(_processOutput));

                            SpiderCommandAdapter.MenageSpiderCommand((byte)_processOutput[0], (byte)_processOutput[1], (byte)_processOutput[2], ref _socketManager);
                        }


                        Thread.Sleep(500);
                    }
                }
            }
        }

        public void EnqueueElementToSensorDataQueue(SensorData sensorData)
        {
            if (sensorData != null && _sensorDataQueue != null)
            {
                lock (_sensorDataQueue)
                {
                    _sensorDataQueue.Enqueue(sensorData);
                }
            }
        }

        private SensorData DequeueElementToSensorDataQueue()
        {
            SensorData sensorData = null;
            if (_sensorDataQueue != null)
            {
                lock (_sensorDataQueue)
                {
                    if (_sensorDataQueue.Count > 0)
                    {
                        _sensorDataQueue.TryDequeue(out sensorData);
                    }
                }

            }
            return sensorData;
        }

        /// <summary>
        /// Restituisci i valori interi ricavati dai neuroni di output della rete neurale.
        /// Utile per la comunicazione seriale che accetta dei byte
        /// </summary>
        /// <param name="annOutput"></param>
        private void PrepareDataForSerialTrasmission(ref double[] annOutput)
        {
            try
            {
                for (int idx = 0; idx < annOutput.Length; idx++)
                {
                    annOutput[idx] = Math.Round(annOutput[idx], 0);
                }
            }
            catch (OverflowException)
            {

                for (int idx = 0; idx < annOutput.Length; idx++)
                {
                    annOutput[idx] = 0;
                }
            }
        }
    }
}
