using SpiderBot.Controller;
using SpiderBot.Model;
using System;
using System.Drawing;
using System.Windows.Forms;
using ArtificialIntelligence;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO.Ports;
using System.IO;
using System.Diagnostics;

namespace SpiderBot.View
{
    public partial class Main : Form
    {
        /// <summary>
        /// Contiene i valori dei colori utilizzati all'interno della logica di Business
        /// </summary>
        private struct PanelColors
        {
            public const int PANEL_MOUSEOVER = (unchecked((int)0xFF448AFF));
            public const int PANEL_MOUSELEAVE = (unchecked((int)0xFF90CAF9));

        }

        #region Membri
        private SensorManager _sensorManager;
        private Panel _selectedPanel;
        private int _centerX;
        private int _centerY;
        private SensorManager.ProximityDirection _proximityDirection = SensorManager.ProximityDirection.ND;
        private SensorManager.ProximityDistance _proximityDistance = SensorManager.ProximityDistance.ND;
        private SensorManager.LuxIntensity _luxIntensity = SensorManager.LuxIntensity.ND;
        private AIManager _aiManager;
        #endregion

        #region public Methods
        /// <summary>
        /// Costruttore di default
        /// </summary>
        public Main()
        {
            InitializeComponent();

            _sensorManager = new SensorManager();
         
        }
        #endregion

        #region Callbacks
        /// <summary>
        /// Gestione caricamento winform
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {


            this.uiPanelSensor1.Tag = SensorManager.ProximityDistance.FAR;
            this.uiPanelSensor2.Tag = SensorManager.ProximityDistance.MED;
            this.uiPanelSensor3.Tag = SensorManager.ProximityDistance.NEAR;

            //Registro le callback del mouse per gli eventi sul sensore di prossimita'
            //Ingresso
            this.uiPanelSensor1.MouseMove += PanelSensor_mousemove;
            this.uiPanelSensor2.MouseMove += PanelSensor_mousemove;
            this.uiPanelSensor3.MouseMove += PanelSensor_mousemove;
            //Uscita
            this.uiPanelSensor1.MouseLeave += PanelSensor_mouseleave;
            this.uiPanelSensor2.MouseLeave += PanelSensor_mouseleave;
            this.uiPanelSensor3.MouseLeave += PanelSensor_mouseleave;
            //Aggiorno la lista delle porte seriali
            uiCmbSerialPorts.DataSource = SerialPort.GetPortNames();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            uiLblVersion.Text = fvi.FileVersion;
        }

        /// <summary>
        /// Funzione di callback richiamata ogni volta
        /// che viene effettuato un movimento del mouse sul relativo panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelSensor_mousemove(object sender, MouseEventArgs e)
        {
            ResetColorPanelProximitySensor();

            _selectedPanel = (Panel)sender;
            _centerX = _selectedPanel.Width / 2;
            _centerY = _selectedPanel.Height / 2;
            _selectedPanel.BackColor = Color.FromArgb(PanelColors.PANEL_MOUSEOVER);

            _proximityDirection = _sensorManager.RetrieveObstaclePosition(_centerX, _centerY, e.X, e.Y);
            _proximityDistance = (SensorManager.ProximityDistance)_selectedPanel.Tag;

            uiLblProximityPosition.Text = GetStringRappresentation<SensorManager.ProximityDirection>(_proximityDirection);
            uiLblProximityDistance.Text = GetStringRappresentation<SensorManager.ProximityDistance>(_proximityDistance);
        }

        /// <summary>
        /// Funzione di callback richiamata ogni volta
        /// che il cursore del mouse lascia l'area visibile del relativo panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelSensor_mouseleave(object sender, EventArgs e)
        {
            //Resetto le componenti grafiche del sensore di prossimita'
            uiLblProximityPosition.Text = GetStringRappresentation<SensorManager.ProximityDirection>(SensorManager.ProximityDirection.ND);
            uiLblProximityDistance.Text = GetStringRappresentation<SensorManager.ProximityDistance>(SensorManager.ProximityDistance.ND);
            ResetColorPanelProximitySensor();

            //Resetto lo stato del sensore di prossimita'
            _proximityDirection = SensorManager.ProximityDirection.ND;
            _proximityDistance = SensorManager.ProximityDistance.ND;
        }

        private void uiTrackLuxIntensity_ValueChanged(object sender, EventArgs e)
        {
            _luxIntensity = ((SensorManager.LuxIntensity)uiTrackLuxIntensity.Value);
        }

        private void uiBtnStart_Click(object sender, EventArgs e)
        {
            if (uiBtnStart.Tag.ToString().Equals("STOPPED"))
            {
                _aiManager = new AIManager();
                _aiManager.Configure((int)uiNumericNInputNeurons.Value, (int)uiNumericNHiddenNeurons.Value, (int)uiNumericNOutputNeurons.Value,
                                     (double)uiNumericLearnRate.Value, (double)uiNumericMomentum.Value, (double)uiNumericError.Value,
                                     uiTxtFilePathInputDataset.Tag.ToString(), uiTxtFilePathOutputDataset.Tag.ToString(),uiCmbSerialPorts.Text);
                _aiManager.Start();

                TimerRefreshSensorData.Enabled = true;
                TimerUpdateUi.Enabled = true;

                uiBtnStart.Text = "STOP";
                uiBtnStart.Image = Properties.Resources.ic_cancel;
                uiBtnStart.BackColor = Color.FromArgb( unchecked((int)0xFFE52B3E));
                uiBtnStart.Tag = "STARTED";
            }
            else
            {
                _aiManager.Stop();

                uiBtnStart.Text = "START";
                uiBtnStart.Image = Properties.Resources.ic_start;
                uiBtnStart.BackColor = Color.FromArgb(unchecked((int)0xFF2BCC6F));
                uiBtnStart.Tag = "STOPPED";
            }
        }

        private void uiBtnSerialRefresh_Click(object sender, EventArgs e)
        {
            uiCmbSerialPorts.DataSource = SerialPort.GetPortNames();
        }

        private void TimerRefreshSensorData_Tick(object sender, EventArgs e)
        {
            if (_aiManager != null)
            {
                _aiManager.EnqueueElementToSensorDataQueue(new SensorData()
                {
                    ProximityDirection = _proximityDirection,
                    ProximityDistance = _proximityDistance,
                    LuxIntensity = _luxIntensity
                });
            }
        }
        private void TimerUpdateUi_Tick(object sender, EventArgs e)
        {
            //Cancello i grafici
            uiChartOutputNeuron.Series[0].Points.Clear();
            uiChartOutputNeuron.Series[1].Points.Clear();

            uiLblAnnState.Text = GetStringRappresentation<NeuralNetwork.NetworkState>(_aiManager.NetworkState);
            uiLblAnnTrainError.Text = _aiManager.TrainError.ToString("0.#########");

            uiImgNeuralTexture.Image = _aiManager.NeuralTexture;
            
            //Verifico che non ci siano punti sul grafico e che la rete neurale abbia effettuato  il training
            if (uiChartTrainError.Series[0].Points.Count == 0 && _aiManager.NetworkState == NeuralNetwork.NetworkState.TRAINED)
            {
                foreach (double sample in _aiManager.BufferTrainError)
                {
                    uiChartTrainError.Series[0].Points.Add(sample);

                }
            }

            //Se ci sono dati di output dalla rete neurale, li visualizzo
            if (_aiManager.ProcessOutput != null)
            {
                foreach (double sample in _aiManager.ProcessOutput)
                {
                    uiChartOutputNeuron.Series[0].Points.Add(sample);

                }
            }
            //Se ci sono dati di input della rete neurale, li visualizzo
            if(_aiManager.InputToAnalize != null)
            {
                foreach (double sample in _aiManager.InputToAnalize)
                {
                    uiChartOutputNeuron.Series[1].Points.Add(sample);

                }
            }
        }

        private void uiTxtFilePathInputDataset_Click(object sender, EventArgs e)
        {
            uiSelectFileDialog.ShowDialog();
            uiTxtFilePathInputDataset.Tag = uiSelectFileDialog.FileName;
            uiTxtFilePathInputDataset.Text = Path.GetFileName(uiSelectFileDialog.FileName);
        }

        private void uiTxtFilePathOutputDataset_Click(object sender, EventArgs e)
        {
            uiSelectFileDialog.ShowDialog();
            uiTxtFilePathOutputDataset.Tag = uiSelectFileDialog.FileName;
            uiTxtFilePathOutputDataset.Text = Path.GetFileName(uiSelectFileDialog.FileName);
        }

        #endregion

        #region Metodi
        private void ResetColorPanelProximitySensor()
        {
            this.uiPanelSensor1.BackColor = Color.FromArgb(PanelColors.PANEL_MOUSELEAVE);
            this.uiPanelSensor2.BackColor = Color.FromArgb(PanelColors.PANEL_MOUSELEAVE);
            this.uiPanelSensor3.BackColor = Color.FromArgb(PanelColors.PANEL_MOUSELEAVE);
        }

        private string GetStringRappresentation<T>(T enumName)
        {
            return ((T)enumName).ToString();
        }

         #endregion

        
    }
}
