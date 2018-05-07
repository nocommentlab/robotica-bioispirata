using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderBot.Controller
{
    class SensorManager
    {
        #region Membri di classe
        private int _deltaX;
        private int _deltaY;
        private double _atanRadians;
        private double _atanDegrees;
        private byte _quadrant;
        #endregion

        
        public enum ProximityDirection
        {
            ND,
            FRONT,
            RIGHT,
            BACK,
            LEFT,
            
        }

        public enum ProximityDistance
        {
            ND,
            FAR,
            MED,
            NEAR,

        }

        public enum LuxIntensity
        {
            ND,
            LOW,
            HIGH,
            
        }

        public enum MovementSpeed
        {
            ND,
            LOW,
            MED,
            HIGH
        }

        public struct Quadrants
        {
            public const int FIRST = 1;
            public const int SECOND = 2;
            public const int THIRD = 3;
            public const int FOURTH = 4;
        }

        /// <summary>
        /// Resituisce la posizione dell'ostacolo
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        /// <returns></returns>
        public ProximityDirection RetrieveObstaclePosition(int centerX, int centerY,int mouseX, int mouseY)
        {
            _deltaX = mouseX - centerX;
            _deltaY = centerY - mouseY;
            _atanRadians = Math.Atan2(_deltaY, _deltaX);
            _atanDegrees = (_atanRadians * 180) / Math.PI;
            _quadrant = EvaluateQuadrant(_atanRadians);
            return getDirection(_atanDegrees, _quadrant);
        }

        /// <summary>
        /// Ricava la posizione da cui proviene l'input
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="quadrant"></param>
        /// <returns></returns>
        private ProximityDirection getDirection(double degrees, byte quadrant)
        {
            if ((degrees > 45 && degrees < 90 && quadrant == Quadrants.FIRST) || (degrees > 90 && degrees < 135 && quadrant == Quadrants.SECOND)) return ProximityDirection.FRONT;
            if ((degrees > 135 && degrees < 180 && quadrant == Quadrants.SECOND) || (degrees > -180 && degrees < -135 && quadrant == Quadrants.THIRD)) return ProximityDirection.LEFT;
            if ((degrees > -135 && degrees < -90 && quadrant == Quadrants.THIRD) || (degrees > -90 && degrees < -45 && quadrant == Quadrants.FOURTH)) return ProximityDirection.BACK;
            if ((degrees > -45 && degrees < 0 && quadrant == Quadrants.FOURTH) || (degrees > 0 && degrees < 45 && quadrant == Quadrants.FIRST)) return ProximityDirection.RIGHT;
            return ProximityDirection.ND;
        }

        /// <summary>
        /// Restituisce il quadrante in cui e' allocato il punto
        /// </summary>
        /// <param name="radiants">Angolo espresso in radianti</param>
        /// <returns></returns>
        private byte EvaluateQuadrant(double radiants)
        {
            if (radiants > 0 && radiants < Math.PI / 2)
            {
                return 1;
            }
            if (radiants > (Math.PI / 2) && radiants < Math.PI)
            {
                return 2;
            }
            if (radiants > (Math.PI * -1) && radiants < ((Math.PI * -1) / 2))
            {
                return 3;
            }
            if (radiants > ((Math.PI * -1) / 2) && radiants < 0)
            {
                return 4;
            }
            return 0;
        }

        

    }
}
