using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderBot.Controller
{
    /// <summary>
    /// Trasform the neural network output to spider protocol
    /// </summary>
    class SpiderCommandAdapter
    {
        #region Members

        private static readonly byte[] WALKFOWARD = new byte[] { 0x80, 0x40, 0x81 };
        private static readonly byte[] TURNRIGHT = new byte[] { 0x80, 0x46, 0x81 };
        private static readonly byte[] WALKBACKWARD = new byte[] { 0x80, 0x42, 0x81 };
        private static readonly byte[] TURNLEFT = new byte[] { 0x80, 0x44, 0x81 };

        private static readonly byte[] TILTFOWARD = new byte[] { 0x80, 0x2a, 0x49, 0x40, 0x81 };
        private static readonly byte[] TILTRIGHT = new byte[] { 0x80, 0x2a, 0x40, 0x36, 0x81 };
        private static readonly byte[] TILTBACKWARD = new byte[] { 0x80, 0x2a, 0x36, 0x40, 0x81 };
        private static readonly byte[] TILTLEFT = new byte[] { 0x80, 0x2a, 0x40, 0x4a, 0x81 };
        #endregion

        #region Properties
        #endregion

        public static void MenageSpiderCommand(byte annOutputDirection,
                                               byte annOutputDistance,
                                               byte annOutputLux,
                                               ref SocketManager socketManager)
        {
            if(annOutputDistance!=0)
            {
                switch(annOutputDistance)
                { 
                    case 1:
                        socketManager.EnqueueElementToSocketData(new Model.SocketData(TILTFOWARD));
                        break;
                    case 2:
                        socketManager.EnqueueElementToSocketData(new Model.SocketData(TILTRIGHT));
                        break;
                    case 3:
                        socketManager.EnqueueElementToSocketData(new Model.SocketData(TILTBACKWARD));
                        break;
                    case 4:
                        socketManager.EnqueueElementToSocketData(new Model.SocketData(TILTLEFT));
                        break;
                }
            }
            else if(annOutputDirection != 0)
            {
                switch(annOutputDirection)
                { 
                    case 1:
                            socketManager.EnqueueElementToSocketData(new Model.SocketData(WALKFOWARD));
                            break;
                    case 2:
                            socketManager.EnqueueElementToSocketData(new Model.SocketData(TURNRIGHT));
                        break;
                    case 3:
                            socketManager.EnqueueElementToSocketData(new Model.SocketData(WALKBACKWARD));
                        break;
                    case 4:
                            socketManager.EnqueueElementToSocketData(new Model.SocketData(TURNLEFT));
                        break;
                }
            }
        }

    }
}
