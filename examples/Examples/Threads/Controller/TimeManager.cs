using NocommentLab.Threads.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NocommentLab.Threads.Controller
{
    /// <summary>
    /// Manages the DateTime updating
    /// </summary>
    class TimeManager
    {
        #region Constants
        private const int THREAD_PERIODICITY = 1000;
        #endregion

        #region Events
        public event EventHandler<TimeEventArgs> OnUpdateTime;
        #endregion
        
        #region Member
        private Thread _tAction;
        private bool _requestStop;
        #endregion

        #region Properties

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TimeManager()
        {
            _requestStop = false;
        }

        /// <summary>
        /// Starts the thread
        /// </summary>
        public void Start()
        {
            _tAction = new Thread(new ThreadStart(Action));
            _tAction.Priority = ThreadPriority.Lowest;
            _tAction.Start();
        }

        /// <summary>
        /// Stop the thread
        /// </summary>
        public void Stop()
        {
            if(null != _tAction)
            {
                _requestStop = true;
                _tAction.Join();
                _tAction = null;
            }
        }

        private void Action()
        {
            while(!_requestStop)
            {
                OnUpdateTime?.Invoke(this, new TimeEventArgs(DateTime.Now));
                Thread.Sleep(THREAD_PERIODICITY);
            }
        }
    }
}
