using SpiderBot.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderBot.Controller
{
    class SocketManager
    {
        #region Members
        private Thread _tAction;
        private string _ipAddress;
        private int _port;
        private bool _requestStop;
        private Socket _socket;
        private ConcurrentQueue<SocketData> _socketDataQueue;
        private SocketData _tempSocketData;
        #endregion

        #region Properties
        public bool RequestStop { set => _requestStop = value; }
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SocketManager(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
            _socketDataQueue = new ConcurrentQueue<SocketData>();
        }

        /// <summary>
        /// Starts the socket communication
        /// </summary>
        public void StartCommunication()
        {
            if(Connect() == true)
            {
                _tAction = new Thread(new ThreadStart(Action));
                _tAction.Priority = ThreadPriority.Normal;
                _tAction.Start();
            }
        }

        /// <summary>
        /// Stops the socket communication
        /// </summary>
        public void StopCommunication()
        {
            if (_tAction != null)
            {
                _requestStop = true;
                _tAction.Join();
                _tAction = null;
                _socket.Close();
            }
        }

        /// <summary>
        /// Adds an element to the socket data queue
        /// </summary>
        /// <param name="socketData"></param>
        public void EnqueueElementToSocketData(SocketData socketData)
        {
            if(socketData != null && _socketDataQueue != null)
            {
                lock(_socketDataQueue)
                {
                    _socketDataQueue.Enqueue(socketData);
                }
            }
        }

        private SocketData DequeueElementFromSocketData()
        {
            SocketData socketData = null;
            if(_socketDataQueue != null)
            {
                lock(_socketDataQueue)
                {
                    if(_socketDataQueue.Count > 0)
                    {
                        _socketDataQueue.TryDequeue(out socketData);
                    }
                }
            }
            return socketData;
        }

        private void Action()
        {
            while(!_requestStop)
            {
                if(_socket.Connected)
                {
                    _tempSocketData = DequeueElementFromSocketData();
                    if(_tempSocketData != null)
                    {
                        _socket.Send(_tempSocketData.ToArray());
                    }
                }
            }

        }


        /// <summary>
        /// Establishes the socket connection
        /// </summary>
        /// <returns></returns>
        private bool Connect()
        {
            bool result = false;

            try
            {
                /* Creates the endpoint */
                IPEndPoint remoteServer = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
                /* Generates the socket communication */
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                /* Establish the connection */
                _socket.Connect(remoteServer);

                result = _socket.Connected;
            }
            catch(Exception)
            {
                result = false;
            }

            return result;
        }

    }
}
