using SpiderBot.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderBot.Controller
{
    class SerialManger
    {
        #region Members
        private int BUFFER_SIZE = 0x03;
        private SerialPort _serialPort;
        private string _serialPortName;
        private Thread _tAction;
        private bool _requestStop;
        private ConcurrentQueue<RS232Data> _rs232DataQueue; //TODO: da modificare con il tipo di SerialData
        private RS232Data _tempRs232Data;
        #endregion

        #region Proprieties
        public bool RequestStop
        {
            set
            {
                _requestStop = value;
            }
        }
        #endregion

        #region public Methods
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="serialPortName">Nome porta seriale connessione</param>
        public SerialManger(string serialPortName)
        {
            this._serialPortName = serialPortName;
            this._rs232DataQueue = new ConcurrentQueue<RS232Data>();
        }

        /// <summary>
        /// Effettua la connessione ed avvia il thread di invio dei dati verso il robot
        /// </summary>
        public void StartCommunication()
        {
            if (Connect() == true)
            {
                _tAction = new Thread(new ThreadStart(Action));
                _tAction.Priority = ThreadPriority.Normal;
                _tAction.Start();
            }
        }

        /// <summary>
        /// Interrompe il thread di invio dei dati verso il robot e chiude la connessione
        /// </summary>
        public void StopCommunication()
        {
            if (_tAction != null)
            {
                _requestStop = true;
                _tAction.Join();
                _tAction = null;
                _serialPort.Close();
            }

        }

        /// <summary>
        /// Aggiunge un elemento della coda in modalità thread-safe
        /// </summary>
        /// <param name="rs232Data"></param>
        public void EnqueueElementToRS232DataQueue(RS232Data rs232Data)
        {
            if (rs232Data != null && _rs232DataQueue != null)
            {
                lock (_rs232DataQueue)
                {
                    _rs232DataQueue.Enqueue(rs232Data);
                }
            }
        }
        #endregion

        #region private Methods

        /// <summary>
        /// Effettua la connessione attraverso la porta seriale
        /// </summary>
        /// <returns>TRUE se connesso</returns>
        private bool Connect()
        {
            bool result = false;

            try {
                _serialPort = new SerialPort(_serialPortName);
                _serialPort.BaudRate = 9600;
                _serialPort.Open();
                result = _serialPort.IsOpen;
            }
            catch (ArgumentException)
            {
                result = false;
            }
            return result;
        }
        
        /// <summary>
        /// Effettua lo svuotamento della coda dei dati con successiva scrittura sulla porta seriale
        /// </summary>
        private void Action()
        {
            while (!_requestStop)
            {
                if (_serialPort.IsOpen)
                {
                    _tempRs232Data = DequeueElementToRS232DataQueue();
                    if(_tempRs232Data != null)
                    {
                        
                        _serialPort.Write(_tempRs232Data.ToArray(), 0, BUFFER_SIZE);
                    }
                }

                Thread.Sleep(500);
            }

        }

        /// <summary>
        /// Effettua la pop di un elemento della coda in modalita' thread-safe
        /// </summary>
        /// <returns>Elemento della coda</returns>
        private RS232Data DequeueElementToRS232DataQueue()
        {
            RS232Data sensorData = null;
            if (_rs232DataQueue != null)
            {
                lock (_rs232DataQueue)
                {
                    if (_rs232DataQueue.Count > 0)
                    {
                        _rs232DataQueue.TryDequeue(out sensorData);
                    }
                }

            }
            return sensorData;
        }

        #endregion
    }
}
