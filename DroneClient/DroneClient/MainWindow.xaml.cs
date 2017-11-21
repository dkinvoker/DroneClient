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
        private byte[] _reciveByteArray = new byte[32];
        private Int16[] _reciveAnalogs = new Int16[16];

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
                this.FromSimulatorTextBlock.Text = string.Empty;
                _socket.Receive(_reciveByteArray);
                //Konwersja bajtów na inty16-bitowe
                for (int i = 0; i < _reciveByteArray.Length; i+=2)
                {
                    _reciveAnalogs[i / 2] = BitConverter.ToInt16(_reciveByteArray, i);
                    this.FromSimulatorTextBlock.Text = i + ": " + _reciveAnalogs[i / 2];
                }

                

            }
            catch (Exception exception)
            {
                //Narazie zlewam tę sytuację, albowiem synchronizacja wątków i wgl nie działa
                //this.LogTextBlock.Text = exception.Message;
            }
        }

    }
}
