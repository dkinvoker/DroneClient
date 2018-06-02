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

namespace DroneClient
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket _socket = null;
        private Timer _reciveTimer = null;
        private byte[] _reciveByteArray = new byte[_howManyAnalogs * sizeof(float) + _howManyCommandsBytes];
        private float[] _reciveAnalogs = new float[4];
        private const int _howManyCommandsBytes = 1;
        private const int _howManyAnalogs = 4;
        private byte[] _commandsArrayAsBytes = new byte[_howManyCommandsBytes];

        public MainWindow()
        {
            InitializeComponent();
            _reciveTimer = new Timer(100);
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
                _reciveTimer.Elapsed += _reciveTimer_Elapsed;
                _reciveTimer.Start();
            }
            catch (Exception exception)
            {
                this.LogTextBlock.Text = exception.Message;
            }

        }

        private void _reciveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                string infoToAnalogDisplay = string.Empty;

                _reciveTimer.Stop();
                _socket.Receive(_reciveByteArray);
                //Konwersja bajtów na floaty
                for (int i = 0; i < _howManyAnalogs * sizeof(float); i += sizeof(float))
                {
                    _reciveAnalogs[i / sizeof(float)] = BitConverter.ToSingle(_reciveByteArray, i);                   
                }

                infoToAnalogDisplay += "roll: " + _reciveAnalogs[0] + "\n";
                infoToAnalogDisplay += "pitch: " + _reciveAnalogs[1] + "\n";
                infoToAnalogDisplay += "gaz: " + _reciveAnalogs[2] + "\n";
                infoToAnalogDisplay += "yaw: " + _reciveAnalogs[3] + "\n";

                //Wyodrębnianie komend
                for (int i = _howManyAnalogs * sizeof(float); i < _reciveByteArray.Length; ++i)
                {
                    _commandsArrayAsBytes[i - _howManyAnalogs * sizeof(float)] = _reciveByteArray[i];
                }
                #region TEST
                ////-----W CELACH TESTOWYCH-----
                //if (_commandsArrayAsBytes[0] != 0)
                //{
                //    int i = 10;
                //}
                ////----------------------------
                #endregion

                string intoToCommandDisplay = CommandsText(BytesIntoBooleanArray(_commandsArrayAsBytes));

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

                _reciveTimer.Start();
                

            }
            catch (Exception exception)
            {
                //Gdyby się nie zastartował w traju
                _reciveTimer.Start();
                this.Dispatcher.Invoke(() => { this.FromSimulatorTextBlock.Text = exception.Message; });
                //Narazie zlewam tę sytuację, albowiem synchronizacja wątków i wgl nie działa
                //this.LogTextBlock.Text = exception.Message;
            }
        }

        private Boolean[] BytesIntoBooleanArray(byte[] commandsArray)
        {
            Boolean[] returner = new Boolean[commandsArray.Length * 8];
            int WhihByte = -1;

            for (int i = 0; i < returner.Length; ++i)
            {
                if (i % 8 == 0)
                {
                    ++WhihByte;
                }
                returner[i] = (commandsArray[WhihByte] & (1 << i % 8)) == 1 << i % 8;
            }

            return returner;
        }

        private string CommandsText(Boolean[] commands)
        {
            string returner = string.Empty;

            if (commands[CommandBooleanPossition.EmergencyIndex])
            {
                returner += "Emergency; ";
            }
            if (commands[CommandBooleanPossition.HoverIndex])
            {
                returner += "Hover; ";
            }
            if (commands[CommandBooleanPossition.LandIndex])
            {
                returner += "Land; ";
            }
            if (commands[CommandBooleanPossition.ResetEmergencyIndex])
            {
                returner += "ResetEmergency; ";
            }
            if (commands[CommandBooleanPossition.StartIndex])
            {
                returner += "Start; ";
            }
            if (commands[CommandBooleanPossition.StopIndex])
            {
                returner += "Stop; ";
            }
            if (commands[CommandBooleanPossition.TakeoffIndex])
            {
                returner += "Takeoff; ";
            }

            return returner;
        }

    }
}
