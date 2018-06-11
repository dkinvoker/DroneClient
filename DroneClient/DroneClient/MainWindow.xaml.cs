using AR.Drone.WinApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DroneClient.Helpers;
using DroneClient.DroneHandle;
using AR.Drone.Client;

namespace DroneClient
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket _socket = null;
        private Timer _receiveTimer = null;
        private byte[] _receiveByteArray = new byte[_howManyAnalogs * sizeof(float) + _howManyCommandsBytes];
        private float[] _receiveAnalogs = new float[4];
        private const int _howManyCommandsBytes = 1;
        private const int _howManyAnalogs = 4;
        private byte[] _commandsArrayAsBytes = new byte[_howManyCommandsBytes];

        public MainWindow()
        {
            InitializeComponent();
            _receiveTimer = new Timer(100);
        }

        private void ConnectionTestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                IPAddress adress = IPAddress.Parse(this.AdressTextBox.Text);
                int port = int.Parse(this.PortTextBox.Text);
                _socket.Connect(adress, port);
                LogTextBlock.Text = "Działa!";
                _receiveTimer.Elapsed += _receiveTimer_Elapsed;
                _receiveTimer.Start();
            }
            catch (Exception exception)
            {
                this.LogTextBlock.Text = exception.Message;
            }

        }

        private void _receiveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                string infoToAnalogDisplay = string.Empty;

                _receiveTimer.Stop();
                _socket.Receive(_receiveByteArray);
                //Konwersja bajtów na floaty
                for (int i = 0; i < _howManyAnalogs * sizeof(float); i += sizeof(float))
                {
                    _receiveAnalogs[i / sizeof(float)] = BitConverter.ToSingle(_receiveByteArray, i);                   
                }

                infoToAnalogDisplay += "roll: " + _receiveAnalogs[0] + "\n";
                infoToAnalogDisplay += "pitch: " + _receiveAnalogs[1] + "\n";
                infoToAnalogDisplay += "gaz: " + _receiveAnalogs[2] + "\n";
                infoToAnalogDisplay += "yaw: " + _receiveAnalogs[3] + "\n";

                //Wyodrębnianie komend
                for (int i = _howManyAnalogs * sizeof(float); i < _receiveByteArray.Length; ++i)
                {
                    _commandsArrayAsBytes[i - _howManyAnalogs * sizeof(float)] = _receiveByteArray[i];
                }
                #region TEST
                ////-----W CELACH TESTOWYCH-----
                //if (_commandsArrayAsBytes[0] != 0)
                //{
                //    int i = 10;
                //}
                ////----------------------------
                #endregion

                string intoToCommandDisplay = Drone.CommandsText(_commandsArrayAsBytes.BytesIntoBooleanArray());

                //Ten dispaczer jest po to, aby ten wątek miał dostęp do GUI
                this.Dispatcher.Invoke(() => { this.FromSimulatorTextBlock.Text = infoToAnalogDisplay; });
                if (intoToCommandDisplay != string.Empty)
                {
                    this.Dispatcher.Invoke(() => { this.FromSimulatorCommandsTextBlock.Text = intoToCommandDisplay; });
                }
                ////-----W CELACH TESTOWYCH-----
                //else
                //{
                //    this.Dispatcher.Invoke(() => { this.FromSimulatorCommandsTextBlock.Text = "*" +  this.FromSimulatorCommandsTextBlock.Text; });
                //}
                ////----------------------------

                _receiveTimer.Start();
         
            }
            catch (Exception exception)
            {
                //Gdyby się nie zastartował w traju
                _receiveTimer.Start();
                this.Dispatcher.Invoke(() => { this.FromSimulatorTextBlock.Text = exception.Message; });
                //Narazie zlewam tę sytuację, albowiem synchronizacja wątków i wgl nie działa
                //this.LogTextBlock.Text = exception.Message;
            }
        }
    }
}
