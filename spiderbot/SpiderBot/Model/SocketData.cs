using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderBot.Model
{
    class SocketData
    {
        private byte[] _buffer;

        public SocketData(byte[] buffer)
        {
            _buffer = buffer;
        }

        public byte[] ToArray()
        {
            return _buffer;
        }
    }
}
