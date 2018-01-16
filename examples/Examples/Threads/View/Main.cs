using NocommentLab.Threads.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NocommentLab.Threads.View
{
    public partial class Main : Form
    {
        #region Members
        private TimeManager _timeManager;
        #endregion

        public Main()
        {
            InitializeComponent();
            uiBtnStartStop.Tag = false;
        }

        private void uiBtnStartStop_Click(object sender, EventArgs e)
        {
            /* Checks the button state */
            if(!(bool)uiBtnStartStop.Tag)
            {
                /* Creates a new timemanager istance */
                _timeManager = new TimeManager();
                /* Registers the callback */
                _timeManager.OnUpdateTime += _timeManager_OnUpdateTime;
                /* Starts the thread */
                _timeManager.Start();

                /* Changes the button properties */
                uiBtnStartStop.Tag = true;
                uiBtnStartStop.Text = "STOP";
            }
            else
            {
                /* Stops the thread */
                _timeManager.Stop();
                /* Unregisters the callback */
                _timeManager.OnUpdateTime -= _timeManager_OnUpdateTime;
                _timeManager = null;

                /* Changes the button properties */
                uiBtnStartStop.Tag = false;
                uiBtnStartStop.Text = "START";
            }
        }

        private void _timeManager_OnUpdateTime(object sender, Model.TimeEventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate () { _timeManager_OnUpdateTime(sender, e); }));
            }
            else
            {
                
                    uiLblDateTime.Text = e.Now;
            }
        }
    }
}
