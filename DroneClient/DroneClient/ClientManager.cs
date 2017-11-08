using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DroneClient
{
    class ClientManager
    {
        private TcpClient _tcpClient = new TcpClient();

        //This method establishes a connection between client and the server app
        public void ConnectToTheServer(IPAddress ServerAdress, int port)
        {
            _tcpClient.Connect(ServerAdress, port);
        }

    }
}
