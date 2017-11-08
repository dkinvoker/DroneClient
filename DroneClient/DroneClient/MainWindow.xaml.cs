using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
        private ClientManager _clientManager = new ClientManager();

        public MainWindow()
        {
            InitializeComponent();

        }

        private void ConnectionTestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress adress = IPAddress.Parse(this.AdressTextBox.Text);
                int port = int.Parse(this.PortTextBox.Text);
                _clientManager.ConnectToTheServer(adress, port);
            }
            catch (Exception exception)
            {
                this.ErrorTextBlock.Text = exception.Message;
            }

        }
    }
}
