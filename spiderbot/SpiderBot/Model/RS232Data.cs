using SpiderBot.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderBot.Model
{
    class RS232Data
    {
        #region Members
        private byte _direction;
        private byte _speed;
        private byte _eye;


        #endregion

        #region Proprieties
        public byte Direction
        {
            get
            {
                return _direction;
            }

            set
            {
                _direction = value;
            }
        }

        public byte Speed
        {
            get
            {
                return _speed;
            }

            set
            {
                _speed = value;
            }
        }

        public byte Eye
        {
            get
            {
                return _eye;
            }

            set
            {
                _eye = value;
            }
        }
        #endregion

        /// <summary>
        /// Costruttore di default
        /// </summary>
        public RS232Data(double[] data) {
            try {
                _direction = (byte)data[0];
                _speed = (byte)data[1];
                _eye = (byte)data[2];
            }
            catch (Exception)
            {
                throw new Exception("Malformed RS232Data");
            }
        }

        public byte[] ToArray()
        {
            return new byte[] { _direction,_speed,_eye };
        }
    }
}
