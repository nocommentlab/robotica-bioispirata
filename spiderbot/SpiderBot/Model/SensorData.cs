using SpiderBot.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderBot.Model
{
    class SensorData
    {
        #region Membri
        private SensorManager.ProximityDistance _proximityDistance;
        private SensorManager.ProximityDirection _proximityDirection;
        private SensorManager.LuxIntensity _luxIntensity;
        #endregion

        #region Proprieta'
        public SensorManager.ProximityDistance ProximityDistance
        {
            get
            {
                return _proximityDistance;
            }

            set
            {
                _proximityDistance = value;
            }
        }
        public SensorManager.ProximityDirection ProximityDirection
        {
            get
            {
                return _proximityDirection;
            }

            set
            {
                _proximityDirection = value;
            }
        }
        public SensorManager.LuxIntensity LuxIntensity
        {
            get
            {
                return _luxIntensity;
            }

            set
            {
                _luxIntensity = value;
            }
        }
        #endregion
        
        /// <summary>
        /// Costruttore di default
        /// </summary>
        public SensorData() { }

      
    }
}
