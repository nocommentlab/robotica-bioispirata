using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NocommentLab.Threads.Model
{
    /// <summary>
    /// Contains the actual datetime
    /// </summary>
    class TimeEventArgs:EventArgs
    {
        #region Members
        private DateTime _now;
        #endregion

        #region Properties
        public String Now { get => _now.ToString("yyyy-MM-dd hh:mm:ss"); }
        #endregion 

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TimeEventArgs(DateTime now)
        {
            _now = now;
        }

        
    }
}
